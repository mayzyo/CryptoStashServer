using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace CryptoStashStats.Migrations
{
    public partial class FinanceSchema : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "financeSchema");

            migrationBuilder.CreateTable(
                name: "Currencies",
                schema: "financeSchema",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Ticker = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Currencies", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CurrencyExchanges",
                schema: "financeSchema",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Created = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    LastModified = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CurrencyExchanges", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Wallets",
                schema: "financeSchema",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Owner = table.Column<string>(type: "text", nullable: false),
                    Address = table.Column<string>(type: "text", nullable: false),
                    Balance = table.Column<double>(type: "double precision", nullable: true),
                    CurrencyId = table.Column<int>(type: "integer", nullable: true),
                    Created = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    LastModified = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Wallets", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Wallets_Currencies_CurrencyId",
                        column: x => x.CurrencyId,
                        principalSchema: "financeSchema",
                        principalTable: "Currencies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ExchangeAccounts",
                schema: "financeSchema",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Owner = table.Column<string>(type: "text", nullable: false),
                    CurrencyExchangeId = table.Column<int>(type: "integer", nullable: false),
                    Created = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    LastModified = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExchangeAccounts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ExchangeAccounts_CurrencyExchanges_CurrencyExchangeId",
                        column: x => x.CurrencyExchangeId,
                        principalSchema: "financeSchema",
                        principalTable: "CurrencyExchanges",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ExchangeRates",
                schema: "financeSchema",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Current = table.Column<double>(type: "double precision", nullable: false),
                    BuyerCurrencyId = table.Column<int>(type: "integer", nullable: true),
                    SellerCurrencyId = table.Column<int>(type: "integer", nullable: true),
                    CurrencyExchangeId = table.Column<int>(type: "integer", nullable: true),
                    Created = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    LastModified = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExchangeRates", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ExchangeRates_Currencies_BuyerCurrencyId",
                        column: x => x.BuyerCurrencyId,
                        principalSchema: "financeSchema",
                        principalTable: "Currencies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ExchangeRates_Currencies_SellerCurrencyId",
                        column: x => x.SellerCurrencyId,
                        principalSchema: "financeSchema",
                        principalTable: "Currencies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ExchangeRates_CurrencyExchanges_CurrencyExchangeId",
                        column: x => x.CurrencyExchangeId,
                        principalSchema: "financeSchema",
                        principalTable: "CurrencyExchanges",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ExchangeAccountApiKeys",
                schema: "financeSchema",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PublicKey = table.Column<string>(type: "text", nullable: false),
                    PrivateKey = table.Column<string>(type: "text", nullable: false),
                    ExchangeAccountId = table.Column<int>(type: "integer", nullable: false),
                    Created = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    LastModified = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExchangeAccountApiKeys", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ExchangeAccountApiKeys_ExchangeAccounts_ExchangeAccountId",
                        column: x => x.ExchangeAccountId,
                        principalSchema: "financeSchema",
                        principalTable: "ExchangeAccounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ExchangeAccountBalances",
                schema: "financeSchema",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ExchangeAccountId = table.Column<int>(type: "integer", nullable: true),
                    Created = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    LastModified = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    Savings = table.Column<double>(type: "double precision", nullable: false),
                    CurrencyId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExchangeAccountBalances", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ExchangeAccountBalances_Currencies_CurrencyId",
                        column: x => x.CurrencyId,
                        principalSchema: "financeSchema",
                        principalTable: "Currencies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ExchangeAccountBalances_ExchangeAccounts_ExchangeAccountId",
                        column: x => x.ExchangeAccountId,
                        principalSchema: "financeSchema",
                        principalTable: "ExchangeAccounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Currencies_Ticker_Name",
                schema: "financeSchema",
                table: "Currencies",
                columns: new[] { "Ticker", "Name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CurrencyExchanges_Name",
                schema: "financeSchema",
                table: "CurrencyExchanges",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ExchangeAccountApiKeys_ExchangeAccountId",
                schema: "financeSchema",
                table: "ExchangeAccountApiKeys",
                column: "ExchangeAccountId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ExchangeAccountApiKeys_PublicKey_PrivateKey",
                schema: "financeSchema",
                table: "ExchangeAccountApiKeys",
                columns: new[] { "PublicKey", "PrivateKey" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ExchangeAccountBalances_CurrencyId",
                schema: "financeSchema",
                table: "ExchangeAccountBalances",
                column: "CurrencyId");

            migrationBuilder.CreateIndex(
                name: "IX_ExchangeAccountBalances_ExchangeAccountId",
                schema: "financeSchema",
                table: "ExchangeAccountBalances",
                column: "ExchangeAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_ExchangeAccounts_CurrencyExchangeId",
                schema: "financeSchema",
                table: "ExchangeAccounts",
                column: "CurrencyExchangeId");

            migrationBuilder.CreateIndex(
                name: "IX_ExchangeRates_BuyerCurrencyId",
                schema: "financeSchema",
                table: "ExchangeRates",
                column: "BuyerCurrencyId");

            migrationBuilder.CreateIndex(
                name: "IX_ExchangeRates_CurrencyExchangeId",
                schema: "financeSchema",
                table: "ExchangeRates",
                column: "CurrencyExchangeId");

            migrationBuilder.CreateIndex(
                name: "IX_ExchangeRates_SellerCurrencyId",
                schema: "financeSchema",
                table: "ExchangeRates",
                column: "SellerCurrencyId");

            migrationBuilder.CreateIndex(
                name: "IX_Wallets_Address",
                schema: "financeSchema",
                table: "Wallets",
                column: "Address",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Wallets_CurrencyId",
                schema: "financeSchema",
                table: "Wallets",
                column: "CurrencyId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ExchangeAccountApiKeys",
                schema: "financeSchema");

            migrationBuilder.DropTable(
                name: "ExchangeAccountBalances",
                schema: "financeSchema");

            migrationBuilder.DropTable(
                name: "ExchangeRates",
                schema: "financeSchema");

            migrationBuilder.DropTable(
                name: "Wallets",
                schema: "financeSchema");

            migrationBuilder.DropTable(
                name: "ExchangeAccounts",
                schema: "financeSchema");

            migrationBuilder.DropTable(
                name: "Currencies",
                schema: "financeSchema");

            migrationBuilder.DropTable(
                name: "CurrencyExchanges",
                schema: "financeSchema");
        }
    }
}
