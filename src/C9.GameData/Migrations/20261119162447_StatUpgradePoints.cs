using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace C9.GameData.Migrations
{
    /// <inheritdoc />
    public partial class StatUpgradePoints : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "stat_definitions",
                schema: "game",
                columns: table => new
                {
                    key = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    name = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    sort = table.Column<short>(type: "smallint", nullable: false),
                    upgradable = table.Column<bool>(type: "boolean", nullable: false),
                    per_point = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_stat_definitions", x => x.key);
                    table.CheckConstraint("ck_stat_definitions_per_point", "per_point > 0");
                });

            migrationBuilder.CreateTable(
                name: "stat_point_levels",
                schema: "game",
                columns: table => new
                {
                    level = table.Column<short>(type: "smallint", nullable: false),
                    points = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_stat_point_levels", x => x.level);
                    table.CheckConstraint("ck_stat_point_levels_level", "level BETWEEN 1 AND 999");
                    table.CheckConstraint("ck_stat_point_levels_points", "points >= 0");
                });

            migrationBuilder.CreateTable(
                name: "character_stats",
                schema: "game",
                columns: table => new
                {
                    character_id = table.Column<long>(type: "bigint", nullable: false),
                    stat_key = table.Column<string>(type: "character varying(32)", nullable: false),
                    points = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_character_stats", x => new { x.character_id, x.stat_key });
                    table.CheckConstraint("ck_character_stats_points", "points >= 0");
                    table.ForeignKey(
                        name: "fk_character_stats_characters_character_id",
                        column: x => x.character_id,
                        principalSchema: "game",
                        principalTable: "characters",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_character_stats_stat_definitions_stat_key",
                        column: x => x.stat_key,
                        principalSchema: "game",
                        principalTable: "stat_definitions",
                        principalColumn: "key",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "class_stat_bases",
                schema: "game",
                columns: table => new
                {
                    class_code = table.Column<short>(type: "smallint", nullable: false),
                    stat_key = table.Column<string>(type: "character varying(32)", nullable: false),
                    value = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_class_stat_bases", x => new { x.class_code, x.stat_key });
                    table.CheckConstraint("ck_class_stat_bases_value", "value >= 0");
                    table.ForeignKey(
                        name: "fk_class_stat_bases_stat_definitions_stat_key",
                        column: x => x.stat_key,
                        principalSchema: "game",
                        principalTable: "stat_definitions",
                        principalColumn: "key",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                schema: "game",
                table: "stat_definitions",
                columns: new[] { "key", "name", "per_point", "sort", "upgradable" },
                values: new object[,]
                {
                    { "accuracy", "Accuracy", 1, (short)13, true },
                    { "air_attack", "Air Attack", 1, (short)25, true },
                    { "air_resistance", "Air Resistance", 1, (short)29, true },
                    { "attack_speed", "Attack Speed", 1, (short)15, true },
                    { "cast_speed", "Casting Speed", 1, (short)17, true },
                    { "concentration", "Concentration Rate", 1, (short)18, true },
                    { "critical_damage", "Critical Damage", 1, (short)11, true },
                    { "critical_rate", "Critical Rate", 1, (short)10, true },
                    { "critical_resistance", "Critical Resistance", 1, (short)12, true },
                    { "earth_attack", "Earth Attack", 1, (short)24, true },
                    { "earth_resistance", "Earth Resistance", 1, (short)28, true },
                    { "evasion", "Evasion", 1, (short)14, true },
                    { "fire_attack", "Fire Attack", 1, (short)22, true },
                    { "fire_resistance", "Fire Resistance", 1, (short)26, true },
                    { "guard_limit", "Guard Limit", 1, (short)19, true },
                    { "guard_recovery", "Guard Limit Recovery", 1, (short)20, true },
                    { "health", "Health", 1, (short)2, true },
                    { "intelligence", "Intelligence", 1, (short)1, true },
                    { "magic_attack", "Magic Attack", 1, (short)7, true },
                    { "magic_defense", "Magic Defense", 1, (short)9, true },
                    { "max_hp", "Maximum HP", 1, (short)4, true },
                    { "max_mp", "Maximum MP", 1, (short)5, true },
                    { "move_speed", "Movement Speed", 1, (short)16, true },
                    { "physical_attack", "Physical Attack", 1, (short)6, true },
                    { "physical_defense", "Physical Defense", 1, (short)8, true },
                    { "piercing", "Piercing Chance", 1, (short)21, true },
                    { "strength", "Strength", 1, (short)0, true },
                    { "water_attack", "Water Attack", 1, (short)23, true },
                    { "water_resistance", "Water Resistance", 1, (short)27, true },
                    { "wisdom", "Wisdom", 1, (short)3, true }
                });

            migrationBuilder.InsertData(
                schema: "game",
                table: "stat_point_levels",
                columns: new[] { "level", "points" },
                values: new object[,]
                {
                    { (short)1, 5 },
                    { (short)2, 5 },
                    { (short)3, 5 },
                    { (short)4, 5 },
                    { (short)5, 5 },
                    { (short)6, 5 },
                    { (short)7, 5 },
                    { (short)8, 5 },
                    { (short)9, 5 },
                    { (short)10, 5 },
                    { (short)11, 5 },
                    { (short)12, 5 },
                    { (short)13, 5 },
                    { (short)14, 5 },
                    { (short)15, 5 },
                    { (short)16, 5 },
                    { (short)17, 5 },
                    { (short)18, 5 },
                    { (short)19, 5 },
                    { (short)20, 5 },
                    { (short)21, 5 },
                    { (short)22, 5 },
                    { (short)23, 5 },
                    { (short)24, 5 },
                    { (short)25, 5 },
                    { (short)26, 5 },
                    { (short)27, 5 },
                    { (short)28, 5 },
                    { (short)29, 5 },
                    { (short)30, 5 },
                    { (short)31, 5 },
                    { (short)32, 5 },
                    { (short)33, 5 },
                    { (short)34, 5 },
                    { (short)35, 5 },
                    { (short)36, 5 },
                    { (short)37, 5 },
                    { (short)38, 5 },
                    { (short)39, 5 },
                    { (short)40, 5 },
                    { (short)41, 5 },
                    { (short)42, 5 },
                    { (short)43, 5 },
                    { (short)44, 5 },
                    { (short)45, 5 },
                    { (short)46, 5 },
                    { (short)47, 5 },
                    { (short)48, 5 },
                    { (short)49, 5 },
                    { (short)50, 5 },
                    { (short)51, 5 },
                    { (short)52, 5 },
                    { (short)53, 5 },
                    { (short)54, 5 },
                    { (short)55, 5 },
                    { (short)56, 5 },
                    { (short)57, 5 },
                    { (short)58, 5 },
                    { (short)59, 5 },
                    { (short)60, 5 },
                    { (short)61, 5 },
                    { (short)62, 5 },
                    { (short)63, 5 },
                    { (short)64, 5 },
                    { (short)65, 5 },
                    { (short)66, 5 },
                    { (short)67, 5 },
                    { (short)68, 5 },
                    { (short)69, 5 },
                    { (short)70, 5 },
                    { (short)71, 5 },
                    { (short)72, 5 },
                    { (short)73, 5 },
                    { (short)74, 5 },
                    { (short)75, 5 },
                    { (short)76, 5 },
                    { (short)77, 5 },
                    { (short)78, 5 },
                    { (short)79, 5 },
                    { (short)80, 5 },
                    { (short)81, 5 },
                    { (short)82, 5 },
                    { (short)83, 5 },
                    { (short)84, 5 },
                    { (short)85, 5 },
                    { (short)86, 5 },
                    { (short)87, 5 },
                    { (short)88, 5 },
                    { (short)89, 5 },
                    { (short)90, 5 },
                    { (short)91, 5 },
                    { (short)92, 5 },
                    { (short)93, 5 },
                    { (short)94, 5 },
                    { (short)95, 5 }
                });

            migrationBuilder.InsertData(
                schema: "game",
                table: "class_stat_bases",
                columns: new[] { "class_code", "stat_key", "value" },
                values: new object[,]
                {
                    { (short)0, "accuracy", 0 },
                    { (short)0, "air_attack", 0 },
                    { (short)0, "air_resistance", 0 },
                    { (short)0, "attack_speed", 0 },
                    { (short)0, "cast_speed", 0 },
                    { (short)0, "concentration", 0 },
                    { (short)0, "critical_damage", 0 },
                    { (short)0, "critical_rate", 0 },
                    { (short)0, "critical_resistance", 0 },
                    { (short)0, "earth_attack", 0 },
                    { (short)0, "earth_resistance", 0 },
                    { (short)0, "evasion", 0 },
                    { (short)0, "fire_attack", 0 },
                    { (short)0, "fire_resistance", 0 },
                    { (short)0, "guard_limit", 0 },
                    { (short)0, "guard_recovery", 0 },
                    { (short)0, "health", 10 },
                    { (short)0, "intelligence", 10 },
                    { (short)0, "magic_attack", 0 },
                    { (short)0, "magic_defense", 0 },
                    { (short)0, "max_hp", 100 },
                    { (short)0, "max_mp", 100 },
                    { (short)0, "move_speed", 0 },
                    { (short)0, "physical_attack", 0 },
                    { (short)0, "physical_defense", 0 },
                    { (short)0, "piercing", 0 },
                    { (short)0, "strength", 10 },
                    { (short)0, "water_attack", 0 },
                    { (short)0, "water_resistance", 0 },
                    { (short)0, "wisdom", 10 },
                    { (short)1, "accuracy", 0 },
                    { (short)1, "air_attack", 0 },
                    { (short)1, "air_resistance", 0 },
                    { (short)1, "attack_speed", 0 },
                    { (short)1, "cast_speed", 0 },
                    { (short)1, "concentration", 0 },
                    { (short)1, "critical_damage", 0 },
                    { (short)1, "critical_rate", 0 },
                    { (short)1, "critical_resistance", 0 },
                    { (short)1, "earth_attack", 0 },
                    { (short)1, "earth_resistance", 0 },
                    { (short)1, "evasion", 0 },
                    { (short)1, "fire_attack", 0 },
                    { (short)1, "fire_resistance", 0 },
                    { (short)1, "guard_limit", 0 },
                    { (short)1, "guard_recovery", 0 },
                    { (short)1, "health", 10 },
                    { (short)1, "intelligence", 10 },
                    { (short)1, "magic_attack", 0 },
                    { (short)1, "magic_defense", 0 },
                    { (short)1, "max_hp", 100 },
                    { (short)1, "max_mp", 100 },
                    { (short)1, "move_speed", 0 },
                    { (short)1, "physical_attack", 0 },
                    { (short)1, "physical_defense", 0 },
                    { (short)1, "piercing", 0 },
                    { (short)1, "strength", 10 },
                    { (short)1, "water_attack", 0 },
                    { (short)1, "water_resistance", 0 },
                    { (short)1, "wisdom", 10 },
                    { (short)2, "accuracy", 0 },
                    { (short)2, "air_attack", 0 },
                    { (short)2, "air_resistance", 0 },
                    { (short)2, "attack_speed", 0 },
                    { (short)2, "cast_speed", 0 },
                    { (short)2, "concentration", 0 },
                    { (short)2, "critical_damage", 0 },
                    { (short)2, "critical_rate", 0 },
                    { (short)2, "critical_resistance", 0 },
                    { (short)2, "earth_attack", 0 },
                    { (short)2, "earth_resistance", 0 },
                    { (short)2, "evasion", 0 },
                    { (short)2, "fire_attack", 0 },
                    { (short)2, "fire_resistance", 0 },
                    { (short)2, "guard_limit", 0 },
                    { (short)2, "guard_recovery", 0 },
                    { (short)2, "health", 10 },
                    { (short)2, "intelligence", 10 },
                    { (short)2, "magic_attack", 0 },
                    { (short)2, "magic_defense", 0 },
                    { (short)2, "max_hp", 100 },
                    { (short)2, "max_mp", 100 },
                    { (short)2, "move_speed", 0 },
                    { (short)2, "physical_attack", 0 },
                    { (short)2, "physical_defense", 0 },
                    { (short)2, "piercing", 0 },
                    { (short)2, "strength", 10 },
                    { (short)2, "water_attack", 0 },
                    { (short)2, "water_resistance", 0 },
                    { (short)2, "wisdom", 10 },
                    { (short)3, "accuracy", 0 },
                    { (short)3, "air_attack", 0 },
                    { (short)3, "air_resistance", 0 },
                    { (short)3, "attack_speed", 0 },
                    { (short)3, "cast_speed", 0 },
                    { (short)3, "concentration", 0 },
                    { (short)3, "critical_damage", 0 },
                    { (short)3, "critical_rate", 0 },
                    { (short)3, "critical_resistance", 0 },
                    { (short)3, "earth_attack", 0 },
                    { (short)3, "earth_resistance", 0 },
                    { (short)3, "evasion", 0 },
                    { (short)3, "fire_attack", 0 },
                    { (short)3, "fire_resistance", 0 },
                    { (short)3, "guard_limit", 0 },
                    { (short)3, "guard_recovery", 0 },
                    { (short)3, "health", 10 },
                    { (short)3, "intelligence", 10 },
                    { (short)3, "magic_attack", 0 },
                    { (short)3, "magic_defense", 0 },
                    { (short)3, "max_hp", 100 },
                    { (short)3, "max_mp", 100 },
                    { (short)3, "move_speed", 0 },
                    { (short)3, "physical_attack", 0 },
                    { (short)3, "physical_defense", 0 },
                    { (short)3, "piercing", 0 },
                    { (short)3, "strength", 10 },
                    { (short)3, "water_attack", 0 },
                    { (short)3, "water_resistance", 0 },
                    { (short)3, "wisdom", 10 },
                    { (short)4, "accuracy", 0 },
                    { (short)4, "air_attack", 0 },
                    { (short)4, "air_resistance", 0 },
                    { (short)4, "attack_speed", 0 },
                    { (short)4, "cast_speed", 0 },
                    { (short)4, "concentration", 0 },
                    { (short)4, "critical_damage", 0 },
                    { (short)4, "critical_rate", 0 },
                    { (short)4, "critical_resistance", 0 },
                    { (short)4, "earth_attack", 0 },
                    { (short)4, "earth_resistance", 0 },
                    { (short)4, "evasion", 0 },
                    { (short)4, "fire_attack", 0 },
                    { (short)4, "fire_resistance", 0 },
                    { (short)4, "guard_limit", 0 },
                    { (short)4, "guard_recovery", 0 },
                    { (short)4, "health", 10 },
                    { (short)4, "intelligence", 10 },
                    { (short)4, "magic_attack", 0 },
                    { (short)4, "magic_defense", 0 },
                    { (short)4, "max_hp", 100 },
                    { (short)4, "max_mp", 100 },
                    { (short)4, "move_speed", 0 },
                    { (short)4, "physical_attack", 0 },
                    { (short)4, "physical_defense", 0 },
                    { (short)4, "piercing", 0 },
                    { (short)4, "strength", 10 },
                    { (short)4, "water_attack", 0 },
                    { (short)4, "water_resistance", 0 },
                    { (short)4, "wisdom", 10 }
                });

            migrationBuilder.CreateIndex(
                name: "ix_character_stats_stat_key",
                schema: "game",
                table: "character_stats",
                column: "stat_key");

            migrationBuilder.CreateIndex(
                name: "ix_class_stat_bases_stat_key",
                schema: "game",
                table: "class_stat_bases",
                column: "stat_key");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "character_stats",
                schema: "game");

            migrationBuilder.DropTable(
                name: "class_stat_bases",
                schema: "game");

            migrationBuilder.DropTable(
                name: "stat_point_levels",
                schema: "game");

            migrationBuilder.DropTable(
                name: "stat_definitions",
                schema: "game");
        }
    }
}
