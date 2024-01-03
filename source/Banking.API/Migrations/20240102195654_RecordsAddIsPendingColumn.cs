using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Banking.API.Migrations
{
    /// <inheritdoc />
    public partial class RecordsAddIsPendingColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsPending",
                schema: "BankingSchema",
                table: "Records",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsPending",
                schema: "BankingSchema",
                table: "Records");
        }
    }
}
