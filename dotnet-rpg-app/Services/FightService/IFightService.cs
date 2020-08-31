using System.Collections.Generic;
using System.Threading.Tasks;
using dotnet_rpg_app.Dtos.Fight;
using dotnet_rpg_app.Models;

namespace dotnet_rpg_app.Services.FightService
{
    public interface IFightService
    {
        Task<ServiceResponse<AttackResultDto>> WeaponAttack(WeaponAttackDto weaponAttack);
        Task<ServiceResponse<AttackResultDto>> SkillAttack(SkillAttackDto skillAttack);
        Task<ServiceResponse<FightResultDto>> Fight(FightRequestDto fightRequest);
        Task<ServiceResponse<List<HighScoreDto>>> GetHighScore();
    }
}