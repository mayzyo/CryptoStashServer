using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace CryptoStashStats.Migrations
{
    public partial class AddPoolTbls : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "MiningPoolId",
                table: "Worker",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "MiningPool",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MiningPool", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PoolBalance",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Current = table.Column<int>(type: "integer", nullable: false),
                    MiningPoolId = table.Column<int>(type: "integer", nullable: false),
                    WalletId = table.Column<int>(type: "integer", nullable: false)
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
                    table.ForeignKey(
                        name: "FK_PoolBalance_Wallet_WalletId",
                        column: x => x.WalletId,
                        principalTable: "Wallet",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Worker_MiningPoolId",
                table: "Worker",
                column: "MiningPoolId");

            migrationBuilder.CreateIndex(
                name: "IX_MiningPool_Name",
                table: "MiningPool",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PoolBalance_MiningPoolId_WalletId",
                table: "PoolBalance",
                columns: new[] { "MiningPoolId", "WalletId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PoolBalance_WalletId",
                table: "PoolBalance",
                column: "WalletId");

            migrationBuilder.AddForeignKey(
                name: "FK_Worker_MiningPool_MiningPoolId",
                table: "Worker",
                column: "MiningPoolId",
                principalTable: "MiningPool",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Worker_MiningPool_MiningPoolId",
                table: "Worker");

            migrationBuilder.DropTable(
                name: "PoolBalance");

            migrationBuilder.DropTable(
                name: "MiningPool");

            migrationBuilder.DropIndex(
                name: "IX_Worker_MiningPoolId",
                table: "Worker");

            migrationBuilder.DropColumn(
                name: "MiningPoolId",
                table: "Worker");
        }
    }
}
