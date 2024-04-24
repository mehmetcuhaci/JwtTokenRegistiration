using JwtSignFinal.Handler;
using JwtSignFinal.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TokenHandler = JwtSignFinal.Handler.TokenHandler;

namespace JwtSignFinal.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        public AuthController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet]
        public IActionResult Get(ApiUsers newUser)
        {
            Token token = TokenHandler.CreateToken(_configuration,newUser);
            return Ok(token);
        }
        
    }
}
