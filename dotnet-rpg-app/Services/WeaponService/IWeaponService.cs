using System.Threading.Tasks;
using dotnet_rpg_app.Dtos.Character;
using dotnet_rpg_app.Dtos.Weapon;
using dotnet_rpg_app.Models;

namespace dotnet_rpg_app.Services.WeaponService
{
    public interface IWeaponService
    {
        Task<ServiceResponse<GetCharacterDto>> AddWeapon(AddWeaponDto newWeapon);
    }
}