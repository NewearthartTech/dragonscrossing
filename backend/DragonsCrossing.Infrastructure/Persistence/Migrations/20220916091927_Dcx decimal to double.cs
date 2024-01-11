using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DragonsCrossing.Infrastructure.Persistence.Migrations
{
    public partial class Dcxdecimaltodouble : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<double>(
                name: "Dcx",
                table: "HeroInventory",
                type: "float",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(12,9)",
                oldPrecision: 12,
                oldScale: 9);

            migrationBuilder.UpdateData(
                table: "HeroTemplate",
                keyColumn: "Id",
                keyValue: 1,
                column: "DateCreated",
                value: new DateTime(2022, 9, 16, 3, 19, 26, 567, DateTimeKind.Local).AddTicks(6971));

            migrationBuilder.UpdateData(
                table: "Player",
                keyColumn: "Id",
                keyValue: 1,
                column: "DateCreated",
                value: new DateTime(2022, 9, 16, 3, 19, 26, 567, DateTimeKind.Local).AddTicks(7183));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "Dcx",
                table: "HeroInventory",
                type: "decimal(12,9)",
                precision: 12,
                scale: 9,
                nullable: false,
                oldClrType: typeof(double),
                oldType: "float");

            migrationBuilder.UpdateData(
                table: "HeroTemplate",
                keyColumn: "Id",
                keyValue: 1,
                column: "DateCreated",
                value: new DateTime(2022, 9, 16, 2, 36, 57, 608, DateTimeKind.Local).AddTicks(5774));

            migrationBuilder.UpdateData(
                table: "Player",
                keyColumn: "Id",
                keyValue: 1,
                column: "DateCreated",
                value: new DateTime(2022, 9, 16, 2, 36, 57, 608, DateTimeKind.Local).AddTicks(5984));
        }
    }
}
