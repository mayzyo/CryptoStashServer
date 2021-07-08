using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace CryptoStashStats.Migrations
{
    public partial class CreateMinerDB : Migration
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
                name: "Wallet",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Address = table.Column<string>(type: "text", nullable: false),
                    CoinId = table.Column<int>(type: "integer", nullable: true),
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
                name: "Worker",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    WalletId = table.Column<int>(type: "integer", nullable: false),
                    Created = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    LastModified = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Worker", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Worker_Wallet_WalletId",
                        column: x => x.WalletId,
                        principalTable: "Wallet",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Hashrate",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Current = table.Column<int>(type: "integer", nullable: false),
                    Average = table.Column<int>(type: "integer", nullable: false),
                    Reported = table.Column<int>(type: "integer", nullable: false),
                    WorkerId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Hashrate", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Hashrate_Worker_WorkerId",
                        column: x => x.WorkerId,
                        principalTable: "Worker",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

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
                name: "IX_Hashrate_WorkerId",
                table: "Hashrate",
                column: "WorkerId");

            migrationBuilder.CreateIndex(
                name: "IX_Wallet_Address",
                table: "Wallet",
                column: "Address",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Wallet_CoinId",
                table: "Wallet",
                column: "CoinId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Worker_Name",
                table: "Worker",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Worker_WalletId",
                table: "Worker",
                column: "WalletId",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Hashrate");

            migrationBuilder.DropTable(
                name: "Worker");

            migrationBuilder.DropTable(
                name: "Wallet");

            migrationBuilder.DropTable(
                name: "Coin");
        }
    }
}
