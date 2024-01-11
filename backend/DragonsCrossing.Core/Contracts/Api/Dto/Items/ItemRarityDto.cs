using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace DragonsCrossing.Core.Contracts.Api.Dto.Items
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum ItemRarityDto
    {
        // Map to GearRarity
        Common,    //55
        Uncommon,  //25
        Rare,      //12  
        Legendary, //6
        Mythic,    //2
        Unknown   
    }
}
