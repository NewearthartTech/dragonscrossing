using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DragonsCrossing.Core.Contracts.Api.Dto.Players
{
    public class PlayerGameSettingDto
    {
        [Required]
        public bool AutoRoll { get; set; }

        [Required]
        public bool PlayMusic { get; set; }

        [Required]
        public int MusicVolume { get; set; }

        [Required]
        public bool PlayVoice { get; set; }

        [Required]
        public int VoiceVolume { get; set; }

        [Required]
        public bool PlaySoundEffects { get; set; }

        [Required]
        public int SoundEffectsVolume { get; set; }
    }
}
