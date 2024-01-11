using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DragonsCrossing.Core.Contracts.Api.Dto.Heroes
{
    public  class HeroMintingParam
    {
        public int heroGeneration { get; set; }

        public int moneySpentToMint { get; set; }
    }
}
