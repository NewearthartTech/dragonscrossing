using DragonsCrossing.Core.Contracts.Api.Dto.Heroes;
using DragonsCrossing.Domain.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Range = DragonsCrossing.Core.Contracts.Api.Dto.Monsters.Range;


namespace DragonsCrossing.Core.Common;

/// <summary>
/// Used to throw exception with text that should be shown to the end user
/// Other Exceptions are logged but not shown to the front end
/// </summary>
public class ExceptionWithCode : Exception
{
    public System.Net.HttpStatusCode Status { get; }

    public ExceptionWithCode(string message,
        System.Net.HttpStatusCode status = System.Net.HttpStatusCode.BadRequest,
        Exception? innerException = null
        ):base(message: message, innerException: innerException)
    {
        Status = status;
    }
}

public static class ExtensionMethods
{
    /// <summary>
    /// returns a random item from the list
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="list"></param>
    /// <returns></returns>
    public static T Rand<T> (this List<T> list)
    {
        Random random = new Random();
        return list[random.Next(0, list.Count)];
    }   
    
    /// <summary>
    /// Gets a random item from a list based on the values in the list.
    /// The values should be a percentage because the min value is always 0
    /// </summary>
    /// <param name="orderedList"></param>
    public static double GetRandomItemFromList(List<double> orderedList)
    {
        double max = orderedList.Sum();
        orderedList.Sort();
        Random random = new Random();
        var randNum = new RangeDouble(0, max).GetRandom();
        double total = 0;            

        for (int i = 0; i < orderedList.Count(); i++)
        {
            total += orderedList[i];
            if (randNum < total)
                return orderedList[i];
        }
        return -1;
    }

    public static Random GetRandomWithSeed()
    {
        return new Random((int)DateTimeOffset.UtcNow.ToUnixTimeMilliseconds());
    }

    public static T GetRandom<T>(this List<T> list, int startIndex = 0, int endIndex = -1)
    {
        endIndex = endIndex < 0 ? list.Count -1 : endIndex;
        var randomIndex = new Random().Next(startIndex, endIndex);
        return list[randomIndex];
    }

    /// <summary>
    /// Returns a random item from the list based on the value passed in the lambda (ie. the selector).
    /// The value in the lambda has to be a double or int. The idea is, the lambda/selector 
    /// tells the algorithm how likely that particular element in the list will be returned.
    /// </summary>
    /// <typeparam name="T">Any complex class with a double or int property</typeparam>
    /// <param name="list">The list of complex classes</param>
    /// <param name="selector">The double or int property that determines how likely the class will be chosen</param>
    /// <returns></returns>
    public static T GetRandom<T> (this List<T> list, Func<T, double> selector)
    {
        double max = list.Sum(selector);
        double randNum = new Random().NextDouble(0, max);
        double total = 0;
        for (int i = 0; i < list.Count(); i++)
        {
            total += selector(list[i]);
            if (randNum < total)
                return list[i];
        }
        // we should never get here... but just in case we do, return the last item instead of throwing an exception
        // TODO: log a warning here
        return list.Last();
    }

    /// <summary>
    /// Will round positive and negative numbers. .5 is always rounded up.
    /// Ex: 1.5 becomes 2. 
    /// </summary>
    /// <param name="targetValue"></param>
    /// <returns></returns>
    public static int RoundToNearestInt(this double targetValue)
    {
        if (targetValue < 0)
        {
            return (int)(targetValue - 0.5);
        }
        return (int)(targetValue + 0.5);
    }

    /// <summary>
    /// Generates a random number between 0 and 100 and returns
    /// true if the random number generated is less than the targetValue.
    /// </summary>
    /// <param name="targetValue">a value between 0 and 100.</param>
    /// <returns></returns>
    public static bool IsSuccessfulRoll(this double targetValue)
    {
        double randNum = new Random().NextDouble(0, 100);
        return randNum < targetValue;
    }

    /// <summary>
    /// Generates a random number between 0 and 100 and returns
    /// true if the random number generated is less than the targetValue.
    /// </summary>
    /// <param name="targetValue">a value between 0 and 100.</param>
    /// <returns></returns>
    public static bool IsSuccessfulRoll(this int targetValue)
    {
        int randNum = new Random().Next(0, 100);
        return randNum < targetValue;
    }

    public static double GetRandom(this RangeDouble targetValue)
    {
        if (targetValue.Min > targetValue.Max)
            throw new ArgumentException("Max must be greater than or equal to min");
        Random r = new Random();
        return r.NextDouble(targetValue.Min, targetValue.Max);
    }

    /// <summary>
    /// returns a random double within the min/max values inclusive.
    /// Because it's a double it can return a decimal number like 23.85
    /// </summary>
    /// <param name="random"></param>
    /// <param name="minValue"></param>
    /// <param name="maxValue"></param>
    /// <returns></returns>
    public static double NextDouble(
    this Random random,
    double minValue,
    double maxValue)
    {
        return random.NextDouble() * (maxValue - minValue) + minValue;
    }



    public static void UpdateFromTemplate<TDto,TTemplate>(this TDto dto, TTemplate template, string[]? propsToIgnore = null)
    {
        var dtoProps = typeof(TDto)
    .GetProperties(BindingFlags.Public | BindingFlags.Instance)
    .ToDictionary(k => k.Name, v => v);

        var templateProps = typeof(TTemplate).GetProperties(BindingFlags.Public | BindingFlags.Instance);

        if (typeof(IPolymorphicBase).IsAssignableFrom(typeof(TTemplate)))
        {
            //filter out type.
            templateProps = templateProps.Where(p => p.Name != "type").ToArray();
        }

        foreach (var templateProp in templateProps)
        {
            if (!dtoProps.ContainsKey(templateProp.Name))
            {
                //don't assert 
                //Debug.Assert(false, "We have template and DTO mismatch");
                continue;
            }

            if(null != propsToIgnore && propsToIgnore.Contains(templateProp.Name))
            {
                continue;
            }

            var dtoProp = dtoProps[templateProp.Name];
            if (!dtoProp.CanWrite)
            {
                Debug.Assert(false, "We have template and DTO mismatch");
                continue;
            }

            object value = templateProp.GetValue(template);

            if (null == value)
                continue;

            // This part is to translate the affectedAttributes property from the item template 
            if (templateProp.PropertyType == typeof(Dictionary<AffectedHeroStatTypeDto, Range>))
            {
                Dictionary<AffectedHeroStatTypeDto, int> affectedAttributes = new Dictionary<AffectedHeroStatTypeDto, int>();

                var dic = value as Dictionary<AffectedHeroStatTypeDto, Range>;

                foreach (var entry in dic)
                {
                    var dicKey = entry.Key;
                    var dicValue = new Random().Next(entry.Value.lower, entry.Value.upper + 1);
                    affectedAttributes.Add(dicKey, dicValue);
                }

                value = affectedAttributes;
            }      

            if (templateProp.PropertyType == typeof(Range))
            {
                var range = value as Range;

                value = new Random().Next(range.lower, range.upper + 1);
            }

            dtoProp.SetValue(dto, value);
        }

    }

}
