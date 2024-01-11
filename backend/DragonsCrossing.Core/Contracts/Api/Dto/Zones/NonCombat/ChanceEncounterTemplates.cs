using DragonsCrossing.NewCombatLogic;
using static DragonsCrossing.Core.Common.ExtensionMethods;
using static DragonsCrossing.Core.Helper.DataHelper;

namespace DragonsCrossing.Core.Contracts.Api.Dto.Zones;

public partial class TileDto
{
    public enum ChanceEncounterEnum
    {
        foreignBerries,
        freshwaterOrb,
        gambler,
        unknown,
        lovecraftianMonster,
        wonderingWizard,
        rustingArandomWeapon,
        riddler,
        cocytus2,
    }
}

public partial class NonCombatHelpers
{
    readonly IDiceService _dice;
    public NonCombatHelpers(IDiceService dice)
    {
        _dice = dice;
    }

    public ChanceEncounter GetCocytus2NonCombatEncounter()
    {
        var template = new ChanceEncounterTemplate
        {
            Slug="33_2",
            EncounterName= "Wandering Wizard Cocytus",
            ChanceEncounterType = TileDto.ChanceEncounterEnum.cocytus2,
            IntroText= "Hello Adventurer. I'll send you back to Aedos for safekeeping.",
            Choices = new EncounterChoiceTemplate[] {
                new EncounterChoiceTemplate
                {
                    choiceSlug="15",
                    choiceText="Yes, send me back to Aedos, Cocytus.",
                    goodOutcomeChance=100,
                    goodOutcomeText="Back to Aedos you go!"
                }
            }
        };

        return CreateChanceEncounterFromTemplate(template);
    }

    public ChanceEncounter GetRandomChanceEncounter()
    {
        var allChanceEncounterTemplate = GetChanceEncounterTemplateData(false);

        var allChanceEncounters = allChanceEncounterTemplate.Select(temp => CreateChanceEncounterFromTemplate(temp)).ToArray();

        if (null == allChanceEncounters)
        {
            throw new InvalidDataException($"Unable to load chance encounter data");
        }

        int random = _dice.Roll(allChanceEncounters.Length, Combats.DiceRollReason.GenerateRandomChanceEncounter) - 1;

        return allChanceEncounters[random];
    }
}