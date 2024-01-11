using DragonsCrossing.Domain.Combats;
using DragonsCrossing.Domain.Common;
using DragonsCrossing.Domain.Heroes;
using DragonsCrossing.Domain.Zones;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DragonsCrossing.Domain.GameStates
{
    /// <summary>
    /// The last state the hero was in - whether he was in the middle of combat,
    /// or just visiting a zone or tile.
    /// A gamestate is tied to a hero (not a player).
    /// If there is no Gamestate for a hero, a default gamestate should be created.
    /// Only store 1 gamestate per hero, don't worry about keeping a history.
    /// </summary>
    public class GameState : ChangeTracking
    {
        public int Id { get; set; }
        public virtual Hero Hero { get; set; }
        
        /// <summary>
        /// The current zone the hero is in. Defaults to first zone.
        /// </summary>
        public virtual Zone CurrentZone { get; set; }        

        /// <summary>
        /// The last zone the hero was in. If the hero is new or first time playing or
        /// for some reason there isn't a previous zone, then default to Aedos Town
        /// </summary>
        public int CurrentZoneId { get; set; }

        /// <summary>
        /// This will be the previously generated Combat object including the monster that was already generated.
        /// This also includes the current tile the hero is on.
        /// </summary>
        public virtual Combat? Combat { get; set; }       
        
        /// <summary>
        /// See brandon's class: hero-level-up.get
        /// Also see heroTypes.ts
        /// We need this because they first roll for the level up before they purchase the level up in DCX. 
        /// So if they close the window, we don't want to allow them to re-roll for stats gained.
        /// </summary>
        //public HeroLevelUp LevelUp { get; set; }
    }
}
