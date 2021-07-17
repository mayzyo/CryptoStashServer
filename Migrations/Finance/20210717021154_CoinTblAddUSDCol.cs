using Microsoft.EntityFrameworkCore.Migrations;

namespace CryptoStashStats.Migrations.Finance
{
    public partial class CoinTblAddUSDCol : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "USD",
                table: "Coin",
                type: "double precision",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "USD",
                table: "Coin");
        }
    }
}
