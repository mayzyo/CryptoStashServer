using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace CryptoStashServer.Migrations.Mining
{
    public partial class AddMiningSchema : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "miningSchema");

            migrationBuilder.CreateTable(
                name: "MiningPools",
                schema: "miningSchema",
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
                    table.PrimaryKey("PK_MiningPools", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Tokens",
                schema: "miningSchema",
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
                name: "MiningAccounts",
                schema: "miningSchema",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Owner = table.Column<string>(type: "text", nullable: false),
                    Identifier = table.Column<string>(type: "text", nullable: false),
                    MiningPoolId = table.Column<int>(type: "integer", nullable: false),
                    Created = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    LastModified = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MiningAccounts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MiningAccounts_MiningPools_MiningPoolId",
                        column: x => x.MiningPoolId,
                        principalSchema: "miningSchema",
                        principalTable: "MiningPools",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MiningPoolToken",
                schema: "miningSchema",
                columns: table => new
                {
                    MiningPoolsId = table.Column<int>(type: "integer", nullable: false),
                    TokensId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MiningPoolToken", x => new { x.MiningPoolsId, x.TokensId });
                    table.ForeignKey(
                        name: "FK_MiningPoolToken_MiningPools_MiningPoolsId",
                        column: x => x.MiningPoolsId,
                        principalSchema: "miningSchema",
                        principalTable: "MiningPools",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MiningPoolToken_Tokens_TokensId",
                        column: x => x.TokensId,
                        principalSchema: "miningSchema",
                        principalTable: "Tokens",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MiningAccountBalances",
                schema: "miningSchema",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    MiningAccountId = table.Column<int>(type: "integer", nullable: true),
                    Created = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    LastModified = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    Savings = table.Column<double>(type: "double precision", nullable: false),
                    TokenId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MiningAccountBalances", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MiningAccountBalances_MiningAccounts_MiningAccountId",
                        column: x => x.MiningAccountId,
                        principalSchema: "miningSchema",
                        principalTable: "MiningAccounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MiningAccountBalances_Tokens_TokenId",
                        column: x => x.TokenId,
                        principalSchema: "miningSchema",
                        principalTable: "Tokens",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MiningWorkers",
                schema: "miningSchema",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    MiningAccountId = table.Column<int>(type: "integer", nullable: false),
                    Created = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    LastModified = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MiningWorkers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MiningWorkers_MiningAccounts_MiningAccountId",
                        column: x => x.MiningAccountId,
                        principalSchema: "miningSchema",
                        principalTable: "MiningAccounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MiningWorkerHashRates",
                schema: "miningSchema",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Current = table.Column<int>(type: "integer", nullable: false),
                    Average = table.Column<int>(type: "integer", nullable: false),
                    Reported = table.Column<int>(type: "integer", nullable: false),
                    MiningWorkerId = table.Column<int>(type: "integer", nullable: false),
                    Created = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    LastModified = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MiningWorkerHashRates", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MiningWorkerHashRates_MiningWorkers_MiningWorkerId",
                        column: x => x.MiningWorkerId,
                        principalSchema: "miningSchema",
                        principalTable: "MiningWorkers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MiningAccountBalances_MiningAccountId",
                schema: "miningSchema",
                table: "MiningAccountBalances",
                column: "MiningAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_MiningAccountBalances_TokenId",
                schema: "miningSchema",
                table: "MiningAccountBalances",
                column: "TokenId");

            migrationBuilder.CreateIndex(
                name: "IX_MiningAccounts_Identifier_Owner_MiningPoolId",
                schema: "miningSchema",
                table: "MiningAccounts",
                columns: new[] { "Identifier", "Owner", "MiningPoolId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MiningAccounts_MiningPoolId",
                schema: "miningSchema",
                table: "MiningAccounts",
                column: "MiningPoolId");

            migrationBuilder.CreateIndex(
                name: "IX_MiningPools_Name",
                schema: "miningSchema",
                table: "MiningPools",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MiningPoolToken_TokensId",
                schema: "miningSchema",
                table: "MiningPoolToken",
                column: "TokensId");

            migrationBuilder.CreateIndex(
                name: "IX_MiningWorkerHashRates_MiningWorkerId",
                schema: "miningSchema",
                table: "MiningWorkerHashRates",
                column: "MiningWorkerId");

            migrationBuilder.CreateIndex(
                name: "IX_MiningWorkers_MiningAccountId",
                schema: "miningSchema",
                table: "MiningWorkers",
                column: "MiningAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_MiningWorkers_Name_MiningAccountId",
                schema: "miningSchema",
                table: "MiningWorkers",
                columns: new[] { "Name", "MiningAccountId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Tokens_Name_Ticker_Address",
                schema: "miningSchema",
                table: "Tokens",
                columns: new[] { "Name", "Ticker", "Address" },
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MiningAccountBalances",
                schema: "miningSchema");

            migrationBuilder.DropTable(
                name: "MiningPoolToken",
                schema: "miningSchema");

            migrationBuilder.DropTable(
                name: "MiningWorkerHashRates",
                schema: "miningSchema");

            migrationBuilder.DropTable(
                name: "Tokens",
                schema: "miningSchema");

            migrationBuilder.DropTable(
                name: "MiningWorkers",
                schema: "miningSchema");

            migrationBuilder.DropTable(
                name: "MiningAccounts",
                schema: "miningSchema");

            migrationBuilder.DropTable(
                name: "MiningPools",
                schema: "miningSchema");
        }
    }
}
