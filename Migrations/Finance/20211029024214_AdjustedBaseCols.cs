using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CryptoStashStats.Migrations.Finance
{
    public partial class AdjustedBaseCols : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Created",
                table: "Coin");

            migrationBuilder.DropColumn(
                name: "LastModified",
                table: "Coin");

            //migrationBuilder.AddColumn<string>(
            //    name: "AuthJson",
            //    table: "Account",
            //    type: "text",
            //    nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Created",
                table: "Account",
                type: "timestamp without time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "LastModified",
                table: "Account",
                type: "timestamp without time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AuthJson",
                table: "Account");

            migrationBuilder.DropColumn(
                name: "Created",
                table: "Account");

            migrationBuilder.DropColumn(
                name: "LastModified",
                table: "Account");

            migrationBuilder.AddColumn<DateTime>(
                name: "Created",
                table: "Coin",
                type: "timestamp without time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "LastModified",
                table: "Coin",
                type: "timestamp without time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }
    }
}
