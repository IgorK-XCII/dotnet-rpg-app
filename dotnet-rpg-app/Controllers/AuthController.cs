using System.Threading.Tasks;
using dotnet_rpg_app.Data;
using dotnet_rpg_app.Dtos.User;
using dotnet_rpg_app.Models;
using Microsoft.AspNetCore.Mvc;

namespace dotnet_rpg_app.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthRepository _authRepo;

        public AuthController(IAuthRepository authRepo)
        {
            _authRepo = authRepo;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(UserRegisterDto request)
        {
            ServiceResponse<int> response =
                await _authRepo.Register(new User {Username = request.Username}, request.Password);
            if (!response.Success) return BadRequest(response);
            return Ok(response);
        }
        
        [HttpPost("login")]
        public async Task<IActionResult> Login(UserLoginDto request)
        {
            ServiceResponse<string> response =
                await _authRepo.Login(request.Username, request.Password);
            if (!response.Success) return BadRequest(response);
            return Ok(response);
        }
    }
}