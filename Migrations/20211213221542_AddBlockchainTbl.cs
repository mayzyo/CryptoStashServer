using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace CryptoStashStats.Migrations
{
    public partial class AddBlockchainTbl : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Wallets_Address",
                schema: "financeSchema",
                table: "Wallets");

            migrationBuilder.AddColumn<int>(
                name: "BlockchainId",
                schema: "financeSchema",
                table: "Wallets",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Blockchains",
                schema: "financeSchema",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    NativeCurrencyId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Blockchains", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Blockchains_Currencies_NativeCurrencyId",
                        column: x => x.NativeCurrencyId,
                        principalSchema: "financeSchema",
                        principalTable: "Currencies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "BlockchainCurrency",
                schema: "financeSchema",
                columns: table => new
                {
                    BlockchainsId = table.Column<int>(type: "integer", nullable: false),
                    CurrenciesId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BlockchainCurrency", x => new { x.BlockchainsId, x.CurrenciesId });
                    table.ForeignKey(
                        name: "FK_BlockchainCurrency_Blockchains_BlockchainsId",
                        column: x => x.BlockchainsId,
                        principalSchema: "financeSchema",
                        principalTable: "Blockchains",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BlockchainCurrency_Currencies_CurrenciesId",
                        column: x => x.CurrenciesId,
                        principalSchema: "financeSchema",
                        principalTable: "Currencies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

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

            migrationBuilder.CreateIndex(
                name: "IX_BlockchainCurrency_CurrenciesId",
                schema: "financeSchema",
                table: "BlockchainCurrency",
                column: "CurrenciesId");

            migrationBuilder.CreateIndex(
                name: "IX_Blockchains_Name",
                schema: "financeSchema",
                table: "Blockchains",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Blockchains_NativeCurrencyId",
                schema: "financeSchema",
                table: "Blockchains",
                column: "NativeCurrencyId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Wallets_Blockchains_BlockchainId",
                schema: "financeSchema",
                table: "Wallets",
                column: "BlockchainId",
                principalSchema: "financeSchema",
                principalTable: "Blockchains",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Wallets_Blockchains_BlockchainId",
                schema: "financeSchema",
                table: "Wallets");

            migrationBuilder.DropTable(
                name: "BlockchainCurrency",
                schema: "financeSchema");

            migrationBuilder.DropTable(
                name: "Blockchains",
                schema: "financeSchema");

            migrationBuilder.DropIndex(
                name: "IX_Wallets_Address_BlockchainId",
                schema: "financeSchema",
                table: "Wallets");

            migrationBuilder.DropIndex(
                name: "IX_Wallets_BlockchainId",
                schema: "financeSchema",
                table: "Wallets");

            migrationBuilder.DropColumn(
                name: "BlockchainId",
                schema: "financeSchema",
                table: "Wallets");

            migrationBuilder.CreateIndex(
                name: "IX_Wallets_Address",
                schema: "financeSchema",
                table: "Wallets",
                column: "Address",
                unique: true);
        }
    }
}
