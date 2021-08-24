using Microsoft.EntityFrameworkCore.Migrations;

namespace CryptoStashStats.Migrations
{
    public partial class AddAPNCol : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Apn",
                table: "User",
                type: "text",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Apn",
                table: "User");
        }
    }
}
