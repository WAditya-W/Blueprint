using Blueprint.Dtos;

namespace Blueprint.Services
{
    public interface IAuthService
    {
        Task<AuthResult> Register(RegisterDto registerDto);
        Task<AuthResult> Login(LoginDto loginDto);
        Task<AuthResult> Logout(string token);
        string GetUsernameFromToken(string token);
    }
}
