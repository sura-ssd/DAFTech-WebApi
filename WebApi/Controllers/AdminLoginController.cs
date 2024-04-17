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
    public class AdminLoginController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly AdminDbContext _adminDbContext;

        public AdminLoginController(IConfiguration configuration, AdminDbContext adminDbContext)
        {
            _configuration = configuration;
            _adminDbContext = adminDbContext;
        }

        [HttpPost]
        public IActionResult Login([FromBody] AdminLogin model)
        {
            var admin = _adminDbContext.Admin.FirstOrDefault(c => c.Username == model.Username && c.Password == model.Password);

            if (admin != null)
            {
                // Authentication successful, generate JWT token
                var token = GenerateJwtToken(admin.Username);

                if (!string.IsNullOrEmpty(admin.ProfileImage))
                {
                    admin.ProfileImage = $"{Request.Scheme}://{Request.Host}/Assets/{admin.ProfileImage}";
                }
                // Return the token and admin data
                return Ok(new
                {
                    token,
                    admin.ProfileImage,
                    admin.FirstName,
                    admin.LastName,
                    admin.PhoneNumber,
                    admin.Email,
                    admin.Role
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
                Expires = DateTime.UtcNow.AddHours(3), // Token expiration time
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }

}
