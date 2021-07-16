using Microsoft.EntityFrameworkCore.Migrations;

namespace CryptoStashStats.Migrations.Miner
{
    public partial class EditWorkerTbl : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Hashrate_Worker_WorkerId",
                table: "Hashrate");

            migrationBuilder.DropIndex(
                name: "IX_Worker_Name",
                table: "Worker");

            migrationBuilder.AlterColumn<int>(
                name: "WorkerId",
                table: "Hashrate",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Worker_Address",
                table: "Worker",
                column: "Address",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Hashrate_Worker_WorkerId",
                table: "Hashrate",
                column: "WorkerId",
                principalTable: "Worker",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Hashrate_Worker_WorkerId",
                table: "Hashrate");

            migrationBuilder.DropIndex(
                name: "IX_Worker_Address",
                table: "Worker");

            migrationBuilder.AlterColumn<int>(
                name: "WorkerId",
                table: "Hashrate",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.CreateIndex(
                name: "IX_Worker_Name",
                table: "Worker",
                column: "Name",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Hashrate_Worker_WorkerId",
                table: "Hashrate",
                column: "WorkerId",
                principalTable: "Worker",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
