using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DragonsCrossing.Domain.Heroes
{
    public class HeroName
    {
        public HeroName()
        {
        }
        public HeroName(string name)
        {
            this.Name = name;
        }
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
