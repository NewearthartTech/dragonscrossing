using DragonsCrossing.Domain.Items;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DragonsCrossing.Infrastructure.Persistence.Configurations
{
    public class PlayerBackpackConfiguration : IEntityTypeConfiguration<PlayerBackpack>
    {
        public void Configure(EntityTypeBuilder<PlayerBackpack> builder)
        {
            builder.ToTable(nameof(PlayerBackpack));
            //TODO: ask eric what kind of numbers would be stored here. For decimals, 9 decimal places is the most .net supports. If we need more precision, probably should use double.
            builder.Property(b => b.SecuredDcx).HasPrecision(12, 9);
        }
    }
}
