using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DragonsCrossing.Core.Contracts.Api.Dto.Combats
{
    public class OpportunityResultTypeDto
    {
        [Required]
        public OpportunityResultTypeEnum ResultType { get; set; }

        [Required]
        public string Description { get; set; }
    }
}
