using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DragonsCrossing.Core.Contracts.Api.Dto.Players
{
    public class PlayerLoginRequestDto
    {
        [Required]
        public string Username { get; set; }

        [Required]
        public int Password { get; set; }

    }
}
