using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DragonsCrossing.Infrastructure.Persistence.Migrations
{
    public partial class InitialSep2022 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ArmorTemplate",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    HeroClass = table.Column<string>(type: "nvarchar(25)", maxLength: 25, nullable: true),
                    Defense_Min = table.Column<double>(type: "float", nullable: false),
                    Defense_Max = table.Column<double>(type: "float", nullable: false),
                    CriticalHit_Min = table.Column<double>(type: "float", nullable: true),
                    CriticalHit_Max = table.Column<double>(type: "float", nullable: true),
                    ArmorType = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    SlotType = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: false),
                    DateCreated = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DateModified = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    PurchasePrice = table.Column<int>(type: "int", nullable: false),
                    SellPrice = table.Column<int>(type: "int", nullable: false),
                    IsStartingGear = table.Column<bool>(type: "bit", nullable: false),
                    ImageBaseUrl = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ArmorTemplate", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CombatLoot",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UnsecuredDcx = table.Column<double>(type: "float(12)", precision: 12, scale: 9, nullable: false),
                    Gold = table.Column<int>(type: "int", nullable: false),
                    ExperiencePoints = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CombatLoot", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "GearAffixTemplate",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Effect = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: false),
                    EffectDescription = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GearAffixTemplate", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "HeroLevel",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Number = table.Column<int>(type: "int", nullable: false),
                    MaxExperiencePoints = table.Column<int>(type: "int", nullable: false),
                    AdditionalQuests = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HeroLevel", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "HeroName",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HeroName", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "HeroSkillTemplate",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    HeroClass = table.Column<string>(type: "nvarchar(25)", maxLength: 25, nullable: false),
                    IsStartingSkill = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HeroSkillTemplate", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "HeroTemplate",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ImageBaseUrl = table.Column<string>(type: "nvarchar(75)", maxLength: 75, nullable: false),
                    StartingStatPointsEachStat_Min = table.Column<int>(type: "int", nullable: false),
                    StartingStatPointsEachStat_Max = table.Column<int>(type: "int", nullable: false),
                    NoWeaponDamage_Min = table.Column<double>(type: "float", nullable: false),
                    NoWeaponDamage_Max = table.Column<double>(type: "float", nullable: false),
                    TotalSkillPoints = table.Column<int>(type: "int", nullable: false),
                    TotalDailyQuests = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    MaxHitPoints = table.Column<int>(type: "int", nullable: false),
                    DateCreated = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DateModified = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HeroTemplate", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MonsterPersonality",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PersonalityType = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    EffectChance = table.Column<double>(type: "float", nullable: false),
                    EffectAmount = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MonsterPersonality", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "NftItemTemplate",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Type = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    ImageBaseUrl = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    GoldCostToOpen = table.Column<int>(type: "int", nullable: false),
                    DcxCostToOpen = table.Column<decimal>(type: "decimal(12,9)", precision: 12, scale: 9, nullable: false),
                    DateCreated = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DateModified = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NftItemTemplate", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PlayerGameSetting",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IsModestyOn = table.Column<bool>(type: "bit", nullable: false),
                    BypassCombatDiceRolls = table.Column<bool>(type: "bit", nullable: false),
                    VolumeLevel = table.Column<int>(type: "int", nullable: false),
                    MaxVolumeLevel = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlayerGameSetting", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "WeaponTemplate",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    HeroClass = table.Column<string>(type: "nvarchar(25)", maxLength: 25, nullable: false),
                    SlotType = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: false),
                    BaseDamage_Min = table.Column<int>(type: "int", nullable: false),
                    BaseDamage_Max = table.Column<int>(type: "int", nullable: false),
                    BonusDamage_Min = table.Column<double>(type: "float", nullable: true),
                    BonusDamage_Max = table.Column<double>(type: "float", nullable: true),
                    Parry_Min = table.Column<double>(type: "float", nullable: true),
                    Parry_Max = table.Column<double>(type: "float", nullable: true),
                    Dodge_Min = table.Column<double>(type: "float", nullable: true),
                    Dodge_Max = table.Column<double>(type: "float", nullable: true),
                    CriticalHit_Min = table.Column<double>(type: "float", nullable: true),
                    CriticalHit_Max = table.Column<double>(type: "float", nullable: true),
                    DoubleHit_Min = table.Column<double>(type: "float", nullable: true),
                    DoubleHit_Max = table.Column<double>(type: "float", nullable: true),
                    WeaponType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    DateCreated = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DateModified = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    PurchasePrice = table.Column<int>(type: "int", nullable: false),
                    SellPrice = table.Column<int>(type: "int", nullable: false),
                    IsStartingGear = table.Column<bool>(type: "bit", nullable: false),
                    ImageBaseUrl = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WeaponTemplate", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Zone",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Order = table.Column<int>(type: "int", nullable: false),
                    LoreEncountersRequired = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Zone", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "GearAffixArmorSlot",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GearAffixTemplateId = table.Column<int>(type: "int", nullable: false),
                    ArmorSlotType = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GearAffixArmorSlot", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GearAffixArmorSlot_GearAffixTemplate_GearAffixTemplateId",
                        column: x => x.GearAffixTemplateId,
                        principalTable: "GearAffixTemplate",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GearAffixWeaponSlot",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GearAffixTemplateId = table.Column<int>(type: "int", nullable: false),
                    WeaponSlotType = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GearAffixWeaponSlot", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GearAffixWeaponSlot_GearAffixTemplate_GearAffixTemplateId",
                        column: x => x.GearAffixTemplateId,
                        principalTable: "GearAffixTemplate",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "HeroSkillEffect",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TurnDuration = table.Column<int>(type: "int", nullable: false),
                    HeroSkillTemplateId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HeroSkillEffect", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HeroSkillEffect_HeroSkillTemplate_HeroSkillTemplateId",
                        column: x => x.HeroSkillTemplateId,
                        principalTable: "HeroSkillTemplate",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "MonsterAttribute",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MonsterPersonalityId = table.Column<int>(type: "int", nullable: false),
                    Type = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MonsterAttribute", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MonsterAttribute_MonsterPersonality_MonsterPersonalityId",
                        column: x => x.MonsterPersonalityId,
                        principalTable: "MonsterPersonality",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Player",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    Nonce = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SignedSignature = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    BlockchainPublicAddress = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    GameSettingId = table.Column<int>(type: "int", nullable: false),
                    DateCreated = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DateModified = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Player", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Player_PlayerGameSetting_GameSettingId",
                        column: x => x.GameSettingId,
                        principalTable: "PlayerGameSetting",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "HeroStatModifier",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StatType = table.Column<int>(type: "int", nullable: false),
                    EffectAmount = table.Column<int>(type: "int", nullable: false),
                    EffectAmountOffsetRange_Min = table.Column<double>(type: "float", nullable: false),
                    EffectAmountOffsetRange_Max = table.Column<double>(type: "float", nullable: false),
                    ArmorTemplateId = table.Column<int>(type: "int", nullable: true),
                    WeaponTemplateId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HeroStatModifier", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HeroStatModifier_ArmorTemplate_ArmorTemplateId",
                        column: x => x.ArmorTemplateId,
                        principalTable: "ArmorTemplate",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_HeroStatModifier_WeaponTemplate_WeaponTemplateId",
                        column: x => x.WeaponTemplateId,
                        principalTable: "WeaponTemplate",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "GearAffixTier",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GearAffixTemplateId = table.Column<int>(type: "int", nullable: false),
                    ZoneId = table.Column<int>(type: "int", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    Amount = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GearAffixTier", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GearAffixTier_GearAffixTemplate_GearAffixTemplateId",
                        column: x => x.GearAffixTemplateId,
                        principalTable: "GearAffixTemplate",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GearAffixTier_Zone_ZoneId",
                        column: x => x.ZoneId,
                        principalTable: "Zone",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "HeroSkillTemplateZone",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ZoneId = table.Column<int>(type: "int", nullable: false),
                    HeroSkillTemplateId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HeroSkillTemplateZone", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HeroSkillTemplateZone_HeroSkillTemplate_HeroSkillTemplateId",
                        column: x => x.HeroSkillTemplateId,
                        principalTable: "HeroSkillTemplate",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_HeroSkillTemplateZone_Zone_ZoneId",
                        column: x => x.ZoneId,
                        principalTable: "Zone",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Tile",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ZoneId = table.Column<int>(type: "int", nullable: false),
                    Type = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Order = table.Column<int>(type: "int", nullable: false),
                    HeroLevelRequired_Min = table.Column<int>(type: "int", nullable: false),
                    HeroLevelRequired_Max = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tile", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Tile_Zone_ZoneId",
                        column: x => x.ZoneId,
                        principalTable: "Zone",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Hero",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PlayerId = table.Column<int>(type: "int", nullable: false),
                    HeroNameId = table.Column<int>(type: "int", nullable: false),
                    HeroTemplateId = table.Column<int>(type: "int", nullable: false),
                    LevelId = table.Column<int>(type: "int", nullable: false),
                    Generation = table.Column<int>(type: "int", nullable: false),
                    HeroClass = table.Column<string>(type: "nvarchar(25)", maxLength: 25, nullable: false),
                    Rarity = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Gender = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    ImageBaseUrl = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    SkillPoints = table.Column<int>(type: "int", nullable: false),
                    UnusedSkillPoints = table.Column<int>(type: "int", nullable: false),
                    IsAscended = table.Column<bool>(type: "bit", nullable: false),
                    UnusedDailyQuests = table.Column<int>(type: "int", nullable: false),
                    IsHearthstoneAvailable = table.Column<bool>(type: "bit", nullable: false),
                    DateCreated = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DateModified = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Hero", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Hero_HeroLevel_LevelId",
                        column: x => x.LevelId,
                        principalTable: "HeroLevel",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Hero_HeroName_HeroNameId",
                        column: x => x.HeroNameId,
                        principalTable: "HeroName",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Hero_HeroTemplate_HeroTemplateId",
                        column: x => x.HeroTemplateId,
                        principalTable: "HeroTemplate",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Hero_Player_PlayerId",
                        column: x => x.PlayerId,
                        principalTable: "Player",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PlayerBackpack",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PlayerId = table.Column<int>(type: "int", nullable: false),
                    SecuredDcx = table.Column<decimal>(type: "decimal(12,9)", precision: 12, scale: 9, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlayerBackpack", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PlayerBackpack_Player_PlayerId",
                        column: x => x.PlayerId,
                        principalTable: "Player",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MonsterTemplateOld",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ImageFilePath = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MaxHitPoints_Min = table.Column<int>(type: "int", nullable: false),
                    MaxHitPoints_Max = table.Column<int>(type: "int", nullable: false),
                    AppearanceChance = table.Column<double>(type: "float", nullable: false),
                    Power_Min = table.Column<int>(type: "int", nullable: false),
                    Power_Max = table.Column<int>(type: "int", nullable: false),
                    Charisma_Min = table.Column<int>(type: "int", nullable: false),
                    Charisma_Max = table.Column<int>(type: "int", nullable: false),
                    Level_Min = table.Column<int>(type: "int", nullable: false),
                    Level_Max = table.Column<int>(type: "int", nullable: false),
                    DodgeRate_Min = table.Column<int>(type: "int", nullable: false),
                    DodgeRate_Max = table.Column<int>(type: "int", nullable: false),
                    Quickness_Min = table.Column<int>(type: "int", nullable: false),
                    Quickness_Max = table.Column<int>(type: "int", nullable: false),
                    HitRate_Min = table.Column<int>(type: "int", nullable: false),
                    HitRate_Max = table.Column<int>(type: "int", nullable: false),
                    Damage_Min = table.Column<int>(type: "int", nullable: false),
                    Damage_Max = table.Column<int>(type: "int", nullable: false),
                    CharismaOpportunityChance = table.Column<int>(type: "int", nullable: false),
                    CombatOpportunityChance = table.Column<int>(type: "int", nullable: false),
                    CriticalHitChance_Min = table.Column<double>(type: "float", nullable: false),
                    CriticalHitChance_Max = table.Column<double>(type: "float", nullable: false),
                    ParryChance_Min = table.Column<double>(type: "float", nullable: false),
                    ParryChance_Max = table.Column<double>(type: "float", nullable: false),
                    MitigationMelee_Min = table.Column<double>(type: "float", nullable: false),
                    MitigationMelee_Max = table.Column<double>(type: "float", nullable: false),
                    MitigationRange_Min = table.Column<double>(type: "float", nullable: false),
                    MitigationRange_Max = table.Column<double>(type: "float", nullable: false),
                    MitigationMagic_Min = table.Column<double>(type: "float", nullable: false),
                    MitigationMagic_Max = table.Column<double>(type: "float", nullable: false),
                    WeaponDescription = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ArmorDescription = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    MonsterClass = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    SpecialAbilityCastChance = table.Column<int>(type: "int", nullable: false),
                    TileId = table.Column<int>(type: "int", nullable: false),
                    DcxLootChance = table.Column<double>(type: "float", nullable: false),
                    DcxLootAmount_Min = table.Column<double>(type: "float", nullable: false),
                    DcxLootAmount_Max = table.Column<double>(type: "float", nullable: false),
                    GoldLootChance = table.Column<double>(type: "float", nullable: false),
                    GoldLootAmount_Min = table.Column<int>(type: "int", nullable: false),
                    GoldLootAmount_Max = table.Column<int>(type: "int", nullable: false),
                    DateCreated = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DateModified = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MonsterTemplate", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MonsterTemplate_Tile_TileId",
                        column: x => x.TileId,
                        principalTable: "Tile",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "DiscoveredTile",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TileId = table.Column<int>(type: "int", nullable: false),
                    HeroId = table.Column<int>(type: "int", nullable: false),
                    IsComplete = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DiscoveredTile", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DiscoveredTile_Hero_HeroId",
                        column: x => x.HeroId,
                        principalTable: "Hero",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DiscoveredTile_Tile_TileId",
                        column: x => x.TileId,
                        principalTable: "Tile",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "HeroCombatStats",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    HeroId = table.Column<int>(type: "int", nullable: false),
                    UnusedStats = table.Column<int>(type: "int", nullable: false),
                    HitPoints = table.Column<int>(type: "int", nullable: false),
                    BaseHitPoints = table.Column<int>(type: "int", nullable: false),
                    ExperiencePoints = table.Column<int>(type: "int", nullable: false),
                    BaseAgility = table.Column<int>(type: "int", nullable: false),
                    BaseWisdom = table.Column<int>(type: "int", nullable: false),
                    BaseStrength = table.Column<int>(type: "int", nullable: false),
                    BaseQuickness = table.Column<int>(type: "int", nullable: false),
                    BaseCharisma = table.Column<int>(type: "int", nullable: false),
                    DateCreated = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DateModified = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HeroCombatStats", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HeroCombatStats_Hero_HeroId",
                        column: x => x.HeroId,
                        principalTable: "Hero",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "HeroInventory",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    HeroId = table.Column<int>(type: "int", nullable: false),
                    DateCreated = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DateModified = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Gold = table.Column<int>(type: "int", nullable: false),
                    Dcx = table.Column<decimal>(type: "decimal(12,9)", precision: 12, scale: 9, nullable: false),
                    MaxAvailableSlots = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HeroInventory", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HeroInventory_Hero_HeroId",
                        column: x => x.HeroId,
                        principalTable: "Hero",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "HeroSkill",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SkillTemplateId = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: false),
                    HeroId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HeroSkill", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HeroSkill_Hero_HeroId",
                        column: x => x.HeroId,
                        principalTable: "Hero",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_HeroSkill_HeroSkillTemplate_SkillTemplateId",
                        column: x => x.SkillTemplateId,
                        principalTable: "HeroSkillTemplate",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Monster",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MonsterTemplateId = table.Column<int>(type: "int", nullable: false),
                    HitPoints = table.Column<int>(type: "int", nullable: false),
                    MaxHitPoints = table.Column<int>(type: "int", nullable: false),
                    Power = table.Column<double>(type: "float", nullable: false),
                    Charisma = table.Column<double>(type: "float", nullable: false),
                    Level = table.Column<int>(type: "int", nullable: false),
                    DodgeRate = table.Column<int>(type: "int", nullable: false),
                    HitRate = table.Column<int>(type: "int", nullable: false),
                    Quickness = table.Column<double>(type: "float", nullable: false),
                    CriticalHitChance = table.Column<double>(type: "float", nullable: false),
                    ParryChance = table.Column<double>(type: "float", nullable: false),
                    MitigationMelee = table.Column<double>(type: "float", nullable: false),
                    MitigationRange = table.Column<double>(type: "float", nullable: false),
                    MitigationMagic = table.Column<double>(type: "float", nullable: false),
                    SpecialAbilityCastChance = table.Column<double>(type: "float", nullable: false),
                    PersonalityId = table.Column<int>(type: "int", nullable: false),
                    DateCreated = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DateModified = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Monster", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Monster_MonsterPersonality_PersonalityId",
                        column: x => x.PersonalityId,
                        principalTable: "MonsterPersonality",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Monster_MonsterTemplate_MonsterTemplateId",
                        column: x => x.MonsterTemplateId,
                        principalTable: "MonsterTemplateOld",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MonsterItemLoot",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MonsterTemplateId = table.Column<int>(type: "int", nullable: false),
                    WeaponTemplateId = table.Column<int>(type: "int", nullable: true),
                    ArmorTemplateId = table.Column<int>(type: "int", nullable: true),
                    NftItemTemplateId = table.Column<int>(type: "int", nullable: true),
                    LootDropChance = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MonsterItemLoot", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MonsterItemLoot_ArmorTemplate_ArmorTemplateId",
                        column: x => x.ArmorTemplateId,
                        principalTable: "ArmorTemplate",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_MonsterItemLoot_MonsterTemplate_MonsterTemplateId",
                        column: x => x.MonsterTemplateId,
                        principalTable: "MonsterTemplateOld",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MonsterItemLoot_NftItemTemplate_NftItemTemplateId",
                        column: x => x.NftItemTemplateId,
                        principalTable: "NftItemTemplate",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_MonsterItemLoot_WeaponTemplate_WeaponTemplateId",
                        column: x => x.WeaponTemplateId,
                        principalTable: "WeaponTemplate",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "MonsterSpecialAbility",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    MonsterTemplateId = table.Column<int>(type: "int", nullable: false),
                    UsesTurn = table.Column<bool>(type: "bit", nullable: false),
                    IsGuaranteedDamage = table.Column<bool>(type: "bit", nullable: false),
                    CanCastAgain = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MonsterSpecialAbility", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MonsterSpecialAbility_MonsterTemplate_MonsterTemplateId",
                        column: x => x.MonsterTemplateId,
                        principalTable: "MonsterTemplateOld",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Armor",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    HeroInventoryId = table.Column<int>(type: "int", nullable: false),
                    IsEquipped = table.Column<bool>(type: "bit", nullable: false),
                    Rarity = table.Column<int>(type: "int", nullable: false),
                    ArmorTemplateId = table.Column<int>(type: "int", nullable: false),
                    Defense = table.Column<double>(type: "float", nullable: false),
                    CriticalHitRate = table.Column<double>(type: "float", nullable: true),
                    SlotNumber = table.Column<int>(type: "int", nullable: true),
                    CombatLootId = table.Column<int>(type: "int", nullable: true),
                    DateCreated = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DateModified = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Armor", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Armor_ArmorTemplate_ArmorTemplateId",
                        column: x => x.ArmorTemplateId,
                        principalTable: "ArmorTemplate",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Armor_CombatLoot_CombatLootId",
                        column: x => x.CombatLootId,
                        principalTable: "CombatLoot",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Armor_HeroInventory_HeroInventoryId",
                        column: x => x.HeroInventoryId,
                        principalTable: "HeroInventory",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "NftItem",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PlayerId = table.Column<int>(type: "int", nullable: false),
                    NftItemTemplateId = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    SlotNumber = table.Column<int>(type: "int", nullable: true),
                    CombatLootId = table.Column<int>(type: "int", nullable: true),
                    HeroInventoryId = table.Column<int>(type: "int", nullable: true),
                    PlayerBackpackId = table.Column<int>(type: "int", nullable: true),
                    DateCreated = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DateModified = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NftItem", x => x.Id);
                    table.ForeignKey(
                        name: "FK_NftItem_CombatLoot_CombatLootId",
                        column: x => x.CombatLootId,
                        principalTable: "CombatLoot",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_NftItem_HeroInventory_HeroInventoryId",
                        column: x => x.HeroInventoryId,
                        principalTable: "HeroInventory",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_NftItem_NftItemTemplate_NftItemTemplateId",
                        column: x => x.NftItemTemplateId,
                        principalTable: "NftItemTemplate",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_NftItem_PlayerBackpack_PlayerBackpackId",
                        column: x => x.PlayerBackpackId,
                        principalTable: "PlayerBackpack",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Weapon",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    HeroInventoryId = table.Column<int>(type: "int", nullable: false),
                    IsEquipped = table.Column<bool>(type: "bit", nullable: false),
                    Rarity = table.Column<int>(type: "int", nullable: false),
                    WeaponTemplateId = table.Column<int>(type: "int", nullable: false),
                    CombatStat = table.Column<int>(type: "int", nullable: false),
                    ParryRate = table.Column<double>(type: "float", nullable: true),
                    DodgeRate = table.Column<double>(type: "float", nullable: true),
                    CriticalHitRate = table.Column<double>(type: "float", nullable: true),
                    DoubleHitRate = table.Column<double>(type: "float", nullable: true),
                    BonusDamage = table.Column<double>(type: "float", nullable: true),
                    SlotNumber = table.Column<int>(type: "int", nullable: true),
                    CombatLootId = table.Column<int>(type: "int", nullable: true),
                    DateCreated = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DateModified = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Weapon", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Weapon_CombatLoot_CombatLootId",
                        column: x => x.CombatLootId,
                        principalTable: "CombatLoot",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Weapon_HeroInventory_HeroInventoryId",
                        column: x => x.HeroInventoryId,
                        principalTable: "HeroInventory",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Weapon_WeaponTemplate_WeaponTemplateId",
                        column: x => x.WeaponTemplateId,
                        principalTable: "WeaponTemplate",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Combat",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TileId = table.Column<int>(type: "int", nullable: false),
                    MonsterId = table.Column<int>(type: "int", nullable: false),
                    HeroId = table.Column<int>(type: "int", nullable: false),
                    CombatLootId = table.Column<int>(type: "int", nullable: true),
                    IsCombatOver = table.Column<bool>(type: "bit", nullable: false),
                    IsCombatOpportunityAvailable = table.Column<bool>(type: "bit", nullable: false),
                    IsCharismaOpportunityAvailable = table.Column<bool>(type: "bit", nullable: false),
                    CombatOpportunityResult = table.Column<int>(type: "int", nullable: true),
                    CharismaOpportunityResult = table.Column<int>(type: "int", nullable: true),
                    UserConfirmedCombatEnd = table.Column<bool>(type: "bit", nullable: false),
                    Round = table.Column<int>(type: "int", nullable: false),
                    IsHeroDead = table.Column<bool>(type: "bit", nullable: false),
                    IsMonsterDead = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Combat", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Combat_CombatLoot_CombatLootId",
                        column: x => x.CombatLootId,
                        principalTable: "CombatLoot",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Combat_Hero_HeroId",
                        column: x => x.HeroId,
                        principalTable: "Hero",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Combat_Monster_MonsterId",
                        column: x => x.MonsterId,
                        principalTable: "Monster",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Combat_Tile_TileId",
                        column: x => x.TileId,
                        principalTable: "Tile",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MonsterSpecialAbilityEffect",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MonsterSpecialAbilityId = table.Column<int>(type: "int", nullable: false),
                    Type = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    EffectWho = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Amount = table.Column<int>(type: "int", nullable: false),
                    TurnDuration = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MonsterSpecialAbilityEffect", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MonsterSpecialAbilityEffect_MonsterSpecialAbility_MonsterSpecialAbilityId",
                        column: x => x.MonsterSpecialAbilityId,
                        principalTable: "MonsterSpecialAbility",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ArmorAffix",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SlotType = table.Column<int>(type: "int", nullable: false),
                    ArmorId = table.Column<int>(type: "int", nullable: true),
                    GearAffixTemplateId = table.Column<int>(type: "int", nullable: false),
                    TierId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ArmorAffix", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ArmorAffix_Armor_ArmorId",
                        column: x => x.ArmorId,
                        principalTable: "Armor",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ArmorAffix_GearAffixTemplate_GearAffixTemplateId",
                        column: x => x.GearAffixTemplateId,
                        principalTable: "GearAffixTemplate",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ArmorAffix_GearAffixTier_TierId",
                        column: x => x.TierId,
                        principalTable: "GearAffixTier",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "HeroStatModifierResult",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    HeroStatEffectedId = table.Column<int>(type: "int", nullable: false),
                    StatType = table.Column<int>(type: "int", nullable: false),
                    EffectAmount = table.Column<int>(type: "int", nullable: false),
                    ArmorId = table.Column<int>(type: "int", nullable: true),
                    WeaponId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HeroStatModifierResult", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HeroStatModifierResult_Armor_ArmorId",
                        column: x => x.ArmorId,
                        principalTable: "Armor",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_HeroStatModifierResult_Weapon_WeaponId",
                        column: x => x.WeaponId,
                        principalTable: "Weapon",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "WeaponAffix",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SlotType = table.Column<int>(type: "int", nullable: false),
                    WeaponId = table.Column<int>(type: "int", nullable: true),
                    GearAffixTemplateId = table.Column<int>(type: "int", nullable: false),
                    TierId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WeaponAffix", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WeaponAffix_GearAffixTemplate_GearAffixTemplateId",
                        column: x => x.GearAffixTemplateId,
                        principalTable: "GearAffixTemplate",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_WeaponAffix_GearAffixTier_TierId",
                        column: x => x.TierId,
                        principalTable: "GearAffixTier",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_WeaponAffix_Weapon_WeaponId",
                        column: x => x.WeaponId,
                        principalTable: "Weapon",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "CombatDetail",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CombatId = table.Column<int>(type: "int", nullable: false),
                    Type = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    IsSuccess = table.Column<bool>(type: "bit", nullable: false),
                    Amount = table.Column<double>(type: "float", nullable: false),
                    Order = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CombatDetail", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CombatDetail_Combat_CombatId",
                        column: x => x.CombatId,
                        principalTable: "Combat",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GameState",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    HeroId = table.Column<int>(type: "int", nullable: false),
                    CurrentZoneId = table.Column<int>(type: "int", nullable: false),
                    CombatId = table.Column<int>(type: "int", nullable: true),
                    DateCreated = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DateModified = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GameState", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GameState_Combat_CombatId",
                        column: x => x.CombatId,
                        principalTable: "Combat",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_GameState_Hero_HeroId",
                        column: x => x.HeroId,
                        principalTable: "Hero",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GameState_Zone_CurrentZoneId",
                        column: x => x.CurrentZoneId,
                        principalTable: "Zone",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "HeroSkillCasted",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SkillCastedId = table.Column<int>(type: "int", nullable: false),
                    ActiveUntilTurn = table.Column<int>(type: "int", nullable: false),
                    CombatId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HeroSkillCasted", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HeroSkillCasted_Combat_CombatId",
                        column: x => x.CombatId,
                        principalTable: "Combat",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_HeroSkillCasted_HeroSkill_SkillCastedId",
                        column: x => x.SkillCastedId,
                        principalTable: "HeroSkill",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "MonsterSpecialAbilityCasted",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MonsterId = table.Column<int>(type: "int", nullable: false),
                    EffectId = table.Column<int>(type: "int", nullable: false),
                    ActiveUntilRound = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MonsterSpecialAbilityCasted", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MonsterSpecialAbilityCasted_Monster_MonsterId",
                        column: x => x.MonsterId,
                        principalTable: "Monster",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MonsterSpecialAbilityCasted_MonsterSpecialAbilityEffect_EffectId",
                        column: x => x.EffectId,
                        principalTable: "MonsterSpecialAbilityEffect",
                        principalColumn: "Id");
                });

            migrationBuilder.InsertData(
                table: "ArmorTemplate",
                columns: new[] { "Id", "ArmorType", "CreatedBy", "DateCreated", "DateModified", "Description", "HeroClass", "ImageBaseUrl", "IsStartingGear", "ModifiedBy", "Name", "PurchasePrice", "SellPrice", "SlotType", "CriticalHit_Max", "CriticalHit_Min", "Defense_Max", "Defense_Min" },
                values: new object[] { 1, "Leathers", null, null, null, "Standard body armor for defending", null, "/images/items/weapons/BasicLeatherArmor.jpg", true, null, "Basic Leather Armor", 1, 1, "Chest", 1.0, 0.5, 2.75, 2.75 });

            migrationBuilder.InsertData(
                table: "GearAffixTemplate",
                columns: new[] { "Id", "Effect", "EffectDescription" },
                values: new object[,]
                {
                    { 1, "Draining", "Integer Lifesteal" },
                    { 2, "Truthful", "Increases chance to hit" },
                    { 3, "Disarming", "Chance to have monster do 50% reduced damage for next attack." },
                    { 4, "Savage", "Deal increased integer damage on subsequent hits." },
                    { 5, "Stunning", "Chance to have monster unable to use special attack next turn and has 50% reduced dodge next turn as well" },
                    { 6, "Quick", "Increase to dodge." },
                    { 7, "Thick", "Increase to armor mitigation." },
                    { 8, "Reflective", "Integer return of damage when hit." },
                    { 9, "Recovering", "Recovers HP after battle is completed." },
                    { 10, "Deft", "Increased parry %" },
                    { 11, "Doubling", "Increased % chance to swing twice." },
                    { 12, "Fetching", "Increased Charisma" },
                    { 13, "Strong", "Increased Strength" },
                    { 14, "Agile", "Increased Agility" },
                    { 15, "Wise", "Increased Wisdom" },
                    { 16, "Vital", "Increased HP" }
                });

            migrationBuilder.InsertData(
                table: "HeroLevel",
                columns: new[] { "Id", "AdditionalQuests", "MaxExperiencePoints", "Number" },
                values: new object[,]
                {
                    { 1, 0, 20, 1 },
                    { 2, 0, 41, 2 },
                    { 3, 1, 63, 3 },
                    { 4, 1, 86, 4 }
                });

            migrationBuilder.InsertData(
                table: "HeroName",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "Legolas" },
                    { 2, "Aragon" },
                    { 3, "Gandalf" },
                    { 4, "Harry Potter" },
                    { 5, "Snape" },
                    { 6, "Dumbeldor" },
                    { 7, "Voldermort" },
                    { 8, "Sauron" },
                    { 9, "Gimly" },
                    { 10, "Samwise" }
                });

            migrationBuilder.InsertData(
                table: "HeroTemplate",
                columns: new[] { "Id", "CreatedBy", "DateCreated", "DateModified", "ImageBaseUrl", "IsActive", "MaxHitPoints", "ModifiedBy", "TotalDailyQuests", "TotalSkillPoints", "NoWeaponDamage_Max", "NoWeaponDamage_Min", "StartingStatPointsEachStat_Max", "StartingStatPointsEachStat_Min" },
                values: new object[] { 1, "HeroTemplateSeed", new DateTime(2022, 9, 10, 0, 54, 59, 475, DateTimeKind.Local).AddTicks(6555), null, "/img/api/heroes/", true, 25, null, 20, 20, 4.0, 1.0, 10, 4 });

            migrationBuilder.InsertData(
                table: "MonsterPersonality",
                columns: new[] { "Id", "EffectAmount", "EffectChance", "PersonalityType" },
                values: new object[,]
                {
                    { 1, -30.0, 7.0, "Sickly" },
                    { 2, 30.0, 7.0, "Inspired" },
                    { 3, -30.0, 7.0, "Fatigued" },
                    { 4, 30.0, 7.0, "Brutal" },
                    { 5, -30.0, 7.0, "Lazy" },
                    { 6, 30.0, 7.0, "Lean" },
                    { 7, -30.0, 7.0, "Stupid" },
                    { 8, 30.0, 7.0, "Arcane" },
                    { 9, -30.0, 7.0, "Impotent" },
                    { 10, 30.0, 7.0, "Reckless" }
                });

            migrationBuilder.InsertData(
                table: "MonsterPersonality",
                columns: new[] { "Id", "EffectAmount", "EffectChance", "PersonalityType" },
                values: new object[,]
                {
                    { 11, 30.0, 7.0, "Deadly" },
                    { 12, 0.0, 23.0, "None" }
                });

            migrationBuilder.InsertData(
                table: "NftItemTemplate",
                columns: new[] { "Id", "CreatedBy", "DateCreated", "DateModified", "DcxCostToOpen", "Description", "GoldCostToOpen", "ImageBaseUrl", "ModifiedBy", "Name", "Type" },
                values: new object[,]
                {
                    { 1, null, null, null, 1m, "Shiny golden egg", 200, "/images/items/nft/hero_egg", null, "Hero Egg", "Shard" },
                    { 2, null, null, null, 1m, "Book of mystic arts", 200, "/images/items/nft/skill_book", null, "Skill Book", "Skillbook" }
                });

            migrationBuilder.InsertData(
                table: "PlayerGameSetting",
                columns: new[] { "Id", "BypassCombatDiceRolls", "IsModestyOn", "MaxVolumeLevel", "VolumeLevel" },
                values: new object[] { 1, false, false, 0, 0 });

            migrationBuilder.InsertData(
                table: "WeaponTemplate",
                columns: new[] { "Id", "CreatedBy", "DateCreated", "DateModified", "Description", "HeroClass", "ImageBaseUrl", "IsStartingGear", "ModifiedBy", "Name", "PurchasePrice", "SellPrice", "SlotType", "WeaponType", "BaseDamage_Max", "BaseDamage_Min", "BonusDamage_Max", "BonusDamage_Min", "Parry_Max", "Parry_Min" },
                values: new object[,]
                {
                    { 1, null, null, null, "A standard sword for cutting foes", "Warrior", "/img/api/items/BasicGreatsword.jpg", true, null, "Basic Greatsword", 1, 1, "TwoHand", "Greatsword", 10, 1, 1.0, -2.0, 4.0, 2.0 },
                    { 2, null, null, null, "A standard crossbow for poking holes into foes", "Ranger", "/images/items/weapons/BasicCrossbow.jpg", true, null, "Basic Crossbow", 1, 1, "TwoHand", "Crossbow", 10, 1, 1.0, -2.0, 4.0, 2.0 },
                    { 3, null, null, null, "A standard staff for casting magic", "Mage", "/images/items/weapons/BasicStaff.jpg", true, null, "Basic Staff", 1, 1, "TwoHand", "Staff", 8, 1, 1.0, -2.0, 4.0, 2.0 },
                    { 4, null, null, null, "Stabs things", "Ranger", "/images/items/weapons/simple-shortsword", false, null, "Simple Shortsword", 1, 1, "PrimaryHand", "Staff", 6, 1, 2.0, -1.0, 4.0, 2.0 },
                    { 5, null, null, null, "chops things", "Warrior", "/images/items/weapons/runed-axe", false, null, "Runed Axe", 1, 1, "PrimaryHand", "Handaxe", 6, 1, 3.0, -1.0, 4.0, 2.0 }
                });

            migrationBuilder.InsertData(
                table: "Zone",
                columns: new[] { "Id", "LoreEncountersRequired", "Name", "Order" },
                values: new object[,]
                {
                    { 1, 0, "Aedos", 1 },
                    { 2, 2, "Wild Prairie", 2 },
                    { 3, 3, "Mysterious Forest", 3 },
                    { 4, 4, "Foul Wastes", 4 },
                    { 5, 5, "Treacherous Peaks", 5 },
                    { 6, 6, "Dark Tower", 6 }
                });

            migrationBuilder.InsertData(
                table: "GearAffixTier",
                columns: new[] { "Id", "Amount", "GearAffixTemplateId", "Type", "ZoneId" },
                values: new object[,]
                {
                    { 1, 1.0, 1, 1, 1 },
                    { 2, 1.0, 1, 2, 1 },
                    { 3, 2.0, 1, 3, 1 },
                    { 4, 2.0, 1, 4, 1 },
                    { 5, 2.0, 1, 1, 2 },
                    { 6, 2.0, 1, 2, 2 },
                    { 7, 3.0, 1, 3, 2 },
                    { 8, 4.0, 1, 4, 2 },
                    { 9, 0.5, 2, 1, 1 },
                    { 10, 1.0, 2, 2, 1 },
                    { 11, 1.5, 2, 3, 1 },
                    { 12, 2.0, 2, 4, 1 },
                    { 13, 0.75, 2, 1, 2 },
                    { 14, 1.25, 2, 2, 2 },
                    { 15, 1.75, 2, 3, 2 },
                    { 16, 2.25, 2, 4, 2 },
                    { 17, 1.0, 3, 1, 1 },
                    { 18, 2.0, 3, 2, 1 },
                    { 19, 3.0, 3, 3, 1 },
                    { 20, 4.0, 3, 4, 1 },
                    { 21, 2.0, 3, 1, 2 },
                    { 22, 3.0, 3, 2, 2 },
                    { 23, 4.0, 3, 3, 2 },
                    { 24, 5.0, 3, 4, 2 }
                });

            migrationBuilder.InsertData(
                table: "GearAffixWeaponSlot",
                columns: new[] { "Id", "GearAffixTemplateId", "WeaponSlotType" },
                values: new object[,]
                {
                    { 1, 1, "PrimaryHand" },
                    { 2, 1, "TwoHand" },
                    { 3, 2, "PrimaryHand" },
                    { 4, 2, "TwoHand" },
                    { 5, 3, "PrimaryHand" },
                    { 6, 3, "TwoHand" },
                    { 7, 4, "PrimaryHand" },
                    { 8, 4, "TwoHand" },
                    { 9, 5, "PrimaryHand" },
                    { 10, 5, "TwoHand" },
                    { 11, 13, "PrimaryHand" },
                    { 12, 13, "TwoHand" },
                    { 13, 14, "PrimaryHand" },
                    { 14, 14, "TwoHand" },
                    { 15, 15, "PrimaryHand" },
                    { 16, 15, "TwoHand" },
                    { 17, 16, "PrimaryHand" },
                    { 18, 16, "TwoHand" }
                });

            migrationBuilder.InsertData(
                table: "MonsterAttribute",
                columns: new[] { "Id", "MonsterPersonalityId", "Type" },
                values: new object[,]
                {
                    { 1, 1, "Charisma" },
                    { 2, 2, "Charisma" },
                    { 3, 3, "Power" },
                    { 4, 4, "Power" },
                    { 5, 5, "Quickness" },
                    { 6, 6, "Quickness" },
                    { 7, 7, "SpecialAbilityChance" },
                    { 8, 8, "SpecialAbilityChance" },
                    { 9, 9, "CriticalHitChance" },
                    { 10, 10, "CriticalHitChance" },
                    { 11, 11, "Charisma" },
                    { 12, 11, "Power" },
                    { 13, 11, "Quickness" },
                    { 14, 11, "SpecialAbilityChance" },
                    { 15, 11, "CriticalHitChance" }
                });

            migrationBuilder.InsertData(
                table: "Player",
                columns: new[] { "Id", "BlockchainPublicAddress", "CreatedBy", "DateCreated", "DateModified", "GameSettingId", "ModifiedBy", "Name", "Nonce", "SignedSignature" },
                values: new object[] { 1, "Ox88888888888888889999", "PlayerSeed", new DateTime(2022, 9, 10, 0, 54, 59, 475, DateTimeKind.Local).AddTicks(6760), null, 1, null, "Test Player 1", new Guid("00000000-0000-0000-0000-000000000000"), "22222222222222" });

            migrationBuilder.InsertData(
                table: "Tile",
                columns: new[] { "Id", "Name", "Order", "Type", "ZoneId", "HeroLevelRequired_Max", "HeroLevelRequired_Min" },
                values: new object[,]
                {
                    { 1, "Enchanted Fields", 1, "RegularCombat", 2, 4, 1 },
                    { 2, "Sylvan Woodlands", 1, "RegularCombat", 3, 7, 3 },
                    { 3, "Pilgrims' Clearing", 2, "DailyCombat", 3, 8, 5 },
                    { 4, "Odorous Bog", 1, "RegularCombat", 4, 11, 6 },
                    { 5, "Ancient Battlefield", 2, "DailyCombat", 4, 12, 7 },
                    { 6, "Terrorswamp", 3, "Boss", 4, 13, 13 },
                    { 7, "Mountain Fortress", 1, "RegularCombat", 5, 15, 10 },
                    { 8, "Griffon's Nest", 2, "DailyCombat", 5, 16, 11 },
                    { 9, "Summoner's Summit", 3, "Boss", 5, 16, 16 },
                    { 10, "Labyrinthian Dungeon", 1, "RegularCombat", 6, 20, 16 },
                    { 11, "Slaver Row", 2, "DailyCombat", 6, 20, 17 },
                    { 12, "Wing of the Jailer", 3, "Boss", 6, 20, 20 },
                    { 13, "Laboratory of the Archmagus", 4, "FinalBoss", 6, 20, 20 }
                });

            migrationBuilder.InsertData(
                table: "MonsterTemplateOld",
                columns: new[] { "Id", "AppearanceChance", "ArmorDescription", "CharismaOpportunityChance", "CombatOpportunityChance", "CreatedBy", "DateCreated", "DateModified", "DcxLootChance", "Description", "GoldLootChance", "ImageFilePath", "ModifiedBy", "MonsterClass", "Name", "SpecialAbilityCastChance", "TileId", "WeaponDescription", "Charisma_Max", "Charisma_Min", "CriticalHitChance_Max", "CriticalHitChance_Min", "Damage_Max", "Damage_Min", "DcxLootAmount_Max", "DcxLootAmount_Min", "DodgeRate_Max", "DodgeRate_Min", "GoldLootAmount_Max", "GoldLootAmount_Min", "HitRate_Max", "HitRate_Min", "Level_Max", "Level_Min", "MaxHitPoints_Max", "MaxHitPoints_Min", "MitigationMagic_Max", "MitigationMagic_Min", "MitigationMelee_Max", "MitigationMelee_Min", "MitigationRange_Max", "MitigationRange_Min", "ParryChance_Max", "ParryChance_Min", "Power_Max", "Power_Min", "Quickness_Max", "Quickness_Min" },
                values: new object[,]
                {
                    { 1, 20.0, "Worn Rags and Leather", 20, 10, null, null, null, 10.0, "Grumbling, stumbling, filthy and ragged, this little green creature's size does little to hide the wanton violence begging to burst at the first thing that looks at him the wrong way. For better or worse, today that happens to be you.", 0.0, "\\images\\monsters\\wandering_goblin.jpg", null, "Warrior", "Wandering Goblin", 22, 1, "Dagger", 7, 2, 3.0, 1.0, 9, 2, 1.8300000000000001, 1.23, 43, 37, 2, 1, 5, 0, 3, 1, 35, 30, 10.0, 0.0, 10.0, 0.0, 20.0, 10.0, 2.0, 1.0, 9, 2, 12, 2 },
                    { 2, 20.0, "Chain Shirt", 40, 30, null, null, null, 20.0, "Ah, a fellow adventurer. Those runes make it look like he has seen a thing or two, and you think you've seen him in town before.", 0.0, "\\images\\monsters\\rival_adventurer.jpg", null, "Ranger", "Rival Adventurer", 15, 1, "Runed Axe", 9, 5, 5.0, 2.0, 6, 2, 1.9299999999999999, 1.3300000000000001, 22, 10, 2, 1, 45, 38, 4, 1, 50, 40, 0.0, 0.0, 20.0, 10.0, 20.0, 10.0, 10.0, 5.0, 7, 3, 12, 2 },
                    { 3, 15.0, "something", 25, 15, null, null, null, 22.0, "It's fearsome. It's gray. It's big, and it's furry. Yep, that's a giant wolf.", 0.0, "\\images\\monsters\\giant_wolf.jpg", null, "Warrior", "Giant Wolf", 10, 1, "something", 12, 6, 8.0, 4.0, 7, 2, 1.8600000000000001, 1.4299999999999999, 40, 30, 3, 2, 45, 38, 4, 1, 60, 40, 15.0, 0.0, 15.0, 0.0, 15.0, 0.0, 8.0, 4.0, 5, 4, 12, 2 },
                    { 4, 15.0, "Rough Hide", 25, 15, null, null, null, 50.0, "A mighty stag stands before you, and you feel the absurd urge to kneel before such a princely animal.", 0.0, "\\images\\monsters\\mighty_stag.jpg", null, "Warrior", "Mighty Stag", 30, 1, "Antlers", 12, 6, 8.0, 4.0, 7, 2, 2.2200000000000002, 1.53, 40, 30, 4, 2, 45, 38, 4, 2, 60, 40, 15.0, 0.0, 15.0, 0.0, 15.0, 0.0, 8.0, 4.0, 5, 4, 12, 2 }
                });

            migrationBuilder.InsertData(
                table: "MonsterItemLoot",
                columns: new[] { "Id", "ArmorTemplateId", "LootDropChance", "MonsterTemplateId", "NftItemTemplateId", "WeaponTemplateId" },
                values: new object[,]
                {
                    { 1, null, 50.0, 1, null, 4 },
                    { 2, null, 50.0, 1, null, 5 },
                    { 3, null, 50.0, 2, null, 4 },
                    { 4, null, 50.0, 2, null, 5 },
                    { 5, null, 50.0, 3, null, 4 },
                    { 6, null, 50.0, 3, null, 5 },
                    { 7, null, 50.0, 4, null, 4 },
                    { 8, null, 50.0, 4, null, 5 }
                });

            migrationBuilder.InsertData(
                table: "MonsterSpecialAbility",
                columns: new[] { "Id", "CanCastAgain", "Description", "IsGuaranteedDamage", "MonsterTemplateId", "Name", "UsesTurn" },
                values: new object[,]
                {
                    { 1, true, "The goblin rotates furiously, attacking with both daggers in a flurry of condensed rage.", false, 1, "Whirly Gig", true },
                    { 2, true, "The runes on his axe glow distractingly, and you find it harder to focus on your own weapon work.", false, 2, "Runic Axe", false },
                    { 3, true, "Slavering, the wolf bites you with its foaming maw.", false, 3, "Savage Bite", true },
                    { 4, false, "The countenance of this lord of the forest is staggering. His sheer majesty pushes you away.", false, 4, "Majesty", false }
                });

            migrationBuilder.InsertData(
                table: "MonsterSpecialAbilityEffect",
                columns: new[] { "Id", "Amount", "EffectWho", "MonsterSpecialAbilityId", "TurnDuration", "Type" },
                values: new object[,]
                {
                    { 1, 120, "Monster", 1, 1, "DamageRate" },
                    { 2, -50, "Hero", 2, 2, "Parry" },
                    { 3, 140, "Monster", 3, 1, "DamageRate" },
                    { 4, 10, "Monster", 4, 1, "DodgeRate" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Armor_ArmorTemplateId",
                table: "Armor",
                column: "ArmorTemplateId");

            migrationBuilder.CreateIndex(
                name: "IX_Armor_CombatLootId",
                table: "Armor",
                column: "CombatLootId");

            migrationBuilder.CreateIndex(
                name: "IX_Armor_HeroInventoryId",
                table: "Armor",
                column: "HeroInventoryId");

            migrationBuilder.CreateIndex(
                name: "IX_ArmorAffix_ArmorId",
                table: "ArmorAffix",
                column: "ArmorId");

            migrationBuilder.CreateIndex(
                name: "IX_ArmorAffix_GearAffixTemplateId",
                table: "ArmorAffix",
                column: "GearAffixTemplateId");

            migrationBuilder.CreateIndex(
                name: "IX_ArmorAffix_TierId",
                table: "ArmorAffix",
                column: "TierId");

            migrationBuilder.CreateIndex(
                name: "IX_Combat_CombatLootId",
                table: "Combat",
                column: "CombatLootId");

            migrationBuilder.CreateIndex(
                name: "IX_Combat_HeroId",
                table: "Combat",
                column: "HeroId");

            migrationBuilder.CreateIndex(
                name: "IX_Combat_MonsterId",
                table: "Combat",
                column: "MonsterId");

            migrationBuilder.CreateIndex(
                name: "IX_Combat_TileId",
                table: "Combat",
                column: "TileId");

            migrationBuilder.CreateIndex(
                name: "IX_CombatDetail_CombatId",
                table: "CombatDetail",
                column: "CombatId");

            migrationBuilder.CreateIndex(
                name: "IX_DiscoveredTile_HeroId_TileId",
                table: "DiscoveredTile",
                columns: new[] { "HeroId", "TileId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DiscoveredTile_TileId",
                table: "DiscoveredTile",
                column: "TileId");

            migrationBuilder.CreateIndex(
                name: "IX_GameState_CombatId",
                table: "GameState",
                column: "CombatId");

            migrationBuilder.CreateIndex(
                name: "IX_GameState_CurrentZoneId",
                table: "GameState",
                column: "CurrentZoneId");

            migrationBuilder.CreateIndex(
                name: "IX_GameState_HeroId",
                table: "GameState",
                column: "HeroId");

            migrationBuilder.CreateIndex(
                name: "IX_GearAffixArmorSlot_GearAffixTemplateId",
                table: "GearAffixArmorSlot",
                column: "GearAffixTemplateId");

            migrationBuilder.CreateIndex(
                name: "IX_GearAffixTier_GearAffixTemplateId",
                table: "GearAffixTier",
                column: "GearAffixTemplateId");

            migrationBuilder.CreateIndex(
                name: "IX_GearAffixTier_ZoneId",
                table: "GearAffixTier",
                column: "ZoneId");

            migrationBuilder.CreateIndex(
                name: "IX_GearAffixWeaponSlot_GearAffixTemplateId",
                table: "GearAffixWeaponSlot",
                column: "GearAffixTemplateId");

            migrationBuilder.CreateIndex(
                name: "IX_Hero_HeroNameId",
                table: "Hero",
                column: "HeroNameId");

            migrationBuilder.CreateIndex(
                name: "IX_Hero_HeroTemplateId",
                table: "Hero",
                column: "HeroTemplateId");

            migrationBuilder.CreateIndex(
                name: "IX_Hero_LevelId",
                table: "Hero",
                column: "LevelId");

            migrationBuilder.CreateIndex(
                name: "IX_Hero_PlayerId",
                table: "Hero",
                column: "PlayerId");

            migrationBuilder.CreateIndex(
                name: "IX_HeroCombatStats_HeroId",
                table: "HeroCombatStats",
                column: "HeroId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_HeroInventory_HeroId",
                table: "HeroInventory",
                column: "HeroId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_HeroLevel_Number",
                table: "HeroLevel",
                column: "Number",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_HeroSkill_HeroId",
                table: "HeroSkill",
                column: "HeroId");

            migrationBuilder.CreateIndex(
                name: "IX_HeroSkill_SkillTemplateId",
                table: "HeroSkill",
                column: "SkillTemplateId");

            migrationBuilder.CreateIndex(
                name: "IX_HeroSkillCasted_CombatId",
                table: "HeroSkillCasted",
                column: "CombatId");

            migrationBuilder.CreateIndex(
                name: "IX_HeroSkillCasted_SkillCastedId",
                table: "HeroSkillCasted",
                column: "SkillCastedId");

            migrationBuilder.CreateIndex(
                name: "IX_HeroSkillEffect_HeroSkillTemplateId",
                table: "HeroSkillEffect",
                column: "HeroSkillTemplateId");

            migrationBuilder.CreateIndex(
                name: "IX_HeroSkillTemplateZone_HeroSkillTemplateId",
                table: "HeroSkillTemplateZone",
                column: "HeroSkillTemplateId");

            migrationBuilder.CreateIndex(
                name: "IX_HeroSkillTemplateZone_ZoneId",
                table: "HeroSkillTemplateZone",
                column: "ZoneId");

            migrationBuilder.CreateIndex(
                name: "IX_HeroStatModifier_ArmorTemplateId",
                table: "HeroStatModifier",
                column: "ArmorTemplateId");

            migrationBuilder.CreateIndex(
                name: "IX_HeroStatModifier_WeaponTemplateId",
                table: "HeroStatModifier",
                column: "WeaponTemplateId");

            migrationBuilder.CreateIndex(
                name: "IX_HeroStatModifierResult_ArmorId",
                table: "HeroStatModifierResult",
                column: "ArmorId");

            migrationBuilder.CreateIndex(
                name: "IX_HeroStatModifierResult_WeaponId",
                table: "HeroStatModifierResult",
                column: "WeaponId");

            migrationBuilder.CreateIndex(
                name: "IX_Monster_MonsterTemplateId",
                table: "Monster",
                column: "MonsterTemplateId");

            migrationBuilder.CreateIndex(
                name: "IX_Monster_PersonalityId",
                table: "Monster",
                column: "PersonalityId");

            migrationBuilder.CreateIndex(
                name: "IX_MonsterAttribute_MonsterPersonalityId",
                table: "MonsterAttribute",
                column: "MonsterPersonalityId");

            migrationBuilder.CreateIndex(
                name: "IX_MonsterItemLoot_ArmorTemplateId",
                table: "MonsterItemLoot",
                column: "ArmorTemplateId");

            migrationBuilder.CreateIndex(
                name: "IX_MonsterItemLoot_MonsterTemplateId",
                table: "MonsterItemLoot",
                column: "MonsterTemplateId");

            migrationBuilder.CreateIndex(
                name: "IX_MonsterItemLoot_NftItemTemplateId",
                table: "MonsterItemLoot",
                column: "NftItemTemplateId");

            migrationBuilder.CreateIndex(
                name: "IX_MonsterItemLoot_WeaponTemplateId",
                table: "MonsterItemLoot",
                column: "WeaponTemplateId");

            migrationBuilder.CreateIndex(
                name: "IX_MonsterSpecialAbility_MonsterTemplateId",
                table: "MonsterSpecialAbility",
                column: "MonsterTemplateId");

            migrationBuilder.CreateIndex(
                name: "IX_MonsterSpecialAbilityCasted_EffectId",
                table: "MonsterSpecialAbilityCasted",
                column: "EffectId");

            migrationBuilder.CreateIndex(
                name: "IX_MonsterSpecialAbilityCasted_MonsterId",
                table: "MonsterSpecialAbilityCasted",
                column: "MonsterId");

            migrationBuilder.CreateIndex(
                name: "IX_MonsterSpecialAbilityEffect_MonsterSpecialAbilityId",
                table: "MonsterSpecialAbilityEffect",
                column: "MonsterSpecialAbilityId");

            migrationBuilder.CreateIndex(
                name: "IX_MonsterTemplate_TileId",
                table: "MonsterTemplateOld",
                column: "TileId");

            migrationBuilder.CreateIndex(
                name: "IX_NftItem_CombatLootId",
                table: "NftItem",
                column: "CombatLootId");

            migrationBuilder.CreateIndex(
                name: "IX_NftItem_HeroInventoryId",
                table: "NftItem",
                column: "HeroInventoryId");

            migrationBuilder.CreateIndex(
                name: "IX_NftItem_NftItemTemplateId",
                table: "NftItem",
                column: "NftItemTemplateId");

            migrationBuilder.CreateIndex(
                name: "IX_NftItem_PlayerBackpackId",
                table: "NftItem",
                column: "PlayerBackpackId");

            migrationBuilder.CreateIndex(
                name: "IX_Player_BlockchainPublicAddress",
                table: "Player",
                column: "BlockchainPublicAddress",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Player_GameSettingId",
                table: "Player",
                column: "GameSettingId");

            migrationBuilder.CreateIndex(
                name: "IX_PlayerBackpack_PlayerId",
                table: "PlayerBackpack",
                column: "PlayerId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Tile_ZoneId",
                table: "Tile",
                column: "ZoneId");

            migrationBuilder.CreateIndex(
                name: "IX_Weapon_CombatLootId",
                table: "Weapon",
                column: "CombatLootId");

            migrationBuilder.CreateIndex(
                name: "IX_Weapon_HeroInventoryId",
                table: "Weapon",
                column: "HeroInventoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Weapon_WeaponTemplateId",
                table: "Weapon",
                column: "WeaponTemplateId");

            migrationBuilder.CreateIndex(
                name: "IX_WeaponAffix_GearAffixTemplateId",
                table: "WeaponAffix",
                column: "GearAffixTemplateId");

            migrationBuilder.CreateIndex(
                name: "IX_WeaponAffix_TierId",
                table: "WeaponAffix",
                column: "TierId");

            migrationBuilder.CreateIndex(
                name: "IX_WeaponAffix_WeaponId",
                table: "WeaponAffix",
                column: "WeaponId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ArmorAffix");

            migrationBuilder.DropTable(
                name: "CombatDetail");

            migrationBuilder.DropTable(
                name: "DiscoveredTile");

            migrationBuilder.DropTable(
                name: "GameState");

            migrationBuilder.DropTable(
                name: "GearAffixArmorSlot");

            migrationBuilder.DropTable(
                name: "GearAffixWeaponSlot");

            migrationBuilder.DropTable(
                name: "HeroCombatStats");

            migrationBuilder.DropTable(
                name: "HeroSkillCasted");

            migrationBuilder.DropTable(
                name: "HeroSkillEffect");

            migrationBuilder.DropTable(
                name: "HeroSkillTemplateZone");

            migrationBuilder.DropTable(
                name: "HeroStatModifier");

            migrationBuilder.DropTable(
                name: "HeroStatModifierResult");

            migrationBuilder.DropTable(
                name: "MonsterAttribute");

            migrationBuilder.DropTable(
                name: "MonsterItemLoot");

            migrationBuilder.DropTable(
                name: "MonsterSpecialAbilityCasted");

            migrationBuilder.DropTable(
                name: "NftItem");

            migrationBuilder.DropTable(
                name: "WeaponAffix");

            migrationBuilder.DropTable(
                name: "Combat");

            migrationBuilder.DropTable(
                name: "HeroSkill");

            migrationBuilder.DropTable(
                name: "Armor");

            migrationBuilder.DropTable(
                name: "MonsterSpecialAbilityEffect");

            migrationBuilder.DropTable(
                name: "NftItemTemplate");

            migrationBuilder.DropTable(
                name: "PlayerBackpack");

            migrationBuilder.DropTable(
                name: "GearAffixTier");

            migrationBuilder.DropTable(
                name: "Weapon");

            migrationBuilder.DropTable(
                name: "Monster");

            migrationBuilder.DropTable(
                name: "HeroSkillTemplate");

            migrationBuilder.DropTable(
                name: "ArmorTemplate");

            migrationBuilder.DropTable(
                name: "MonsterSpecialAbility");

            migrationBuilder.DropTable(
                name: "GearAffixTemplate");

            migrationBuilder.DropTable(
                name: "CombatLoot");

            migrationBuilder.DropTable(
                name: "HeroInventory");

            migrationBuilder.DropTable(
                name: "WeaponTemplate");

            migrationBuilder.DropTable(
                name: "MonsterPersonality");

            migrationBuilder.DropTable(
                name: "MonsterTemplateOld");

            migrationBuilder.DropTable(
                name: "Hero");

            migrationBuilder.DropTable(
                name: "Tile");

            migrationBuilder.DropTable(
                name: "HeroLevel");

            migrationBuilder.DropTable(
                name: "HeroName");

            migrationBuilder.DropTable(
                name: "HeroTemplate");

            migrationBuilder.DropTable(
                name: "Player");

            migrationBuilder.DropTable(
                name: "Zone");

            migrationBuilder.DropTable(
                name: "PlayerGameSetting");
        }
    }
}
