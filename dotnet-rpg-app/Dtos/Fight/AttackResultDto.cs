namespace dotnet_rpg_app.Dtos.Fight
{
    public class AttackResultDto
    {
        public string Attacker { get; set; }
        public string Defender { get; set; }
        public int AttackerHp { get; set; }
        public int DefenderHp { get; set; }
        public int Damage { get; set; }
    }
}