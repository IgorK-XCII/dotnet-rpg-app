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
        public async Task<IActionResult> WeaponAttack(WeaponAttackDto weaponAttack) =>
            ActionResultHandler(await _fightService.WeaponAttack(weaponAttack));

        [HttpPost("skillAttack")]
        public async Task<IActionResult> SkillAttack(SkillAttackDto skillAttack) =>
            ActionResultHandler(await _fightService.SkillAttack(skillAttack));

        [HttpPost]
        public async Task<IActionResult> Fight(FightRequestDto fightRequest) =>
            ActionResultHandler(await _fightService.Fight(fightRequest));

        [HttpGet]
        public async Task<IActionResult> GetHighScore() => ActionResultHandler(await _fightService.GetHighScore());
        
        private IActionResult ActionResultHandler<T>(ServiceResponse<T> response)
        {
            if (!response.Success) return NotFound(response);
            return Ok(response);
        }
    }
}