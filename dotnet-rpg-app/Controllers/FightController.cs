using System.Collections.Generic;
using System.Threading.Tasks;
using dotnet_rpg_app.Dtos.Fight;
using dotnet_rpg_app.Models;
using dotnet_rpg_app.Services.FightService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace dotnet_rpg_app.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class FightController : ControllerBase
    {
        private readonly IFightService _fightService;

        public FightController(IFightService fightService)
        {
            _fightService = fightService;
        }
        
        [HttpPost("weaponAttack")]
        public async Task<IActionResult> WeaponAttack(WeaponAttackDto weaponAttack)
        {
            ServiceResponse<AttackResultDto> result = await _fightService.WeaponAttack(weaponAttack);
            if (!result.Success) return NotFound(result);
            return Ok(result);
        }

        [HttpPost("skillAttack")]
        public async Task<IActionResult> SkillAttack(SkillAttackDto skillAttack)
        {
            ServiceResponse<AttackResultDto> result = await _fightService.SkillAttack(skillAttack);
            if (!result.Success) return NotFound(result);
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Fight(FightRequestDto fightRequest)
        {
            ServiceResponse<FightResultDto> result = await _fightService.Fight(fightRequest);
            if (!result.Success) return NotFound(result);
            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetHighScore()
        {
            ServiceResponse<List<HighScoreDto>> result = await _fightService.GetHighScore();
            if (!result.Success) return NotFound(result);
            return Ok(result);
        }
    }
}