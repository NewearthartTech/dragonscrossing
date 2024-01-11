using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DragonsCrossing.Infrastructure.Persistence.Migrations
{
    public partial class HeroCombatStatsRenameBasetoMax : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "BaseWisdom",
                table: "HeroCombatStats",
                newName: "MaxWisdom");

            migrationBuilder.RenameColumn(
                name: "BaseStrength",
                table: "HeroCombatStats",
                newName: "MaxStrength");

            migrationBuilder.RenameColumn(
                name: "BaseQuickness",
                table: "HeroCombatStats",
                newName: "MaxQuickness");

            migrationBuilder.RenameColumn(
                name: "BaseHitPoints",
                table: "HeroCombatStats",
                newName: "MaxHitPoints");

            migrationBuilder.RenameColumn(
                name: "BaseCharisma",
                table: "HeroCombatStats",
                newName: "MaxCharisma");

            migrationBuilder.RenameColumn(
                name: "BaseAgility",
                table: "HeroCombatStats",
                newName: "MaxAgility");

            migrationBuilder.UpdateData(
                table: "HeroTemplate",
                keyColumn: "Id",
                keyValue: 1,
                column: "DateCreated",
                value: new DateTime(2022, 9, 18, 0, 55, 14, 656, DateTimeKind.Local).AddTicks(3349));

            migrationBuilder.UpdateData(
                table: "Player",
                keyColumn: "Id",
                keyValue: 1,
                column: "DateCreated",
                value: new DateTime(2022, 9, 18, 0, 55, 14, 656, DateTimeKind.Local).AddTicks(3551));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "MaxWisdom",
                table: "HeroCombatStats",
                newName: "BaseWisdom");

            migrationBuilder.RenameColumn(
                name: "MaxStrength",
                table: "HeroCombatStats",
                newName: "BaseStrength");

            migrationBuilder.RenameColumn(
                name: "MaxQuickness",
                table: "HeroCombatStats",
                newName: "BaseQuickness");

            migrationBuilder.RenameColumn(
                name: "MaxHitPoints",
                table: "HeroCombatStats",
                newName: "BaseHitPoints");

            migrationBuilder.RenameColumn(
                name: "MaxCharisma",
                table: "HeroCombatStats",
                newName: "BaseCharisma");

            migrationBuilder.RenameColumn(
                name: "MaxAgility",
                table: "HeroCombatStats",
                newName: "BaseAgility");

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
    }
}
