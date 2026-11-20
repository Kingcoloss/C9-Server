using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace C9.GameData.Migrations
{
    /// <inheritdoc />
    public partial class AccountSecondPassword : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "account_second_passwords",
                schema: "game",
                columns: table => new
                {
                    account_id = table.Column<long>(type: "bigint", nullable: false),
                    password_hash = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    question = table.Column<short>(type: "smallint", nullable: false),
                    answer_hash = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    fail_count = table.Column<short>(type: "smallint", nullable: false),
                    locked_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_account_second_passwords", x => x.account_id);
                    table.CheckConstraint("ck_account_second_passwords_fail_count", "fail_count BETWEEN 0 AND 5");
                    table.CheckConstraint("ck_account_second_passwords_question", "question BETWEEN 0 AND 9");
                    table.ForeignKey(
                        name: "fk_account_second_passwords_accounts_account_id",
                        column: x => x.account_id,
                        principalSchema: "game",
                        principalTable: "accounts",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "account_second_passwords",
                schema: "game");
        }
    }
}
