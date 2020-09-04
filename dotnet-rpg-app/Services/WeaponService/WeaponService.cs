using System;
using System.Threading.Tasks;
using AutoMapper;
using dotnet_rpg_app.Data;
using dotnet_rpg_app.Dtos.Character;
using dotnet_rpg_app.Dtos.Weapon;
using dotnet_rpg_app.Models;
using dotnet_rpg_app.Services.CharacterService;

namespace dotnet_rpg_app.Services.WeaponService
{
    public class WeaponService : IWeaponService
    {
        private readonly IMapper _mapper;
        private readonly DataContext _context;
        private readonly ICharacterService _characterService;

        public WeaponService(IMapper mapper, DataContext context, ICharacterService characterService)
        {
            _context = context;
            _characterService = characterService;
            _mapper = mapper;
        }
        public async Task<ServiceResponse<GetCharacterDto>> AddWeapon(AddWeaponDto newWeapon)
        {
            try
            {
                Character character = await _characterService.GetCharacterById(newWeapon.CharacterId, AuthType.WithAuth);
                Weapon weapon = _mapper.Map<Weapon>(newWeapon);
                await _context.Weapons.AddAsync(weapon);
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
                    Message = e.Message,
                };
            }
        }
    }
}