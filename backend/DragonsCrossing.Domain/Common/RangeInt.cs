using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DragonsCrossing.Domain.Common
{
    /// <summary>
    /// Represents a min/max range usually used to produce a random number.
    /// This can also represent a dice roll.
    /// Ex: 2d6 == min:2, max: 12
    /// Ex2: 3d7 == min:3, max: 21
    /// </summary>
    public class RangeInt
    {
        public RangeInt(int min, int max)
        {
            Min = min;
            Max = max;
        }
        public int Min { get; set; }
        public int Max { get; set; }

        /// <summary>
        /// Generates a random number between min and max inclusive.
        /// Both min and max are included in the random selection
        /// </summary>
        /// <returns></returns>
        public int GetRandom()
        {
            // max + 1 because Max is not inclusive
            return new Random().Next(Min, Max + 1);
        }
    }
}
