using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DragonsCrossing.Domain.Items
{
    public enum NftItemType
    {
        /// <summary>
        /// Aka. Hero Egg, Aka. unsummoned hero.
        /// </summary>
        Shard,
        /// <summary>
        /// An unused hero skill that can be passed around to different heroes.
        /// This can either be identified, or unidentified.
        /// </summary>
        Skillbook,
        /// <summary>
        /// 
        /// </summary>
        Homestead,
    }
}
