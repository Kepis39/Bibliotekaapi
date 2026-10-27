using Bibliotekaapi.Models;
using Bibliotekaapi.Models.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Bibliotekaapi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AutorizeController : ControllerBase
    {
        private readonly AuthService service;
        public AutorizeController(AuthService _service)
        {
            service = _service;
        }

        [HttpPost("login")]
        public async Task<ActionResult<string>> Login(string login, string password)
        {
            if (service.Login(login, password) != null)
            {
                var claims = new List<Claim> { new Claim(ClaimTypes.Name, login) };
                var jwt = new JwtSecurityToken(
                    issuer: AuthOptions.ISSUER,
                    audience: AuthOptions.AUDIENCE,
                    claims: claims,
                    expires: DateTime.UtcNow.Add(TimeSpan.FromMinutes(2)),
                    signingCredentials: new SigningCredentials(AuthOptions.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256));

                return new JwtSecurityTokenHandler().WriteToken(jwt);
            }
            return BadRequest();
        }

        [HttpPost("register")]
        public async Task<ActionResult<Reader>> Register([FromBody] Reader reader)
        {
            if (service.Register(reader)) return Ok(reader);
            return BadRequest();
        }
    }
}
