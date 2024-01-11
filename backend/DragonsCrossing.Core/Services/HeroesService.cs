using DragonsCrossing.Core.Common;
using DragonsCrossing.Core.Contracts.Api;
using DragonsCrossing.Core.Contracts.Api.Dto.Combats;
using DragonsCrossing.Core.Contracts.Api.Dto.Heroes;
using DragonsCrossing.Core.Contracts.Api.Dto.Zones;
using DragonsCrossing.Core.Helper;
using DragonsCrossing.NewCombatLogic;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using Newtonsoft.Json;
using static DragonsCrossing.Core.Helper.DataHelper;
using Range = DragonsCrossing.Core.Contracts.Api.Dto.Monsters.Range;

namespace DragonsCrossing.Core.Services
{
    public class HeroesService : IHeroesService
    {
        readonly IBlockchainService _blockchainService;
        readonly IDataHelperService _dataHelperService;
        readonly IPerpetualDbService _perpetualDb;
        readonly ILogger _logger;
        readonly IDiceService _dice;
        readonly IServiceProvider _sp;
        readonly Web3Config _web3Config;


        

        public HeroesService(
            IDiceService dice,
            IServiceProvider sp,
            IConfiguration config,
            IBlockchainService blockchainService,
            IDataHelperService dataHelperService,
            IPerpetualDbService perpetualDb,
            ILogger<HeroesService> logger
        )
        {
            _sp = sp;
            _dice = dice;
            _dataHelperService = dataHelperService;
            _logger = logger;
            _perpetualDb = perpetualDb;
            this._blockchainService = blockchainService;
            this._web3Config = config.GetSection("web3").Get<Web3Config>() ?? new Web3Config();
        }


        public async Task<HeroDto> GetHero(int heroId, bool perpetualOnly = false)
        {
            var enrolledInSeason = 0;

            {
                var hero = await _perpetualDb.getCollection<HeroDto>().Find(c => c.id == heroId).SingleOrDefaultAsync();

                if (null == hero)
                {
                    _logger.LogDebug($"Hero {heroId} Not in Db creating now");

                    hero = await CreateNewHero(heroId);
                }

                if (perpetualOnly)
                {
                    return hero;
                }


                if(0 == hero.seasonId)
                {
                    _logger.LogDebug($"Hero {heroId} Not in a season. returning perpetual Hero");
                    return hero;
                }

                enrolledInSeason = hero.seasonId;

            }

            _logger.LogDebug($"Hero {heroId} in a season. returning seasonal Hero");

            var seasonsDb = ActivatorUtilities.CreateInstance<SeasonsDbService>(_sp, enrolledInSeason);

            return await seasonsDb.getCollection<DbGameState>().Find(h => h.HeroId == heroId)
                .Project(g => g.Hero)
                .SingleAsync();
            
        }


        public class ReservedHero
        {
            public Gender gender { get; set; }
            public CharacterClassDto characterClass { get; set; }
            public ZoneBackgroundTypeDto zoneBackgroundType { get; set; }
        }

