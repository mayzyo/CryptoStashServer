using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CryptoStashStats.Migrations.Miner
{
    public partial class AdjustedBaseCols : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Created",
                table: "PoolBalance");

            migrationBuilder.DropColumn(
                name: "LastModified",
                table: "PoolBalance");

            migrationBuilder.DropColumn(
                name: "Created",
                table: "MiningPool");

            migrationBuilder.DropColumn(
                name: "LastModified",
                table: "MiningPool");

            migrationBuilder.AddColumn<string>(
                name: "Owner",
                table: "Worker",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Owner",
                table: "Worker");

            migrationBuilder.AddColumn<DateTime>(
                name: "Created",
                table: "PoolBalance",
                type: "timestamp without time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "LastModified",
                table: "PoolBalance",
                type: "timestamp without time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "Created",
                table: "MiningPool",
                type: "timestamp without time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "LastModified",
                table: "MiningPool",
                type: "timestamp without time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }
    }
}
