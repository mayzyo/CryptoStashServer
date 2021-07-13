using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace CryptoStashStats.Migrations.Finance
{
    public partial class AddFinanceTbls : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Coin",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: true),
                    Ticker = table.Column<string>(type: "text", nullable: false),
                    Created = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    LastModified = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Coin", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Provider",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Provider", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Wallet",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<int>(type: "integer", nullable: true),
                    CoinId = table.Column<int>(type: "integer", nullable: true),
                    Address = table.Column<string>(type: "text", nullable: false),
                    Balance = table.Column<double>(type: "double precision", nullable: false),
                    Created = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    LastModified = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Wallet", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Wallet_Coin_CoinId",
                        column: x => x.CoinId,
                        principalTable: "Coin",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Account",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<int>(type: "integer", nullable: true),
                    ProviderId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Account", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Account_Provider_ProviderId",
                        column: x => x.ProviderId,
                        principalTable: "Provider",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AccountBalance",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Current = table.Column<double>(type: "double precision", nullable: false),
                    AccountId = table.Column<int>(type: "integer", nullable: false),
                    CoinId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccountBalance", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AccountBalance_Account_AccountId",
                        column: x => x.AccountId,
                        principalTable: "Account",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AccountBalance_Coin_CoinId",
                        column: x => x.CoinId,
                        principalTable: "Coin",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Account_ProviderId",
                table: "Account",
                column: "ProviderId");

            migrationBuilder.CreateIndex(
                name: "IX_AccountBalance_AccountId_CoinId",
                table: "AccountBalance",
                columns: new[] { "AccountId", "CoinId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AccountBalance_CoinId",
                table: "AccountBalance",
                column: "CoinId");

            migrationBuilder.CreateIndex(
                name: "IX_Coin_Name",
                table: "Coin",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Coin_Ticker",
                table: "Coin",
                column: "Ticker",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Provider_Name",
                table: "Provider",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Wallet_Address",
                table: "Wallet",
                column: "Address",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Wallet_CoinId",
                table: "Wallet",
                column: "CoinId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AccountBalance");

            migrationBuilder.DropTable(
                name: "Wallet");

            migrationBuilder.DropTable(
                name: "Account");

            migrationBuilder.DropTable(
                name: "Coin");

            migrationBuilder.DropTable(
                name: "Provider");
        }
    }
}
