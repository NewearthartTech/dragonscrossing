using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DragonsCrossing.Core.Common
{
    public static class RandomEnum<T> where T : Enum
    {
        /// <summary>
        /// returns a random enum from the specified type
        /// </summary>
        /// <returns></returns>
        public static T Rand()
        {
            var random = new Random();
            var enumList = Enum.GetValues(typeof(T)).Cast<T>().ToList();
            return enumList[random.Next(0, enumList.Count)];
        }
    }
}
