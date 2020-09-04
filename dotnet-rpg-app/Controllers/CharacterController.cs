using System.Collections.Generic;
using System.Threading.Tasks;
using dotnet_rpg_app.Dtos.Character;
using dotnet_rpg_app.Models;
using dotnet_rpg_app.Services.CharacterService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace dotnet_rpg_app.Controllers
{
    [Authorize]
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
        public async Task<IActionResult> GetSingle(int id) =>
            ActionResultHandler(await _characterService.GetCharacterDtoById(id));

        [HttpPost]
        public async Task<IActionResult> AddCharacter(AddCharacterDto newCharacter) =>
            ActionResultHandler(await _characterService.AddCharacter(newCharacter));

        [HttpPut]
        public async Task<IActionResult> UpdateCharacter(UpdateCharacterDto updatedCharacter) =>
            ActionResultHandler(await _characterService.UpdateCharacter(updatedCharacter));

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCharacter(int id) =>
            ActionResultHandler(await _characterService.DeleteCharacter(id));
        
        private IActionResult ActionResultHandler<T>(ServiceResponse<T> response)
        {
            if (!response.Success) return NotFound(response);
            return Ok(response);
        }
    }
}