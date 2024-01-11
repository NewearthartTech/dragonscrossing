using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DragonsCrossing.Infrastructure.Persistence.Migrations
{
    public partial class HeroSkillEffectWhoandType : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Amount",
                table: "HeroSkillEffect",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "EffectWho",
                table: "HeroSkillEffect",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Type",
                table: "HeroSkillEffect",
                type: "int",
                nullable: false,
                defaultValue: 0);

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

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Amount",
                table: "HeroSkillEffect");

            migrationBuilder.DropColumn(
                name: "EffectWho",
                table: "HeroSkillEffect");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "HeroSkillEffect");

            migrationBuilder.UpdateData(
                table: "HeroTemplate",
                keyColumn: "Id",
                keyValue: 1,
                column: "DateCreated",
                value: new DateTime(2022, 9, 15, 23, 7, 27, 204, DateTimeKind.Local).AddTicks(2818));

            migrationBuilder.UpdateData(
                table: "Player",
                keyColumn: "Id",
                keyValue: 1,
                column: "DateCreated",
                value: new DateTime(2022, 9, 15, 23, 7, 27, 204, DateTimeKind.Local).AddTicks(3091));
        }
    }
}
