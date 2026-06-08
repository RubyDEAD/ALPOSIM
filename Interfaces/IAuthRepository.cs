using alposim.DTO;
using alposim.Models;
namespace alposim.Interfaces;

public interface IAuthRepository
{
    Task<AuthResponseDto> Login(LoginDto loginDto);
    Task<User> Register(RegisterDto registerDto);
    Task<bool> UserExists(string username);
}