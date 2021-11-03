using Microsoft.EntityFrameworkCore.Migrations;

namespace CryptoStashStats.Migrations.Miner
{
    public partial class AddOwnerColToPoolBalance : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Owner",
                table: "PoolBalance",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Owner",
                table: "PoolBalance");
        }
    }
}
