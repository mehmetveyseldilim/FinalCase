using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Banking.API.Migrations
{
    /// <inheritdoc />
    public partial class RecordsAddUserIdColumnAndAccountIdNullable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsCompleted",
                schema: "BankingSchema",
                table: "Records");

            migrationBuilder.AlterColumn<int>(
                name: "AccountId",
                schema: "BankingSchema",
                table: "Records",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddColumn<int>(
                name: "UserId",
                schema: "BankingSchema",
                table: "Records",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserId",
                schema: "BankingSchema",
                table: "Records");

            migrationBuilder.AlterColumn<int>(
                name: "AccountId",
                schema: "BankingSchema",
                table: "Records",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsCompleted",
                schema: "BankingSchema",
                table: "Records",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }
    }
}
