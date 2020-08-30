using System.Threading.Tasks;
using dotnet_rpg_app.Dtos.Fight;
using dotnet_rpg_app.Models;

namespace dotnet_rpg_app.Services.FightService
{
    public interface IFightService
    {
        Task<ServiceResponse<AttackResultDto>> WeaponAttack(WeaponAttackDto weaponAttack);
    }
}