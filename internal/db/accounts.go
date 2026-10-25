package db

import (
	"context"
	"database/sql"
	"errors"
	"strings"
	"time"

	"golang.org/x/crypto/bcrypt"
)

// Account is one row of the accounts table.
type Account struct {
	ID           int64
	Username     string
	PasswordHash string
	Email        string
	GMLevel      int
	IsAdmin      bool
	BannedUntil  time.Time // zero = not banned
	BanReason    string
	Cash         int64
	CreatedAt    time.Time
	LastLoginAt  time.Time
	LastLoginIP  string
}

// Banned reports whether the account is banned at t.
func (a *Account) Banned(t time.Time) bool {
	return !a.BannedUntil.IsZero() && t.Before(a.BannedUntil)
}

// CheckPassword compares a plaintext password with the stored hash.
func (a *Account) CheckPassword(pw string) bool {
	return bcrypt.CompareHashAndPassword([]byte(a.PasswordHash), []byte(pw)) == nil
}

// HashPassword returns the bcrypt hash stored in password_hash.
func HashPassword(pw string) (string, error) {
	h, err := bcrypt.GenerateFromPassword([]byte(pw), bcrypt.DefaultCost)
	return string(h), err
}

const accountCols = `id, username, password_hash, COALESCE(email,''), gm_level, is_admin,
	banned_until, COALESCE(ban_reason,''), cash, created_at, last_login_at, COALESCE(last_login_ip,'')`

func scanAccount(row interface{ Scan(...any) error }) (*Account, error) {
	var a Account
	var banned, created, last sql.NullString
	err := row.Scan(&a.ID, &a.Username, &a.PasswordHash, &a.Email, &a.GMLevel, &a.IsAdmin,
		&banned, &a.BanReason, &a.Cash, &created, &last, &a.LastLoginIP)
	if errors.Is(err, sql.ErrNoRows) {
		return nil, ErrNotFound
	}
	if err != nil {
		return nil, err
	}
	a.BannedUntil = parseTime(banned)
	a.CreatedAt = parseTime(created)
	a.LastLoginAt = parseTime(last)
	return &a, nil
}

// CreateAccount inserts a new account with a bcrypt-hashed password.
func (s *Store) CreateAccount(ctx context.Context, username, password string, isAdmin bool) (*Account, error) {
	username = strings.TrimSpace(username)
	if username == "" || password == "" {
		return nil, errors.New("username and password are required")
	}
	h, err := HashPassword(password)
	if err != nil {
		return nil, err
	}
	res, err := s.DB.ExecContext(ctx,
		`INSERT INTO accounts (username, password_hash, is_admin, created_at) VALUES (?, ?, ?, ?)`,
		username, h, isAdmin, now())
	if err != nil {
		return nil, err
	}
	id, _ := res.LastInsertId()
	return s.AccountByID(ctx, id)
}

// AccountByID loads an account by id.
func (s *Store) AccountByID(ctx context.Context, id int64) (*Account, error) {
	return scanAccount(s.DB.QueryRowContext(ctx, `SELECT `+accountCols+` FROM accounts WHERE id = ?`, id))
}

// AccountByName loads an account by username (case-insensitive).
func (s *Store) AccountByName(ctx context.Context, username string) (*Account, error) {
	return scanAccount(s.DB.QueryRowContext(ctx, `SELECT `+accountCols+` FROM accounts WHERE username = ?`, username))
}

// ListAccounts returns accounts whose name contains query, newest first.
func (s *Store) ListAccounts(ctx context.Context, query string, limit, offset int) ([]*Account, error) {
	rows, err := s.DB.QueryContext(ctx, `SELECT `+accountCols+` FROM accounts
		WHERE username LIKE ? ORDER BY id DESC LIMIT ? OFFSET ?`, "%"+query+"%", limit, offset)
	if err != nil {
		return nil, err
	}
	defer rows.Close()
	var out []*Account
	for rows.Next() {
		a, err := scanAccount(rows)
		if err != nil {
			return nil, err
		}
		out = append(out, a)
	}
	return out, rows.Err()
}

// CountAccounts returns the number of accounts.
func (s *Store) CountAccounts(ctx context.Context) (int, error) {
	var n int
	err := s.DB.QueryRowContext(ctx, `SELECT COUNT(*) FROM accounts`).Scan(&n)
	return n, err
}

// CountAdmins returns the number of accounts allowed into the admin panel.
func (s *Store) CountAdmins(ctx context.Context) (int, error) {
	var n int
	err := s.DB.QueryRowContext(ctx, `SELECT COUNT(*) FROM accounts WHERE is_admin = 1`).Scan(&n)
	return n, err
}

// SetPassword replaces an account's password.
func (s *Store) SetPassword(ctx context.Context, id int64, password string) error {
	h, err := HashPassword(password)
	if err != nil {
		return err
	}
	_, err = s.DB.ExecContext(ctx, `UPDATE accounts SET password_hash = ? WHERE id = ?`, h, id)
	return err
}

// SetBan bans an account until the given time; a zero time lifts the ban.
func (s *Store) SetBan(ctx context.Context, id int64, until time.Time, reason string) error {
	var u any
	if !until.IsZero() {
		u = until.UTC().Format(time.RFC3339)
	}
	_, err := s.DB.ExecContext(ctx, `UPDATE accounts SET banned_until = ?, ban_reason = ? WHERE id = ?`, u, reason, id)
	return err
}

// SetRoles updates the GM level and admin flag.
func (s *Store) SetRoles(ctx context.Context, id int64, gmLevel int, isAdmin bool) error {
	_, err := s.DB.ExecContext(ctx, `UPDATE accounts SET gm_level = ?, is_admin = ? WHERE id = ?`, gmLevel, isAdmin, id)
	return err
}

// AddCash adds (or with a negative amount, removes) shop currency.
func (s *Store) AddCash(ctx context.Context, id int64, amount int64) error {
	_, err := s.DB.ExecContext(ctx, `UPDATE accounts SET cash = MAX(0, cash + ?) WHERE id = ?`, amount, id)
	return err
}

// RecordLogin stores a login attempt and, on success, the last login info.
func (s *Store) RecordLogin(ctx context.Context, accountID int64, username, ip string, success bool, reason string) error {
	var acc any
	if accountID != 0 {
		acc = accountID
	}
	t := now()
	if _, err := s.DB.ExecContext(ctx, `INSERT INTO login_history (account_id, username, ip, success, reason, created_at)
		VALUES (?, ?, ?, ?, ?, ?)`, acc, username, ip, success, reason, t); err != nil {
		return err
	}
	if success && accountID != 0 {
		_, err := s.DB.ExecContext(ctx, `UPDATE accounts SET last_login_at = ?, last_login_ip = ? WHERE id = ?`, t, ip, accountID)
		return err
	}
	return nil
}
