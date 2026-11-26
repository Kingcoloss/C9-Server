using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace C9.GameData.Migrations
{
    /// <inheritdoc />
    public partial class GiftBoxInventory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_character_items_character_id_location_slot",
                schema: "game",
                table: "character_items");

            migrationBuilder.CreateSequence(
                name: "item_serial_seq",
                schema: "game");

            migrationBuilder.AddColumn<long>(
                name: "claimed_character_id",
                schema: "game",
                table: "item_deliveries",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "item_serial",
                schema: "game",
                table: "item_deliveries",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "opened_at",
                schema: "game",
                table: "item_deliveries",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "starter",
                schema: "game",
                table: "item_bundles",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<short>(
                name: "bag",
                schema: "game",
                table: "character_items",
                type: "smallint",
                nullable: false,
                defaultValue: (short)0);

            migrationBuilder.AddColumn<long>(
                name: "game_serial",
                schema: "game",
                table: "character_items",
                type: "bigint",
                nullable: false,
                defaultValueSql: "nextval('game.item_serial_seq')");

            migrationBuilder.CreateIndex(
                name: "ix_item_deliveries_claimed_character_id",
                schema: "game",
                table: "item_deliveries",
                column: "claimed_character_id");

            migrationBuilder.CreateIndex(
                name: "ix_item_bundles_starter",
                schema: "game",
                table: "item_bundles",
                column: "starter",
                unique: true,
                filter: "starter");

            migrationBuilder.CreateIndex(
                name: "ix_character_items_character_id_location_bag_slot",
                schema: "game",
                table: "character_items",
                columns: new[] { "character_id", "location", "bag", "slot" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_character_items_game_serial",
                schema: "game",
                table: "character_items",
                column: "game_serial",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "fk_item_deliveries_characters_claimed_character_id",
                schema: "game",
                table: "item_deliveries",
                column: "claimed_character_id",
                principalSchema: "game",
                principalTable: "characters",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_item_deliveries_characters_claimed_character_id",
                schema: "game",
                table: "item_deliveries");

            migrationBuilder.DropIndex(
                name: "ix_item_deliveries_claimed_character_id",
                schema: "game",
                table: "item_deliveries");

            migrationBuilder.DropIndex(
                name: "ix_item_bundles_starter",
                schema: "game",
                table: "item_bundles");

            migrationBuilder.DropIndex(
                name: "ix_character_items_character_id_location_bag_slot",
                schema: "game",
                table: "character_items");

            migrationBuilder.DropIndex(
                name: "ix_character_items_game_serial",
                schema: "game",
                table: "character_items");

            migrationBuilder.DropColumn(
                name: "claimed_character_id",
                schema: "game",
                table: "item_deliveries");

            migrationBuilder.DropColumn(
                name: "item_serial",
                schema: "game",
                table: "item_deliveries");

            migrationBuilder.DropColumn(
                name: "opened_at",
                schema: "game",
                table: "item_deliveries");

            migrationBuilder.DropColumn(
                name: "starter",
                schema: "game",
                table: "item_bundles");

            migrationBuilder.DropColumn(
                name: "bag",
                schema: "game",
                table: "character_items");

            migrationBuilder.DropColumn(
                name: "game_serial",
                schema: "game",
                table: "character_items");

            migrationBuilder.DropSequence(
                name: "item_serial_seq",
                schema: "game");

            migrationBuilder.CreateIndex(
                name: "ix_character_items_character_id_location_slot",
                schema: "game",
                table: "character_items",
                columns: new[] { "character_id", "location", "slot" },
                unique: true);
        }
    }
}
