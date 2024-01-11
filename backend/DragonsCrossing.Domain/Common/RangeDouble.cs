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
    public class RangeDouble
    {        
        public RangeDouble()
        {
        }
        public RangeDouble(double min, double max)
        {
            this.Min = min;
            this.Max = max;
        }

        /// <summary>
        /// this is inclusive
        /// </summary>
        public double Min { get; set; }

        /// <summary>
        /// This is inclusive (ie. <= Max)
        /// </summary>
        public double Max { get; set; }        
    }
}
