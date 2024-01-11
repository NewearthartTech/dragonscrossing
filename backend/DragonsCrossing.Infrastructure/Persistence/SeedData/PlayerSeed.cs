using DragonsCrossing.Domain.Items;
using DragonsCrossing.Domain.Players;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DragonsCrossing.Infrastructure.Persistence.SeedData
{
    /// <summary>
    /// NOTE: This class is only used for testing purposes. Remove this class once we go live.
    /// </summary>
    public class PlayerSeed
    {
        public PlayerSeed(ModelBuilder builder)
        {
            builder.Entity<PlayerGameSetting>().HasData(
                new PlayerGameSetting()
                {
                    Id = 1,
                    BypassCombatDiceRolls = false,
                    IsModestyOn = false,
                });

            //builder.Entity<NftItem>().HasData(
            //    new List<NftItem>()
            //    {
            //        new NftItem()
            //        {
            //            Id = 1,
            //            PlayerId = 1,
                                  
            //        }
            //    });

            //builder.Entity<BlockChainWallet>().HasData(
            //    new BlockChainWallet()
            //    {
            //        Id=1,
            //        PlayerId = 1,
            //        Dcx = 100,
            //        PublicAddress = "111111111111111111",
            //        //NftItems = new List<NftItem>()
            //        //{
            //        //    new NftItem()
            //        //    {
            //        //        Id = 1,
            //        //        Name = "Hero Egg",
            //        //        PurchasePrice = 200,
            //        //        Description = "Shiny goldend egg",
            //        //        SalePrice = 200/2,
            //        //        ImageFilePath = @"/images/items/nft/hero_egg.png",
            //        //        Type = NftItemType.HeroEgg,
            //        //    }
            //        //}
            //    });

            builder.Entity<Player>(b =>
            {
                b.HasData(
                    new Player()
                    {
                        Id = 1,
                        //WalletId = 1,
                        Name = "Test Player 1",
                        CreatedBy = nameof(PlayerSeed),
                        DateCreated = DateTime.Now,
                        SignedSignature = "22222222222222",
                        BlockchainPublicAddress = "Ox88888888888888889999",
                        GameSettingId = 1,
                    });                
            });                
        }
    }
}
