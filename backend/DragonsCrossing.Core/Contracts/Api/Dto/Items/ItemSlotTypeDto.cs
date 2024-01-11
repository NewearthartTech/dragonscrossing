using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DragonsCrossing.Core.Contracts.Api.Dto.Items
{
    //What type of Item it is
    //this is a property of the item
    // The item can only be dragged in correct postion if allowed

    [Newtonsoft.Json.JsonConverter(typeof(Newtonsoft.Json.Converters.StringEnumConverter))]
    public enum ItemSlotTypeDto
    {
        head,
        chest,
        mainHand,
        offHand,
        ring,
        feet,


        //a two handed Item goes in the main hand and prevents the offhand to have another item
        twoHand,


        shard, // This is a shard, this is only available to gen0 heros
        unidentifiedSkill,
        unlearnedSkill,

        //nftable items that does special things
        nftAction,

        unknown = 1001,
    }

    
}
