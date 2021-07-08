using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace CryptoStashStats.Migrations
{
    public partial class AddPayoutTbl : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Payout",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    MiningPoolId = table.Column<int>(type: "integer", nullable: false),
                    WalletId = table.Column<int>(type: "integer", nullable: false),
                    TXHash = table.Column<string>(type: "text", nullable: false),
                    Amount = table.Column<double>(type: "double precision", nullable: false),
                    Confirmed = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    IsConfirmed = table.Column<bool>(type: "boolean", nullable: false),
                    ConfirmAttempts = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Payout", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Payout_MiningPool_MiningPoolId",
                        column: x => x.MiningPoolId,
                        principalTable: "MiningPool",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Payout_Wallet_WalletId",
                        column: x => x.WalletId,
                        principalTable: "Wallet",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Payout_MiningPoolId_WalletId",
                table: "Payout",
                columns: new[] { "MiningPoolId", "WalletId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Payout_TXHash",
                table: "Payout",
                column: "TXHash",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Payout_WalletId",
                table: "Payout",
                column: "WalletId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Payout");
        }
    }
}
