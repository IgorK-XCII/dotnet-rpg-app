using System;
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
                ServiceResponse<GetCharacterDto> attackerResponse = await _characterService.GetCharacterById(weaponAttack.AttackerId);
                Character attacker = _mapper.Map<Character>(attackerResponse.Data);

                if (!attackerResponse.Success) throw new Exception("Attacker not found.");

                Character defender = await _context.Characters.FirstOrDefaultAsync(ch => ch.Id == weaponAttack.DefenderId);
                if (defender == null) throw new Exception("Defender not found.");

                int damage = new Random().Next(attacker.Strength);
                if (attacker.Weapon != null) damage += attacker.Weapon.Damage;
                damage -= new Random().Next(defender.Defence);
                if (damage > 0) defender.Health -= damage;
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
                Console.WriteLine(e);
                response.Success = false;
                response.Message = e.Message;
            }

            return response;
        }
    }
}