using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace C9.GameData.Migrations
{
    /// <inheritdoc />
    public partial class StatClientClassBase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Client class table values replace the placeholder 10 only where an operator has not changed it.
            migrationBuilder.Sql(@"
UPDATE game.class_stat_bases AS b SET value = v.value
FROM (VALUES
    (0, 'strength', 10), (0, 'intelligence', 5), (0, 'health', 9), (0, 'wisdom', 1),
    (1, 'strength', 6), (1, 'intelligence', 5), (1, 'health', 5), (1, 'wisdom', 9),
    (2, 'strength', 5), (2, 'intelligence', 1), (2, 'health', 4), (2, 'wisdom', 2),
    (3, 'strength', 8), (3, 'intelligence', 7), (3, 'health', 7), (3, 'wisdom', 3),
    (4, 'strength', 8), (4, 'intelligence', 4), (4, 'health', 5), (4, 'wisdom', 8)
) AS v(class_code, stat_key, value)
WHERE b.class_code = v.class_code AND b.stat_key = v.stat_key AND b.value = 10;");

            migrationBuilder.InsertData(
                schema: "game",
                table: "stat_definitions",
                columns: new[] { "key", "name", "per_point", "sort", "upgradable" },
                values: new object[,]
                {
                    { "hp_recovery", "HP Recovery", 1, (short)31, true },
                    { "mp_recovery", "MP Recovery", 1, (short)32, true },
                    { "valiance", "Valiance", 1, (short)30, true }
                });

            migrationBuilder.InsertData(
                schema: "game",
                table: "class_stat_bases",
                columns: new[] { "class_code", "stat_key", "value" },
                values: new object[,]
                {
                    { (short)0, "hp_recovery", 0 },
                    { (short)0, "mp_recovery", 0 },
                    { (short)0, "valiance", 0 },
                    { (short)1, "hp_recovery", 0 },
                    { (short)1, "mp_recovery", 0 },
                    { (short)1, "valiance", 0 },
                    { (short)2, "hp_recovery", 0 },
                    { (short)2, "mp_recovery", 0 },
                    { (short)2, "valiance", 0 },
                    { (short)3, "hp_recovery", 0 },
                    { (short)3, "mp_recovery", 0 },
                    { (short)3, "valiance", 0 },
                    { (short)4, "hp_recovery", 0 },
                    { (short)4, "mp_recovery", 0 },
                    { (short)4, "valiance", 0 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                schema: "game",
                table: "class_stat_bases",
                keyColumns: new[] { "class_code", "stat_key" },
                keyValues: new object[] { (short)0, "hp_recovery" });

            migrationBuilder.DeleteData(
                schema: "game",
                table: "class_stat_bases",
                keyColumns: new[] { "class_code", "stat_key" },
                keyValues: new object[] { (short)0, "mp_recovery" });

            migrationBuilder.DeleteData(
                schema: "game",
                table: "class_stat_bases",
                keyColumns: new[] { "class_code", "stat_key" },
                keyValues: new object[] { (short)0, "valiance" });

            migrationBuilder.DeleteData(
                schema: "game",
                table: "class_stat_bases",
                keyColumns: new[] { "class_code", "stat_key" },
                keyValues: new object[] { (short)1, "hp_recovery" });

            migrationBuilder.DeleteData(
                schema: "game",
                table: "class_stat_bases",
                keyColumns: new[] { "class_code", "stat_key" },
                keyValues: new object[] { (short)1, "mp_recovery" });

            migrationBuilder.DeleteData(
                schema: "game",
                table: "class_stat_bases",
                keyColumns: new[] { "class_code", "stat_key" },
                keyValues: new object[] { (short)1, "valiance" });

            migrationBuilder.DeleteData(
                schema: "game",
                table: "class_stat_bases",
                keyColumns: new[] { "class_code", "stat_key" },
                keyValues: new object[] { (short)2, "hp_recovery" });

            migrationBuilder.DeleteData(
                schema: "game",
                table: "class_stat_bases",
                keyColumns: new[] { "class_code", "stat_key" },
                keyValues: new object[] { (short)2, "mp_recovery" });

            migrationBuilder.DeleteData(
                schema: "game",
                table: "class_stat_bases",
                keyColumns: new[] { "class_code", "stat_key" },
                keyValues: new object[] { (short)2, "valiance" });

            migrationBuilder.DeleteData(
                schema: "game",
                table: "class_stat_bases",
                keyColumns: new[] { "class_code", "stat_key" },
                keyValues: new object[] { (short)3, "hp_recovery" });

            migrationBuilder.DeleteData(
                schema: "game",
                table: "class_stat_bases",
                keyColumns: new[] { "class_code", "stat_key" },
                keyValues: new object[] { (short)3, "mp_recovery" });

            migrationBuilder.DeleteData(
                schema: "game",
                table: "class_stat_bases",
                keyColumns: new[] { "class_code", "stat_key" },
                keyValues: new object[] { (short)3, "valiance" });

            migrationBuilder.DeleteData(
                schema: "game",
                table: "class_stat_bases",
                keyColumns: new[] { "class_code", "stat_key" },
                keyValues: new object[] { (short)4, "hp_recovery" });

            migrationBuilder.DeleteData(
                schema: "game",
                table: "class_stat_bases",
                keyColumns: new[] { "class_code", "stat_key" },
                keyValues: new object[] { (short)4, "mp_recovery" });

            migrationBuilder.DeleteData(
                schema: "game",
                table: "class_stat_bases",
                keyColumns: new[] { "class_code", "stat_key" },
                keyValues: new object[] { (short)4, "valiance" });

            migrationBuilder.DeleteData(
                schema: "game",
                table: "stat_definitions",
                keyColumn: "key",
                keyValue: "hp_recovery");

            migrationBuilder.DeleteData(
                schema: "game",
                table: "stat_definitions",
                keyColumn: "key",
                keyValue: "mp_recovery");

            migrationBuilder.DeleteData(
                schema: "game",
                table: "stat_definitions",
                keyColumn: "key",
                keyValue: "valiance");

            migrationBuilder.UpdateData(
                schema: "game",
                table: "class_stat_bases",
                keyColumns: new[] { "class_code", "stat_key" },
                keyValues: new object[] { (short)0, "health" },
                column: "value",
                value: 10);

            migrationBuilder.UpdateData(
                schema: "game",
                table: "class_stat_bases",
                keyColumns: new[] { "class_code", "stat_key" },
                keyValues: new object[] { (short)0, "intelligence" },
                column: "value",
                value: 10);

            migrationBuilder.UpdateData(
                schema: "game",
                table: "class_stat_bases",
                keyColumns: new[] { "class_code", "stat_key" },
                keyValues: new object[] { (short)0, "wisdom" },
                column: "value",
                value: 10);

            migrationBuilder.UpdateData(
                schema: "game",
                table: "class_stat_bases",
                keyColumns: new[] { "class_code", "stat_key" },
                keyValues: new object[] { (short)1, "health" },
                column: "value",
                value: 10);

            migrationBuilder.UpdateData(
                schema: "game",
                table: "class_stat_bases",
                keyColumns: new[] { "class_code", "stat_key" },
                keyValues: new object[] { (short)1, "intelligence" },
                column: "value",
                value: 10);

            migrationBuilder.UpdateData(
                schema: "game",
                table: "class_stat_bases",
                keyColumns: new[] { "class_code", "stat_key" },
                keyValues: new object[] { (short)1, "strength" },
                column: "value",
                value: 10);

            migrationBuilder.UpdateData(
                schema: "game",
                table: "class_stat_bases",
                keyColumns: new[] { "class_code", "stat_key" },
                keyValues: new object[] { (short)1, "wisdom" },
                column: "value",
                value: 10);

            migrationBuilder.UpdateData(
                schema: "game",
                table: "class_stat_bases",
                keyColumns: new[] { "class_code", "stat_key" },
                keyValues: new object[] { (short)2, "health" },
                column: "value",
                value: 10);

            migrationBuilder.UpdateData(
                schema: "game",
                table: "class_stat_bases",
                keyColumns: new[] { "class_code", "stat_key" },
                keyValues: new object[] { (short)2, "intelligence" },
                column: "value",
                value: 10);

            migrationBuilder.UpdateData(
                schema: "game",
                table: "class_stat_bases",
                keyColumns: new[] { "class_code", "stat_key" },
                keyValues: new object[] { (short)2, "strength" },
                column: "value",
                value: 10);

            migrationBuilder.UpdateData(
                schema: "game",
                table: "class_stat_bases",
                keyColumns: new[] { "class_code", "stat_key" },
                keyValues: new object[] { (short)2, "wisdom" },
                column: "value",
                value: 10);

            migrationBuilder.UpdateData(
                schema: "game",
                table: "class_stat_bases",
                keyColumns: new[] { "class_code", "stat_key" },
                keyValues: new object[] { (short)3, "health" },
                column: "value",
                value: 10);

            migrationBuilder.UpdateData(
                schema: "game",
                table: "class_stat_bases",
                keyColumns: new[] { "class_code", "stat_key" },
                keyValues: new object[] { (short)3, "intelligence" },
                column: "value",
                value: 10);

            migrationBuilder.UpdateData(
                schema: "game",
                table: "class_stat_bases",
                keyColumns: new[] { "class_code", "stat_key" },
                keyValues: new object[] { (short)3, "strength" },
                column: "value",
                value: 10);

            migrationBuilder.UpdateData(
                schema: "game",
                table: "class_stat_bases",
                keyColumns: new[] { "class_code", "stat_key" },
                keyValues: new object[] { (short)3, "wisdom" },
                column: "value",
                value: 10);

            migrationBuilder.UpdateData(
                schema: "game",
                table: "class_stat_bases",
                keyColumns: new[] { "class_code", "stat_key" },
                keyValues: new object[] { (short)4, "health" },
                column: "value",
                value: 10);

            migrationBuilder.UpdateData(
                schema: "game",
                table: "class_stat_bases",
                keyColumns: new[] { "class_code", "stat_key" },
                keyValues: new object[] { (short)4, "intelligence" },
                column: "value",
                value: 10);

            migrationBuilder.UpdateData(
                schema: "game",
                table: "class_stat_bases",
                keyColumns: new[] { "class_code", "stat_key" },
                keyValues: new object[] { (short)4, "strength" },
                column: "value",
                value: 10);

            migrationBuilder.UpdateData(
                schema: "game",
                table: "class_stat_bases",
                keyColumns: new[] { "class_code", "stat_key" },
                keyValues: new object[] { (short)4, "wisdom" },
                column: "value",
                value: 10);
        }
    }
}
