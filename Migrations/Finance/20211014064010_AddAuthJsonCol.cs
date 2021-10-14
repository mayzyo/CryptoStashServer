using Microsoft.EntityFrameworkCore.Migrations;

namespace CryptoStashStats.Migrations.Finance
{
    public partial class AddAuthJsonCol : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AuthJson",
                table: "Account",
                type: "text",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AuthJson",
                table: "Account");
        }
    }
}
