using System.Threading.Tasks;
using dotnet_rpg_app.Dtos.Character;
using dotnet_rpg_app.Dtos.CharacterSkill;
using dotnet_rpg_app.Models;

namespace dotnet_rpg_app.Services.CharacterSkillService
{
    public interface ICharacterSkillService
    {
        Task<ServiceResponse<GetCharacterDto>> AddCharacterSkill(AddCharacterSkillDto newCharacterSkill);
    }
}