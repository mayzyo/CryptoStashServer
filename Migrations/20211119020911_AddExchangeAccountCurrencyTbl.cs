using Microsoft.EntityFrameworkCore.Migrations;

namespace CryptoStashStats.Migrations
{
    public partial class AddExchangeAccountCurrencyTbl : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CurrencyExchangeAccount",
                schema: "financeSchema",
                columns: table => new
                {
                    CurrenciesId = table.Column<int>(type: "integer", nullable: false),
                    ExchangeAccountsId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CurrencyExchangeAccount", x => new { x.CurrenciesId, x.ExchangeAccountsId });
                    table.ForeignKey(
                        name: "FK_CurrencyExchangeAccount_Currencies_CurrenciesId",
                        column: x => x.CurrenciesId,
                        principalSchema: "financeSchema",
                        principalTable: "Currencies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CurrencyExchangeAccount_ExchangeAccounts_ExchangeAccountsId",
                        column: x => x.ExchangeAccountsId,
                        principalSchema: "financeSchema",
                        principalTable: "ExchangeAccounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CurrencyExchangeAccount_ExchangeAccountsId",
                schema: "financeSchema",
                table: "CurrencyExchangeAccount",
                column: "ExchangeAccountsId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CurrencyExchangeAccount",
                schema: "financeSchema");
        }
    }
}
