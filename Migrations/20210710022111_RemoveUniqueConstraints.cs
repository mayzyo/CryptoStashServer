using Microsoft.EntityFrameworkCore.Migrations;

namespace CryptoStashStats.Migrations
{
    public partial class RemoveUniqueConstraints : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Worker_WalletId",
                table: "Worker");

            migrationBuilder.DropIndex(
                name: "IX_Wallet_CoinId",
                table: "Wallet");

            migrationBuilder.CreateIndex(
                name: "IX_Worker_WalletId",
                table: "Worker",
                column: "WalletId");

            migrationBuilder.CreateIndex(
                name: "IX_Wallet_CoinId",
                table: "Wallet",
                column: "CoinId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Worker_WalletId",
                table: "Worker");

            migrationBuilder.DropIndex(
                name: "IX_Wallet_CoinId",
                table: "Wallet");

            migrationBuilder.CreateIndex(
                name: "IX_Worker_WalletId",
                table: "Worker",
                column: "WalletId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Wallet_CoinId",
                table: "Wallet",
                column: "CoinId",
                unique: true);
        }
    }
}
