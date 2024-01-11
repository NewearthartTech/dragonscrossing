using System;
using System.ComponentModel.DataAnnotations;

namespace DragonsCrossing.Core.Sagas;

public class SummonHeroOrder : NFTUseBase
{
    readonly static public string NO_SPECIAL_MINTS = "none";

    [Required]
    public string orderHash { get; set; } = Guid.NewGuid().ToString();

    [Required]
    public string mintProps { get; set; } = "none";

    [Required]
    public int heroIdToTransfer { get; set; } = 0;

}


public class IdentifySkillOrder : NFTUseBase
{
    public int newItemTokenId { get; set; }
}

public class LearnSkillOrder : NFTUseBase 
{
    public int levelRequirement { get; set; }
    public int heroId { get; set; } 
}

public class NftActionOrder : NFTUseBase
{
    public int heroId { get; set; }
}

public abstract class NFTUseBase: DcxOrder
{
    /// <summary>
    /// The NFT tokenId we are trying to use
    /// Will depend on type of NFTUse
    /// </summary>
    public int nftTokenId { get; set; }


    [Required]
    public string authorization { get; set; } = String.Empty;


    /// <summary>
    /// The player that summoned
    /// </summary>
    [Required]
    public string userId { get; set; } = string.Empty;


}

