// See https://aka.ms/new-console-template for more information



using System.Reflection;
using Newtonsoft.Json;
using DragonsCrossing.Core.Contracts.Api.Dto.Items;
using Newtonsoft.Json.Converters;
using CsvHelper;
using System.Globalization;
using CSVFile;
using Range = DragonsCrossing.Core.Contracts.Api.Dto.Monsters.Range;
using System.Text.RegularExpressions;
using DragonsCrossing.Core.Contracts.Api.Dto.Heroes;
using DragonsCrossing.Core.Contracts.Api.Dto.Combats;
using DragonsCrossing.Domain.Heroes;
using System.Collections;
using DragonsCrossing.Core.Contracts.Api.Dto.Monsters;
using System.Linq;
using DragonsCrossing.Core.Contracts.Api.Dto.Skill;
using DragonsCrossing.Core.Contracts.Api.Dto.Zones;
using DragonsCrossing.Core.Helper;




//map of template file to the folder
var templatesMap = new Dictionary<string, string>
{
    {"dcxTemplate-Items","items" },
    {"dcxTemplate-Monsters","monsters" },
    {"dcxTemplate-UnidentifiedSkills","skillItems" },
    {"dcxTemplate-UnlearnedSkills","skills" }
};

var specialAbilityArray = new MonsterSpecialAbilityDto[] { };

var arrayHeaders = new ArrayHeader[] { };

var lineNo = 1;

try
{
    var myArgs = Environment.GetCommandLineArgs();

    if(myArgs[1] == "ensure_sync")
    {
        ensureSureTemplatesSync();
        return;
    }

    if(myArgs.Length < 3)
    {
        throw new Exception("missing arguments: createTemplates [templatesFolder] [template.json]");
    }

    //Users/dee/Downloads/dcxTemplate-Items-Sheet1.csv
    var templateFile = myArgs[2];

    var found = templatesMap.Where(t => templateFile.Contains(t.Key)).Select(t => t.Value).SingleOrDefault();
    if (null == found)
        throw new Exception($"template file name {templateFile} is not valid. Must contain one of \n {string.Join("\n", templatesMap.Select(t=>t.Key))}");

    //Users/dee/dragonCross/DragonsCrossingApi/DragonsCrossing.Core/templates
    var templateFolder =  Path.Combine(myArgs[1], found)  ;


    Console.WriteLine($"Conversion started {templateFile} -> {templateFolder}");

    IEnumerable? templates = null;

    switch (found)
    {
        case "skillItems":
            templates = ReadTemplateFile<SkillItem>(templateFile);
            templateFolder = Path.Combine(myArgs[1], "items");
            break;
        case "skills":
            templates = ReadTemplateFile<UnlearnedHeroSkill>(templateFile);
            break;
        case "items":
            templates = ReadTemplateFile<ItemTemplate>(templateFile);
            break;
        case "monsters":
            {
                templates = ReadTemplateFile<MonsterTemplate>(templateFile);

                var justFolder = Path.GetDirectoryName(templateFile);
                if (string.IsNullOrWhiteSpace(justFolder))
                    throw new Exception($"cannot find folder for file {templateFile}");

                var abilityfiles = Directory.EnumerateFiles(justFolder).Where(f => f.Contains("dcxTemplate-SpecialAbility")).ToArray();
                if(abilityfiles.Length != 1)
                {
                    throw new Exception($"folder {justFolder} needs one file with name containing dcxTemplate-SpecialAbility");
                }
                specialAbilityArray = ReadTemplateFile<MonsterSpecialAbilityDto>(abilityfiles[0]).ToArray();

            }
            break;
        default:
            throw new NotImplementedException();
    }

    if (!Directory.Exists(templateFolder))
    {
        Directory.CreateDirectory(templateFolder);
    }

    if (null == templates)
        throw new Exception("failed to read Template");

    lineNo = 1;
    foreach (var templt in templates)
    {
        object? created = null;
        string outfileName = "";

        switch (found)
        {
            case "skillItems":
                created = templt;
                outfileName = ((SkillItem)templt).slug + ".json";
                break;
            case "skills":
                created = templt;
                outfileName = ((UnlearnedHeroSkill)templt).slug + ".json";
                break;
            case "items":
                created = templt;
                outfileName = "item_" + ((ItemTemplate)templt).slug + ".json";
                break;
            case "monsters":
                created = templt;
                outfileName = "monster_" + ((MonsterTemplate)templt).MonsterSlug + ".json";
                break;
            default:
                throw new NotImplementedException();

        }

        var formatedString = JsonConvert.SerializeObject(created, Formatting.Indented);

        outfileName = Path.Combine(templateFolder, outfileName);

        File.WriteAllText(outfileName, formatedString, System.Text.Encoding.UTF8);
        Console.WriteLine($"Conversion done line {lineNo} -> {outfileName}");

    }
    
    Console.WriteLine("all rows done");
}
catch(Exception ex)
{
    Console.Error.WriteLine($"failed to complete at line {lineNo} : {ex}");
}


