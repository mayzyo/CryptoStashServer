using Microsoft.EntityFrameworkCore.Migrations;

namespace CryptoStashStats.Migrations.Finance
{
    public partial class AddOwnerColToWallet : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Owner",
                table: "Wallet",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Owner",
                table: "Wallet");
        }
    }
}