        readonly static Dictionary<int, ReservedHero> _reservedHeros = new Dictionary<int, ReservedHero>
        {
            { 1, new ReservedHero{ gender = Gender.Female, characterClass = CharacterClassDto.Ranger, zoneBackgroundType = ZoneBackgroundTypeDto.Swamp} },
            { 2, new ReservedHero{ gender = Gender.Male, characterClass = CharacterClassDto.Warrior, zoneBackgroundType = ZoneBackgroundTypeDto.Mountains} },
            { 3, new ReservedHero{ gender = Gender.Female, characterClass = CharacterClassDto.Mage, zoneBackgroundType = ZoneBackgroundTypeDto.Fields} },
            { 4, new ReservedHero{ gender = Gender.Male, characterClass = CharacterClassDto.Mage, zoneBackgroundType = ZoneBackgroundTypeDto.Swamp} },
            { 5, new ReservedHero{ gender = Gender.Male, characterClass = CharacterClassDto.Mage, zoneBackgroundType = ZoneBackgroundTypeDto.Mountains} },
            { 6, new ReservedHero{ gender = Gender.Male, characterClass = CharacterClassDto.Ranger, zoneBackgroundType = ZoneBackgroundTypeDto.Swamp} },
            { 7, new ReservedHero{ gender = Gender.Female, characterClass = CharacterClassDto.Warrior, zoneBackgroundType = ZoneBackgroundTypeDto.Mountains} },
            { 8, new ReservedHero{ gender = Gender.Female, characterClass = CharacterClassDto.Warrior, zoneBackgroundType = ZoneBackgroundTypeDto.Fields} },
            { 9, new ReservedHero{ gender = Gender.Female, characterClass = CharacterClassDto.Ranger, zoneBackgroundType = ZoneBackgroundTypeDto.Mountains} },
            { 10, new ReservedHero{ gender = Gender.Male, characterClass = CharacterClassDto.Warrior, zoneBackgroundType = ZoneBackgroundTypeDto.Swamp} },
            { 11, new ReservedHero{ gender = Gender.Male, characterClass = CharacterClassDto.Ranger, zoneBackgroundType = ZoneBackgroundTypeDto.Fields} },
            { 12, new ReservedHero{ gender = Gender.Female, characterClass = CharacterClassDto.Mage, zoneBackgroundType = ZoneBackgroundTypeDto.Mountains} },

        };


