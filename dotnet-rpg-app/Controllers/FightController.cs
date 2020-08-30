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
    }
}