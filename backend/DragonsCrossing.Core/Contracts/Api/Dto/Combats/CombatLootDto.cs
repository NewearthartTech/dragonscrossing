using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DragonsCrossing.Core.Contracts.Api.Dto.Combats
{
    public class CombatLootDto
    {
        [Required]
        public int Id { get; set; }

        [Required]
        public decimal UnsecuredDcx { get; set; }

        [Required]
        public int Gold { get; set; }
        //public HashSet<NftItem> Items { get; set; }
        //public HashSet<Weapon> Weapons { get; set; }
        //public HashSet<Armor> Armor { get; set; }
    }
}
