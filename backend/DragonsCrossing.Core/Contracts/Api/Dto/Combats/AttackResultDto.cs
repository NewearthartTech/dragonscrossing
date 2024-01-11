using DragonsCrossing.Core.Contracts.Api.Dto.Heroes;
using DragonsCrossing.Core.Contracts.Api.Dto.Monsters;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DragonsCrossing.Core.Contracts.Api.Dto.Combats
{
    public class AttackResultDto
    {
        [BsonIgnore]
        public bool IsHit { get; set; } // In an hero AttackResult, the IsHit is true when the hero successfully attacted the monster that round.
        [BsonIgnore]
        public int TotalDamage { get; set; }
        [BsonIgnore]
        public int BonusDamage { get; set; }
        [BsonIgnore]
        public int CritDamage { get; set; } // Is crit damage the extra damage apart from the original damage value? Which is 50% of the summed dmg so far. 
        [BsonIgnore]
        public int ArmorMitigation { get; set; } // how to calculate ArmorMitigation
        [BsonIgnore]
        public int ParryMitigation { get; set; } // how to calculate ParryMitigation
        [BsonIgnore]
        public int StatusEffectBonus { get; set; } //Extra damage done by monster's special ability for that round.
        [BsonIgnore]
        public int StatusEffectMitigation { get; set; }
        [BsonIgnore]
        public List<DieResultDto> Dice { get; set; } = new List<DieResultDto>();

    }

    public class HeroAttackResultDto : AttackResultDto
    {
        /// <summary>
        /// These stats are used by skills functions to temporarirly change Hero stats
        /// </summary>
        [Required]
        public AlteredCalculatedStats[] heroAlteredStats { get; set; } = new AlteredCalculatedStats[] { };
    }
    public class MonsterAttackResultDto : AttackResultDto
    {

        /// <summary>
        /// These stats are used by skills functions to temporarirly change Monster stats
        /// </summary>
        [Required]
        public AlteredMonsterCalculatedStats[] monsteralteredStats { get; set; } = new AlteredMonsterCalculatedStats[] { };
    }

}
