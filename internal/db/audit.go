package db

import (
	"context"
	"database/sql"
	"time"
)

// AuditEntry is one row of the audit log.
type AuditEntry struct {
	ID        int64
	ActorID   int64
	ActorName string
	Action    string
	Target    string
	Detail    string
	CreatedAt time.Time
}

// Audit records an admin action. actorID 0 means the server itself.
func (s *Store) Audit(ctx context.Context, actorID int64, action, target, detail string) error {
	var actor any
	if actorID != 0 {
		actor = actorID
	}
	_, err := s.DB.ExecContext(ctx, `INSERT INTO audit_log (actor_id, action, target, detail, created_at)
		VALUES (?, ?, ?, ?, ?)`, actor, action, target, detail, now())
	return err
}

// RecentAudit returns the newest audit entries.
func (s *Store) RecentAudit(ctx context.Context, limit int) ([]AuditEntry, error) {
	rows, err := s.DB.QueryContext(ctx, `SELECT l.id, COALESCE(l.actor_id, 0), COALESCE(a.username, 'system'),
		l.action, COALESCE(l.target, ''), COALESCE(l.detail, ''), l.created_at
		FROM audit_log l LEFT JOIN accounts a ON a.id = l.actor_id ORDER BY l.id DESC LIMIT ?`, limit)
	if err != nil {
		return nil, err
	}
	defer rows.Close()
	var out []AuditEntry
	for rows.Next() {
		var e AuditEntry
		var created sql.NullString
		if err := rows.Scan(&e.ID, &e.ActorID, &e.ActorName, &e.Action, &e.Target, &e.Detail, &created); err != nil {
			return nil, err
		}
		e.CreatedAt = parseTime(created)
		out = append(out, e)
	}
	return out, rows.Err()
}

// World is one entry of the server list.
type World struct {
	ID         int
	Name       string
	Host       string
	Port       int
	Status     int
	MaxPlayers int
}

// Worlds returns every world in display order.
func (s *Store) Worlds(ctx context.Context) ([]World, error) {
	rows, err := s.DB.QueryContext(ctx, `SELECT id, name, host, port, status, max_players FROM worlds ORDER BY sort_order, id`)
	if err != nil {
		return nil, err
	}
	defer rows.Close()
	var out []World
	for rows.Next() {
		var w World
		if err := rows.Scan(&w.ID, &w.Name, &w.Host, &w.Port, &w.Status, &w.MaxPlayers); err != nil {
			return nil, err
		}
		out = append(out, w)
	}
	return out, rows.Err()
}

// UpdateWorld changes a world's address, name and status.
func (s *Store) UpdateWorld(ctx context.Context, w World) error {
	_, err := s.DB.ExecContext(ctx, `UPDATE worlds SET name = ?, host = ?, port = ?, status = ?, max_players = ? WHERE id = ?`,
		w.Name, w.Host, w.Port, w.Status, w.MaxPlayers, w.ID)
	return err
}
