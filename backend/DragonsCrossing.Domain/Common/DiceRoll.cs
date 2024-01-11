using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DragonsCrossing.Domain.Common
{
    public class DiceRoll
    {
        public int NumberOfDice { get; set; } = 1;
        public int NumberOfSides { get; set; } = 2;

        public int RollDice()
        {
            int total = 0;
            Random roll = new Random();
            for (int i = 0; i < NumberOfDice; i++)
            {
                total += roll.Next(1, NumberOfSides + 1);
            }
            return total;
        }
    }
}