        async Task<HeroDto> CreateNewHero(int heroId)
        {
            var collection = _perpetualDb.getCollection<HeroDto>();

            //var createHeroParams = TEST_OWNED.Contains(heroId)?new CreateHeroParameters(): await _blockchainService.GetHeroMintedParams(heroId);
            var createHeroParams = new CreateHeroParameters();

            var hero = new HeroDto
            {
                id = heroId,
                generation = (_reservedHeros.ContainsKey(heroId)|| createHeroParams.isGenesisHero) ? 0 : 1

            };

            if(0 == hero.generation && !_reservedHeros.ContainsKey(heroId) && !_web3Config.devMode)
            {
                throw new Exception("no more Gen 0");
            }

            var executingAssembly = System.Reflection.Assembly.GetExecutingAssembly();

            if (_reservedHeros.ContainsKey(heroId))
            {
                hero.gender = _reservedHeros[heroId].gender;
                hero.image = "hero1.png";
            }
            else
            {
                var mintedGender = createHeroParams.mintParams.gender();
                if (null == mintedGender)
                {
                    var diceRoll = _dice.Roll(2, DiceRollReason.MintHeroGender);
                    hero.gender = 1 == diceRoll ? Gender.Male : Gender.Female;
                }
                else
                {
                    hero.gender = mintedGender.Value;
                }

            }

            #region hero name generation

            if (HeroDto.isDefaultChainFromId(heroId))
            {

                var heroFirstName = string.Empty;
                var heroLastName = string.Empty;

                var templateName = Gender.Male == hero.gender ? "male_hero_first_names" : "female_hero_first_names";
                templateName = $"{executingAssembly.GetName().Name}.templates.Hero_Names.{templateName}.json";

                using (Stream stream = executingAssembly.GetManifestResourceStream(templateName)!)
                using (StreamReader reader = new StreamReader(stream))
                {
                    string result = reader.ReadToEnd();

                    var names = JsonConvert.DeserializeObject<string[]>(result);

                    if (null == names)
                        throw new Exception($"failed to load hero first names from template {templateName}");

                    var diceRoll = _dice.Roll(names.Length, DiceRollReason.MintHeroName);
                    heroFirstName = names[diceRoll - 1];
                }

                templateName = $"{executingAssembly.GetName().Name}.templates.Hero_Names.hero_last_names.json";
                using (Stream stream = executingAssembly.GetManifestResourceStream(templateName)!)
                using (StreamReader reader = new StreamReader(stream))
                {
                    string result = reader.ReadToEnd();

                    var names = JsonConvert.DeserializeObject<string[]>(result);

                    if (null == names)
                        throw new Exception($"failed to load hero last names from template {templateName}");

                    var diceRoll = _dice.Roll(names.Length, DiceRollReason.MintHeroName);
                    heroLastName = names[diceRoll - 1];
                }

                hero.name = $"{heroFirstName} {heroLastName}";
            }
            else {
                var dfkVal =  await _blockchainService.DFkHerofromDcxId( heroId);

                hero.name = $"DFK - {dfkVal.getDFKShortName()}";

                hero.isLoanedHero = new LoanedHero
                {
                    loanerType = LoanerHeroType.DFK
                };
            }
            #endregion


            if (_reservedHeros.ContainsKey(heroId))
            {
                hero.heroClass = _reservedHeros[heroId].characterClass;
            }
            else
            {
                var mintedCharacterClass = createHeroParams.mintParams.heroClass();

                if (null == mintedCharacterClass)
                {
                    var characterClasses = Enum.GetValues(typeof(CharacterClassDto)).Cast<CharacterClassDto>().ToArray();
                    var diceRoll = _dice.Roll(characterClasses.Length, DiceRollReason.MintHeroClass);
                    hero.heroClass = characterClasses[diceRoll - 1];
                }
                else
                {
                    hero.heroClass = mintedCharacterClass.Value;
                }


            }

            var heroTemplate = new Contracts.Api.Dto.Heroes.HeroTemplate();
            hero.UpdateFromTemplate(heroTemplate);


            if(!heroTemplate.HeroMintingAttributes.TryGetValue(hero.heroClass, out var otherAttributes))
            {
                throw new Exception($"hero class {hero.heroClass} not found in heroTemplate.HeroMintingAttributes");
            }

            hero.UpdateFromTemplate(otherAttributes);

            hero.inventory = otherAttributes.equippedItemsTemplates
                        .Select(t => _dataHelperService.CreateItemFromTemplate(t, Contracts.Api.Dto.Zones.DcxZones.aedos
                        //, hero.rarity
                        ))
                        .ToArray();

            #region Skill Creation
            var learnedHeroSkills = otherAttributes.learnedSkillTemplates.Select(t => 
            {
                var unlearnedSkill = CreateSkillFromTemplate(t);
                var learnedSkill = unlearnedSkill.CreateSkillFromUnlearned();

                return learnedSkill;
            }).ToArray();

            if (null == learnedHeroSkills)
                throw new Exception("failed to create Learned skill from unlearned");

            hero.skills = learnedHeroSkills;

            #endregion Skill Creation

            // Will both gen0 and gen1 hero go through here and get saved into DB?
            //hero.generation = heroMintingParam?.heroGeneration??0;

            if (_reservedHeros.ContainsKey(heroId))
            {
                hero.rarity = HeroRarityDto.Mythic;
            }
            else
            {
                // If hero class is not null, that means we are minting with boosted mint program.
                hero.rarity = GetHeroRarity(hero, createHeroParams.mintParams.heroClass() != null ? true : false);
            }

            //update mintTime params
            hero.remainingHitPointsPercentage = 100.0;
            hero.usedUpSkillPoints = 0;

            #region Zone background
            if (_reservedHeros.ContainsKey(heroId))
            {
                hero.zoneBackgroundType = _reservedHeros[heroId].zoneBackgroundType;
            }
            else
            {
                int zoneBackgroundRoll = _dice.Roll(5, DiceRollReason.ZoneBackgroundRoll);
                // The first zone enum is labeled as 0 so we need to minus 1 from die roll result.
                int realZoneBackgroundSelection = zoneBackgroundRoll - 1;
                if (Enum.IsDefined(typeof(ZoneBackgroundTypeDto), realZoneBackgroundSelection))
                {
                    hero.zoneBackgroundType = (ZoneBackgroundTypeDto)(realZoneBackgroundSelection);
                }
            }
            #endregion Zone background

            hero.UpdateMaxExperiencePoints();


            try
            {
                await collection.InsertOneAsync(hero);
                return hero;
            }
            catch(Exception ex)
            {
                _logger.LogInformation($"Failed to insert hero id {hero.id}. It probably exists by now", ex);

                return await collection.Find(h => h.id == hero.id).SingleAsync();
            }

            
        }
        

