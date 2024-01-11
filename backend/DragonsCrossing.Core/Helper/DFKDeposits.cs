using System;
using System.ComponentModel.DataAnnotations;
using MongoDB.Bson.Serialization.Attributes;

namespace DragonsCrossing.Core.Helper;

/// <summary>
/// we want to keep track of deposits made using DFK 
/// </summary>
[BsonIgnoreExtraElements]
[MongoCollection("dfkDeposits")]

public class DFKDeposit
{
    [BsonId]
    public string txHash { get; set; } = string.Empty;

    
    public string depositFrom { get; set; } = string.Empty;

    
    public decimal dcxValue { get; set; }

    public DateTime observedAt { get; set; } = DateTime.Now.ToUniversalTime();

    /// <summary>
    /// description of what this Tx was used for
    /// </summary>
    public string? usedUpBy { get; set; }

}

