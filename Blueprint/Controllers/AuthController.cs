using Blueprint.Dtos;
using Blueprint.Services;
using Microsoft.AspNetCore.Mvc;

namespace Blueprint.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
        {
            var result = await _authService.Register(registerDto);

            if (!result.Success)
            {
                if (result.StatusCode == (int)ResponseCode.Conflict)
                    return Conflict(new { Success = true, result.StatusCode, result.Message, result.Data });

                return StatusCode((int)ResponseCode.InternalServerError, result);
            }

            return Ok(new { Success = true, result.StatusCode, result.Message, result.Data });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            var result = await _authService.Login(loginDto);

            if (!result.Success)
            {
                if (result.StatusCode == (int)ResponseCode.BadRequest)
                    return BadRequest(new { Success = true, result.StatusCode, result.Message, result.Data });

                return StatusCode((int)ResponseCode.InternalServerError, result);
            }

            return Ok(new { Success = true, result.StatusCode, result.Message, result.Token });
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "").Trim();

            var result = await _authService.Logout(token);

            if (!result.Success)
            {
                if (result.StatusCode == (int)ResponseCode.NotFound)
                    return NotFound(new { Success = true, result.StatusCode, result.Message, result.Data });

                if (result.StatusCode == (int)ResponseCode.BadRequest)
                    return BadRequest(new { Success = true, result.StatusCode, result.Message, result.Data });
                return StatusCode((int)ResponseCode.InternalServerError, result);
            }

            return Ok(new { Success = true, result.StatusCode, result.Message });
        }
    }
}
