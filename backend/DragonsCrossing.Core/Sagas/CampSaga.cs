using System;
using System.ComponentModel.DataAnnotations;
using DragonsCrossing.Core.Common;
using DragonsCrossing.Core.Contracts.Api.Dto.Combats;
using DragonsCrossing.Core.Contracts.Api.Dto.Heroes;
using DragonsCrossing.Core.Services;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.IdGenerators;
using MongoDB.Driver;
using DragonsCrossing.NewCombatLogic;
using Newtonsoft.Json;
using DragonsCrossing.Core.Helper;
using DragonsCrossing.Core.Contracts.Api.Dto.Items;

namespace DragonsCrossing.Core.Sagas;

public abstract class CampingOrder : DcxOrder
{
    /// <summary>
    /// Which hero has the securable NFT in inventory
    /// </summary>
    [Required]
    public int heroId { get; set; }

    /// <summary>
    /// This is used for backend to tell the smart contract that it can mint this NFT
    /// </summary>
    [Required]
    public string authorizaton { get; set; } = string.Empty;

    public Guid correlationId { get; set; } = Guid.NewGuid();

}

public class SecuredNFTsOrder : CampingOrder
{

    [Required]
    public int itemNftId { get; set; }


    [Required]
    public string recepientAddress { get; set; } = String.Empty;


    [Required]
    public string itemId { get; set; } = String.Empty;

    [MongoIndex]
    public static void CreateIndexes(IMongoCollection<SecuredNFTsOrder> collection)
    {
        collection.Indexes.CreateOne(
            new CreateIndexModel<SecuredNFTsOrder>(
            new IndexKeysDefinitionBuilder<SecuredNFTsOrder>()
                .Ascending(f => f.itemNftId)
                , new CreateIndexOptions<SecuredNFTsOrder>
                {
                    Unique = true,
                    PartialFilterExpression = Builders<SecuredNFTsOrder>.Filter.Eq(o => o.type, nameof(SecuredNFTsOrder))
                }
            ));
    }

}

public class ClaimDcxOrder : CampingOrder
{

}

[MongoCollection("NftizedItems")]
[BsonIgnoreExtraElements]
[BsonDiscriminator(RootClass = true)]
[BsonKnownTypes(typeof(NftizedDFKItem))]
public class NftizedItem
{
    /// <summary>
    /// We use nft id as the ID for these
    /// </summary>
    [BsonId]
    public int itemId
    {
        get { return item.nftTokenId; }
        set { }
    }

    public ItemDto item { get; set; } = new ItemDto();
}


public class NftizedDFKItem : NftizedItem { };


