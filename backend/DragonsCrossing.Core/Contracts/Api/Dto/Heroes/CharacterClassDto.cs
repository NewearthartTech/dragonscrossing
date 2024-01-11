using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace DragonsCrossing.Core.Contracts.Api.Dto.Heroes
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum CharacterClassDto
    {
        Warrior,
        Ranger,
        Mage
    }
}
