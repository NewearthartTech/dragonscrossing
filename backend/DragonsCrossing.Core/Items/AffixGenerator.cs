using DragonsCrossing.Core.Common;
using DragonsCrossing.Domain.Common;
using DragonsCrossing.Domain.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DragonsCrossing.Core.Items
{
    public class AffixGenerator<T> where T : GearAffix
    {
        public List<T>? GetGearAffixes(GearRarity rarity, List<GearAffixTemplate> gearAffixTemplates, int zoneId)
        {
            // assign the affixes
            T? newGearAffix1 = null;
            T? newGearAffix2 = null;
            switch (rarity)
            {
                case GearRarity.Common:
                    newGearAffix1 = GetRandomGearAffix(gearAffixTemplates, 5, 1, 2, zoneId);
                    break;
                case GearRarity.Uncommon:
                    newGearAffix1 = GetRandomGearAffix(gearAffixTemplates, 15, 1, 3, zoneId);
                    break;
                case GearRarity.Rare:
                    newGearAffix1 = GetRandomGearAffix(gearAffixTemplates, 30, 1, 3, zoneId);
                    newGearAffix2 = GetRandomGearAffix(gearAffixTemplates, 30, 1, 3, zoneId);
                    break;
                case GearRarity.Epic:
                    newGearAffix1 = GetRandomGearAffix(gearAffixTemplates, 100, 1, 4, zoneId);
                    newGearAffix2 = GetRandomGearAffix(gearAffixTemplates, 50, 1, 4, zoneId);
                    break;
                case GearRarity.Legendary:
                    newGearAffix1 = GetRandomGearAffix(gearAffixTemplates, 100, 2, 4, zoneId);
                    newGearAffix2 = GetRandomGearAffix(gearAffixTemplates, 100, 2, 4, zoneId);
                    break;
            }

            var newAffixes = new List<T>();
            if (newGearAffix1 != null)
                newAffixes.Add(newGearAffix1);
            if (newGearAffix2 != null)
                newAffixes.Add(newGearAffix2);
            return newAffixes.Any() ? newAffixes : null;
        }

        /// <summary>
        /// Gets a random weapon or armor affix
        /// </summary>
        /// <param name="gearAffixTemplates">the affix templates used to generate the random affix</param>
        /// <param name="percentChance">a whole number between 0 and 100</param>
        /// <param name="startTier">a number between 1 and 4</param>
        /// <param name="endTier">a number between 1 and 4</param>
        /// <returns></returns>
        private T? GetRandomGearAffix(List<GearAffixTemplate> gearAffixTemplates, int percentChance, int startTier, int endTier, int zoneId)
        {
            if (percentChance.IsSuccessfulRoll())
            {
                T gearAffix;
                if (typeof(T) == typeof(WeaponAffix))
                    gearAffix = new WeaponAffix() as T;
                else if (typeof(T) == typeof(ArmorAffix))
                    gearAffix = new ArmorAffix() as T;
                else
                    throw new TypeAccessException($"Unknown GearAffix type: {typeof(T)}");

                // get random affix
                var randomAffixTemplate = gearAffixTemplates.GetRandom();
                gearAffix.GearAffixTemplate = randomAffixTemplate;

                // get random tier
                var randomTier = new RangeInt(startTier, endTier).GetRandom();
                gearAffix.Tier = randomAffixTemplate.Tiers
                    .Single(t => (int)t.Type == randomTier && t.ZoneId == zoneId);
                return gearAffix;
            }
            return null;
        }
    }
}
