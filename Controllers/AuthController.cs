using alposim.DTO;
using alposim.Repository;
using Microsoft.AspNetCore.Mvc;
using alposim.Interfaces;
using alposim.Models;
namespace alposim.Controllers
{
    [Route("api/[controller]")] 
    [ApiController]
    
    public class AuthController : Controller
    {
        
        private readonly IAuthRepository _authRepository;

        public AuthController(IAuthRepository authRepository)
        {
            _authRepository = authRepository;
        }


        [HttpPost("register")]
        public async Task<IActionResult> RegisterAsync(RegisterDto registerDto)
        {
            try
            {
                var user = await _authRepository.Register(registerDto);
                return Ok(new
                {
                    message = "User registered sucessfully",
                    username = user.Username,
                    role = user.Role
                });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> LoginAsync(LoginDto loginDto)
        {
            try
            {
                var response = await _authRepository.Login(loginDto);
                return Ok(response);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }
        }
    }
}