using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DragonsCrossing.Domain.Common
{
    public static class MathRound
    {
        public static int Down(int value)
        {
            return (int)Math.Round((double)value, MidpointRounding.ToZero);
        }

        public static double Down(double value)
        {
            return (int)Math.Round((double)value, MidpointRounding.ToZero);
        }
    }
}
