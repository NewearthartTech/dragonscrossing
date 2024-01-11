using DragonsCrossing.Domain.Monsters;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DragonsCrossing.Infrastructure.Persistence.Configurations
{
    public class MonsterTemplateConfiguration : IEntityTypeConfiguration<MonsterTemplateOld>
    {
        public void Configure(EntityTypeBuilder<MonsterTemplateOld> builder)
        {
            builder.ToTable(nameof(MonsterTemplateOld));
            builder.OwnsOne(m => m.MitigationMagic);
            builder.OwnsOne(m => m.MitigationMelee);
            builder.OwnsOne(m => m.MitigationRange);
            builder.OwnsOne(m => m.Power);
            builder.OwnsOne(m => m.MaxHitPoints);
            builder.OwnsOne(m => m.HitRate);
            builder.OwnsOne(m => m.Quickness);
            builder.OwnsOne(m => m.Charisma);
            builder.OwnsOne(m => m.CriticalHitChance);
            builder.OwnsOne(m => m.Damage);
            builder.OwnsOne(m => m.DodgeRate);
            builder.OwnsOne(m => m.Level);
            builder.OwnsOne(m => m.ParryChance);
            builder.OwnsOne(m => m.Power);
            builder.OwnsOne(m => m.DcxLootAmount);
            builder.OwnsOne(m => m.GoldLootAmount);

            builder.Property(e => e.ArmorDescription)
                .HasMaxLength(50);

            builder.Property(e => e.WeaponDescription)
                .HasMaxLength(50);

            // enums to strings
            builder.Property(e => e.MonsterClass)
                .HasConversion<string>()
                .HasMaxLength(30);

            // auto include properties
            builder.Navigation(b => b.SpecialAbilities).AutoInclude();

            // disable cascade delete to prevent "cycles" error in EF. Pluse we probably don't want to delete a tile if a MonsterTemplateOld is deleted.
            builder.HasOne(e => e.Tile)
                .WithMany()
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
