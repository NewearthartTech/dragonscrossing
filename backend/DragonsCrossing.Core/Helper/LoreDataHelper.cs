using DragonsCrossing.Core.Common;
using DragonsCrossing.Core.Contracts.Api.Dto.Items;
using DragonsCrossing.Domain.Items;
using DragonsCrossing.NewCombatLogic;
using Newtonsoft.Json;
using Org.BouncyCastle.Utilities;
using System.Reflection;
using static DragonsCrossing.Core.Contracts.Api.Dto.Zones.TileDto;

namespace DragonsCrossing.Core.Helper
{
    public class LoreAnswer
    {
        public Dictionary<string, string> LoreAnswers { get; set; } = new Dictionary<string, string>();
    }

    public static partial class DataHelper
    {
        public static string GetLoreEncounterAnswer(string slug)
        {
            var executingAssembly = System.Reflection.Assembly.GetExecutingAssembly();

            if (null == executingAssembly)
                throw new InvalidOperationException("executingAssembly cannot be null");

            var templateName = $"{executingAssembly.GetName().Name}.templates.nonCombat.lore_answer.json";

            using (var stream = executingAssembly.GetManifestResourceStream(templateName))
            {
                if (null == stream)
                {
                    throw new Exception($"template {templateName} not found");
                }
                using (StreamReader reader = new StreamReader(stream))
                {
                    string result = reader.ReadToEnd();

                    var loreAnswer = JsonConvert.DeserializeObject<LoreAnswer>(result);

                    if (null == loreAnswer)
                        throw new InvalidDataException($"item template {templateName} could not be serialized");


                    loreAnswer.LoreAnswers.TryGetValue(slug, out var answer);

                    return answer;

                }
            }
        }

        public static ChanceEncounter CreateChanceEncounterFromTemplate(ChanceEncounterTemplate template)
        {
            ChanceEncounter encounter = new ChanceEncounter();


            string[] propToIgnore = new string[] { nameof(ChanceEncounterTemplate.Choices) };
            encounter.UpdateFromTemplate(template, propToIgnore);

            encounter.Choices = template.Choices.Select(temp => CreateChanceEncounterChoiceFromTemplate(temp)).ToArray(); ;

            return encounter;
        }

        public static EncounterChoice CreateChanceEncounterChoiceFromTemplate(EncounterChoiceTemplate template)
        {
            EncounterChoice encounterChoice = new EncounterChoice();
            encounterChoice.UpdateFromTemplate(template);

            return encounterChoice;
        }

        public static ChanceEncounterTemplate[] GetChanceEncounterTemplateData(bool withGamblerChoices)
        {
            var executingAssembly = System.Reflection.Assembly.GetExecutingAssembly();
            var chanceEncounters = new ChanceEncounterTemplate[] { };
            if (null == executingAssembly)
                throw new InvalidOperationException("executingAssembly cannot be null");

            var templateName = $"{executingAssembly.GetName().Name}.templates.nonCombat.chanceEncounter_result.json";

            using (var stream = executingAssembly.GetManifestResourceStream(templateName))
            {
                if (null == stream)
                {
                    throw new Exception($"template {templateName} not found");
                }
                using (StreamReader reader = new StreamReader(stream))
                {
                    string result = reader.ReadToEnd();

                    chanceEncounters = JsonConvert.DeserializeObject<ChanceEncounterTemplate[]>(result);

                    if (null == chanceEncounters)
                        throw new InvalidDataException($"Chance encounter template {templateName} could not be serialized");
                }
            }
            var newChanceEncounters = new ChanceEncounterTemplate[] { };

            if (!withGamblerChoices)
            {
                foreach (var encounter in chanceEncounters)
                {
                    // remove the choices if its a gambler encounter
                    if (encounter.ChanceEncounterType == ChanceEncounterEnum.gambler)
                    {
                        encounter.Choices = new EncounterChoiceTemplate[] { };
                    }

                    newChanceEncounters = newChanceEncounters.Concat(new[] { encounter }).ToArray();
                }

                return newChanceEncounters;
            }

            return chanceEncounters;
        }
    }
}