async void ensureSureTemplatesSync()
{
    Console.WriteLine("ensure template sync");
    var monsterTemplates = TileDto.loadMonsterTemplates();

    /*
    foreach(var template in monsterTemplates)
    {
        var created = template.LootItemsTemplates
            .Select(t => DataHelper.CreateItemFromTemplate(t.Key, HeroRarityDto.Common, DcxZones.foulWastes)).ToArray();
    }
    */

    Console.WriteLine("All monster loots created");

} 


IEnumerable<TTemplate> ReadTemplateFile<TTemplate>(string templateFile) where TTemplate:new()
{
    using (var reader = new StreamReader(templateFile))
    using (CSVReader cr = new CSVReader(reader, new CSVSettings
    {

    }))
    {
        arrayHeaders = arrayHeaders.Concat(cr.Headers.Select(h =>
        {
            if (!h.Contains("_"))
                return new ArrayHeader();

            var parts = h.Split("_");
            if (parts.Length != 2)
                return new ArrayHeader();

            if (!int.TryParse(parts[1], out var order))
            {
                return new ArrayHeader();
            }

            return new ArrayHeader
            {
                name = parts[0],
                order = order,
                columnHeader = h
            };
        }).Where(o => !string.IsNullOrWhiteSpace(o.name))).ToArray();


        foreach (string[] line in cr)
        {
            lineNo++;

            var myObj = cr.Headers.Select((h, i) =>
            new
            {
                key = h,
                value = line[i]
            }).ToDictionary(k => k.key, v => v.value);

            yield return LoadFromJson<TTemplate>(myObj);

        }

    }
}

TTemplate LoadFromJson<TTemplate>(Dictionary<string,string> dto) where TTemplate:new()
{
    var template = new TTemplate();

    var templateProps = typeof(TTemplate).GetProperties(BindingFlags.Public | BindingFlags.Instance);

    foreach (var prop in templateProps)
    {
        var propName = prop.Name;

        string data = string.Empty;

        if (dto.ContainsKey(propName))
        {
            data = dto[propName];
        }
        else
        {
            var mutiCols = arrayHeaders.Where(a => a.name == propName).ToArray();

            if(mutiCols.Length > 0)
            {
                data = JsonConvert.SerializeObject(mutiCols.Select(m =>
                {
                    return dto.ContainsKey(m.columnHeader) ?dto[m.columnHeader]:string.Empty;
                }));

            }
            
        }

        if (string.IsNullOrWhiteSpace(data))
        {
            continue;
        }

        var myType = prop.PropertyType;
        var nulableType = Nullable.GetUnderlyingType(myType);
        if (null != nulableType)
        {
            myType = nulableType;
        }


        object? value = null;

        if(myType == typeof(string))
        {
            value = data;
        }
        else if (myType == typeof(int))
        {
            if(!int.TryParse(data,out var intVal))
            {
                throw new Exception($"{data} is not integer");
            }

            value = intVal;
        }
        else if (myType == typeof(string[]))
        {
            value = data.Split("\n");
        }
        else if (myType == typeof(bool))
        {
            if(!bool.TryParse(data, out var foundBool))
            {
                throw new Exception($"value for {propName} is not bool");
            }

            value = foundBool;
        }
        else if (myType == typeof(Range))
        {
            value = HandleRange(data);
        }
        else if (myType.IsEnum)
        {
            if(!Enum.TryParse(myType, data.ToString(),true, out var enumValue))
            {
                throw new Exception($"failed to get enum value for {propName}:{data}");
            }

            value = enumValue;
        }
        else
        {

            switch (propName)
            {
                case nameof(SkillItem.skill):
                    value = HandleUnidentifiedHeroSkill(data);
                    break;

                case nameof(ItemTemplate.affectedAttributes):
                    value = HandleEffectedAtributes(data);
                    break;
                case nameof(ItemTemplate.dieDamage):
                case nameof(MonsterTemplate.DieDamage):
                    value = HandleDieDto(data);
                    break;
                case nameof(ItemTemplate.heroStatComplianceDictionary):
                    value = HandleHeroStatComplianceDictionary(data);
                    break;
                case nameof(ItemTemplate.allowedHeroClassList):
                    value = HandleEnumArray<CharacterClassDto>(data);
                    break;

                case nameof(MonsterTemplate.LootItemsTemplates):
                    value = HandleLootItems(data);
                    break;
                case nameof(MonsterTemplate.SpecialAbility):
                    var ability = specialAbilityArray.FirstOrDefault(a => a.Name == data);
                    if (null == ability)
                        throw new Exception($"failed to find ability for {data}");
                    value = ability;
                    break;

                case nameof(MonsterSpecialAbilityDto.Affects):
                    value = handleStatusEffects(data);
                    break;

                default:
                    Console.WriteLine($"propname {propName} not Handeled");
                    break;
            }


        }

        if(null != value)
            prop.SetValue(template, value);
    }

    return template;

}

