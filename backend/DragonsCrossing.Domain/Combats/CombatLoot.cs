using DragonsCrossing.Domain.Items;
using DragonsCrossing.Domain.Weapons;
using DragonsCrossing.Domain.Armors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DragonsCrossing.Domain.Combats
{
    /// <summary>
    /// Contains all the loot the person found. 
    /// Valid loot: Weapons/Armor, Gold, UnsecuredDcx, UnsecuredHeroEgg, UnsecuredSkillBook, Experience
    /// NOTE: This is deleted at the end of every combat. This means there will only ever be 1 of these per hero.
    /// </summary>
    public class CombatLoot
    {
        public int Id { get; set; }
        public double UnsecuredDcx { get; set; }
        public int Gold { get; set; }
        public List<NftItem>? NftItems { get; set; }
        public List<Weapon>? Weapons { get; set; }
        public List<Armor>? Armor { get; set; }
        public int ExperiencePoints { get; set; }
    }
}
