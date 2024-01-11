using DragonsCrossing.Core.Contracts.Api.Dto.Heroes;
using DragonsCrossing.Core.Contracts.Api.Dto.Items;
using Newtonsoft.Json;
using System.Reflection;

namespace DragonsCrossing.Api.Utilities
{
    public class ExtentionMethods
    {
        public static long[] ParseColonSeperatedIds(string? data)
        {
            return string.IsNullOrWhiteSpace(data) ? new long[] { } :
                                            data.Split(":").Select(h => long.Parse(h)).ToArray();
        }

        /// <summary>
        /// used to load a configuration object from a publicly accesible URL
        /// </summary>
        public static async Task<T> LoadObjectFromURL<T>(string url)
        {
            using (HttpClient client = new HttpClient())
            using (HttpResponseMessage response = await client.GetAsync(url))
            {
                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception($"status code: {response.StatusCode} - {response.ReasonPhrase}");
                }

                using (HttpContent content = response.Content)
                {
                    var jsonStr = await content.ReadAsStringAsync();

                    try
                    {
                        return JsonConvert.DeserializeObject<T>(jsonStr)!;
                    }
                    catch (Exception ex)
                    {
                        throw new Exception("invalid json", ex);
                    }
                    
                }

            }
        }  

        public static bool IsHeroStatCompliedForItem(ItemDto itemToEquip, HeroDto hero)
        {
            var complianceDic = itemToEquip.heroStatComplianceDictionary.ToDictionary(k => k.Key.ToString().ToLower(), v => v.Value);
            var heroProps = typeof(HeroDto).GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (var heroProp in heroProps)
            {
                if (!complianceDic.ContainsKey(heroProp.Name.ToLower()))
                {
                    //don't assert 
                    //Debug.Assert(false, "We have template and DTO mismatch");
                    continue;
                }

                var requiredStatValue = complianceDic[heroProp.Name.ToLower()];

                object statValue = heroProp.GetValue(hero);

                if (null == statValue)
                    continue;

                if (heroProp.PropertyType == typeof(int))
                {
                    // If the required stat value is greater than stat value, return false because hero cannot equip the item
                    if ((int)statValue < requiredStatValue)
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        public static bool IsHeroLevelCompliedForItem(ItemDto itemToEquip, HeroDto hero)
        {
            // Hero level needs to be equal to or greater than the item levelRequirement
            return hero.level >= itemToEquip.levelRequirement;
        }
    }
}
