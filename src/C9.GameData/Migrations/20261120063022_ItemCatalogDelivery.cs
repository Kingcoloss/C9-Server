using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace C9.GameData.Migrations
{
    /// <inheritdoc />
    public partial class ItemCatalogDelivery : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "name_ko",
                schema: "game",
                table: "item_catalog",
                type: "character varying(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "name_en",
                schema: "game",
                table: "item_catalog",
                type: "character varying(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "description_en",
                schema: "game",
                table: "item_catalog",
                type: "character varying(4000)",
                maxLength: 4000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "icon",
                schema: "game",
                table: "item_catalog",
                type: "character varying(128)",
                maxLength: 128,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "item_bundles",
                schema: "game",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    enabled = table.Column<bool>(type: "boolean", nullable: false),
                    created_by = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_item_bundles", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "item_bundle_entries",
                schema: "game",
                columns: table => new
                {
                    bundle_id = table.Column<long>(type: "bigint", nullable: false),
                    line = table.Column<short>(type: "smallint", nullable: false),
                    item_id = table.Column<int>(type: "integer", nullable: false),
                    quantity = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_item_bundle_entries", x => new { x.bundle_id, x.line });
                    table.CheckConstraint("ck_item_bundle_entries_quantity", "quantity > 0");
                    table.ForeignKey(
                        name: "fk_item_bundle_entries_item_bundles_bundle_id",
                        column: x => x.bundle_id,
                        principalSchema: "game",
                        principalTable: "item_bundles",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_item_bundle_entries_item_catalog_item_id",
                        column: x => x.item_id,
                        principalSchema: "game",
                        principalTable: "item_catalog",
                        principalColumn: "item_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "item_deliveries",
                schema: "game",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    batch_id = table.Column<Guid>(type: "uuid", nullable: false),
                    target = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    account_id = table.Column<long>(type: "bigint", nullable: false),
                    character_id = table.Column<long>(type: "bigint", nullable: true),
                    item_id = table.Column<int>(type: "integer", nullable: false),
                    quantity = table.Column<int>(type: "integer", nullable: false),
                    bundle_id = table.Column<long>(type: "bigint", nullable: true),
                    message = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    status = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    created_by = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    reason = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    delivered_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    cancelled_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    cancelled_by = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_item_deliveries", x => x.id);
                    table.CheckConstraint("ck_item_deliveries_quantity", "quantity > 0");
                    table.CheckConstraint("ck_item_deliveries_target", "(target = 'Character') = (character_id IS NOT NULL)");
                    table.ForeignKey(
                        name: "fk_item_deliveries_accounts_account_id",
                        column: x => x.account_id,
                        principalSchema: "game",
                        principalTable: "accounts",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_item_deliveries_characters_character_id",
                        column: x => x.character_id,
                        principalSchema: "game",
                        principalTable: "characters",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_item_deliveries_item_bundles_bundle_id",
                        column: x => x.bundle_id,
                        principalSchema: "game",
                        principalTable: "item_bundles",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_item_deliveries_item_catalog_item_id",
                        column: x => x.item_id,
                        principalSchema: "game",
                        principalTable: "item_catalog",
                        principalColumn: "item_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "ix_item_bundle_entries_item_id",
                schema: "game",
                table: "item_bundle_entries",
                column: "item_id");

            migrationBuilder.CreateIndex(
                name: "ix_item_bundles_name",
                schema: "game",
                table: "item_bundles",
                column: "name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_item_deliveries_account_id_status",
                schema: "game",
                table: "item_deliveries",
                columns: new[] { "account_id", "status" });

            migrationBuilder.CreateIndex(
                name: "ix_item_deliveries_batch_id",
                schema: "game",
                table: "item_deliveries",
                column: "batch_id");

            migrationBuilder.CreateIndex(
                name: "ix_item_deliveries_bundle_id",
                schema: "game",
                table: "item_deliveries",
                column: "bundle_id");

            migrationBuilder.CreateIndex(
                name: "ix_item_deliveries_character_id_status",
                schema: "game",
                table: "item_deliveries",
                columns: new[] { "character_id", "status" });

            migrationBuilder.CreateIndex(
                name: "ix_item_deliveries_created_at",
                schema: "game",
                table: "item_deliveries",
                column: "created_at");

            migrationBuilder.CreateIndex(
                name: "ix_item_deliveries_item_id",
                schema: "game",
                table: "item_deliveries",
                column: "item_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "item_bundle_entries",
                schema: "game");

            migrationBuilder.DropTable(
                name: "item_deliveries",
                schema: "game");

            migrationBuilder.DropTable(
                name: "item_bundles",
                schema: "game");

            migrationBuilder.DropColumn(
                name: "description_en",
                schema: "game",
                table: "item_catalog");

            migrationBuilder.DropColumn(
                name: "icon",
                schema: "game",
                table: "item_catalog");

            migrationBuilder.AlterColumn<string>(
                name: "name_ko",
                schema: "game",
                table: "item_catalog",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(200)",
                oldMaxLength: 200,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "name_en",
                schema: "game",
                table: "item_catalog",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(200)",
                oldMaxLength: 200,
                oldNullable: true);
        }
    }
}
