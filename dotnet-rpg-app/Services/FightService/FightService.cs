using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using dotnet_rpg_app.Data;
using dotnet_rpg_app.Dtos.Character;
using dotnet_rpg_app.Dtos.Fight;
using dotnet_rpg_app.Models;
using dotnet_rpg_app.Services.CharacterService;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace dotnet_rpg_app.Services.FightService
{
    public class FightService : IFightService
    {
        private readonly IMapper _mapper;
        private readonly ICharacterService _characterService;
        private readonly DataContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public FightService(IMapper mapper, ICharacterService characterService, DataContext context, IHttpContextAccessor httpContextAccessor)
        {
            _mapper = mapper;
            _characterService = characterService;
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<ServiceResponse<AttackResultDto>> WeaponAttack(WeaponAttackDto weaponAttack)
        {
            ServiceResponse<AttackResultDto> response = new ServiceResponse<AttackResultDto>();
            try
            {
                ServiceResponse<GetCharacterDto> attackerResponse = await _characterService.GetCharacterById(weaponAttack.AttackerId);
                Character attacker = _mapper.Map<Character>(attackerResponse.Data);

                if (!attackerResponse.Success) throw new Exception("Attacker not found.");

                Character defender = await _context.Characters.FirstOrDefaultAsync(ch => ch.Id == weaponAttack.DefenderId);
                if (defender == null) throw new Exception("Defender not found.");

                int damage = DoWeaponAttack(attacker, defender);
                if (defender.Health <= 0) response.Message = $"{defender.Name} has fallen!";

                await _characterService.UpdateCharacter(_mapper.Map<UpdateCharacterDto>(defender));
                
                response.Data = new AttackResultDto
                {
                    Attacker = attacker.Name,
                    AttackerHp = attacker.Health,
                    Defender = defender.Name,
                    DefenderHp = defender.Health,
                    Damage = damage,
                };
            }
            catch (Exception e)
            {
                response.Success = false;
                response.Message = e.Message;
            }

            return response;
        }

        public async Task<ServiceResponse<AttackResultDto>> SkillAttack(SkillAttackDto skillAttack)
        {
            ServiceResponse<AttackResultDto> response = new ServiceResponse<AttackResultDto>();
            try
            {
                Character attacker = await _context.Characters
                    .Include(ch => ch.CharacterSkills)
                    .ThenInclude(cs => cs.Skill)
                    .FirstOrDefaultAsync(ch => ch.Id == skillAttack.AttackerId && ch.User.Id ==
                        int.Parse(_httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier)));
                
                if (attacker == null) throw new Exception("Attacker not found.");
                
                Skill skill = attacker.CharacterSkills.Select(cs => cs.Skill).FirstOrDefault(s => s.Id == skillAttack.SkillId);
                if (skill == null) throw new Exception("Skill not found");
                
                Character defender = await _context.Characters.FirstOrDefaultAsync(ch => ch.Id == skillAttack.DefenderId);
                if (defender == null) throw new Exception("Defender not found.");

                int damage = DoSkillAttack(attacker, defender, skill);
                if (defender.Health <= 0) response.Message = $"{defender.Name} has fallen!";

                response.Data = new AttackResultDto
                {
                    Attacker = attacker.Name,
                    AttackerHp = attacker.Health,
                    Defender = defender.Name,
                    DefenderHp = defender.Health,
                    Damage = damage,
                };
            }
            catch (Exception e)
            {
                response.Success = false;
                response.Message = e.Message;
            }

            return response;
        }

        public async Task<ServiceResponse<FightResultDto>> Fight(FightRequestDto fightRequest)
        {
            ServiceResponse<FightResultDto> response = new ServiceResponse<FightResultDto>();
            try
            {
                List<Character> characters = await _context.Characters
                    .Include(ch => ch.Weapon)
                    .Include(ch => ch.CharacterSkills)
                    .ThenInclude(cs => cs.Skill)
                    .Where(ch => fightRequest.CharacterIds.Contains(ch.Id)).ToListAsync();
                
                response.Data = new FightResultDto();

                bool defeated = false;
                while (!defeated)
                {
                    foreach (Character attacker in characters)
                    {
                        List<Character> defenders = characters.Where(ch => ch.Id != attacker.Id).ToList();
                        Character defender = defenders[new Random().Next(defenders.Count)];

                        int damage;
                        string attackUsed;
                        
                        bool useWeapon = new Random().Next(2) == 0;
                        if (useWeapon)
                        {
                            damage = DoWeaponAttack(attacker, defender);
                            attackUsed = attacker.Weapon.Name;
                        }
                        else
                        {
                            int randomSkill = new Random().Next(attacker.CharacterSkills.Count);
                            Skill skill = attacker.CharacterSkills[randomSkill].Skill;
                            attackUsed = skill.Name;
                            damage = DoSkillAttack(attacker, defender, skill);
                        }
                        
                        response.Data.Log.Add($"{attacker.Name} attacks {defender.Name} using {attackUsed} with {(damage >= 0 ? damage.ToString() : 0.ToString())} damage");

                        if (defender.Health <= 0)
                        {
                            defeated = true;
                            attacker.Victories++;
                            defender.Defeats++;
                            response.Data.Log.Add($"{attacker.Name} wins with {attacker.Health} HP left");
                            response.Data.Log.Add($"{defender.Name} has been defeated!");
                            break;
                        }
                    }
                }
                foreach (Character character in characters)
                {
                    character.Fights++;
                    character.Health = 100;
                }
                _context.Characters.UpdateRange(characters);
                await _context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                response.Success = false;
                response.Message = e.Message;
            }

            return response;
        }

        public async Task<ServiceResponse<List<HighScoreDto>>> GetHighScore()
        {
            List<Character> characters = await _context.Characters
                .Where(ch => ch.Fights > 0)
                .OrderByDescending(ch => ch.Victories)
                .ThenBy(ch => ch.Defeats)
                .ToListAsync();
            return new ServiceResponse<List<HighScoreDto>>
            {
                Data = characters.Select(ch => _mapper.Map<HighScoreDto>(ch)).ToList(),
                Message = characters.Count == 0 ? "Characters haven't fought yet!" : null,
                Success = characters.Count != 0
            };
        }

        private int DoWeaponAttack(Character attacker, Character defender)
        {
            int damage = new Random().Next(attacker.Strength);
            if (attacker.Weapon != null) damage += attacker.Weapon.Damage;
            damage -= new Random().Next(defender.Defence);
            if (damage > 0) defender.Health -= damage;
            return damage;
        }

        private int DoSkillAttack(Character attacker, Character defender, Skill skill)
        {
            int damage = new Random().Next(attacker.Intelligence) + skill.Damage;
            damage -= new Random().Next(defender.Defence);
            if (damage > 0) defender.Health -= damage;
            return damage;
        }
    }
}