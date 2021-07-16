using Microsoft.EntityFrameworkCore.Migrations;

namespace CryptoStashStats.Migrations.Miner
{
    public partial class EditWorkerTbl2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Worker_Address",
                table: "Worker");

            migrationBuilder.CreateIndex(
                name: "IX_Worker_Name_Address",
                table: "Worker",
                columns: new[] { "Name", "Address" },
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Worker_Name_Address",
                table: "Worker");

            migrationBuilder.CreateIndex(
                name: "IX_Worker_Address",
                table: "Worker",
                column: "Address",
                unique: true);
        }
    }
}
