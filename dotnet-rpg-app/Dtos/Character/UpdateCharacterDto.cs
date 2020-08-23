using dotnet_rpg_app.Models;

namespace dotnet_rpg_app.Dtos.Character
{
    public class UpdateCharacterDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = "Frodo";
        public int Health { get; set; } = 100;
        public int Strength { get; set; } = 10;
        public int Defence { get; set; } = 10;
        public int Intelligence { get; set; } = 10;
        public int Luck { get; set; } = 10;
        public RpgClasses Class { get; set; } = RpgClasses.Necromancer;
    }
}