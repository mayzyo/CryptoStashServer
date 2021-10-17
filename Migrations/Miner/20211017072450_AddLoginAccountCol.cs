using Microsoft.EntityFrameworkCore.Migrations;

namespace CryptoStashStats.Migrations.Miner
{
    public partial class AddLoginAccountCol : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "LoginAccount",
                table: "PoolBalance",
                type: "text",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LoginAccount",
                table: "PoolBalance");
        }
    }
}
