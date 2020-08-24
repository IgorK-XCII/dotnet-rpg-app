using AutoMapper;
using dotnet_rpg_app.Dtos.Character;
using dotnet_rpg_app.Models;

namespace dotnet_rpg_app
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<Character, GetCharacterDto>();
            CreateMap<AddCharacterDto, Character>();
        }
    }
}