using System.Collections.Generic;
using dotnet_rpg_app.Dtos.Skill;
using dotnet_rpg_app.Dtos.Weapon;
using dotnet_rpg_app.Models;

namespace dotnet_rpg_app.Dtos.Character
{
    public class GetCharacterDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = "Frodo";
        public int Health { get; set; } = 100;
        public int Strength { get; set; } = 10;
        public int Defence { get; set; } = 10;
        public int Intelligence { get; set; } = 10;
        public int Luck { get; set; } = 10;
        public RpgClasses Class { get; set; } = RpgClasses.Necromancer;
        public GetWeaponDto Weapon { get; set; }
        public List<GetSkillDto> Skills { get; set; }
    }
}