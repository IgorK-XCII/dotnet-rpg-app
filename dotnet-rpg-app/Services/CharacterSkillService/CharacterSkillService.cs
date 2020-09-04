using System;
using System.Threading.Tasks;
using AutoMapper;
using dotnet_rpg_app.Data;
using dotnet_rpg_app.Dtos.Character;
using dotnet_rpg_app.Dtos.CharacterSkill;
using dotnet_rpg_app.Models;
using dotnet_rpg_app.Services.CharacterService;
using Microsoft.EntityFrameworkCore;

namespace dotnet_rpg_app.Services.CharacterSkillService
{
    public class CharacterSkillService : ICharacterSkillService
    {
        private readonly IMapper _mapper;
        private readonly DataContext _context;
        private readonly ICharacterService _characterService;

        public CharacterSkillService(IMapper mapper, DataContext context, ICharacterService characterService)
        {
            _mapper = mapper;
            _context = context;
            _characterService = characterService;
        }
        public async Task<ServiceResponse<GetCharacterDto>> AddCharacterSkill(AddCharacterSkillDto newCharacterSkill)
        {
            try
            {
                Character character = await _characterService.GetCharacterById(newCharacterSkill.CharacterId, AuthType.WithAuth);
                if (character == null) throw new Exception("Character not found!");
                
                Skill skill = await _context.Skills.FirstAsync(sk => sk.Id == newCharacterSkill.SkillId);
                
                CharacterSkill characterSkill = new CharacterSkill
                {
                    Character = character,
                    Skill = skill
                };
                await _context.CharacterSkills.AddAsync(characterSkill);
                await _context.SaveChangesAsync();
                
                return new ServiceResponse<GetCharacterDto>
                {
                    Data = _mapper.Map<GetCharacterDto>(character)
                };
            }
            catch (Exception e)
            {
                return new ServiceResponse<GetCharacterDto>
                {
                    Success = false,
                    Message = e.Message
                };
            }
        }
    }
}