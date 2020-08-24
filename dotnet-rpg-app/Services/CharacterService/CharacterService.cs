using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using AutoMapper;
using dotnet_rpg_app.Dtos.Character;
using dotnet_rpg_app.Models;

namespace dotnet_rpg_app.Services.CharacterService
{
    public class CharacterService : ICharacterService
    {
        private readonly IMapper _mapper;

        private static readonly List<Character> Characters = new List<Character>
        {
            new Character {Name = "Ingvar"},
            new Character {Id = 1, Name = "Sam", Class = RpgClasses.Assassin}
        };
        
        public CharacterService(IMapper mapper)
        {
            _mapper = mapper;
        }

        public async Task<ServiceResponse<List<GetCharacterDto>>> GetAllCharacters()
        {
            return new ServiceResponse<List<GetCharacterDto>>
            {
                Data = Characters.Select(ch => _mapper.Map<GetCharacterDto>(ch)).ToList()
            };
        }
        
        public async Task<ServiceResponse<GetCharacterDto>> GetCharacterById(int id)
        {
            return new ServiceResponse<GetCharacterDto>
            {
                Data = _mapper.Map<GetCharacterDto>(Characters.FirstOrDefault(ch => ch.Id == id)),
            };
        } 

        public async Task<ServiceResponse<List<GetCharacterDto>>> AddCharacter(AddCharacterDto newCharacter)
        {
            Character character = _mapper.Map<Character>(newCharacter);
            character.Id = Characters.Max(ch => ch.Id) + 1;
            Characters.Add(character);
            return new ServiceResponse<List<GetCharacterDto>>
            {
                Data = Characters.Select(ch => _mapper.Map<GetCharacterDto>(ch)).ToList()
            };
        }

        public async Task<ServiceResponse<GetCharacterDto>> UpdateCharacter(UpdateCharacterDto updatedCharacter)
        {
            Character character = Characters.FirstOrDefault(ch => ch.Id == updatedCharacter.Id);
            PropertyInfo[] oldProperty =  character?.GetType().GetProperties();
            for (int i = 0; i < oldProperty?.Length; i++)
            {
                typeof(Character).GetProperties()[i].SetValue(character, typeof(UpdateCharacterDto).GetProperties()[i].GetValue(updatedCharacter));
            }
            
            return new ServiceResponse<GetCharacterDto>
            {
                Data = _mapper.Map<GetCharacterDto>(character),
                Message = character == null ? "Character not found!" : null,
                Success = character != null
            };
        }

        public async Task<ServiceResponse<List<GetCharacterDto>>> DeleteCharacter(int id)
        {
            ServiceResponse<List<GetCharacterDto>> response = new ServiceResponse<List<GetCharacterDto>>();
            try
            {
                Character character = Characters.First(ch => ch.Id == id);
                Characters.Remove(character);
                response.Data = Characters.Select(ch => _mapper.Map<GetCharacterDto>(ch)).ToList();
            }
            catch (Exception e)
            {
                response.Message = e.Message;
                response.Success = false;
            }

            return response;
        }
    }
}