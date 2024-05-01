using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace JwtSignFinal.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VerificationController : ControllerBase
    {
        private readonly DataContext _dataContext;
        public VerificationController(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        [HttpGet("verify-email")]
        public IActionResult VerifyEmail(string token, IConfiguration configuration)
        {
            try
            {
                var handler = new JwtSecurityTokenHandler();
                var validationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Token:SecurityKey"])),
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidIssuer = configuration["Token:Issuer"],
                    ValidAudience = configuration["Token:Audience"]
                };

                var principal = handler.ValidateToken(token, validationParameters, out SecurityToken validatedToken);
                var Id = principal.Claims.First(c => c.Type == "Id").Value;

                var user = _dataContext.Users.FirstOrDefault(u => u.Id == int.Parse(Id) && u.VerificationToken == token);
                if (user == null || user.IsEmailVerified == 1)
                {
                    return NotFound("User not found or already verified.");
                }

                user.IsEmailVerified = 1;
                user.VerificationToken = null; // verify olduktan sonra sil
                _dataContext.SaveChanges();

                return Ok("Email has been verified.");
            }
            catch (Exception ex)
            {
                return BadRequest("Invalid token.");
            }
        }
    }
}
