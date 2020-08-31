using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using dotnet_rpg_app.Dtos.Character;
using dotnet_rpg_app.Dtos.Fight;
using dotnet_rpg_app.Dtos.Skill;
using dotnet_rpg_app.Dtos.Weapon;
using dotnet_rpg_app.Models;

namespace dotnet_rpg_app
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<Character, GetCharacterDto>().ForMember(dto => dto.Skills,
                character => 
                    character.MapFrom(ch=> ch.CharacterSkills.Select(cs => cs.Skill)));
            CreateMap<AddCharacterDto, Character>();
            CreateMap<AddWeaponDto, Weapon>();
            CreateMap<Weapon, GetWeaponDto>();
            CreateMap<GetWeaponDto, Weapon>();
            CreateMap<Skill, GetSkillDto>();
            CreateMap<Character, UpdateCharacterDto>();
            CreateMap<Character, HighScoreDto>();
        }
    }
}