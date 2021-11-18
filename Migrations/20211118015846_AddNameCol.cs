using Microsoft.EntityFrameworkCore.Migrations;

namespace CryptoStashStats.Migrations
{
    public partial class AddNameCol : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Name",
                schema: "financeSchema",
                table: "ExchangeAccounts",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Name",
                schema: "financeSchema",
                table: "ExchangeAccounts");
        }
    }
}
