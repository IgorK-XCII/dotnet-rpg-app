using System.Threading.Tasks;
using dotnet_rpg_app.Dtos.Character;
using dotnet_rpg_app.Dtos.CharacterSkill;
using dotnet_rpg_app.Models;
using dotnet_rpg_app.Services.CharacterSkillService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace dotnet_rpg_app.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class CharacterSkillController : ControllerBase
    {
        private readonly ICharacterSkillService _characterSkillService;

        public CharacterSkillController(ICharacterSkillService characterSkillService)
        {
            _characterSkillService = characterSkillService;
        }

        [HttpPost]
        public async Task<IActionResult> AddCharacterSkill(AddCharacterSkillDto newCharacterSkill)
        {
            ServiceResponse<GetCharacterDto> response = await _characterSkillService.AddCharacterSkill(newCharacterSkill);
            if (!response.Success) return BadRequest(response);
            return Ok(response);
        }
    }
}