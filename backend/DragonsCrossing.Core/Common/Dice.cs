using System;
using System.Security.Cryptography;
using DragonsCrossing.Core.Common;
using DragonsCrossing.Core.Contracts.Api.Dto.Combats;
using Microsoft.Extensions.Logging;
using static DragonsCrossing.Core.Common.ExtensionMethods;

namespace DragonsCrossing.NewCombatLogic;

public interface IDiceService
{
    int Roll(int Faces,
        DiceRollReason reason = DiceRollReason.General,
        IList<DieResultDto>? diceList = null, int dieModifier = 0);
}

public class DiceService: IDiceService
{
    public static Dictionary<DiceRollReason, int> _fixedResults = new Dictionary<DiceRollReason, int>();

    public static Dictionary<DiceRollReason, int> AddFixedResult(string reason, int value)
    {
        if(!Enum.TryParse<DiceRollReason>(reason,true, out var parsed))
        {
            throw new ExceptionWithCode("invalid reason");
        }

        // If we pass in a 0 for die value, we clear the cache for that reason.
        if (value == 0)
        {
            _fixedResults.Remove(parsed);
        }
        else 
        {
            _fixedResults[parsed] = value;

        }

        return _fixedResults;
    }

    readonly ILogger _logger;
    public DiceService(ILogger<DiceService> logger)
    {
        _logger = logger;
    }

    public int Roll(int faces,
        DiceRollReason reason = DiceRollReason.General,
        IList<DieResultDto>? dicelist = null, int dieModifier = 0)
    {
        //todo: take this out for prod
        if (_fixedResults.ContainsKey(reason))
            return _fixedResults[reason];

        /*
        var r = GetRandomWithSeed();

        // Next will return a random number greater than or equal to the lower boundary and less than (no equal) the upper boundary
        // That's why we need to set the boundary to Faces + 1 so we can get both 1 and 2 as results for a 2-face die
        var result =  r.Next(1, Faces + 1);
        */

        var result = RandomNumberGenerator.GetInt32(faces) + 1;

        if (null != dicelist)
        {
            dicelist.Add(new DieResultDto
            {
                RollFor = reason,
                Sides = faces,
                Result = result,
                Modifier = dieModifier
            });
        }

        var finalResult = result + dieModifier;

        _logger.LogDebug($"DiceRoll reason:{reason}  result:{result}, faces:{faces}, modifier : {dieModifier}, final result: {finalResult}");

        return finalResult;
    }

    
}



