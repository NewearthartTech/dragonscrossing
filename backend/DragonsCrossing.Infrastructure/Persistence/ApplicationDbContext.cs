using DragonsCrossing.Domain.Armors;
using DragonsCrossing.Domain.Combats;
using DragonsCrossing.Domain.Common;
using DragonsCrossing.Domain.GameStates;
using DragonsCrossing.Domain.Heroes;
using DragonsCrossing.Domain.Items;
using DragonsCrossing.Domain.Monsters;
using DragonsCrossing.Domain.Players;
using DragonsCrossing.Domain.Weapons;
using DragonsCrossing.Domain.Zones;
using DragonsCrossing.Infrastructure.Persistence.Configurations;
using DragonsCrossing.Infrastructure.Persistence.SeedData;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DragonsCrossing.Infrastructure.Persistence
{
    public class ApplicationDbContext : DbContext //, IApplicationDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {            
        }
        public DbSet<HeroTemplate> HeroTemplates { get; set; }
        public DbSet<HeroName> HeroNames { get; set; }
        public DbSet<Hero> Heroes { get; set; }
        public DbSet<Monster> Monsters {get; set; }
        public DbSet<MonsterTemplateOld> MonsterTemplates { get; set; }
        public DbSet<MonsterItemLoot> MonsterItemsLoot { get; set; }
        public DbSet<Weapon> Weapons { get; set; }
        public DbSet<WeaponTemplate> WeaponTemplates { get; set; }
        public DbSet<Armor> Armors { get; set; }
        public DbSet<ArmorTemplate> ArmorTemplates { get; set; }
        public DbSet<Player> Players { get; set; }
        public DbSet<Combat> Combats { get; set; }
        public DbSet<CombatLoot> CombatLoots { get; set; }
        public DbSet<GameState> GameStates { get; set; }
        public DbSet<Zone> Zones { get; set; }
        public DbSet<Tile> Tiles { get; set; }
        public DbSet<DiscoveredTile> DiscoveredTiles { get; set; }
        public DbSet<MonsterPersonality> MonsterPersonalities { get; set; }
        public DbSet<HeroLevel> HeroLevels { get; set; }
        public DbSet<HeroInventory> HeroInventories { get; set; }
        public DbSet<GearAffixTemplate> GearAffixTemplates { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {            
            base.OnModelCreating(builder); // call the base class method first
            builder.Ignore<PlayerBackpack>(); // don't store this in the db.

            // Player
            new PlayerConfiguration().Configure(builder.Entity<Player>());

            // Gamestate
            new GameStateConfiguration().Configure(builder.Entity<GameState>());

            // Hero
            new HeroTemplateConfiguration().Configure(builder.Entity<HeroTemplate>());
            new HeroConfiguration().Configure(builder.Entity<Hero>());
            new HeroNameConfiguration().Configure(builder.Entity<HeroName>());
            new HeroStatModifierConfiguration().Configure(builder.Entity<HeroStatModifier>());
            new HeroLevelConfiguration().Configure(builder.Entity<HeroLevel>());
            new HeroSkillConfiguration().Configure(builder.Entity<HeroSkill>());
            new HeroSkillCastedConfiguration().Configure(builder.Entity<HeroSkillCasted>());
            new HeroSkillTemplateConfiguration().Configure(builder.Entity<HeroSkillTemplate>());

            // Inventory
            new WeaponConfiguration().Configure(builder.Entity<Weapon>());
            new WeaponTemplateConfiguration().Configure(builder.Entity<WeaponTemplate>());
            new ArmorTemplateConfiguration().Configure(builder.Entity<ArmorTemplate>());
            new ArmorConfiguration().Configure(builder.Entity<Armor>());
            new NftItemTemplateConfiguration().Configure(builder.Entity<NftItemTemplate>());
            new HeroInventoryConfiguration().Configure(builder.Entity<HeroInventory>());
            new PlayerBackpackConfiguration().Configure(builder.Entity<PlayerBackpack>());

            // Gear affixes
            new GearAffixTemplateConfiguration().Configure(builder.Entity<GearAffixTemplate>());
            new ArmorAffixConfiguration().Configure(builder.Entity<ArmorAffix>());
            new WeaponAffixConfiguration().Configure(builder.Entity<WeaponAffix>());
            new GearAffixWeaponSlotConfiguration().Configure(builder.Entity<GearAffixWeaponSlot>());

            // Combat
            new CombatConfiguration().Configure(builder.Entity<Combat>());
            new CombatLootConfiguration().Configure(builder.Entity<CombatLoot>());
            new CombatDetailConfiguration().Configure(builder.Entity<CombatDetail>());

            // Monster
            new MonsterTemplateConfiguration().Configure(builder.Entity<MonsterTemplateOld>());
            new MonsterConfiguration().Configure(builder.Entity<Monster>());
            new MonsterPersonalityConfiguration().Configure(builder.Entity<MonsterPersonality>());
            new MonsterAttributeConfiguration().Configure(builder.Entity<MonsterAttribute>());
            new MonsterSpecialAbilityConfiguration().Configure(builder.Entity<MonsterSpecialAbility>());
            new MonsterSpecialAbilityEffectConfiguration().Configure(builder.Entity<MonsterSpecialAbilityEffect>());
            new MonsterSpecialAbilityCastedConfiguration().Configure(builder.Entity<MonsterSpecialAbilityCasted>());
            new MonsterItemLootConfiguration().Configure(builder.Entity<MonsterItemLoot>());

            // Zone
            new ZoneConfiguration().Configure(builder.Entity<Zone>());
            new TileConfiguration().Configure(builder.Entity<Tile>());
            new DiscoveredTileConfiguration().Configure(builder.Entity<DiscoveredTile>());            

            SeedData(builder);
        }

        public Task<int> SaveChangesAsync()
        {
            foreach (var entry in ChangeTracker.Entries<ChangeTracking>())
            {                
                switch (entry.State)
                {
                    case EntityState.Added:
                        entry.Entity.CreatedBy = "";// _currentUserService.UserId;
                        entry.Entity.DateCreated = DateTime.Now; // _dateTime.Now;
                        break;

                    case EntityState.Modified:
                        entry.Entity.ModifiedBy = ""; // _currentUserService.UserId;
                        entry.Entity.DateModified = DateTime.Now; //_dateTime.Now;
                        break;
                }
            }
            return base.SaveChangesAsync();
        }

        public void SeedData(ModelBuilder builder)
        {
            //this.Database.EnsureCreated(); // supposedly will re-run seed data - haven't tested it yet

            // Zones and tiles
            new ZoneSeed(builder);
            new TileSeed(builder);

            // Hero
            new HeroTemplateSeed(builder);
            new HeroNameSeed(builder);
            new HeroLevelSeed(builder);

            // Player
            new PlayerSeed(builder);

            // Monster
            new MonsterTemplateSeed(builder);
            new MonsterSpecialAbilitySeed(builder);
            new MonsterPersonalitySeed(builder);
            new MonsterItemLootSeed(builder);

            // Items
            new WeaponTemplateSeed(builder);
            new ArmorTemplateSeed(builder);
            new GearAffixTemplateSeed(builder);
            new GearAffixTierSeed(builder);
            new GearAffixWeaponSlotSeed(builder);
            new NftItemTemplateSeed(builder);
        }
    }
}
