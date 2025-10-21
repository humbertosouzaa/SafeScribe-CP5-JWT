using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SafeScribe.Api.Data;
using SafeScribe.Api.DTOs;
using SafeScribe.Api.Models;
using SafeScribe.Api.Services;

namespace SafeScribe.Api.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _db;
        private readonly ITokenService _tokenService;
        private readonly ITokenBlacklistService _blacklist;

        public AuthController(AppDbContext db, ITokenService tokenService, ITokenBlacklistService blacklist)
        {
            _db = db; _tokenService = tokenService; _blacklist = blacklist;
        }

        [HttpPost("registrar")]
        [AllowAnonymous]
        public async Task<IActionResult> Register(UserRegisterDto dto)
        {
            if (await _db.Users.AnyAsync(u => u.Username == dto.Username))
                return Conflict("Username já existe.");

            var user = new User
            {
                Username = dto.Username,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                Role = dto.Role
            };

            _db.Users.Add(user);
            await _db.SaveChangesAsync();
            return Created("", new { user.Id, user.Username, user.Role });
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginRequestDto dto)
        {
            var user = await _db.Users.FirstOrDefaultAsync(u => u.Username == dto.Username);
            if (user == null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
                return Unauthorized("Credenciais inválidas.");

            var token = _tokenService.GenerateToken(user, out var jti);
            return Ok(new { access_token = token, token_type = "Bearer", jti });
        }

        [HttpPost("logout")]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            var jti = User.FindFirst("jti")?.Value;
            if (string.IsNullOrEmpty(jti)) return BadRequest("Token sem JTI.");
            await _blacklist.AddToBlacklistAsync(jti);
            return Ok(new { message = "Logout efetuado. Token invalidado." });
        }
    }
}
