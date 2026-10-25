// Package config loads the server configuration from a TOML file.
package config

import (
	"fmt"
	"os"

	"github.com/BurntSushi/toml"
)

// Config is the whole server configuration.
type Config struct {
	Database DatabaseConfig `toml:"database"`
	Login    LoginConfig    `toml:"login"`
	Game     GameConfig     `toml:"game"`
	Admin    AdminConfig    `toml:"admin"`
	Log      LogConfig      `toml:"log"`
}

// DatabaseConfig selects where game data is stored.
type DatabaseConfig struct {
	// Path of the SQLite database file.
	Path string `toml:"path"`
}

// LoginConfig is the login (auth) server that the client connects to first.
// The client reads its address from [SERVER] IP1/PORT1 in version.ini.
type LoginConfig struct {
	Listen string `toml:"listen"`
	// AutoRegister creates an account on the first login with an unknown
	// username, which is handy for a small private server.
	AutoRegister bool `toml:"auto_register"`
}

// GameConfig is the game (world) server the login server hands players to.
type GameConfig struct {
	Listen string `toml:"listen"`
	// PublicHost and PublicPort are what the login server tells the client
	// to connect to. They must be reachable from the players' machines.
	PublicHost string `toml:"public_host"`
	PublicPort int    `toml:"public_port"`
	WorldName  string `toml:"world_name"`
	MaxPlayers int    `toml:"max_players"`
}

// AdminConfig is the web admin panel.
type AdminConfig struct {
	Enabled bool   `toml:"enabled"`
	Listen  string `toml:"listen"`
	// SessionSecret signs admin session cookies. Leave empty to generate a
	// random one at start (admins are logged out on every restart).
	SessionSecret string `toml:"session_secret"`
	// BootstrapUser and BootstrapPassword create the first admin account
	// when the database has no admin yet.
	BootstrapUser     string `toml:"bootstrap_user"`
	BootstrapPassword string `toml:"bootstrap_password"`
}

// LogConfig controls logging.
type LogConfig struct {
	Level string `toml:"level"` // debug, info, warn, error
	// PacketTrace logs every packet in hex. Very noisy.
	PacketTrace bool `toml:"packet_trace"`
}

// Default returns the configuration used when no file is given.
func Default() Config {
	return Config{
		Database: DatabaseConfig{Path: "c9server.db"},
		Login:    LoginConfig{Listen: "0.0.0.0:10721", AutoRegister: false},
		Game: GameConfig{
			Listen:     "0.0.0.0:10722",
			PublicHost: "127.0.0.1",
			PublicPort: 10722,
			WorldName:  "C9 Private",
			MaxPlayers: 500,
		},
		Admin: AdminConfig{Enabled: true, Listen: "127.0.0.1:8080"},
		Log:   LogConfig{Level: "info"},
	}
}

// Load reads path on top of the defaults. A missing file is not an error.
func Load(path string) (Config, error) {
	cfg := Default()
	if path == "" {
		return cfg, nil
	}
	if _, err := os.Stat(path); os.IsNotExist(err) {
		return cfg, nil
	}
	if _, err := toml.DecodeFile(path, &cfg); err != nil {
		return cfg, fmt.Errorf("config %s: %w", path, err)
	}
	return cfg, cfg.Validate()
}

// Validate checks values that would make the server fail later.
func (c Config) Validate() error {
	if c.Database.Path == "" {
		return fmt.Errorf("database.path is empty")
	}
	if c.Login.Listen == "" {
		return fmt.Errorf("login.listen is empty")
	}
	if c.Game.Listen == "" || c.Game.PublicHost == "" || c.Game.PublicPort == 0 {
		return fmt.Errorf("game.listen, game.public_host and game.public_port are required")
	}
	return nil
}
