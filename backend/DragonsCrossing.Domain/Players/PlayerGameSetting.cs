using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DragonsCrossing.Domain.Players
{
    public class PlayerGameSetting
    {
        public int Id { get; set; }
        /// <summary>
        /// If turned on, any immodest images will be replaced with modest ones.
        /// </summary>
        public bool IsModestyOn { get; set; } = false;
        public bool BypassCombatDiceRolls { get; set; } = false;
        public int VolumeLevel { get; set; }
        public int MaxVolumeLevel { get; set; }
    }
}