        #region Private methods

        private HeroRarityDto GetHeroRarity(HeroDto hero, bool isBoostedMint)
        {
            //Dictionary<double, Range> dictionary = new Dictionary<double, Range>();

            //var executingAssembly = System.Reflection.Assembly.GetExecutingAssembly();

            //var templateName = $"{executingAssembly.GetName().Name}.ScheduleTableRef.HeroRarityMoneySchedule.json";

            //using (Stream stream = executingAssembly.GetManifestResourceStream(templateName))
            //if (stream != null)
            //{
            //    using (StreamReader reader = new StreamReader(stream))
            //    {
            //        string json = reader.ReadToEnd();

            //        dictionary = JsonConvert.DeserializeObject<Dictionary<double, Range>>(json);
            //    };
            //}

            // Depending on how much the player pays for the hero, we give them more chances to get rarer hero
            //double multiplier = dictionary.Where(r => moneyPaid >= r.Value.lower && moneyPaid < r.Value.upper).Select(r => r.Key).FirstOrDefault();

            //if (multiplier < 1)
            //{
            //    multiplier = 1;
            //    _logger.LogInformation($"Failed to retrieve hero rarity money schedule value, money Paid: ${moneyPaid}");
            //}

            //heroRarityRoll = (int)Math.Round(heroRarityRoll * multiplier);

            // If heroRarityRoll exceed the the upper limit of 99, set it back to 99
            //heroRarityRoll = heroRarityRoll >= 99 ? 99 : heroRarityRoll;

            HeroRarityDto rarity;
            int heroRarityRoll = _dice.Roll(10000, DiceRollReason.HeroRarityRoll);

            if (hero.generation == 0)
            {

                if (isBoostedMint)
                {
                    rarity = GenZeroHeroRarityWithBoostedMint.Where(r => heroRarityRoll >= r.Value.lower && heroRarityRoll <= r.Value.upper).Select(r => r.Key).First();
                }
                else 
                {
                    rarity = GenZeroHeroRarity.Where(r => heroRarityRoll >= r.Value.lower && heroRarityRoll <= r.Value.upper).Select(r => r.Key).First();
                }
                
            }
            else
            {
                rarity = NonGenZeroHeroRarity.Where(r => heroRarityRoll >= r.Value.lower && heroRarityRoll <= r.Value.upper).Select(r => r.Key).First();
            }

            return rarity;
        }

        #endregion Private methods

        #region Private variables

        private Dictionary<HeroRarityDto, Range> NonGenZeroHeroRarity { get; set; } = new Dictionary<HeroRarityDto, Range>
        {
            { HeroRarityDto.Common, new Range(){ lower = 0, upper = 5500 } },
            { HeroRarityDto.Uncommon, new Range(){ lower = 5501, upper = 8000 }},
            { HeroRarityDto.Rare, new Range(){ lower = 8001, upper = 9200 }},
            { HeroRarityDto.Legendary, new Range(){ lower = 9201, upper = 9800 }},
            { HeroRarityDto.Mythic, new Range(){ lower = 9801, upper = 10000 }}
        };

        private Dictionary<HeroRarityDto, Range> GenZeroHeroRarity { get; set; } = new Dictionary<HeroRarityDto, Range>
        {
            { HeroRarityDto.Rare, new Range(){ lower = 0, upper = 7500 }},
            { HeroRarityDto.Legendary, new Range(){ lower = 7501, upper = 9500 }},
            { HeroRarityDto.Mythic, new Range(){ lower = 9501, upper = 10000 }}
        };

        private Dictionary<HeroRarityDto, Range> GenZeroHeroRarityWithBoostedMint { get; set; } = new Dictionary<HeroRarityDto, Range>
        {
            { HeroRarityDto.Rare, new Range(){ lower = 0, upper = 6250 }},
            { HeroRarityDto.Legendary, new Range(){ lower = 6251, upper = 9250 }},
            { HeroRarityDto.Mythic, new Range(){ lower = 9251, upper = 10000 }}
        };

        #endregion Private variables
    }
}
