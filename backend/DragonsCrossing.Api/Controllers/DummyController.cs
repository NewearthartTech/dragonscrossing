using DragonsCrossing.Core.Contracts.Api;
using DragonsCrossing.Core.Contracts.Api.Dto.Armors;
using DragonsCrossing.Core.Contracts.Api.Dto.Combats;
using DragonsCrossing.Core.Contracts.Api.Dto.Heroes;
using DragonsCrossing.Core.Contracts.Api.Dto.Items;
using DragonsCrossing.Core.Contracts.Api.Dto.Monsters;
using DragonsCrossing.Core.Contracts.Api.Dto.Players;
using DragonsCrossing.Core.Contracts.Api.Dto.Skill;
using DragonsCrossing.Core.Contracts.Api.Dto.Zones;
using DragonsCrossing.Domain.Armors;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace DragonsCrossing.Api.Controllers
{
    
    [Route("api/dummy")]
    [ApiController]
    public class DummyController : ControllerBase
    {

        public DummyController()
        {
        }

        [HttpGet("4")]
        public async Task<UpdateInventoryRequestDto> DummyRoute4(ItemListDto itemListDto)
        {
            throw new NotImplementedException();
        }

        [HttpGet("9")]
        public async Task<DisplayItemDetailsDto> DummyRoute9(DisplayItemMenuDto displayItemMenuDto)
        {
            throw new NotImplementedException();
        }

        

        [HttpGet("2")]
        public async Task<UpdateSkillStateRequestDto> DummyRoute2(PlayerLoginRequestDto playerLoginRequestDto)
        {
            throw new NotImplementedException();
        }

        [HttpGet("3")]
        public async Task<MonsterLootDto> DummyRoute3(MonsterDto monsterDto)
        {
            throw new NotImplementedException();
        }


        [HttpGet("6")]
        public async Task<AllocateStatsRequestDto> DummyRoute6(EquipItemRequestDto equipItemRequestDto)
        {
            throw new NotImplementedException();
        }

        [HttpGet("7")]
        public async Task<HeroListDto> DummyRoute7(HeroStatModifierResultDtoDto heroStatModifierResultDtoDto)
        {
            throw new NotImplementedException();
        }

        [HttpGet("8")]
        public async Task<LevelUpResponseDto> DummyRoute8(SelectedHeroDto selectedHeroDto)
        {
            throw new NotImplementedException();
        }

        

        [HttpGet("10")]
        public async Task<MoveItemRequestDto> DummyRoute10(DisplayItemMenuDto displayItemMenuDto)
        {
            throw new NotImplementedException();
        }

        [HttpGet("11")]
        public async Task<HeroStatModifierResultDto> DummyRoute11(AddItemToInventoryRequestDto addItemToInventoryRequestDto)
        {
            throw new NotImplementedException();
        }

        
        [HttpGet("12")]
        public async Task<HeroStatModifierResultDto> DummyRoute12(ZoneBackgroundTypeDto addItemToInventoryRequestDto)
        {
            throw new NotImplementedException();
        }
        
    }
}
