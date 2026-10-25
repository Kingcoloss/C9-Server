package db

import (
	"context"
	"database/sql"
	"errors"
	"time"
)

// Character is one row of the characters table.
type Character struct {
	ID           int64
	AccountID    int64
	WorldID      int
	Slot         int
	Name         string
	ClassID      int
	Level        int
	Exp          int64
	Gold         int64
	HP, SP       int
	Fatigue      int
	MapID        int
	X, Y, Z      float32
	Direction    float32
	Appearance   []byte
	PlaySeconds  int64
	CreatedAt    time.Time
	LastPlayedAt time.Time
}

const characterCols = `id, account_id, world_id, slot, name, class_id, level, exp, gold, hp, sp, fatigue,
	map_id, pos_x, pos_y, pos_z, direction, appearance, play_seconds, created_at, last_played_at`

func scanCharacter(row interface{ Scan(...any) error }) (*Character, error) {
	var c Character
	var created, last sql.NullString
	err := row.Scan(&c.ID, &c.AccountID, &c.WorldID, &c.Slot, &c.Name, &c.ClassID, &c.Level, &c.Exp, &c.Gold,
		&c.HP, &c.SP, &c.Fatigue, &c.MapID, &c.X, &c.Y, &c.Z, &c.Direction, &c.Appearance, &c.PlaySeconds,
		&created, &last)
	if errors.Is(err, sql.ErrNoRows) {
		return nil, ErrNotFound
	}
	if err != nil {
		return nil, err
	}
	c.CreatedAt = parseTime(created)
	c.LastPlayedAt = parseTime(last)
	return &c, nil
}

// CharactersOf returns the live characters of an account on a world, by slot.
func (s *Store) CharactersOf(ctx context.Context, accountID int64, worldID int) ([]*Character, error) {
	rows, err := s.DB.QueryContext(ctx, `SELECT `+characterCols+` FROM characters
		WHERE account_id = ? AND world_id = ? AND deleted_at IS NULL ORDER BY slot`, accountID, worldID)
	if err != nil {
		return nil, err
	}
	defer rows.Close()
	var out []*Character
	for rows.Next() {
		c, err := scanCharacter(rows)
		if err != nil {
			return nil, err
		}
		out = append(out, c)
	}
	return out, rows.Err()
}

// CharacterByID loads a live character.
func (s *Store) CharacterByID(ctx context.Context, id int64) (*Character, error) {
	return scanCharacter(s.DB.QueryRowContext(ctx, `SELECT `+characterCols+` FROM characters
		WHERE id = ? AND deleted_at IS NULL`, id))
}

// CharacterNameTaken reports whether a live or deleted character uses name.
func (s *Store) CharacterNameTaken(ctx context.Context, name string) (bool, error) {
	var n int
	err := s.DB.QueryRowContext(ctx, `SELECT COUNT(*) FROM characters WHERE name = ?`, name).Scan(&n)
	return n > 0, err
}

// CreateCharacter inserts c and fills in its ID.
func (s *Store) CreateCharacter(ctx context.Context, c *Character) error {
	res, err := s.DB.ExecContext(ctx, `INSERT INTO characters
		(account_id, world_id, slot, name, class_id, level, exp, gold, hp, sp, fatigue,
		 map_id, pos_x, pos_y, pos_z, direction, appearance, created_at)
		VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?)`,
		c.AccountID, c.WorldID, c.Slot, c.Name, c.ClassID, c.Level, c.Exp, c.Gold, c.HP, c.SP, c.Fatigue,
		c.MapID, c.X, c.Y, c.Z, c.Direction, c.Appearance, now())
	if err != nil {
		return err
	}
	c.ID, err = res.LastInsertId()
	return err
}

// DeleteCharacter soft-deletes a character and renames it so the name can be reused.
func (s *Store) DeleteCharacter(ctx context.Context, id int64) error {
	_, err := s.DB.ExecContext(ctx, `UPDATE characters SET deleted_at = ?, name = name || '#' || id
		WHERE id = ? AND deleted_at IS NULL`, now(), id)
	return err
}

// SaveCharacter writes the fields that change during play.
func (s *Store) SaveCharacter(ctx context.Context, c *Character) error {
	_, err := s.DB.ExecContext(ctx, `UPDATE characters SET level = ?, exp = ?, gold = ?, hp = ?, sp = ?,
		fatigue = ?, map_id = ?, pos_x = ?, pos_y = ?, pos_z = ?, direction = ?, play_seconds = ?,
		last_played_at = ? WHERE id = ?`,
		c.Level, c.Exp, c.Gold, c.HP, c.SP, c.Fatigue, c.MapID, c.X, c.Y, c.Z, c.Direction, c.PlaySeconds, now(), c.ID)
	return err
}

// ListCharacters is used by the admin panel.
func (s *Store) ListCharacters(ctx context.Context, query string, limit, offset int) ([]*Character, error) {
	rows, err := s.DB.QueryContext(ctx, `SELECT `+characterCols+` FROM characters
		WHERE deleted_at IS NULL AND name LIKE ? ORDER BY id DESC LIMIT ? OFFSET ?`, "%"+query+"%", limit, offset)
	if err != nil {
		return nil, err
	}
	defer rows.Close()
	var out []*Character
	for rows.Next() {
		c, err := scanCharacter(rows)
		if err != nil {
			return nil, err
		}
		out = append(out, c)
	}
	return out, rows.Err()
}

// CountCharacters returns the number of live characters.
func (s *Store) CountCharacters(ctx context.Context) (int, error) {
	var n int
	err := s.DB.QueryRowContext(ctx, `SELECT COUNT(*) FROM characters WHERE deleted_at IS NULL`).Scan(&n)
	return n, err
}

// AdminUpdateCharacter changes the fields an admin may edit by hand.
func (s *Store) AdminUpdateCharacter(ctx context.Context, id int64, level int, exp, gold int64) error {
	_, err := s.DB.ExecContext(ctx, `UPDATE characters SET level = ?, exp = ?, gold = ? WHERE id = ?`, level, exp, gold, id)
	return err
}
