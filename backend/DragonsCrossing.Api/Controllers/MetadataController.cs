using DragonsCrossing.Api.Utilities;
using DragonsCrossing.Core;
using DragonsCrossing.Core.Contracts.Api;
using DragonsCrossing.Core.Contracts.Api.Dto.Heroes;
using DragonsCrossing.Core.Contracts.Api.Dto.Players;
using DragonsCrossing.Core.Helper;
using DragonsCrossing.Domain.GameStates;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using DragonsCrossing.NewCombatLogic;
using System.ComponentModel.DataAnnotations;
using DragonsCrossing.Core.Services;
using Nethereum.ABI;
using Nethereum.Web3;
using Nethereum.Util;
using System.Text;
using DragonsCrossing.Core.Sagas;
using DragonsCrossing.Core.Contracts.Api.Dto.Items;
using DragonsCrossing.Core.Common;
using DragonsCrossing.Core.Contracts.Api.Dto.Skill;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace DragonsCrossing.Api.Controllers;


[Route("api/[controller]")]
[ApiController]
public class MetadataController : ControllerBase
{
    readonly IHeroesService _heroesService;
    readonly ILogger _logger;
    protected readonly Web3Config _web3Config;
    protected readonly IPerpetualDbService _perpetualDb;

    public MetadataController(IHeroesService heroesRepository,
        IConfiguration config,
        IPerpetualDbService perpetualDb,
        ILogger<HeroesController> logger)
    {
        this._heroesService = heroesRepository;
        _perpetualDb = perpetualDb;
        _logger = logger;
        _web3Config = config.GetSection("web3").Get<Web3Config>() ?? new Web3Config();
    }

    [HttpGet("ItemImage/{name}")]
    public FileContentResult getHeroImage(string name)
    {
        var heroTemplate = $"NftImages.Items.{name.ToLower()}.png";

        return getImage(heroTemplate);
    }

    [HttpGet("heroImage/{heroId}")]
    public async Task<FileContentResult> getHeroImage(int heroId)
    {
        if (heroId <= 0)
            throw new ArgumentOutOfRangeException(nameof(heroId));

        var templateFileName = "reserved";

        if (heroId > 12 /*_web3Config.reservedHeroCount*/)
        {
            var hero = await _heroesService.GetHero(heroId);
            templateFileName = $"{hero.zoneBackgroundType.ToString().ToLower()}-{hero.gender.ToString().ToLower()}-{hero.heroClass.ToString().ToLower()}-gen{hero.generation}";
        }
        else
        {
            templateFileName = $"hero{heroId}";
        }

        var heroTemplate = $"NftImages.Heros.{templateFileName}.png";

        return getImage(heroTemplate);

    }

    FileContentResult getImage(string imageTemplate)
    {

        var executingAssembly = System.Reflection.Assembly.GetExecutingAssembly();
        if (null == executingAssembly)
            throw new InvalidOperationException("executingAssembly not found");

        var template = $"{executingAssembly.GetName().Name}.{imageTemplate}";
        var templateStream = executingAssembly.GetManifestResourceStream(template);

        if(null == templateStream)
        {
            /*
            _logger.LogError($"image {heroTemplate} not found for hero image, using backup");
            var heroTemplateBackup = "NftImages.Heros.fields-female-mage-gen1.png";
            template = $"{executingAssembly.GetName().Name}.{heroTemplateBackup}";
            templateStream = executingAssembly.GetManifestResourceStream(template);
            */

            throw new ExceptionWithCode("image not found", System.Net.HttpStatusCode.NotFound);
        }

        using (templateStream)
        {
            if (null == templateStream)
            {
                throw new InvalidOperationException($"failed to open stream for template file {template}");
            }

            using (var ms = new MemoryStream())
            {
                templateStream.CopyTo(ms);
                return new FileContentResult(ms.ToArray(), "image/png");
            }
            
        }

    }

    [HttpGet("{heroId}")]
    public Task<OpenSeaMetadata> getHeroCoreById(int heroId)
    {
        return getHeroById(heroId);
    }

