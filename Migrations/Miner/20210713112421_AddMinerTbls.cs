using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace CryptoStashStats.Migrations.Miner
{
    public partial class AddMinerTbls : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MiningPool",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Created = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    LastModified = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MiningPool", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Payout",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    MiningPoolId = table.Column<int>(type: "integer", nullable: true),
                    Address = table.Column<string>(type: "text", nullable: true),
                    TXHash = table.Column<string>(type: "text", nullable: false),
                    Amount = table.Column<double>(type: "double precision", nullable: false),
                    Confirmed = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    IsConfirmed = table.Column<bool>(type: "boolean", nullable: false),
                    ConfirmAttempts = table.Column<int>(type: "integer", nullable: false),
                    Created = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    LastModified = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Payout", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Payout_MiningPool_MiningPoolId",
                        column: x => x.MiningPoolId,
                        principalTable: "MiningPool",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PoolBalance",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Current = table.Column<double>(type: "double precision", nullable: false),
                    MiningPoolId = table.Column<int>(type: "integer", nullable: false),
                    Address = table.Column<string>(type: "text", nullable: false),
                    Created = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    LastModified = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PoolBalance", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PoolBalance_MiningPool_MiningPoolId",
                        column: x => x.MiningPoolId,
                        principalTable: "MiningPool",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Worker",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Address = table.Column<string>(type: "text", nullable: false),
                    MiningPoolId = table.Column<int>(type: "integer", nullable: true),
                    Created = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    LastModified = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Worker", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Worker_MiningPool_MiningPoolId",
                        column: x => x.MiningPoolId,
                        principalTable: "MiningPool",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
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
                    WorkerId = table.Column<int>(type: "integer", nullable: true),
                    Created = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    LastModified = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
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
                name: "IX_Hashrate_WorkerId",
                table: "Hashrate",
                column: "WorkerId");

            migrationBuilder.CreateIndex(
                name: "IX_MiningPool_Name",
                table: "MiningPool",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Payout_MiningPoolId",
                table: "Payout",
                column: "MiningPoolId");

            migrationBuilder.CreateIndex(
                name: "IX_Payout_TXHash",
                table: "Payout",
                column: "TXHash",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PoolBalance_MiningPoolId_Address",
                table: "PoolBalance",
                columns: new[] { "MiningPoolId", "Address" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Worker_MiningPoolId",
                table: "Worker",
                column: "MiningPoolId");

            migrationBuilder.CreateIndex(
                name: "IX_Worker_Name",
                table: "Worker",
                column: "Name",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Hashrate");

            migrationBuilder.DropTable(
                name: "Payout");

            migrationBuilder.DropTable(
                name: "PoolBalance");

            migrationBuilder.DropTable(
                name: "Worker");

            migrationBuilder.DropTable(
                name: "MiningPool");
        }
    }
}
