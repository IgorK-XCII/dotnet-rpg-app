﻿using System.Collections.Generic;
using System.Threading.Tasks;
using dotnet_rpg_app.Dtos.Character;
using dotnet_rpg_app.Models;

namespace dotnet_rpg_app.Services.CharacterService
{
    public enum AuthType
    {
        WithAuth,
        NoneAuth
    }
    public interface ICharacterService
    {
        Task<ServiceResponse<List<GetCharacterDto>>> GetAllCharacters();
        Task<ServiceResponse<GetCharacterDto>> GetCharacterDtoById(int id);
        Task<ServiceResponse<List<GetCharacterDto>>> AddCharacter(AddCharacterDto newCharacter);
        Task<ServiceResponse<GetCharacterDto>> UpdateCharacter(UpdateCharacterDto updatedCharacter);
        Task<ServiceResponse<List<GetCharacterDto>>> DeleteCharacter(int id);
        Task<Character> GetCharacterById(int id, AuthType auth);
        Task<List<Character>> GetGroupOfCharacters(IEnumerable<int> characters);
    }
}