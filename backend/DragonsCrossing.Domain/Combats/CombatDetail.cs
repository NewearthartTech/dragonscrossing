using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DragonsCrossing.Domain.Combats
{
    /// <summary>
    /// The detail of what happened in each state of combat. This serves as a history, troubleshooting, and information
    /// to the user in case they want to know what random values were calculated, etc...
    /// </summary>
    public class CombatDetail
    {
        public CombatDetail()
        {
        }   

        public CombatDetail(CombatDetailType type, int order, bool isSuccess = false, double amount = 0)
        {
            this.Type = type;
            this.Order = order;
            this.IsSuccess = isSuccess;
            this.Amount = amount;
        }

        public int Id { get; set; }

        public int CombatId { get; set; }
        
        public CombatDetailType Type { get; set; }

        /// <summary>
        /// Did the combat state succeed
        /// </summary>
        public bool IsSuccess { get; set; }

        /// <summary>
        /// The value calculated in each state.
        /// Ex: Damage dealt, or parry percent, or special ability damage
        /// </summary>
        public double Amount { get; set; }

        /// <summary>
        /// The order the states flowed
        /// </summary>
        public int Order { get; set; }
    }
}
