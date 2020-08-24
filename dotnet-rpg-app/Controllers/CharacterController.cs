using System.Collections.Generic;
using System.Threading.Tasks;
using dotnet_rpg_app.Dtos.Character;
using dotnet_rpg_app.Models;
using dotnet_rpg_app.Services.CharacterService;
using Microsoft.AspNetCore.Mvc;

namespace dotnet_rpg_app.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CharacterController : ControllerBase
    {
        private readonly ICharacterService _characterService;
        
        public CharacterController(ICharacterService characterService)
        {
            _characterService = characterService;
        }
        
        [HttpGet("getAll")]
        public async Task<IActionResult> Get() => Ok(await _characterService.GetAllCharacters());
        
        [HttpGet("{id}")]
        public async Task<IActionResult> GetSingle(int id) => Ok(await _characterService.GetCharacterById(id));

        [HttpPost]
        public async Task<IActionResult> AddCharacter(AddCharacterDto newCharacter) => Ok(await _characterService.AddCharacter(newCharacter));

        [HttpPut]
        public async Task<IActionResult> UpdateCharacter(UpdateCharacterDto updatedCharacter)
        {
            ServiceResponse<GetCharacterDto> response = await _characterService.UpdateCharacter(updatedCharacter);
            if (response.Data == null) return NotFound(response);
            return Ok(response);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCharacter(int id)
        {
            ServiceResponse<List<GetCharacterDto>> response = await _characterService.DeleteCharacter(id);
            if (response.Data == null) return NotFound(response);
            return Ok(response);
        }
    }
}