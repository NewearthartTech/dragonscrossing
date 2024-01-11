using DragonsCrossing.Domain.Items;
using Newtonsoft.Json;
using Org.BouncyCastle.Utilities;
using System.Reflection;

namespace DragonsCrossing.Core.Helper
{
    public static partial class DataHelper
    {
        public static int CalculateAndRound(int currentValue, double multiplier)
        {
            return (int)Math.Round(currentValue * multiplier);
        }
    }
}
