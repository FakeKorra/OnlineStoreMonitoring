using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using OnlineStoreMonitoring.Application.DTOs;
using OnlineStoreMonitoring.Application.Services;

namespace OnlineStoreMonitoring.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IConfiguration _configuration;

        public AuthController(IAuthService authService, IConfiguration configuration)
        {
            _authService = authService;
            _configuration = configuration;
        }

        [HttpPost("register")]
        public async Task<ActionResult<LoginResponseDto>> Register(CreateUserDto registerDto)
        {
            if (string.IsNullOrEmpty(registerDto.Username) || string.IsNullOrEmpty(registerDto.Password))
            {
                return BadRequest("Username and password are required");
            }

            var user = await _authService.RegisterAsync(registerDto);
            if (user == null)
            {
                return BadRequest("Username already exists");
            }

            var jwtSettings = _configuration.GetSection("Jwt");
            var token = await _authService.LoginAsync(
                new LoginDto { Username = registerDto.Username, Password = registerDto.Password },
                jwtSettings["SecretKey"],
                jwtSettings["Issuer"],
                jwtSettings["Audience"],
                int.Parse(jwtSettings["ExpirationMinutes"])
            );

            return Ok(token);
        }

        [HttpPost("login")]
        public async Task<ActionResult<LoginResponseDto>> Login(LoginDto loginDto)
        {
            var jwtSettings = _configuration.GetSection("Jwt");
            var result = await _authService.LoginAsync(
                loginDto,
                jwtSettings["SecretKey"],
                jwtSettings["Issuer"],
                jwtSettings["Audience"],
                int.Parse(jwtSettings["ExpirationMinutes"])
            );

            if (result == null)
            {
                return Unauthorized("Invalid credentials");
            }

            return Ok(result);
        }
    }
}