StatusEffectDto[] handleStatusEffects(string data)
{
    var dataStrings = JsonConvert.DeserializeObject<string[]>(data);

    if (null == dataStrings)
        throw new Exception("failed to serielaize data for Status Effetcs");

    return dataStrings.Select(dataline =>
    {
        if (string.IsNullOrWhiteSpace(dataline))
            return new StatusEffectDto();

        var rows = dataline.Split("\n").Select(r => r.Trim()).Where(r => !string.IsNullOrWhiteSpace(r)).ToArray();

        var kv = rows.Select(r =>
        {
            var parts = r.Split(":");
            if (parts.Length != 2)
                throw new Exception($"{r} is not valid effected Attribute");


            return new
            {
                key = parts[0].Trim(),
                value = parts[1].Trim()
            };
        }).ToDictionary(k => k.key, v => v.value);

        return LoadFromJson<StatusEffectDto>(kv);
    }).Where(s=>!string.IsNullOrWhiteSpace(s.FriendlyStatName))
        .ToArray();

}

T[] HandleEnumArray<T>(string data) where T : struct
{
    var parts = data.Split(",").Select(r => r.Trim()).Where(r => !string.IsNullOrWhiteSpace(r)).ToArray();

    return parts.Select(p => Enum.Parse<T>(p, true)).ToArray();
}


UnidentifiedHeroSkill HandleUnidentifiedHeroSkill(string data)
{
    var rows = data.Split("\n").Select(r => r.Trim())
        .Where(r => !string.IsNullOrWhiteSpace(r))
        .Select(r =>
        {
            var parts = r.Split(":");
            if (parts.Length != 2)
                throw new Exception($"{r} is not valid row");

            return new
            {
                key = parts[0].Trim(),
                value = parts[1].Trim(),
            };

        })
        .ToDictionary(k => k.key, v => v.value);


    if (!rows.ContainsKey(nameof(UnidentifiedHeroSkill.dcxToIdentify)))
        throw new Exception($"row {data} is not valid");

    return new UnidentifiedHeroSkill
    {
        dcxToIdentify = decimal.Parse(rows[nameof(UnidentifiedHeroSkill.dcxToIdentify)])
    };
    
}

Dictionary<string, MonsterLootCharacteristics> HandleLootItems(string data)
{
    var rows = data.Split("\n").Select(r => r.Trim()).Where(r => !string.IsNullOrWhiteSpace(r)).ToArray();

    return rows.Select(r =>
    {
        var parts = r.Split(":");
        if (parts.Length != 2)
            throw new Exception($"{r} is not valid effected Attribute");


        return new
        {
            key = parts[0],
            value = new MonsterLootCharacteristics
            {
                ChancesOfDrop = int.Parse(parts[1])
            }
                
        };
    }).ToDictionary(k => k.key, v => v.value);
}

Dictionary<HeroStatType, int> HandleHeroStatComplianceDictionary(string data)
{
    var rows = data.Split("\n").Select(r => r.Trim()).Where(r => !string.IsNullOrWhiteSpace(r)).ToArray();

    return rows.Select(r =>
    {
        var parts = r.Split(":");
        if (parts.Length != 2)
            throw new Exception($"{r} is not valid effected Attribute");

        if (!Enum.TryParse<HeroStatType>(parts[0], true, out var atrib))
            throw new Exception($"{parts[0]} is not HeroStatType");

        return new
        {
            key = atrib,
            value = int.Parse(parts[1])
        };
    }).ToDictionary(k => k.key, v => v.value);
}

Dictionary<AffectedHeroStatTypeDto, Range> HandleEffectedAtributes(string data)
{
    var rows = data.Split("\n").Select(r => r.Trim()).Where(r=>!string.IsNullOrWhiteSpace(r)) .ToArray();

    return rows.Select(r =>
    {
        var parts = r.Split(":");
        if (parts.Length != 2)
            throw new Exception($"{r} is not valid effected Attribute");

        if (!Enum.TryParse<AffectedHeroStatTypeDto>(parts[0],true, out var atrib))
            throw new Exception($"{parts[0]} is not AffectedHeroStatTypeDto");

        return new
        {
            key = atrib,
            value = HandleRange(parts[1])
        };
    }).ToDictionary(k=>k.key,v=>v.value);
}

DieDto[] HandleDieDto(string data)
{
    var pattern = new Regex(@"(?<lower>\d+)[dD](?<upper>\d+)");
    var match = pattern.Match(data);

    if (!match.Success)
        throw new Exception($"{data} does not DieDto[]");

    var lower = match.Groups["lower"].ToString();
    var upper = int.Parse(match.Groups["upper"].ToString());


    return Enumerable.Range(0, int.Parse(lower)).Select(i => new DieDto
    {
        Sides = upper
    }).ToArray();

}


Range HandleRange(string data)
{
    var pattern = new Regex(@"(?<lower>\d+)\s*-\s*(?<upper>\d+)");
    var match = pattern.Match(data);

    if (!match.Success)
        throw new Exception($"{data} does not match range");

    var lower = match.Groups["lower"].ToString();
    var upper = match.Groups["upper"].ToString();

    return new Range
    {
        lower = int.Parse(lower),
        upper = int.Parse(upper)

    };

}

public class ArrayHeader
{
    public string name { get; set; } = "";
    public int order { get; set; }
    public string columnHeader { get; set; } = "";
}




