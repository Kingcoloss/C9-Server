// Package db stores accounts, characters and the rest of the game state in
// SQLite. Schema changes live in migrations/*.sql and are applied in order
// at start, tracked with PRAGMA user_version.
package db

import (
	"context"
	"database/sql"
	"embed"
	"errors"
	"fmt"
	"io/fs"
	"sort"
	"strconv"
	"strings"
	"time"

	_ "modernc.org/sqlite" // registers the "sqlite" driver
)

//go:embed migrations/*.sql
var migrationFS embed.FS

// ErrNotFound is returned when a looked-up row does not exist.
var ErrNotFound = errors.New("not found")

// Store wraps the database handle.
type Store struct {
	DB *sql.DB
}

// Open opens (or creates) the SQLite file at path and migrates it.
func Open(ctx context.Context, path string) (*Store, error) {
	dsn := "file:" + path + "?_pragma=foreign_keys(1)&_pragma=journal_mode(WAL)&_pragma=busy_timeout(5000)&_pragma=synchronous(NORMAL)"
	sqldb, err := sql.Open("sqlite", dsn)
	if err != nil {
		return nil, err
	}
	// SQLite allows one writer at a time; a single connection avoids
	// SQLITE_BUSY between our own goroutines.
	sqldb.SetMaxOpenConns(1)
	s := &Store{DB: sqldb}
	if err := s.migrate(ctx); err != nil {
		sqldb.Close()
		return nil, err
	}
	return s, nil
}

// Close closes the database.
func (s *Store) Close() error { return s.DB.Close() }

func (s *Store) migrate(ctx context.Context) error {
	var current int
	if err := s.DB.QueryRowContext(ctx, "PRAGMA user_version").Scan(&current); err != nil {
		return err
	}
	entries, err := fs.ReadDir(migrationFS, "migrations")
	if err != nil {
		return err
	}
	type mig struct {
		version int
		name    string
	}
	var migs []mig
	for _, e := range entries {
		n := e.Name()
		v, err := strconv.Atoi(strings.SplitN(n, "_", 2)[0])
		if err != nil {
			return fmt.Errorf("bad migration name %q", n)
		}
		migs = append(migs, mig{v, n})
	}
	sort.Slice(migs, func(i, j int) bool { return migs[i].version < migs[j].version })
	for _, m := range migs {
		if m.version <= current {
			continue
		}
		body, err := migrationFS.ReadFile("migrations/" + m.name)
		if err != nil {
			return err
		}
		tx, err := s.DB.BeginTx(ctx, nil)
		if err != nil {
			return err
		}
		if _, err := tx.ExecContext(ctx, string(body)); err != nil {
			tx.Rollback()
			return fmt.Errorf("migration %s: %w", m.name, err)
		}
		if _, err := tx.ExecContext(ctx, fmt.Sprintf("PRAGMA user_version = %d", m.version)); err != nil {
			tx.Rollback()
			return err
		}
		if err := tx.Commit(); err != nil {
			return err
		}
	}
	return nil
}

// now returns the timestamp format stored in TEXT columns.
func now() string { return time.Now().UTC().Format(time.RFC3339) }

func parseTime(s sql.NullString) time.Time {
	if !s.Valid {
		return time.Time{}
	}
	t, _ := time.Parse(time.RFC3339, s.String)
	return t
}
