using DragonsCrossing.Core.Contracts.Api.Dto.Items;
using DragonsCrossing.Core.Contracts.Api.Dto.Monsters;
using DragonsCrossing.Core.Contracts.Api.Dto.Skill;
using DragonsCrossing.Core.Contracts.Api.Dto.Zones;

using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.Options;
using MongoDB.Driver;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Range = DragonsCrossing.Core.Contracts.Api.Dto.Monsters.Range;
using DragonsCrossing.Core.Helper;

namespace DragonsCrossing.Core.Contracts.Api.Dto.Heroes
{
    public class HerbalistDto
    {
        // This is used to identify which option the user purchased.
        [Required]
        public HerbalistOption option { get; set; }

        [Required]
        public int quests { get; set; }

        [Required]
        public int percentage { get; set; }
    }

    public enum HerbalistOption
    {
        // TODO: Rename this to match the actual healing schedule. Currently set to thrity-three percent
        fortyPercent,
        oneHundredPercent
    }
}

