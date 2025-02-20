using Blueprint.Dtos;
using Blueprint.Models;
using Blueprint.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Blueprint.Services
{
    public class AuthService : IAuthService
    {
        private readonly IConfiguration _configuration;
        private readonly IAuthRepository _repository;

        public AuthService(IConfiguration configuration, IAuthRepository repository)
        {
            _configuration = configuration;
            _repository = repository;
        }

        public async Task<AuthResult> Register(RegisterDto registerDto)
        {
            var existingUser = await _repository.GetByNameAsync(registerDto.Username);
            if (existingUser != null)
                return new AuthResult { Success = false, StatusCode = (int)ResponseCode.Conflict, Message = $"User name '{registerDto.Username}' already exists." };

            var hashedPassword = HashPassword(registerDto.Password);
            var newUser = new User
            {
                Username = registerDto.Username,
                PasswordHash = hashedPassword
            };
            await _repository.AddAsync(newUser);

            return new AuthResult
            {
                Success = true,
                StatusCode = (int)ResponseCode.Success,
                Message = "User registered successfully.",
                Data = newUser
            };
        }

        public async Task<AuthResult> Login(LoginDto loginDto)
        {
            var user = await _repository.GetByNameAsync(loginDto.Username);
            if (user == null || !VerifyPassword(loginDto.Password, user.PasswordHash))
                return new AuthResult { Success = false, StatusCode = (int)ResponseCode.BadRequest, Message = "Invalid credentials." };

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                new Claim(ClaimTypes.Name, user.Username)
            }),
                Expires = DateTime.UtcNow.AddHours(2),
                Issuer = _configuration["Jwt:Issuer"],
                Audience = _configuration["Jwt:Audience"],
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);

            user.isLogin = true;
            await _repository.UpdateAsync(user);

            return new AuthResult
            {
                Success = true,
                StatusCode = (int)ResponseCode.Success,
                Message = "Login successfully.",
                Token = tokenString
            };
        }

        public async Task<AuthResult> Logout(string token)
        {
            var username = GetUsernameFromToken(token);
            if (string.IsNullOrEmpty(username))
                return new AuthResult { Success = false, StatusCode = (int)ResponseCode.BadRequest, Message = "Invalid token." };

            var user = await _repository.GetByNameAsync(username);
            if (user == null && !user.isLogin)
                return new AuthResult { Success = false, StatusCode = (int)ResponseCode.NotFound, Message = "User not found or not logged in." };

            user.isLogin = false;
            await _repository.UpdateAsync(user);

            return new AuthResult
            {
                Success = true,
                StatusCode = (int)ResponseCode.Success,
                Message = "Logout successfully."
            };
        }

        public string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        public bool VerifyPassword(string password, string hashedPassword)
        {
            return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
        }

        public string GetUsernameFromToken(string token)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var jwtToken = tokenHandler.ReadJwtToken(token);

                var usernameClaim = jwtToken.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.Name || claim.Type == "unique_name");

                if (usernameClaim == null)
                {
                    Console.WriteLine("Username claim not found in token.");
                    return null;
                }

                return usernameClaim.Value;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error extracting username from token: {ex.Message}");
                return null;
            }
        }
    }
}
