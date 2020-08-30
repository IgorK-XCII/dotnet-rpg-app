using System;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using dotnet_rpg_app.Data;
using dotnet_rpg_app.Dtos.Character;
using dotnet_rpg_app.Dtos.CharacterSkill;
using dotnet_rpg_app.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace dotnet_rpg_app.Services.CharacterSkillService
{
    public class CharacterSkillService : ICharacterSkillService
    {
        private readonly IMapper _mapper;
        private readonly DataContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CharacterSkillService(IMapper mapper, DataContext context, IHttpContextAccessor httpContextAccessor)
        {
            _mapper = mapper;
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task<ServiceResponse<GetCharacterDto>> AddCharacterSkill(AddCharacterSkillDto newCharacterSkill)
        {
            try
            {
                Character character = await _context.Characters
                    .Include(ch => ch.Weapon)
                    .Include(ch => ch.CharacterSkills).ThenInclude(sk => sk.Skill)
                    .FirstAsync(ch =>
                    ch.Id == newCharacterSkill.CharacterId && ch.User.Id ==
                    int.Parse(_httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier)));

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