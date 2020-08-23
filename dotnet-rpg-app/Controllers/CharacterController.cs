using System.Collections.Generic;
using System.Linq;
using dotnet_rpg_app.Models;
using Microsoft.AspNetCore.Mvc;

namespace dotnet_rpg_app.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CharacterController : ControllerBase
    {
        private static List<Character> characters = new List<Character>
        {
            new Character {Name = "Ingvar"},
            new Character {Id = 1, Name = "Sam", Class = RpgClasses.Assassin}
        };
        
        [HttpGet("getAll")]
        public IActionResult Get() => Ok(characters);
        
        [HttpGet("{id}")]
        public IActionResult GetSingle(int id) => Ok(characters.FirstOrDefault(el => el.Id == id));

    }
}