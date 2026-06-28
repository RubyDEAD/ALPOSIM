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
                
                Response.Cookies.Append("token", response.Token, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = false,
                    SameSite = SameSiteMode.Strict,
                    Expires = DateTimeOffset.Now.AddMinutes(15)
                });

                var csrfToken = Guid.NewGuid().ToString();
                Response.Cookies.Append("csrf-token", csrfToken, new CookieOptions
                {
                    HttpOnly = false,
                    Secure = false,
                    SameSite = SameSiteMode.Strict,
                    Expires = DateTimeOffset.UtcNow.AddMinutes(15)
                });
                
                return Ok(new { username = response.Username, role = response.Role});
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }
        }
        [HttpPost("logout")]
        public IActionResult Logout()
        {
            Response.Cookies.Delete("token");
            Response.Cookies.Delete("csrf-token");
            return Ok(new { message = "Logged out successfully" });
        }
    }
}