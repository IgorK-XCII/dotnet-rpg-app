using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using AutoMapper;
using dotnet_rpg_app.Data;
using dotnet_rpg_app.Dtos.Character;
using dotnet_rpg_app.Models;
using Microsoft.EntityFrameworkCore;

namespace dotnet_rpg_app.Services.CharacterService
{
    public class CharacterService : ICharacterService
    {
        private readonly IMapper _mapper;
        private readonly DataContext _context;

        public CharacterService(IMapper mapper, DataContext context)
        {
            _mapper = mapper;
            _context = context;
        }

        public async Task<ServiceResponse<List<GetCharacterDto>>> GetAllCharacters(int userId)
        {
            List<Character> dbCharacters = await _context.Characters.Where(ch => ch.User.Id == userId).ToListAsync();
            return new ServiceResponse<List<GetCharacterDto>>
            {
                Data = dbCharacters.Select(ch => _mapper.Map<GetCharacterDto>(ch)).ToList()
            };
        }
        
        public async Task<ServiceResponse<GetCharacterDto>> GetCharacterById(int id)
        {
            return new ServiceResponse<GetCharacterDto>
            {
                Data = _mapper.Map<GetCharacterDto>(await  _context.Characters.FirstOrDefaultAsync(ch => ch.Id == id)),
            };
        } 

        public async Task<ServiceResponse<List<GetCharacterDto>>> AddCharacter(AddCharacterDto newCharacter)
        {
            Character character = _mapper.Map<Character>(newCharacter);
            
            await _context.Characters.AddAsync(character);
            await _context.SaveChangesAsync();
            
            return new ServiceResponse<List<GetCharacterDto>>
            {
                Data = _context.Characters.Select(ch => _mapper.Map<GetCharacterDto>(ch)).ToList()
            };
        }

        public async Task<ServiceResponse<GetCharacterDto>> UpdateCharacter(UpdateCharacterDto updatedCharacter)
        {
            Character character = await _context.Characters.FirstOrDefaultAsync(ch => ch.Id == updatedCharacter.Id);
            PropertyInfo[] oldProperty =  character?.GetType().GetProperties();
            for (int i = 0; i < oldProperty?.Length; i++)
            {
                typeof(Character).GetProperties()[i].SetValue(character, typeof(UpdateCharacterDto).GetProperties()[i].GetValue(updatedCharacter));
            }

            if (character != null)
            {
                _context.Characters.Update(character);
                await _context.SaveChangesAsync();
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
                Character character = await _context.Characters.FirstAsync(ch => ch.Id == id);
                _context.Characters.Remove(character);
                await _context.SaveChangesAsync();
                response.Data = _context.Characters.Select(ch => _mapper.Map<GetCharacterDto>(ch)).ToList();
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
