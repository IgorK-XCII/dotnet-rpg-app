using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using dotnet_rpg_app.Data;
using dotnet_rpg_app.Dtos.Character;
using dotnet_rpg_app.Dtos.Fight;
using dotnet_rpg_app.Models;
using dotnet_rpg_app.Services.CharacterService;
using Microsoft.EntityFrameworkCore;

namespace dotnet_rpg_app.Services.FightService
{
    public class FightService : IFightService
    {
        private readonly IMapper _mapper;
        private readonly ICharacterService _characterService;
        private readonly DataContext _context;

        public FightService(IMapper mapper, ICharacterService characterService, DataContext context)
        {
            _mapper = mapper;
            _characterService = characterService;
            _context = context;
        }

        public async Task<ServiceResponse<AttackResultDto>> WeaponAttack(WeaponAttackDto weaponAttack)
        {
            ServiceResponse<AttackResultDto> response = new ServiceResponse<AttackResultDto>();
            try
            {
                Character attacker = await _characterService.GetCharacterById(weaponAttack.AttackerId, AuthType.WithAuth);
                if (attacker == null) throw new Exception("Attacker not found.");

                Character defender = await _characterService.GetCharacterById(weaponAttack.DefenderId, AuthType.NoneAuth);
                if (defender == null) throw new Exception("Defender not found.");

                int damage = DoWeaponAttack(attacker, defender);
                if (defender.Health <= 0) response.Message = $"{defender.Name} has fallen!";

                await _characterService.UpdateCharacter(_mapper.Map<UpdateCharacterDto>(defender));

                response.Data = AttackResultHandler(attacker, defender, damage);
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
                Character attacker = await _characterService.GetCharacterById(skillAttack.AttackerId, AuthType.WithAuth);
                if (attacker == null) throw new Exception("Attacker not found.");
                
                Skill skill = attacker.CharacterSkills.Select(cs => cs.Skill).FirstOrDefault(s => s.Id == skillAttack.SkillId);
                if (skill == null) throw new Exception("Skill not found");
                
                Character defender = await _characterService.GetCharacterById(skillAttack.DefenderId, AuthType.NoneAuth);
                if (defender == null) throw new Exception("Defender not found.");

                int damage = DoSkillAttack(attacker, defender, skill);
                if (defender.Health <= 0) response.Message = $"{defender.Name} has fallen!";

                response.Data = AttackResultHandler(attacker, defender, damage);
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
                List<Character> characters = await _characterService.GetGroupOfCharacters(fightRequest.CharacterIds);
                
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
                        if (useWeapon || attacker.CharacterSkills.Count == 0)
                        {
                            damage = DoWeaponAttack(attacker, defender);
                            attackUsed = attacker.Weapon?.Name ?? "Fist";
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
        
        private AttackResultDto AttackResultHandler(Character attacker, Character defender, int damage)
        {
            return new AttackResultDto
            {
                Attacker = attacker.Name,
                AttackerHp = attacker.Health,
                Defender = defender.Name,
                DefenderHp = defender.Health,
                Damage = damage,
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