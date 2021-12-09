using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace CryptoStashStats.Migrations
{
    public partial class AddWalletBalancesTbl : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Wallets_Currencies_CurrencyId",
                schema: "financeSchema",
                table: "Wallets");

            migrationBuilder.DropIndex(
                name: "IX_Wallets_CurrencyId",
                schema: "financeSchema",
                table: "Wallets");

            migrationBuilder.DropColumn(
                name: "Balance",
                schema: "financeSchema",
                table: "Wallets");

            migrationBuilder.DropColumn(
                name: "CurrencyId",
                schema: "financeSchema",
                table: "Wallets");

            migrationBuilder.CreateTable(
                name: "CurrencyWallet",
                schema: "financeSchema",
                columns: table => new
                {
                    CurrenciesId = table.Column<int>(type: "integer", nullable: false),
                    WalletsId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CurrencyWallet", x => new { x.CurrenciesId, x.WalletsId });
                    table.ForeignKey(
                        name: "FK_CurrencyWallet_Currencies_CurrenciesId",
                        column: x => x.CurrenciesId,
                        principalSchema: "financeSchema",
                        principalTable: "Currencies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CurrencyWallet_Wallets_WalletsId",
                        column: x => x.WalletsId,
                        principalSchema: "financeSchema",
                        principalTable: "Wallets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WalletBalances",
                schema: "financeSchema",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    WalletId = table.Column<int>(type: "integer", nullable: true),
                    Created = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    LastModified = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    Savings = table.Column<double>(type: "double precision", nullable: false),
                    CurrencyId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WalletBalances", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WalletBalances_Currencies_CurrencyId",
                        column: x => x.CurrencyId,
                        principalSchema: "financeSchema",
                        principalTable: "Currencies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_WalletBalances_Wallets_WalletId",
                        column: x => x.WalletId,
                        principalSchema: "financeSchema",
                        principalTable: "Wallets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CurrencyWallet_WalletsId",
                schema: "financeSchema",
                table: "CurrencyWallet",
                column: "WalletsId");

            migrationBuilder.CreateIndex(
                name: "IX_WalletBalances_CurrencyId",
                schema: "financeSchema",
                table: "WalletBalances",
                column: "CurrencyId");

            migrationBuilder.CreateIndex(
                name: "IX_WalletBalances_WalletId",
                schema: "financeSchema",
                table: "WalletBalances",
                column: "WalletId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CurrencyWallet",
                schema: "financeSchema");

            migrationBuilder.DropTable(
                name: "WalletBalances",
                schema: "financeSchema");

            migrationBuilder.AddColumn<double>(
                name: "Balance",
                schema: "financeSchema",
                table: "Wallets",
                type: "double precision",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CurrencyId",
                schema: "financeSchema",
                table: "Wallets",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Wallets_CurrencyId",
                schema: "financeSchema",
                table: "Wallets",
                column: "CurrencyId");

            migrationBuilder.AddForeignKey(
                name: "FK_Wallets_Currencies_CurrencyId",
                schema: "financeSchema",
                table: "Wallets",
                column: "CurrencyId",
                principalSchema: "financeSchema",
                principalTable: "Currencies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
