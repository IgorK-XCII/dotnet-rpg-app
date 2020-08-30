using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using dotnet_rpg_app.Data;
using dotnet_rpg_app.Dtos.Character;
using dotnet_rpg_app.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace dotnet_rpg_app.Services.CharacterService
{
    public class CharacterService : ICharacterService
    {
        private readonly IMapper _mapper;
        private readonly DataContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CharacterService(IMapper mapper, DataContext context, IHttpContextAccessor httpContextAccessor)
        {
            _mapper = mapper;
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<ServiceResponse<List<GetCharacterDto>>> GetAllCharacters()
        {
            List<Character> dbCharacters = await _context.Characters
                .Include(ch => ch.Weapon)
                .Include(ch => ch.CharacterSkills).ThenInclude(cs => cs.Skill)
                .Where(ch => ch.User.Id == GetUserId()).ToListAsync();
            return new ServiceResponse<List<GetCharacterDto>>
            {
                Data = dbCharacters.Select(ch => _mapper.Map<GetCharacterDto>(ch)).ToList()
            };
        }
        
        public async Task<ServiceResponse<GetCharacterDto>> GetCharacterById(int id)
        {
            return new ServiceResponse<GetCharacterDto>
            {
                Data = _mapper.Map<GetCharacterDto>(await  _context.Characters
                    .Include(ch => ch.Weapon)
                    .Include(ch => ch.CharacterSkills).ThenInclude(cs => cs.Skill)
                    .FirstOrDefaultAsync(ch => ch.Id == id && ch.User.Id == GetUserId())),
            };
        } 

        public async Task<ServiceResponse<List<GetCharacterDto>>> AddCharacter(AddCharacterDto newCharacter)
        {
            Character character = _mapper.Map<Character>(newCharacter);
            character.User = await _context.Users.FirstOrDefaultAsync(user => user.Id == GetUserId());
            
            await _context.Characters.AddAsync(character);
            await _context.SaveChangesAsync();
            
            return new ServiceResponse<List<GetCharacterDto>>
            {
                Data = _context.Characters
                    .Include(ch => ch.Weapon)
                    .Include(ch => ch.CharacterSkills).ThenInclude(cs => cs.Skill)
                    .Where(ch => ch.User.Id == GetUserId()).Select(ch => _mapper.Map<GetCharacterDto>(ch)).ToList()
            };
        }

        public async Task<ServiceResponse<GetCharacterDto>> UpdateCharacter(UpdateCharacterDto updatedCharacter)
        {
            Character character = await _context.Characters.Include(ch => ch.User).FirstOrDefaultAsync(ch => ch.Id == updatedCharacter.Id);
            if (character?.User.Id == GetUserId())
            {
                PropertyInfo[] newProperty =  typeof(UpdateCharacterDto).GetProperties();
                for (int i = 0; i < newProperty.Length; i++)
                {
                    typeof(Character).GetProperties()[i].SetValue(character, typeof(UpdateCharacterDto).GetProperties()[i].GetValue(updatedCharacter));
                }
                
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
                Character character = await _context.Characters.FirstAsync(ch => ch.Id == id && ch.User.Id == GetUserId());
                _context.Characters.Remove(character);
                await _context.SaveChangesAsync();
                response.Data = _context.Characters.Where(ch => ch.User.Id == GetUserId()).Select(ch => _mapper.Map<GetCharacterDto>(ch)).ToList();
            }
            catch (Exception e)
            {
                response.Message = e.Message;
                response.Success = false;
            }

            return response;
        }

        private int GetUserId() => int.Parse(_httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier));
    }
}
