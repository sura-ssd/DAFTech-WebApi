using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using WebApi.Models;
using Microsoft.EntityFrameworkCore;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly ClientDbContext _clientDbContext;

        public LoginController(IConfiguration configuration, ClientDbContext clientDbContext)
        {
            _configuration = configuration;
            _clientDbContext = clientDbContext;
        }

        [HttpPost]
        public IActionResult Login([FromBody] UserLogin model)
        {
            
            var user = _clientDbContext.Client.FirstOrDefault(c => c.Username == model.Username && c.Password == model.Password);

            if (user != null)
            {
                // Authentication successful, generate JWT token
                var token = GenerateJwtToken(user.Username);
                return Ok(new
                {
                    token,
                    user.Id 
                });
            }

            return Unauthorized();
        }

        private string GenerateJwtToken(string username)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var secret = JwtSecretGenerator.GenerateRandomSecret(); // Generate the JWT secret
            var key = Encoding.ASCII.GetBytes(secret); 

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, username)
                }),
                Expires = DateTime.UtcNow.AddHours(2), // Token expiration time
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}
