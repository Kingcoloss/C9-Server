-- Core schema: accounts, worlds, characters, items, admin audit log.
-- Later systems (guild, post, pet, collection, ...) add their own migrations.

CREATE TABLE accounts (
    id               INTEGER PRIMARY KEY AUTOINCREMENT,
    username         TEXT    NOT NULL UNIQUE COLLATE NOCASE,
    password_hash    TEXT    NOT NULL,              -- bcrypt
    email            TEXT,
    gm_level         INTEGER NOT NULL DEFAULT 0,    -- 0 = player, >0 = in-game GM commands
    is_admin         INTEGER NOT NULL DEFAULT 0,    -- may sign in to the web admin panel
    banned_until     TEXT,                          -- RFC3339, NULL = not banned, '9999-...' = permanent
    ban_reason       TEXT,
    cash             INTEGER NOT NULL DEFAULT 0,    -- in-game shop currency
    second_password  TEXT,                          -- bcrypt of the 2nd password (C9Network2ndPassword)
    created_at       TEXT    NOT NULL,
    last_login_at    TEXT,
    last_login_ip    TEXT
);

-- A world is one entry of the server list shown after login.
CREATE TABLE worlds (
    id           INTEGER PRIMARY KEY,
    name         TEXT    NOT NULL,
    host         TEXT    NOT NULL,
    port         INTEGER NOT NULL,
    status       INTEGER NOT NULL DEFAULT 0,        -- 0 = open, 1 = maintenance, 2 = hidden
    max_players  INTEGER NOT NULL DEFAULT 500,
    sort_order   INTEGER NOT NULL DEFAULT 0
);

CREATE TABLE channels (
    world_id     INTEGER NOT NULL REFERENCES worlds(id) ON DELETE CASCADE,
    id           INTEGER NOT NULL,
    name         TEXT    NOT NULL,
    max_players  INTEGER NOT NULL DEFAULT 200,
    PRIMARY KEY (world_id, id)
);

CREATE TABLE characters (
    id               INTEGER PRIMARY KEY AUTOINCREMENT,
    account_id       INTEGER NOT NULL REFERENCES accounts(id) ON DELETE CASCADE,
    world_id         INTEGER NOT NULL,
    slot             INTEGER NOT NULL,
    name             TEXT    NOT NULL UNIQUE COLLATE NOCASE,
    class_id         INTEGER NOT NULL,
    level            INTEGER NOT NULL DEFAULT 1,
    exp              INTEGER NOT NULL DEFAULT 0,
    gold             INTEGER NOT NULL DEFAULT 0,
    hp               INTEGER NOT NULL DEFAULT 0,
    sp               INTEGER NOT NULL DEFAULT 0,
    fatigue          INTEGER NOT NULL DEFAULT 0,
    map_id           INTEGER NOT NULL DEFAULT 0,
    pos_x            REAL    NOT NULL DEFAULT 0,
    pos_y            REAL    NOT NULL DEFAULT 0,
    pos_z            REAL    NOT NULL DEFAULT 0,
    direction        REAL    NOT NULL DEFAULT 0,
    appearance       BLOB,                          -- raw appearance block as the client sends it
    play_seconds     INTEGER NOT NULL DEFAULT 0,
    created_at       TEXT    NOT NULL,
    last_played_at   TEXT,
    deleted_at       TEXT                           -- soft delete; slot is freed when set
);
CREATE INDEX idx_characters_account ON characters(account_id, world_id) WHERE deleted_at IS NULL;

-- Every item instance a character owns, wherever it is.
CREATE TABLE items (
    id             INTEGER PRIMARY KEY AUTOINCREMENT,
    character_id   INTEGER REFERENCES characters(id) ON DELETE CASCADE,
    account_id     INTEGER REFERENCES accounts(id) ON DELETE CASCADE, -- account-wide storage
    container      INTEGER NOT NULL,                -- see internal/game/item.go Container*
    slot           INTEGER NOT NULL,
    item_code      INTEGER NOT NULL,                -- item id from the client data tables
    count          INTEGER NOT NULL DEFAULT 1,
    durability     INTEGER NOT NULL DEFAULT 0,
    enchant        INTEGER NOT NULL DEFAULT 0,
    options        TEXT,                            -- JSON: random options, sockets, ...
    bound          INTEGER NOT NULL DEFAULT 0,
    expires_at     TEXT,
    created_at     TEXT    NOT NULL
);
CREATE INDEX idx_items_owner ON items(character_id, container, slot);

CREATE TABLE skills (
    character_id  INTEGER NOT NULL REFERENCES characters(id) ON DELETE CASCADE,
    skill_id      INTEGER NOT NULL,
    level         INTEGER NOT NULL DEFAULT 1,
    PRIMARY KEY (character_id, skill_id)
);

CREATE TABLE quick_slots (
    character_id  INTEGER NOT NULL REFERENCES characters(id) ON DELETE CASCADE,
    slot          INTEGER NOT NULL,
    kind          INTEGER NOT NULL,                 -- skill / item / action
    ref_id        INTEGER NOT NULL,
    PRIMARY KEY (character_id, slot)
);

CREATE TABLE quests (
    character_id  INTEGER NOT NULL REFERENCES characters(id) ON DELETE CASCADE,
    quest_id      INTEGER NOT NULL,
    state         INTEGER NOT NULL DEFAULT 0,       -- 0 = in progress, 1 = completed
    progress      TEXT,                             -- JSON counters
    updated_at    TEXT    NOT NULL,
    PRIMARY KEY (character_id, quest_id)
);

-- Things admins did in the panel or with in-game GM commands.
CREATE TABLE audit_log (
    id          INTEGER PRIMARY KEY AUTOINCREMENT,
    actor_id    INTEGER,                            -- accounts.id of the admin, NULL = system
    action      TEXT    NOT NULL,
    target      TEXT,
    detail      TEXT,
    created_at  TEXT    NOT NULL
);

CREATE TABLE login_history (
    id          INTEGER PRIMARY KEY AUTOINCREMENT,
    account_id  INTEGER,
    username    TEXT    NOT NULL,
    ip          TEXT,
    success     INTEGER NOT NULL,
    reason      TEXT,
    created_at  TEXT    NOT NULL
);
CREATE INDEX idx_login_history_account ON login_history(account_id, created_at);

-- Scheduled or one-off notices pushed to players.
CREATE TABLE notices (
    id          INTEGER PRIMARY KEY AUTOINCREMENT,
    message     TEXT    NOT NULL,
    created_by  INTEGER,
    created_at  TEXT    NOT NULL
);

CREATE TABLE settings (
    key    TEXT PRIMARY KEY,
    value  TEXT NOT NULL
);

INSERT INTO worlds (id, name, host, port, status, max_players, sort_order)
VALUES (1, 'C9 Private', '127.0.0.1', 10722, 0, 500, 0);
INSERT INTO channels (world_id, id, name) VALUES (1, 1, 'CH 1');
