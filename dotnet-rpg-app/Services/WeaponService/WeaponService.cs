using System;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using dotnet_rpg_app.Data;
using dotnet_rpg_app.Dtos.Character;
using dotnet_rpg_app.Dtos.Weapon;
using dotnet_rpg_app.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace dotnet_rpg_app.Services.WeaponService
{
    public class WeaponService : IWeaponService
    {
        private readonly IMapper _mapper;
        private readonly DataContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public WeaponService(IMapper mapper, DataContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            _mapper = mapper;
        }
        public async Task<ServiceResponse<GetCharacterDto>> AddWeapon(AddWeaponDto newWeapon)
        {
            try
            {
                Character character = await _context.Characters.FirstAsync(ch =>
                    ch.Id == newWeapon.CharacterId && ch.User.Id ==
                    int.Parse(_httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier)));
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