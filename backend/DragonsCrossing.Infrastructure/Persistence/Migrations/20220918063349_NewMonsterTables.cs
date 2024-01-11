using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DragonsCrossing.Infrastructure.Persistence.Migrations
{
    public partial class NewMonsterTables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "HeroTemplate",
                keyColumn: "Id",
                keyValue: 1,
                column: "DateCreated",
                value: new DateTime(2022, 9, 18, 0, 33, 48, 32, DateTimeKind.Local).AddTicks(9606));

            migrationBuilder.UpdateData(
                table: "Player",
                keyColumn: "Id",
                keyValue: 1,
                column: "DateCreated",
                value: new DateTime(2022, 9, 18, 0, 33, 48, 33, DateTimeKind.Local).AddTicks(85));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "HeroTemplate",
                keyColumn: "Id",
                keyValue: 1,
                column: "DateCreated",
                value: new DateTime(2022, 9, 9, 17, 31, 26, 299, DateTimeKind.Local).AddTicks(9867));

            migrationBuilder.UpdateData(
                table: "Player",
                keyColumn: "Id",
                keyValue: 1,
                column: "DateCreated",
                value: new DateTime(2022, 9, 9, 17, 31, 26, 300, DateTimeKind.Local).AddTicks(60));
        }
    }
}
