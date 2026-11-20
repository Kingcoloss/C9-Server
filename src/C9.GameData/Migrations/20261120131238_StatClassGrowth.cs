using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace C9.GameData.Migrations
{
    /// <inheritdoc />
    public partial class StatClassGrowth : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "per_level",
                schema: "game",
                table: "class_stat_bases",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.UpdateData(
                schema: "game",
                table: "class_stat_bases",
                keyColumns: new[] { "class_code", "stat_key" },
                keyValues: new object[] { (short)0, "accuracy" },
                column: "per_level",
                value: 0);

            migrationBuilder.UpdateData(
                schema: "game",
                table: "class_stat_bases",
                keyColumns: new[] { "class_code", "stat_key" },
                keyValues: new object[] { (short)0, "air_attack" },
                column: "per_level",
                value: 0);

            migrationBuilder.UpdateData(
                schema: "game",
                table: "class_stat_bases",
                keyColumns: new[] { "class_code", "stat_key" },
                keyValues: new object[] { (short)0, "air_resistance" },
                column: "per_level",
                value: 0);

            migrationBuilder.UpdateData(
                schema: "game",
                table: "class_stat_bases",
                keyColumns: new[] { "class_code", "stat_key" },
                keyValues: new object[] { (short)0, "attack_speed" },
                column: "per_level",
                value: 0);

            migrationBuilder.UpdateData(
                schema: "game",
                table: "class_stat_bases",
                keyColumns: new[] { "class_code", "stat_key" },
                keyValues: new object[] { (short)0, "cast_speed" },
                column: "per_level",
                value: 0);

            migrationBuilder.UpdateData(
                schema: "game",
                table: "class_stat_bases",
                keyColumns: new[] { "class_code", "stat_key" },
                keyValues: new object[] { (short)0, "concentration" },
                column: "per_level",
                value: 0);

            migrationBuilder.UpdateData(
                schema: "game",
                table: "class_stat_bases",
                keyColumns: new[] { "class_code", "stat_key" },
                keyValues: new object[] { (short)0, "critical_damage" },
                column: "per_level",
                value: 0);

            migrationBuilder.UpdateData(
                schema: "game",
                table: "class_stat_bases",
                keyColumns: new[] { "class_code", "stat_key" },
                keyValues: new object[] { (short)0, "critical_rate" },
                column: "per_level",
                value: 0);

            migrationBuilder.UpdateData(
                schema: "game",
                table: "class_stat_bases",
                keyColumns: new[] { "class_code", "stat_key" },
                keyValues: new object[] { (short)0, "critical_resistance" },
                column: "per_level",
                value: 0);

            migrationBuilder.UpdateData(
                schema: "game",
                table: "class_stat_bases",
                keyColumns: new[] { "class_code", "stat_key" },
                keyValues: new object[] { (short)0, "earth_attack" },
                column: "per_level",
                value: 0);

            migrationBuilder.UpdateData(
                schema: "game",
                table: "class_stat_bases",
                keyColumns: new[] { "class_code", "stat_key" },
                keyValues: new object[] { (short)0, "earth_resistance" },
                column: "per_level",
                value: 0);

            migrationBuilder.UpdateData(
                schema: "game",
                table: "class_stat_bases",
                keyColumns: new[] { "class_code", "stat_key" },
                keyValues: new object[] { (short)0, "evasion" },
                column: "per_level",
                value: 0);

            migrationBuilder.UpdateData(
                schema: "game",
                table: "class_stat_bases",
                keyColumns: new[] { "class_code", "stat_key" },
                keyValues: new object[] { (short)0, "fire_attack" },
                column: "per_level",
                value: 0);

            migrationBuilder.UpdateData(
                schema: "game",
                table: "class_stat_bases",
                keyColumns: new[] { "class_code", "stat_key" },
                keyValues: new object[] { (short)0, "fire_resistance" },
                column: "per_level",
                value: 0);

            migrationBuilder.UpdateData(
                schema: "game",
                table: "class_stat_bases",
                keyColumns: new[] { "class_code", "stat_key" },
                keyValues: new object[] { (short)0, "guard_limit" },
                column: "per_level",
                value: 0);

            migrationBuilder.UpdateData(
                schema: "game",
                table: "class_stat_bases",
                keyColumns: new[] { "class_code", "stat_key" },
                keyValues: new object[] { (short)0, "guard_recovery" },
                column: "per_level",
                value: 0);

            migrationBuilder.UpdateData(
                schema: "game",
                table: "class_stat_bases",
                keyColumns: new[] { "class_code", "stat_key" },
                keyValues: new object[] { (short)0, "health" },
                column: "per_level",
                value: 5);

            migrationBuilder.UpdateData(
                schema: "game",
                table: "class_stat_bases",
                keyColumns: new[] { "class_code", "stat_key" },
                keyValues: new object[] { (short)0, "hp_recovery" },
                column: "per_level",
                value: 0);

            migrationBuilder.UpdateData(
                schema: "game",
                table: "class_stat_bases",
                keyColumns: new[] { "class_code", "stat_key" },
                keyValues: new object[] { (short)0, "intelligence" },
                column: "per_level",
                value: 4);

            migrationBuilder.UpdateData(
                schema: "game",
                table: "class_stat_bases",
                keyColumns: new[] { "class_code", "stat_key" },
                keyValues: new object[] { (short)0, "magic_attack" },
                column: "per_level",
                value: 0);

            migrationBuilder.UpdateData(
                schema: "game",
                table: "class_stat_bases",
                keyColumns: new[] { "class_code", "stat_key" },
                keyValues: new object[] { (short)0, "magic_defense" },
                column: "per_level",
                value: 0);

            migrationBuilder.UpdateData(
                schema: "game",
                table: "class_stat_bases",
                keyColumns: new[] { "class_code", "stat_key" },
                keyValues: new object[] { (short)0, "max_hp" },
                column: "per_level",
                value: 0);

            migrationBuilder.UpdateData(
                schema: "game",
                table: "class_stat_bases",
                keyColumns: new[] { "class_code", "stat_key" },
                keyValues: new object[] { (short)0, "max_mp" },
                column: "per_level",
                value: 0);

            migrationBuilder.UpdateData(
                schema: "game",
                table: "class_stat_bases",
                keyColumns: new[] { "class_code", "stat_key" },
                keyValues: new object[] { (short)0, "move_speed" },
                column: "per_level",
                value: 0);

            migrationBuilder.UpdateData(
                schema: "game",
                table: "class_stat_bases",
                keyColumns: new[] { "class_code", "stat_key" },
                keyValues: new object[] { (short)0, "mp_recovery" },
                column: "per_level",
                value: 0);

            migrationBuilder.UpdateData(
                schema: "game",
                table: "class_stat_bases",
                keyColumns: new[] { "class_code", "stat_key" },
                keyValues: new object[] { (short)0, "physical_attack" },
                column: "per_level",
                value: 0);

            migrationBuilder.UpdateData(
                schema: "game",
                table: "class_stat_bases",
                keyColumns: new[] { "class_code", "stat_key" },
                keyValues: new object[] { (short)0, "physical_defense" },
                column: "per_level",
                value: 0);

            migrationBuilder.UpdateData(
                schema: "game",
                table: "class_stat_bases",
                keyColumns: new[] { "class_code", "stat_key" },
                keyValues: new object[] { (short)0, "piercing" },
                column: "per_level",
                value: 0);

            migrationBuilder.UpdateData(
                schema: "game",
                table: "class_stat_bases",
                keyColumns: new[] { "class_code", "stat_key" },
                keyValues: new object[] { (short)0, "strength" },
                column: "per_level",
                value: 4);

            migrationBuilder.UpdateData(
                schema: "game",
                table: "class_stat_bases",
                keyColumns: new[] { "class_code", "stat_key" },
                keyValues: new object[] { (short)0, "valiance" },
                column: "per_level",
                value: 0);

            migrationBuilder.UpdateData(
                schema: "game",
                table: "class_stat_bases",
                keyColumns: new[] { "class_code", "stat_key" },
                keyValues: new object[] { (short)0, "water_attack" },
                column: "per_level",
                value: 0);

            migrationBuilder.UpdateData(
                schema: "game",
                table: "class_stat_bases",
                keyColumns: new[] { "class_code", "stat_key" },
                keyValues: new object[] { (short)0, "water_resistance" },
                column: "per_level",
                value: 0);

            migrationBuilder.UpdateData(
                schema: "game",
                table: "class_stat_bases",
                keyColumns: new[] { "class_code", "stat_key" },
                keyValues: new object[] { (short)0, "wisdom" },
                column: "per_level",
                value: 2);

            migrationBuilder.UpdateData(
                schema: "game",
                table: "class_stat_bases",
                keyColumns: new[] { "class_code", "stat_key" },
                keyValues: new object[] { (short)1, "accuracy" },
                column: "per_level",
                value: 0);

            migrationBuilder.UpdateData(
                schema: "game",
                table: "class_stat_bases",
                keyColumns: new[] { "class_code", "stat_key" },
                keyValues: new object[] { (short)1, "air_attack" },
                column: "per_level",
                value: 0);

            migrationBuilder.UpdateData(
                schema: "game",
                table: "class_stat_bases",
                keyColumns: new[] { "class_code", "stat_key" },
                keyValues: new object[] { (short)1, "air_resistance" },
                column: "per_level",
                value: 0);

            migrationBuilder.UpdateData(
                schema: "game",
                table: "class_stat_bases",
                keyColumns: new[] { "class_code", "stat_key" },
                keyValues: new object[] { (short)1, "attack_speed" },
                column: "per_level",
                value: 0);

            migrationBuilder.UpdateData(
                schema: "game",
                table: "class_stat_bases",
                keyColumns: new[] { "class_code", "stat_key" },
                keyValues: new object[] { (short)1, "cast_speed" },
                column: "per_level",
                value: 0);

            migrationBuilder.UpdateData(
                schema: "game",
                table: "class_stat_bases",
                keyColumns: new[] { "class_code", "stat_key" },
                keyValues: new object[] { (short)1, "concentration" },
                column: "per_level",
                value: 0);

            migrationBuilder.UpdateData(
                schema: "game",
                table: "class_stat_bases",
                keyColumns: new[] { "class_code", "stat_key" },
                keyValues: new object[] { (short)1, "critical_damage" },
                column: "per_level",
                value: 0);

            migrationBuilder.UpdateData(
                schema: "game",
                table: "class_stat_bases",
                keyColumns: new[] { "class_code", "stat_key" },
                keyValues: new object[] { (short)1, "critical_rate" },
                column: "per_level",
                value: 0);

            migrationBuilder.UpdateData(
                schema: "game",
                table: "class_stat_bases",
                keyColumns: new[] { "class_code", "stat_key" },
                keyValues: new object[] { (short)1, "critical_resistance" },
                column: "per_level",
                value: 0);

            migrationBuilder.UpdateData(
                schema: "game",
                table: "class_stat_bases",
                keyColumns: new[] { "class_code", "stat_key" },
                keyValues: new object[] { (short)1, "earth_attack" },
                column: "per_level",
                value: 0);

            migrationBuilder.UpdateData(
                schema: "game",
                table: "class_stat_bases",
                keyColumns: new[] { "class_code", "stat_key" },
                keyValues: new object[] { (short)1, "earth_resistance" },
                column: "per_level",
                value: 0);

            migrationBuilder.UpdateData(
                schema: "game",
                table: "class_stat_bases",
                keyColumns: new[] { "class_code", "stat_key" },
                keyValues: new object[] { (short)1, "evasion" },
                column: "per_level",
                value: 0);

            migrationBuilder.UpdateData(
                schema: "game",
                table: "class_stat_bases",
                keyColumns: new[] { "class_code", "stat_key" },
                keyValues: new object[] { (short)1, "fire_attack" },
                column: "per_level",
                value: 0);

            migrationBuilder.UpdateData(
                schema: "game",
                table: "class_stat_bases",
                keyColumns: new[] { "class_code", "stat_key" },
                keyValues: new object[] { (short)1, "fire_resistance" },
                column: "per_level",
                value: 0);

            migrationBuilder.UpdateData(
                schema: "game",
                table: "class_stat_bases",
                keyColumns: new[] { "class_code", "stat_key" },
                keyValues: new object[] { (short)1, "guard_limit" },
                column: "per_level",
                value: 0);

            migrationBuilder.UpdateData(
                schema: "game",
                table: "class_stat_bases",
                keyColumns: new[] { "class_code", "stat_key" },
                keyValues: new object[] { (short)1, "guard_recovery" },
                column: "per_level",
                value: 0);

            migrationBuilder.UpdateData(
                schema: "game",
                table: "class_stat_bases",
                keyColumns: new[] { "class_code", "stat_key" },
                keyValues: new object[] { (short)1, "health" },
                column: "per_level",
                value: 3);

            migrationBuilder.UpdateData(
                schema: "game",
                table: "class_stat_bases",
                keyColumns: new[] { "class_code", "stat_key" },
                keyValues: new object[] { (short)1, "hp_recovery" },
                column: "per_level",
                value: 0);

            migrationBuilder.UpdateData(
                schema: "game",
                table: "class_stat_bases",
                keyColumns: new[] { "class_code", "stat_key" },
                keyValues: new object[] { (short)1, "intelligence" },
                column: "per_level",
                value: 3);

            migrationBuilder.UpdateData(
                schema: "game",
                table: "class_stat_bases",
                keyColumns: new[] { "class_code", "stat_key" },
                keyValues: new object[] { (short)1, "magic_attack" },
                column: "per_level",
                value: 0);

            migrationBuilder.UpdateData(
                schema: "game",
                table: "class_stat_bases",
                keyColumns: new[] { "class_code", "stat_key" },
                keyValues: new object[] { (short)1, "magic_defense" },
                column: "per_level",
                value: 0);

            migrationBuilder.UpdateData(
                schema: "game",
                table: "class_stat_bases",
                keyColumns: new[] { "class_code", "stat_key" },
                keyValues: new object[] { (short)1, "max_hp" },
                column: "per_level",
                value: 0);

            migrationBuilder.UpdateData(
                schema: "game",
                table: "class_stat_bases",
                keyColumns: new[] { "class_code", "stat_key" },
                keyValues: new object[] { (short)1, "max_mp" },
                column: "per_level",
                value: 0);

            migrationBuilder.UpdateData(
                schema: "game",
                table: "class_stat_bases",
                keyColumns: new[] { "class_code", "stat_key" },
                keyValues: new object[] { (short)1, "move_speed" },
                column: "per_level",
                value: 0);

            migrationBuilder.UpdateData(
                schema: "game",
                table: "class_stat_bases",
                keyColumns: new[] { "class_code", "stat_key" },
                keyValues: new object[] { (short)1, "mp_recovery" },
                column: "per_level",
                value: 0);

            migrationBuilder.UpdateData(
                schema: "game",
                table: "class_stat_bases",
                keyColumns: new[] { "class_code", "stat_key" },
                keyValues: new object[] { (short)1, "physical_attack" },
                column: "per_level",
                value: 0);

            migrationBuilder.UpdateData(
                schema: "game",
                table: "class_stat_bases",
                keyColumns: new[] { "class_code", "stat_key" },
                keyValues: new object[] { (short)1, "physical_defense" },
                column: "per_level",
                value: 0);

            migrationBuilder.UpdateData(
                schema: "game",
                table: "class_stat_bases",
                keyColumns: new[] { "class_code", "stat_key" },
                keyValues: new object[] { (short)1, "piercing" },
                column: "per_level",
                value: 0);

            migrationBuilder.UpdateData(
                schema: "game",
                table: "class_stat_bases",
                keyColumns: new[] { "class_code", "stat_key" },
                keyValues: new object[] { (short)1, "strength" },
                column: "per_level",
                value: 4);

            migrationBuilder.UpdateData(
                schema: "game",
                table: "class_stat_bases",
                keyColumns: new[] { "class_code", "stat_key" },
                keyValues: new object[] { (short)1, "valiance" },
                column: "per_level",
                value: 0);

            migrationBuilder.UpdateData(
                schema: "game",
                table: "class_stat_bases",
                keyColumns: new[] { "class_code", "stat_key" },
                keyValues: new object[] { (short)1, "water_attack" },
                column: "per_level",
                value: 0);

            migrationBuilder.UpdateData(
                schema: "game",
                table: "class_stat_bases",
                keyColumns: new[] { "class_code", "stat_key" },
                keyValues: new object[] { (short)1, "water_resistance" },
                column: "per_level",
                value: 0);

            migrationBuilder.UpdateData(
                schema: "game",
                table: "class_stat_bases",
                keyColumns: new[] { "class_code", "stat_key" },
                keyValues: new object[] { (short)1, "wisdom" },
                column: "per_level",
                value: 5);

            migrationBuilder.UpdateData(
                schema: "game",
                table: "class_stat_bases",
                keyColumns: new[] { "class_code", "stat_key" },
                keyValues: new object[] { (short)2, "accuracy" },
                column: "per_level",
                value: 0);

            migrationBuilder.UpdateData(
                schema: "game",
                table: "class_stat_bases",
                keyColumns: new[] { "class_code", "stat_key" },
                keyValues: new object[] { (short)2, "air_attack" },
                column: "per_level",
                value: 0);

            migrationBuilder.UpdateData(
                schema: "game",
                table: "class_stat_bases",
                keyColumns: new[] { "class_code", "stat_key" },
                keyValues: new object[] { (short)2, "air_resistance" },
                column: "per_level",
                value: 0);

            migrationBuilder.UpdateData(
                schema: "game",
                table: "class_stat_bases",
                keyColumns: new[] { "class_code", "stat_key" },
                keyValues: new object[] { (short)2, "attack_speed" },
                column: "per_level",
                value: 0);

            migrationBuilder.UpdateData(
                schema: "game",
                table: "class_stat_bases",
                keyColumns: new[] { "class_code", "stat_key" },
                keyValues: new object[] { (short)2, "cast_speed" },
                column: "per_level",
                value: 0);

            migrationBuilder.UpdateData(
                schema: "game",
                table: "class_stat_bases",
                keyColumns: new[] { "class_code", "stat_key" },
                keyValues: new object[] { (short)2, "concentration" },
                column: "per_level",
                value: 0);

            migrationBuilder.UpdateData(
                schema: "game",
                table: "class_stat_bases",
                keyColumns: new[] { "class_code", "stat_key" },
                keyValues: new object[] { (short)2, "critical_damage" },
                column: "per_level",
                value: 0);

            migrationBuilder.UpdateData(
                schema: "game",
                table: "class_stat_bases",
                keyColumns: new[] { "class_code", "stat_key" },
                keyValues: new object[] { (short)2, "critical_rate" },
                column: "per_level",
                value: 0);

            migrationBuilder.UpdateData(
                schema: "game",
                table: "class_stat_bases",
                keyColumns: new[] { "class_code", "stat_key" },
                keyValues: new object[] { (short)2, "critical_resistance" },
                column: "per_level",
                value: 0);

            migrationBuilder.UpdateData(
                schema: "game",
                table: "class_stat_bases",
                keyColumns: new[] { "class_code", "stat_key" },
                keyValues: new object[] { (short)2, "earth_attack" },
                column: "per_level",
                value: 0);

            migrationBuilder.UpdateData(
                schema: "game",
                table: "class_stat_bases",
                keyColumns: new[] { "class_code", "stat_key" },
                keyValues: new object[] { (short)2, "earth_resistance" },
                column: "per_level",
                value: 0);

            migrationBuilder.UpdateData(
                schema: "game",
                table: "class_stat_bases",
                keyColumns: new[] { "class_code", "stat_key" },
                keyValues: new object[] { (short)2, "evasion" },
                column: "per_level",
                value: 0);

            migrationBuilder.UpdateData(
                schema: "game",
                table: "class_stat_bases",
                keyColumns: new[] { "class_code", "stat_key" },
                keyValues: new object[] { (short)2, "fire_attack" },
                column: "per_level",
                value: 0);

            migrationBuilder.UpdateData(
                schema: "game",
                table: "class_stat_bases",
                keyColumns: new[] { "class_code", "stat_key" },
                keyValues: new object[] { (short)2, "fire_resistance" },
                column: "per_level",
                value: 0);

            migrationBuilder.UpdateData(
                schema: "game",
                table: "class_stat_bases",
                keyColumns: new[] { "class_code", "stat_key" },
                keyValues: new object[] { (short)2, "guard_limit" },
                column: "per_level",
                value: 0);

            migrationBuilder.UpdateData(
                schema: "game",
                table: "class_stat_bases",
                keyColumns: new[] { "class_code", "stat_key" },
                keyValues: new object[] { (short)2, "guard_recovery" },
                column: "per_level",
                value: 0);

            migrationBuilder.UpdateData(
                schema: "game",
                table: "class_stat_bases",
                keyColumns: new[] { "class_code", "stat_key" },
                keyValues: new object[] { (short)2, "health" },
                column: "per_level",
                value: 4);

            migrationBuilder.UpdateData(
                schema: "game",
                table: "class_stat_bases",
                keyColumns: new[] { "class_code", "stat_key" },
                keyValues: new object[] { (short)2, "hp_recovery" },
                column: "per_level",
                value: 0);

            migrationBuilder.UpdateData(
                schema: "game",
                table: "class_stat_bases",
                keyColumns: new[] { "class_code", "stat_key" },
                keyValues: new object[] { (short)2, "intelligence" },
                column: "per_level",
                value: 1);

            migrationBuilder.UpdateData(
                schema: "game",
                table: "class_stat_bases",
                keyColumns: new[] { "class_code", "stat_key" },
                keyValues: new object[] { (short)2, "magic_attack" },
                column: "per_level",
                value: 0);

            migrationBuilder.UpdateData(
                schema: "game",
                table: "class_stat_bases",
                keyColumns: new[] { "class_code", "stat_key" },
                keyValues: new object[] { (short)2, "magic_defense" },
                column: "per_level",
                value: 0);

            migrationBuilder.UpdateData(
                schema: "game",
                table: "class_stat_bases",
                keyColumns: new[] { "class_code", "stat_key" },
                keyValues: new object[] { (short)2, "max_hp" },
                column: "per_level",
                value: 0);

            migrationBuilder.UpdateData(
                schema: "game",
                table: "class_stat_bases",
                keyColumns: new[] { "class_code", "stat_key" },
                keyValues: new object[] { (short)2, "max_mp" },
                column: "per_level",
                value: 0);

            migrationBuilder.UpdateData(
                schema: "game",
                table: "class_stat_bases",
                keyColumns: new[] { "class_code", "stat_key" },
                keyValues: new object[] { (short)2, "move_speed" },
                column: "per_level",
                value: 0);

            migrationBuilder.UpdateData(
                schema: "game",
                table: "class_stat_bases",
                keyColumns: new[] { "class_code", "stat_key" },
                keyValues: new object[] { (short)2, "mp_recovery" },
                column: "per_level",
                value: 0);

            migrationBuilder.UpdateData(
                schema: "game",
                table: "class_stat_bases",
                keyColumns: new[] { "class_code", "stat_key" },
                keyValues: new object[] { (short)2, "physical_attack" },
                column: "per_level",
                value: 0);

            migrationBuilder.UpdateData(
                schema: "game",
                table: "class_stat_bases",
                keyColumns: new[] { "class_code", "stat_key" },
                keyValues: new object[] { (short)2, "physical_defense" },
                column: "per_level",
                value: 0);

            migrationBuilder.UpdateData(
                schema: "game",
                table: "class_stat_bases",
                keyColumns: new[] { "class_code", "stat_key" },
                keyValues: new object[] { (short)2, "piercing" },
                column: "per_level",
                value: 0);

            migrationBuilder.UpdateData(
                schema: "game",
                table: "class_stat_bases",
                keyColumns: new[] { "class_code", "stat_key" },
                keyValues: new object[] { (short)2, "strength" },
                column: "per_level",
                value: 4);

            migrationBuilder.UpdateData(
                schema: "game",
                table: "class_stat_bases",
                keyColumns: new[] { "class_code", "stat_key" },
                keyValues: new object[] { (short)2, "valiance" },
                column: "per_level",
                value: 0);

            migrationBuilder.UpdateData(
                schema: "game",
                table: "class_stat_bases",
                keyColumns: new[] { "class_code", "stat_key" },
                keyValues: new object[] { (short)2, "water_attack" },
                column: "per_level",
                value: 0);

            migrationBuilder.UpdateData(
                schema: "game",
                table: "class_stat_bases",
                keyColumns: new[] { "class_code", "stat_key" },
                keyValues: new object[] { (short)2, "water_resistance" },
                column: "per_level",
                value: 0);

            migrationBuilder.UpdateData(
                schema: "game",
                table: "class_stat_bases",
                keyColumns: new[] { "class_code", "stat_key" },
                keyValues: new object[] { (short)2, "wisdom" },
                column: "per_level",
                value: 1);

            migrationBuilder.UpdateData(
                schema: "game",
                table: "class_stat_bases",
                keyColumns: new[] { "class_code", "stat_key" },
                keyValues: new object[] { (short)3, "accuracy" },
                column: "per_level",
                value: 0);

            migrationBuilder.UpdateData(
                schema: "game",
                table: "class_stat_bases",
                keyColumns: new[] { "class_code", "stat_key" },
                keyValues: new object[] { (short)3, "air_attack" },
                column: "per_level",
                value: 0);

            migrationBuilder.UpdateData(
                schema: "game",
                table: "class_stat_bases",
                keyColumns: new[] { "class_code", "stat_key" },
                keyValues: new object[] { (short)3, "air_resistance" },
                column: "per_level",
                value: 0);

            migrationBuilder.UpdateData(
                schema: "game",
                table: "class_stat_bases",
                keyColumns: new[] { "class_code", "stat_key" },
                keyValues: new object[] { (short)3, "attack_speed" },
                column: "per_level",
                value: 0);

            migrationBuilder.UpdateData(
                schema: "game",
                table: "class_stat_bases",
                keyColumns: new[] { "class_code", "stat_key" },
                keyValues: new object[] { (short)3, "cast_speed" },
                column: "per_level",
                value: 0);

            migrationBuilder.UpdateData(
                schema: "game",
                table: "class_stat_bases",
                keyColumns: new[] { "class_code", "stat_key" },
                keyValues: new object[] { (short)3, "concentration" },
                column: "per_level",
                value: 0);

            migrationBuilder.UpdateData(
                schema: "game",
                table: "class_stat_bases",
                keyColumns: new[] { "class_code", "stat_key" },
                keyValues: new object[] { (short)3, "critical_damage" },
                column: "per_level",
                value: 0);

            migrationBuilder.UpdateData(
                schema: "game",
                table: "class_stat_bases",
                keyColumns: new[] { "class_code", "stat_key" },
                keyValues: new object[] { (short)3, "critical_rate" },
                column: "per_level",
                value: 0);

            migrationBuilder.UpdateData(
                schema: "game",
                table: "class_stat_bases",
                keyColumns: new[] { "class_code", "stat_key" },
                keyValues: new object[] { (short)3, "critical_resistance" },
                column: "per_level",
                value: 0);

            migrationBuilder.UpdateData(
                schema: "game",
                table: "class_stat_bases",
                keyColumns: new[] { "class_code", "stat_key" },
                keyValues: new object[] { (short)3, "earth_attack" },
                column: "per_level",
                value: 0);

            migrationBuilder.UpdateData(
                schema: "game",
                table: "class_stat_bases",
                keyColumns: new[] { "class_code", "stat_key" },
                keyValues: new object[] { (short)3, "earth_resistance" },
                column: "per_level",
                value: 0);

            migrationBuilder.UpdateData(
                schema: "game",
                table: "class_stat_bases",
                keyColumns: new[] { "class_code", "stat_key" },
                keyValues: new object[] { (short)3, "evasion" },
                column: "per_level",
                value: 0);

            migrationBuilder.UpdateData(
                schema: "game",
                table: "class_stat_bases",
                keyColumns: new[] { "class_code", "stat_key" },
                keyValues: new object[] { (short)3, "fire_attack" },
                column: "per_level",
                value: 0);

            migrationBuilder.UpdateData(
                schema: "game",
                table: "class_stat_bases",
                keyColumns: new[] { "class_code", "stat_key" },
                keyValues: new object[] { (short)3, "fire_resistance" },
                column: "per_level",
                value: 0);

            migrationBuilder.UpdateData(
                schema: "game",
                table: "class_stat_bases",
                keyColumns: new[] { "class_code", "stat_key" },
                keyValues: new object[] { (short)3, "guard_limit" },
                column: "per_level",
                value: 0);

            migrationBuilder.UpdateData(
                schema: "game",
                table: "class_stat_bases",
                keyColumns: new[] { "class_code", "stat_key" },
                keyValues: new object[] { (short)3, "guard_recovery" },
                column: "per_level",
                value: 0);

            migrationBuilder.UpdateData(
                schema: "game",
                table: "class_stat_bases",
                keyColumns: new[] { "class_code", "stat_key" },
                keyValues: new object[] { (short)3, "health" },
                column: "per_level",
                value: 4);

            migrationBuilder.UpdateData(
                schema: "game",
                table: "class_stat_bases",
                keyColumns: new[] { "class_code", "stat_key" },
                keyValues: new object[] { (short)3, "hp_recovery" },
                column: "per_level",
                value: 0);

            migrationBuilder.UpdateData(
                schema: "game",
                table: "class_stat_bases",
                keyColumns: new[] { "class_code", "stat_key" },
                keyValues: new object[] { (short)3, "intelligence" },
                column: "per_level",
                value: 5);

            migrationBuilder.UpdateData(
                schema: "game",
                table: "class_stat_bases",
                keyColumns: new[] { "class_code", "stat_key" },
                keyValues: new object[] { (short)3, "magic_attack" },
                column: "per_level",
                value: 0);

            migrationBuilder.UpdateData(
                schema: "game",
                table: "class_stat_bases",
                keyColumns: new[] { "class_code", "stat_key" },
                keyValues: new object[] { (short)3, "magic_defense" },
                column: "per_level",
                value: 0);

            migrationBuilder.UpdateData(
                schema: "game",
                table: "class_stat_bases",
                keyColumns: new[] { "class_code", "stat_key" },
                keyValues: new object[] { (short)3, "max_hp" },
                column: "per_level",
                value: 0);

            migrationBuilder.UpdateData(
                schema: "game",
                table: "class_stat_bases",
                keyColumns: new[] { "class_code", "stat_key" },
                keyValues: new object[] { (short)3, "max_mp" },
                column: "per_level",
                value: 0);

            migrationBuilder.UpdateData(
                schema: "game",
                table: "class_stat_bases",
                keyColumns: new[] { "class_code", "stat_key" },
                keyValues: new object[] { (short)3, "move_speed" },
                column: "per_level",
                value: 0);

            migrationBuilder.UpdateData(
                schema: "game",
                table: "class_stat_bases",
                keyColumns: new[] { "class_code", "stat_key" },
                keyValues: new object[] { (short)3, "mp_recovery" },
                column: "per_level",
                value: 0);

            migrationBuilder.UpdateData(
                schema: "game",
                table: "class_stat_bases",
                keyColumns: new[] { "class_code", "stat_key" },
                keyValues: new object[] { (short)3, "physical_attack" },
                column: "per_level",
                value: 0);

            migrationBuilder.UpdateData(
                schema: "game",
                table: "class_stat_bases",
                keyColumns: new[] { "class_code", "stat_key" },
                keyValues: new object[] { (short)3, "physical_defense" },
                column: "per_level",
                value: 0);

            migrationBuilder.UpdateData(
                schema: "game",
                table: "class_stat_bases",
                keyColumns: new[] { "class_code", "stat_key" },
                keyValues: new object[] { (short)3, "piercing" },
                column: "per_level",
                value: 0);

            migrationBuilder.UpdateData(
                schema: "game",
                table: "class_stat_bases",
                keyColumns: new[] { "class_code", "stat_key" },
                keyValues: new object[] { (short)3, "strength" },
                column: "per_level",
                value: 4);

            migrationBuilder.UpdateData(
                schema: "game",
                table: "class_stat_bases",
                keyColumns: new[] { "class_code", "stat_key" },
                keyValues: new object[] { (short)3, "valiance" },
                column: "per_level",
                value: 0);

            migrationBuilder.UpdateData(
                schema: "game",
                table: "class_stat_bases",
                keyColumns: new[] { "class_code", "stat_key" },
                keyValues: new object[] { (short)3, "water_attack" },
                column: "per_level",
                value: 0);

            migrationBuilder.UpdateData(
                schema: "game",
                table: "class_stat_bases",
                keyColumns: new[] { "class_code", "stat_key" },
                keyValues: new object[] { (short)3, "water_resistance" },
                column: "per_level",
                value: 0);

            migrationBuilder.UpdateData(
                schema: "game",
                table: "class_stat_bases",
                keyColumns: new[] { "class_code", "stat_key" },
                keyValues: new object[] { (short)3, "wisdom" },
                column: "per_level",
                value: 2);

            migrationBuilder.UpdateData(
                schema: "game",
                table: "class_stat_bases",
                keyColumns: new[] { "class_code", "stat_key" },
                keyValues: new object[] { (short)4, "accuracy" },
                column: "per_level",
                value: 0);

            migrationBuilder.UpdateData(
                schema: "game",
                table: "class_stat_bases",
                keyColumns: new[] { "class_code", "stat_key" },
                keyValues: new object[] { (short)4, "air_attack" },
                column: "per_level",
                value: 0);

            migrationBuilder.UpdateData(
                schema: "game",
                table: "class_stat_bases",
                keyColumns: new[] { "class_code", "stat_key" },
                keyValues: new object[] { (short)4, "air_resistance" },
                column: "per_level",
                value: 0);

            migrationBuilder.UpdateData(
                schema: "game",
                table: "class_stat_bases",
                keyColumns: new[] { "class_code", "stat_key" },
                keyValues: new object[] { (short)4, "attack_speed" },
                column: "per_level",
                value: 0);

            migrationBuilder.UpdateData(
                schema: "game",
                table: "class_stat_bases",
                keyColumns: new[] { "class_code", "stat_key" },
                keyValues: new object[] { (short)4, "cast_speed" },
                column: "per_level",
                value: 0);

            migrationBuilder.UpdateData(
                schema: "game",
                table: "class_stat_bases",
                keyColumns: new[] { "class_code", "stat_key" },
                keyValues: new object[] { (short)4, "concentration" },
                column: "per_level",
                value: 0);

            migrationBuilder.UpdateData(
                schema: "game",
                table: "class_stat_bases",
                keyColumns: new[] { "class_code", "stat_key" },
                keyValues: new object[] { (short)4, "critical_damage" },
                column: "per_level",
                value: 0);

            migrationBuilder.UpdateData(
                schema: "game",
                table: "class_stat_bases",
                keyColumns: new[] { "class_code", "stat_key" },
                keyValues: new object[] { (short)4, "critical_rate" },
                column: "per_level",
                value: 0);

            migrationBuilder.UpdateData(
                schema: "game",
                table: "class_stat_bases",
                keyColumns: new[] { "class_code", "stat_key" },
                keyValues: new object[] { (short)4, "critical_resistance" },
                column: "per_level",
                value: 0);

            migrationBuilder.UpdateData(
                schema: "game",
                table: "class_stat_bases",
                keyColumns: new[] { "class_code", "stat_key" },
                keyValues: new object[] { (short)4, "earth_attack" },
                column: "per_level",
                value: 0);

            migrationBuilder.UpdateData(
                schema: "game",
                table: "class_stat_bases",
                keyColumns: new[] { "class_code", "stat_key" },
                keyValues: new object[] { (short)4, "earth_resistance" },
                column: "per_level",
                value: 0);

            migrationBuilder.UpdateData(
                schema: "game",
                table: "class_stat_bases",
                keyColumns: new[] { "class_code", "stat_key" },
                keyValues: new object[] { (short)4, "evasion" },
                column: "per_level",
                value: 0);

            migrationBuilder.UpdateData(
                schema: "game",
                table: "class_stat_bases",
                keyColumns: new[] { "class_code", "stat_key" },
                keyValues: new object[] { (short)4, "fire_attack" },
                column: "per_level",
                value: 0);

            migrationBuilder.UpdateData(
                schema: "game",
                table: "class_stat_bases",
                keyColumns: new[] { "class_code", "stat_key" },
                keyValues: new object[] { (short)4, "fire_resistance" },
                column: "per_level",
                value: 0);

            migrationBuilder.UpdateData(
                schema: "game",
                table: "class_stat_bases",
                keyColumns: new[] { "class_code", "stat_key" },
                keyValues: new object[] { (short)4, "guard_limit" },
                column: "per_level",
                value: 0);

            migrationBuilder.UpdateData(
                schema: "game",
                table: "class_stat_bases",
                keyColumns: new[] { "class_code", "stat_key" },
                keyValues: new object[] { (short)4, "guard_recovery" },
                column: "per_level",
                value: 0);

            migrationBuilder.UpdateData(
                schema: "game",
                table: "class_stat_bases",
                keyColumns: new[] { "class_code", "stat_key" },
                keyValues: new object[] { (short)4, "health" },
                column: "per_level",
                value: 3);

            migrationBuilder.UpdateData(
                schema: "game",
                table: "class_stat_bases",
                keyColumns: new[] { "class_code", "stat_key" },
                keyValues: new object[] { (short)4, "hp_recovery" },
                column: "per_level",
                value: 0);

            migrationBuilder.UpdateData(
                schema: "game",
                table: "class_stat_bases",
                keyColumns: new[] { "class_code", "stat_key" },
                keyValues: new object[] { (short)4, "intelligence" },
                column: "per_level",
                value: 2);

            migrationBuilder.UpdateData(
                schema: "game",
                table: "class_stat_bases",
                keyColumns: new[] { "class_code", "stat_key" },
                keyValues: new object[] { (short)4, "magic_attack" },
                column: "per_level",
                value: 0);

            migrationBuilder.UpdateData(
                schema: "game",
                table: "class_stat_bases",
                keyColumns: new[] { "class_code", "stat_key" },
                keyValues: new object[] { (short)4, "magic_defense" },
                column: "per_level",
                value: 0);

            migrationBuilder.UpdateData(
                schema: "game",
                table: "class_stat_bases",
                keyColumns: new[] { "class_code", "stat_key" },
                keyValues: new object[] { (short)4, "max_hp" },
                column: "per_level",
                value: 0);

            migrationBuilder.UpdateData(
                schema: "game",
                table: "class_stat_bases",
                keyColumns: new[] { "class_code", "stat_key" },
                keyValues: new object[] { (short)4, "max_mp" },
                column: "per_level",
                value: 0);

            migrationBuilder.UpdateData(
                schema: "game",
                table: "class_stat_bases",
                keyColumns: new[] { "class_code", "stat_key" },
                keyValues: new object[] { (short)4, "move_speed" },
                column: "per_level",
                value: 0);

            migrationBuilder.UpdateData(
                schema: "game",
                table: "class_stat_bases",
                keyColumns: new[] { "class_code", "stat_key" },
                keyValues: new object[] { (short)4, "mp_recovery" },
                column: "per_level",
                value: 0);

            migrationBuilder.UpdateData(
                schema: "game",
                table: "class_stat_bases",
                keyColumns: new[] { "class_code", "stat_key" },
                keyValues: new object[] { (short)4, "physical_attack" },
                column: "per_level",
                value: 0);

            migrationBuilder.UpdateData(
                schema: "game",
                table: "class_stat_bases",
                keyColumns: new[] { "class_code", "stat_key" },
                keyValues: new object[] { (short)4, "physical_defense" },
                column: "per_level",
                value: 0);

            migrationBuilder.UpdateData(
                schema: "game",
                table: "class_stat_bases",
                keyColumns: new[] { "class_code", "stat_key" },
                keyValues: new object[] { (short)4, "piercing" },
                column: "per_level",
                value: 0);

            migrationBuilder.UpdateData(
                schema: "game",
                table: "class_stat_bases",
                keyColumns: new[] { "class_code", "stat_key" },
                keyValues: new object[] { (short)4, "strength" },
                column: "per_level",
                value: 5);

            migrationBuilder.UpdateData(
                schema: "game",
                table: "class_stat_bases",
                keyColumns: new[] { "class_code", "stat_key" },
                keyValues: new object[] { (short)4, "valiance" },
                column: "per_level",
                value: 0);

            migrationBuilder.UpdateData(
                schema: "game",
                table: "class_stat_bases",
                keyColumns: new[] { "class_code", "stat_key" },
                keyValues: new object[] { (short)4, "water_attack" },
                column: "per_level",
                value: 0);

            migrationBuilder.UpdateData(
                schema: "game",
                table: "class_stat_bases",
                keyColumns: new[] { "class_code", "stat_key" },
                keyValues: new object[] { (short)4, "water_resistance" },
                column: "per_level",
                value: 0);

            migrationBuilder.UpdateData(
                schema: "game",
                table: "class_stat_bases",
                keyColumns: new[] { "class_code", "stat_key" },
                keyValues: new object[] { (short)4, "wisdom" },
                column: "per_level",
                value: 5);

            migrationBuilder.AddCheckConstraint(
                name: "ck_class_stat_bases_per_level",
                schema: "game",
                table: "class_stat_bases",
                sql: "per_level >= 0");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "ck_class_stat_bases_per_level",
                schema: "game",
                table: "class_stat_bases");

            migrationBuilder.DropColumn(
                name: "per_level",
                schema: "game",
                table: "class_stat_bases");
        }
    }
}
