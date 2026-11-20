using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace C9.GameData.Migrations
{
    /// <inheritdoc />
    public partial class ItemCatalogProperties : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "category",
                schema: "game",
                table: "item_catalog",
                type: "character varying(32)",
                maxLength: 32,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "admin_only",
                schema: "game",
                table: "item_catalog",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<List<string>>(
                name: "tags",
                schema: "game",
                table: "item_catalog",
                type: "text[]",
                nullable: false);

            migrationBuilder.AddColumn<short>(
                name: "use_level",
                schema: "game",
                table: "item_catalog",
                type: "smallint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "ix_item_catalog_category",
                schema: "game",
                table: "item_catalog",
                column: "category");

            migrationBuilder.CreateIndex(
                name: "ix_item_catalog_rarity",
                schema: "game",
                table: "item_catalog",
                column: "rarity");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_item_catalog_category",
                schema: "game",
                table: "item_catalog");

            migrationBuilder.DropIndex(
                name: "ix_item_catalog_rarity",
                schema: "game",
                table: "item_catalog");

            migrationBuilder.DropColumn(
                name: "admin_only",
                schema: "game",
                table: "item_catalog");

            migrationBuilder.DropColumn(
                name: "tags",
                schema: "game",
                table: "item_catalog");

            migrationBuilder.DropColumn(
                name: "use_level",
                schema: "game",
                table: "item_catalog");

            migrationBuilder.AlterColumn<string>(
                name: "category",
                schema: "game",
                table: "item_catalog",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(32)",
                oldMaxLength: 32,
                oldNullable: true);
        }
    }
}
