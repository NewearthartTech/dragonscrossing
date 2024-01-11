using DragonsCrossing.Domain.Common;
using DragonsCrossing.Domain.Heroes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DragonsCrossing.Domain.Items
{
    /// <summary>
    /// Applies to hero armor and weapons. A fixed amount of gear is pre-built in the db.
    /// </summary>
    public abstract class GearTemplate : ChangeTracking
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }   

        /// <summary>
        /// This is used when calculating combat damage.
        /// NOTE: not sure if this is needed
        /// </summary>
        //public HeroStatType? HeroStatAffected { get; set; }
        /// <summary>
        /// In Gold. A weapon/armor always has a cost even if it was found (in case it can be bought from the shop again)
        /// If not, then make this nullable
        /// </summary>
        public int PurchasePrice { get; set; }

        /// <summary>
        /// In Gold
        /// </summary>
        public int SellPrice { get; set; }
        /// <summary>
        /// The list of hero stats this gear effects and by how much
        /// aka: Attribute_Effected and Attributes_Effect
        /// </summary>
        public List<HeroStatModifier>? HeroStatModifiers { get; set; }

        /// <summary>
        /// True if this is the weapon or armor the hero starts with
        /// </summary>
        public bool IsStartingGear { get; set; }

        /// <summary>
        /// The image file to be displayed in the UI
        /// </summary>
        public string ImageBaseUrl { get; set; }
    }
}
