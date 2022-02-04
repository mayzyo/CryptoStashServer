using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace CryptoStashServer.Migrations
{
    public partial class AddFinanceSchema : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "financeSchema");

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
                name: "Tokens",
                schema: "financeSchema",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Ticker = table.Column<string>(type: "text", nullable: true),
                    Address = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tokens", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ExchangeAccounts",
                schema: "financeSchema",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Owner = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
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
                name: "Blockchains",
                schema: "financeSchema",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    NativeTokenId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Blockchains", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Blockchains_Tokens_NativeTokenId",
                        column: x => x.NativeTokenId,
                        principalSchema: "financeSchema",
                        principalTable: "Tokens",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ExchangeRates",
                schema: "financeSchema",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Current = table.Column<double>(type: "double precision", nullable: false),
                    BuyerTokenId = table.Column<int>(type: "integer", nullable: true),
                    SellerTokenId = table.Column<int>(type: "integer", nullable: true),
                    CurrencyExchangeId = table.Column<int>(type: "integer", nullable: true),
                    Created = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    LastModified = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExchangeRates", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ExchangeRates_CurrencyExchanges_CurrencyExchangeId",
                        column: x => x.CurrencyExchangeId,
                        principalSchema: "financeSchema",
                        principalTable: "CurrencyExchanges",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ExchangeRates_Tokens_BuyerTokenId",
                        column: x => x.BuyerTokenId,
                        principalSchema: "financeSchema",
                        principalTable: "Tokens",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ExchangeRates_Tokens_SellerTokenId",
                        column: x => x.SellerTokenId,
                        principalSchema: "financeSchema",
                        principalTable: "Tokens",
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
                    TokenId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExchangeAccountBalances", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ExchangeAccountBalances_ExchangeAccounts_ExchangeAccountId",
                        column: x => x.ExchangeAccountId,
                        principalSchema: "financeSchema",
                        principalTable: "ExchangeAccounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ExchangeAccountBalances_Tokens_TokenId",
                        column: x => x.TokenId,
                        principalSchema: "financeSchema",
                        principalTable: "Tokens",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ExchangeAccountToken",
                schema: "financeSchema",
                columns: table => new
                {
                    ExchangeAccountsId = table.Column<int>(type: "integer", nullable: false),
                    TokensId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExchangeAccountToken", x => new { x.ExchangeAccountsId, x.TokensId });
                    table.ForeignKey(
                        name: "FK_ExchangeAccountToken_ExchangeAccounts_ExchangeAccountsId",
                        column: x => x.ExchangeAccountsId,
                        principalSchema: "financeSchema",
                        principalTable: "ExchangeAccounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ExchangeAccountToken_Tokens_TokensId",
                        column: x => x.TokensId,
                        principalSchema: "financeSchema",
                        principalTable: "Tokens",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BlockchainToken",
                schema: "financeSchema",
                columns: table => new
                {
                    BlockchainsId = table.Column<int>(type: "integer", nullable: false),
                    TokensId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BlockchainToken", x => new { x.BlockchainsId, x.TokensId });
                    table.ForeignKey(
                        name: "FK_BlockchainToken_Blockchains_BlockchainsId",
                        column: x => x.BlockchainsId,
                        principalSchema: "financeSchema",
                        principalTable: "Blockchains",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BlockchainToken_Tokens_TokensId",
                        column: x => x.TokensId,
                        principalSchema: "financeSchema",
                        principalTable: "Tokens",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
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
                    BlockchainId = table.Column<int>(type: "integer", nullable: false),
                    Created = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    LastModified = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Wallets", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Wallets_Blockchains_BlockchainId",
                        column: x => x.BlockchainId,
                        principalSchema: "financeSchema",
                        principalTable: "Blockchains",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TokenWallet",
                schema: "financeSchema",
                columns: table => new
                {
                    TokensId = table.Column<int>(type: "integer", nullable: false),
                    WalletsId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TokenWallet", x => new { x.TokensId, x.WalletsId });
                    table.ForeignKey(
                        name: "FK_TokenWallet_Tokens_TokensId",
                        column: x => x.TokensId,
                        principalSchema: "financeSchema",
                        principalTable: "Tokens",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TokenWallet_Wallets_WalletsId",
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
                    TokenId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WalletBalances", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WalletBalances_Tokens_TokenId",
                        column: x => x.TokenId,
                        principalSchema: "financeSchema",
                        principalTable: "Tokens",
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
                name: "IX_Blockchains_Name",
                schema: "financeSchema",
                table: "Blockchains",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Blockchains_NativeTokenId",
                schema: "financeSchema",
                table: "Blockchains",
                column: "NativeTokenId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_BlockchainToken_TokensId",
                schema: "financeSchema",
                table: "BlockchainToken",
                column: "TokensId");

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
                name: "IX_ExchangeAccountBalances_ExchangeAccountId",
                schema: "financeSchema",
                table: "ExchangeAccountBalances",
                column: "ExchangeAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_ExchangeAccountBalances_TokenId",
                schema: "financeSchema",
                table: "ExchangeAccountBalances",
                column: "TokenId");

            migrationBuilder.CreateIndex(
                name: "IX_ExchangeAccounts_CurrencyExchangeId",
                schema: "financeSchema",
                table: "ExchangeAccounts",
                column: "CurrencyExchangeId");

            migrationBuilder.CreateIndex(
                name: "IX_ExchangeAccountToken_TokensId",
                schema: "financeSchema",
                table: "ExchangeAccountToken",
                column: "TokensId");

            migrationBuilder.CreateIndex(
                name: "IX_ExchangeRates_BuyerTokenId",
                schema: "financeSchema",
                table: "ExchangeRates",
                column: "BuyerTokenId");

            migrationBuilder.CreateIndex(
                name: "IX_ExchangeRates_CurrencyExchangeId",
                schema: "financeSchema",
                table: "ExchangeRates",
                column: "CurrencyExchangeId");

            migrationBuilder.CreateIndex(
                name: "IX_ExchangeRates_SellerTokenId",
                schema: "financeSchema",
                table: "ExchangeRates",
                column: "SellerTokenId");

            migrationBuilder.CreateIndex(
                name: "IX_Tokens_Name_Ticker_Address",
                schema: "financeSchema",
                table: "Tokens",
                columns: new[] { "Name", "Ticker", "Address" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TokenWallet_WalletsId",
                schema: "financeSchema",
                table: "TokenWallet",
                column: "WalletsId");

            migrationBuilder.CreateIndex(
                name: "IX_WalletBalances_TokenId",
                schema: "financeSchema",
                table: "WalletBalances",
                column: "TokenId");

            migrationBuilder.CreateIndex(
                name: "IX_WalletBalances_WalletId",
                schema: "financeSchema",
                table: "WalletBalances",
                column: "WalletId");

            migrationBuilder.CreateIndex(
                name: "IX_Wallets_Address_BlockchainId",
                schema: "financeSchema",
                table: "Wallets",
                columns: new[] { "Address", "BlockchainId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Wallets_BlockchainId",
                schema: "financeSchema",
                table: "Wallets",
                column: "BlockchainId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BlockchainToken",
                schema: "financeSchema");

            migrationBuilder.DropTable(
                name: "ExchangeAccountApiKeys",
                schema: "financeSchema");

            migrationBuilder.DropTable(
                name: "ExchangeAccountBalances",
                schema: "financeSchema");

            migrationBuilder.DropTable(
                name: "ExchangeAccountToken",
                schema: "financeSchema");

            migrationBuilder.DropTable(
                name: "ExchangeRates",
                schema: "financeSchema");

            migrationBuilder.DropTable(
                name: "TokenWallet",
                schema: "financeSchema");

            migrationBuilder.DropTable(
                name: "WalletBalances",
                schema: "financeSchema");

            migrationBuilder.DropTable(
                name: "ExchangeAccounts",
                schema: "financeSchema");

            migrationBuilder.DropTable(
                name: "Wallets",
                schema: "financeSchema");

            migrationBuilder.DropTable(
                name: "CurrencyExchanges",
                schema: "financeSchema");

            migrationBuilder.DropTable(
                name: "Blockchains",
                schema: "financeSchema");

            migrationBuilder.DropTable(
                name: "Tokens",
                schema: "financeSchema");
        }
    }
}
