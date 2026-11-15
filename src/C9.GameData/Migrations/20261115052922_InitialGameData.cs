using System;
using System.Collections.Generic;
using System.Net;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace C9.GameData.Migrations
{
    /// <inheritdoc />
    public partial class InitialGameData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "game");

            migrationBuilder.EnsureSchema(
                name: "ops");

            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:PostgresExtension:citext", ",,");

            migrationBuilder.CreateTable(
                name: "accounts",
                schema: "game",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    login_name = table.Column<string>(type: "citext", maxLength: 32, nullable: false),
                    password_hash = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    status = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    slot_count = table.Column<short>(type: "smallint", nullable: false, defaultValue: (short)17),
                    is_gm = table.Column<bool>(type: "boolean", nullable: false),
                    note = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    tags = table.Column<List<string>>(type: "text[]", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    last_login_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_accounts", x => x.id);
                    table.CheckConstraint("ck_accounts_slot_count", "slot_count BETWEEN 1 AND 17");
                });

            migrationBuilder.CreateTable(
                name: "admin_commands",
                schema: "ops",
                columns: table => new
                {
                    request_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    @operator = table.Column<string>(name: "operator", type: "character varying(64)", maxLength: 64, nullable: false),
                    kind = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    character_id = table.Column<long>(type: "bigint", nullable: true),
                    payload = table.Column<string>(type: "jsonb", nullable: false),
                    status = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    applied_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    result = table.Column<string>(type: "jsonb", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_admin_commands", x => x.request_id);
                });

            migrationBuilder.CreateTable(
                name: "announcements",
                schema: "game",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    channel = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    message = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    starts_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    ends_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    repeat_minutes = table.Column<int>(type: "integer", nullable: true),
                    created_by = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_announcements", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "audit_log",
                schema: "ops",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    @operator = table.Column<string>(name: "operator", type: "character varying(64)", maxLength: 64, nullable: false),
                    action = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    target_type = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: true),
                    target_id = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    before = table.Column<string>(type: "jsonb", nullable: true),
                    after = table.Column<string>(type: "jsonb", nullable: true),
                    reason = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    request_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_audit_log", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "character_snapshots",
                schema: "ops",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    character_id = table.Column<long>(type: "bigint", nullable: false),
                    taken_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    cause = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    data = table.Column<string>(type: "jsonb", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_character_snapshots", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "item_catalog",
                schema: "game",
                columns: table => new
                {
                    item_id = table.Column<int>(type: "integer", nullable: false),
                    name_en = table.Column<string>(type: "text", nullable: true),
                    name_ko = table.Column<string>(type: "text", nullable: true),
                    category = table.Column<string>(type: "text", nullable: true),
                    rarity = table.Column<short>(type: "smallint", nullable: true),
                    max_stack = table.Column<int>(type: "integer", nullable: true),
                    raw = table.Column<string>(type: "jsonb", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_item_catalog", x => x.item_id);
                });

            migrationBuilder.CreateTable(
                name: "ledger",
                schema: "game",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    character_id = table.Column<long>(type: "bigint", nullable: false),
                    asset = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    item_id = table.Column<int>(type: "integer", nullable: true),
                    item_serial = table.Column<Guid>(type: "uuid", nullable: true),
                    delta = table.Column<long>(type: "bigint", nullable: false),
                    balance_after = table.Column<long>(type: "bigint", nullable: true),
                    reason = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    @ref = table.Column<string>(name: "ref", type: "character varying(200)", maxLength: 200, nullable: true),
                    map_id = table.Column<int>(type: "integer", nullable: true),
                    request_id = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_ledger", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "rate_policy",
                schema: "game",
                columns: table => new
                {
                    scope = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    cap = table.Column<decimal>(type: "numeric(12,4)", precision: 12, scale: 4, nullable: false),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false),
                    rate_drop = table.Column<decimal>(type: "numeric(12,4)", precision: 12, scale: 4, nullable: false),
                    rate_exp = table.Column<decimal>(type: "numeric(12,4)", precision: 12, scale: 4, nullable: false),
                    rate_rare_drop = table.Column<decimal>(type: "numeric(12,4)", precision: 12, scale: 4, nullable: false),
                    rate_upgrade = table.Column<decimal>(type: "numeric(12,4)", precision: 12, scale: 4, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_rate_policy", x => x.scope);
                });

            migrationBuilder.CreateTable(
                name: "sessions",
                schema: "ops",
                columns: table => new
                {
                    key = table.Column<string>(type: "character varying(96)", maxLength: 96, nullable: false),
                    service = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    account_id = table.Column<long>(type: "bigint", nullable: true),
                    character_id = table.Column<long>(type: "bigint", nullable: true),
                    remote_address = table.Column<IPAddress>(type: "inet", nullable: true),
                    client_hash = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    started_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    ended_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    end_reason = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    last_state = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: true),
                    map_id = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_sessions", x => x.key);
                });

            migrationBuilder.CreateTable(
                name: "world_events",
                schema: "game",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    description = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    starts_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    ends_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    repeat_every_days = table.Column<int>(type: "integer", nullable: true),
                    recur_until = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    extra = table.Column<string>(type: "jsonb", nullable: false),
                    enabled = table.Column<bool>(type: "boolean", nullable: false),
                    created_by = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false),
                    rate_drop = table.Column<decimal>(type: "numeric(12,4)", precision: 12, scale: 4, nullable: false),
                    rate_exp = table.Column<decimal>(type: "numeric(12,4)", precision: 12, scale: 4, nullable: false),
                    rate_rare_drop = table.Column<decimal>(type: "numeric(12,4)", precision: 12, scale: 4, nullable: false),
                    rate_upgrade = table.Column<decimal>(type: "numeric(12,4)", precision: 12, scale: 4, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_world_events", x => x.id);
                    table.CheckConstraint("ck_world_events_repeat", "repeat_every_days IS NULL OR repeat_every_days > 0");
                    table.CheckConstraint("ck_world_events_window", "ends_at > starts_at");
                });

            migrationBuilder.CreateTable(
                name: "world_settings",
                schema: "game",
                columns: table => new
                {
                    key = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    value = table.Column<string>(type: "jsonb", nullable: false),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_world_settings", x => x.key);
                });

            migrationBuilder.CreateTable(
                name: "account_rate_overrides",
                schema: "game",
                columns: table => new
                {
                    account_id = table.Column<long>(type: "bigint", nullable: false),
                    expires_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    note = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false),
                    rate_drop = table.Column<decimal>(type: "numeric(12,4)", precision: 12, scale: 4, nullable: false),
                    rate_exp = table.Column<decimal>(type: "numeric(12,4)", precision: 12, scale: 4, nullable: false),
                    rate_rare_drop = table.Column<decimal>(type: "numeric(12,4)", precision: 12, scale: 4, nullable: false),
                    rate_upgrade = table.Column<decimal>(type: "numeric(12,4)", precision: 12, scale: 4, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_account_rate_overrides", x => x.account_id);
                    table.ForeignKey(
                        name: "fk_account_rate_overrides_accounts_account_id",
                        column: x => x.account_id,
                        principalSchema: "game",
                        principalTable: "accounts",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "account_sanctions",
                schema: "game",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    account_id = table.Column<long>(type: "bigint", nullable: false),
                    kind = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    reason = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    starts_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    ends_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    issued_by = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    revoked_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    revoked_by = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_account_sanctions", x => x.id);
                    table.ForeignKey(
                        name: "fk_account_sanctions_accounts_account_id",
                        column: x => x.account_id,
                        principalSchema: "game",
                        principalTable: "accounts",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "characters",
                schema: "game",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    account_id = table.Column<long>(type: "bigint", nullable: false),
                    slot = table.Column<short>(type: "smallint", nullable: false),
                    name = table.Column<string>(type: "citext", maxLength: 20, nullable: false),
                    job = table.Column<short>(type: "smallint", nullable: false),
                    appearance = table.Column<byte[]>(type: "bytea", nullable: false),
                    level = table.Column<short>(type: "smallint", nullable: false, defaultValue: (short)1),
                    exp = table.Column<long>(type: "bigint", nullable: false),
                    gold = table.Column<long>(type: "bigint", nullable: false),
                    hp = table.Column<int>(type: "integer", nullable: true),
                    mp = table.Column<int>(type: "integer", nullable: true),
                    map_id = table.Column<int>(type: "integer", nullable: true),
                    submap = table.Column<short>(type: "smallint", nullable: true),
                    pos_x = table.Column<float>(type: "real", nullable: true),
                    pos_y = table.Column<float>(type: "real", nullable: true),
                    pos_z = table.Column<float>(type: "real", nullable: true),
                    play_seconds = table.Column<long>(type: "bigint", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    last_played_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    deleted_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_characters", x => x.id);
                    table.CheckConstraint("ck_characters_appearance", "length(appearance) = 44");
                    table.CheckConstraint("ck_characters_gold", "gold >= 0");
                    table.CheckConstraint("ck_characters_level", "level >= 1");
                    table.CheckConstraint("ck_characters_slot", "slot BETWEEN 0 AND 16");
                    table.ForeignKey(
                        name: "fk_characters_accounts_account_id",
                        column: x => x.account_id,
                        principalSchema: "game",
                        principalTable: "accounts",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "character_items",
                schema: "game",
                columns: table => new
                {
                    serial = table.Column<Guid>(type: "uuid", nullable: false),
                    character_id = table.Column<long>(type: "bigint", nullable: false),
                    location = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    slot = table.Column<short>(type: "smallint", nullable: false),
                    item_id = table.Column<int>(type: "integer", nullable: false),
                    quantity = table.Column<int>(type: "integer", nullable: false),
                    enchant = table.Column<short>(type: "smallint", nullable: false),
                    options = table.Column<string>(type: "jsonb", nullable: false),
                    bound = table.Column<bool>(type: "boolean", nullable: false),
                    expires_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_character_items", x => x.serial);
                    table.CheckConstraint("ck_character_items_quantity", "quantity > 0");
                    table.ForeignKey(
                        name: "fk_character_items_characters_character_id",
                        column: x => x.character_id,
                        principalSchema: "game",
                        principalTable: "characters",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "ix_account_sanctions_account_id_starts_at",
                schema: "game",
                table: "account_sanctions",
                columns: new[] { "account_id", "starts_at" });

            migrationBuilder.CreateIndex(
                name: "ix_accounts_login_name",
                schema: "game",
                table: "accounts",
                column: "login_name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_admin_commands_status_created_at",
                schema: "ops",
                table: "admin_commands",
                columns: new[] { "status", "created_at" });

            migrationBuilder.CreateIndex(
                name: "ix_audit_log_operator_at",
                schema: "ops",
                table: "audit_log",
                columns: new[] { "operator", "at" });

            migrationBuilder.CreateIndex(
                name: "ix_audit_log_request_id",
                schema: "ops",
                table: "audit_log",
                column: "request_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_audit_log_target_type_target_id_at",
                schema: "ops",
                table: "audit_log",
                columns: new[] { "target_type", "target_id", "at" });

            migrationBuilder.CreateIndex(
                name: "ix_character_items_character_id_location_slot",
                schema: "game",
                table: "character_items",
                columns: new[] { "character_id", "location", "slot" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_character_items_item_id",
                schema: "game",
                table: "character_items",
                column: "item_id");

            migrationBuilder.CreateIndex(
                name: "ix_character_snapshots_character_id_taken_at",
                schema: "ops",
                table: "character_snapshots",
                columns: new[] { "character_id", "taken_at" });

            migrationBuilder.CreateIndex(
                name: "ix_characters_account_id_slot",
                schema: "game",
                table: "characters",
                columns: new[] { "account_id", "slot" },
                unique: true,
                filter: "deleted_at IS NULL");

            migrationBuilder.CreateIndex(
                name: "ix_characters_name",
                schema: "game",
                table: "characters",
                column: "name",
                unique: true,
                filter: "deleted_at IS NULL");

            migrationBuilder.CreateIndex(
                name: "ix_ledger_at",
                schema: "game",
                table: "ledger",
                column: "at");

            migrationBuilder.CreateIndex(
                name: "ix_ledger_character_id_at",
                schema: "game",
                table: "ledger",
                columns: new[] { "character_id", "at" });

            migrationBuilder.CreateIndex(
                name: "ix_ledger_reason_at",
                schema: "game",
                table: "ledger",
                columns: new[] { "reason", "at" });

            migrationBuilder.CreateIndex(
                name: "ix_ledger_request_id",
                schema: "game",
                table: "ledger",
                column: "request_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_sessions_account_id_started_at",
                schema: "ops",
                table: "sessions",
                columns: new[] { "account_id", "started_at" });

            migrationBuilder.CreateIndex(
                name: "ix_sessions_ended_at",
                schema: "ops",
                table: "sessions",
                column: "ended_at");

            migrationBuilder.CreateIndex(
                name: "ix_world_events_starts_at_ends_at",
                schema: "game",
                table: "world_events",
                columns: new[] { "starts_at", "ends_at" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "account_rate_overrides",
                schema: "game");

            migrationBuilder.DropTable(
                name: "account_sanctions",
                schema: "game");

            migrationBuilder.DropTable(
                name: "admin_commands",
                schema: "ops");

            migrationBuilder.DropTable(
                name: "announcements",
                schema: "game");

            migrationBuilder.DropTable(
                name: "audit_log",
                schema: "ops");

            migrationBuilder.DropTable(
                name: "character_items",
                schema: "game");

            migrationBuilder.DropTable(
                name: "character_snapshots",
                schema: "ops");

            migrationBuilder.DropTable(
                name: "item_catalog",
                schema: "game");

            migrationBuilder.DropTable(
                name: "ledger",
                schema: "game");

            migrationBuilder.DropTable(
                name: "rate_policy",
                schema: "game");

            migrationBuilder.DropTable(
                name: "sessions",
                schema: "ops");

            migrationBuilder.DropTable(
                name: "world_events",
                schema: "game");

            migrationBuilder.DropTable(
                name: "world_settings",
                schema: "game");

            migrationBuilder.DropTable(
                name: "characters",
                schema: "game");

            migrationBuilder.DropTable(
                name: "accounts",
                schema: "game");
        }
    }
}