    [HttpGet("item/{itemId}")]
    public async Task<OpenSeaMetadata> getItemById(int itemId)
    {
        if (itemId <= 0)
            throw new ArgumentOutOfRangeException(nameof(itemId));

        var itemToUse = (await _perpetualDb.getCollection<NftizedItem>().Find(i => i.itemId == itemId)
            .Project(i => i.item)
            .SingleAsync());

        var image = "";
        var name = "";

        switch (itemToUse.slot)
        {
            case ItemSlotTypeDto.shard:
                image = $"{_web3Config.metaDataBaseURI}ItemImage/shard";
                name = "Shard";
                break;

            case ItemSlotTypeDto.unidentifiedSkill:
                name = "UnIdentified skill";
                break;

            case ItemSlotTypeDto.unlearnedSkill:
                {
                    var skillItem = itemToUse as SkillItem;
                    if(null == skillItem)
                    {
                        throw new Exception($"itemToUse {itemId} is not skill Item");
                    }

                    var unLearned = skillItem.skill as UnlearnedHeroSkill;
                    if (null == unLearned)
                        throw new Exception("skill is not UnlearnedHeroSkill");

                    image = $"{_web3Config.metaDataBaseURI}ItemImage/{unLearned.skillClass.ToString().ToLower()}-skill";

                    name = "Skill - " + unLearned.name;

                }
                
                break;
        }


        return new OpenSeaMetadata
        {
            description = $"Dragon's Crossings {name}",
            external_url = "https://dragonscrossings.com",
            name = name,

            image = image,
        };

    }


    object[] CreateAttributes(HeroDto hero, string prefix)
    {
        return (new object[]
            {
                new NumberValueTrait
                {
                    trait_type = $"{prefix} Quickness",
                    value = hero.quickness
                },
                new NumberValueTrait
                {
                    trait_type = $"{prefix} Charisma",
                    value = hero.charisma
                },
                new NumberValueTrait
                {
                    trait_type = $"{prefix} Wisdom",
                    value = hero.wisdom
                },
                new NumberValueTrait
                {
                    trait_type = $"{prefix} Strength",
                    value = hero.strength
                },
                new NumberValueTrait
                {
                    trait_type = $"{prefix} Agility",
                    value = hero.agility
                }
            });

    }

    [HttpGet("hero/{heroId}")]
    public async Task<OpenSeaMetadata> getHeroById(int heroId)
    {
        if (heroId <= 0)
            throw new ArgumentOutOfRangeException(nameof(heroId));

        /*
        if (heroId <= _web3Config.reservedHeroCount)
        {
            return new OpenSeaMetadata
            {
                description = $"Dragon's Crossings Reserved",
                external_url = "https://dragonscrossings.com",
                name = "Reserved",

                image = $"{_web3Config.metaDataBaseURI}heroImage/{heroId}",
            };
        }
        */


        var seasonHero =  await _heroesService.GetHero(heroId);


        var perpetualHero = await _heroesService.GetHero(heroId, perpetualOnly:true);


        var metadata = new OpenSeaMetadata
        {
            description = $"Dragon's Crossings {seasonHero.heroClass} {seasonHero.name}",
            external_url = "https://dragonscrossings.com",
            name = seasonHero.name,

            image = $"{_web3Config.metaDataBaseURI}heroImage/{heroId}", 

            attributes = (new object[]
            {
                new NumberValueTrait
                {
                    trait_type = "Level",
                    value = seasonHero.level
                },
                new NumberValueTrait
                {
                    trait_type = "Generation",
                    value = seasonHero.generation
                },

                new StringValueTrait
                {
                    trait_type = "Rarity",
                    value = seasonHero.rarity.ToString()
                },
                
                new NumberValueTrait
                {
                    trait_type = "Total Hit Points",
                    value = (int)(seasonHero.totalHitPoints)
                },

                new StringValueTrait
                {
                    trait_type = "Gender",
                    value = seasonHero.gender.ToString()
                },
                new StringValueTrait
                {
                    trait_type = "Class",
                    value = seasonHero.heroClass.ToString()
                },

            })
            .Concat(CreateAttributes(seasonHero, "Seasonal "))
            .Concat(CreateAttributes(perpetualHero, "Base "))

            .Concat(seasonHero.skills.Select(s=> new JustValueTrait
            {
                value = "Skill " + s.name
            })).ToArray()
        };

        return metadata;
    }


}

public class OpenSeaMetadata
{
    public string description { get; set; } = String.Empty;
    public string external_url { get; set; } = String.Empty;
    public string image { get; set; } = String.Empty;
    public string name { get; set; } = String.Empty;

    public object[] attributes { get; set; } = new object[] { };

}

public class JustValueTrait
{
    public string value { get; set; } = string.Empty;
}

public class StringValueTrait
{
    public string trait_type { get; set; } = string.Empty;
    public string value { get; set; } = string.Empty;
}

public class NumberValueTrait
{
    public string display_type { get; set; } ="number";

    public string trait_type { get; set; } = string.Empty;
    public int value { get; set; } 
}



