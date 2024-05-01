using JwtSignFinal.Handler;
using JwtSignFinal.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace JwtSignFinal.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles ="Admin")]
    
    public class SignInController : ControllerBase
    {
        private readonly DataContext _dataContext;
        private readonly IConfiguration _configuration;
        public SignInController(DataContext dataContext, IConfiguration configuration)
        {
            _dataContext = dataContext;
            _configuration = configuration;
        }
        [HttpPost]
        public async Task< IActionResult> SignIn(string userName,string passWord)
        {

            if (string.IsNullOrWhiteSpace(userName)||string.IsNullOrWhiteSpace(passWord))
            {
                return BadRequest("Kullanıcı adı veya şifre boş olamaz!");
            }

            var user=await _dataContext.Users
                .FirstOrDefaultAsync(u=>u.UserName == userName);
            
            if (user == null)
            {
                return Unauthorized("Kullanıcı bulunamadı");
            }

            if(user.Password!=passWord)
            {
                return Unauthorized("Şifre yanlış");
            }

            bool isEmailVerified = user.IsEmailVerified == 1;

            if (!isEmailVerified)
            {
                return Unauthorized("Email has not been verified.");
            }
            user.Token = null;
            Token token = TokenHandler.CreateToken(_configuration, user);
            user.Token = token.AccessToken;
            await _dataContext.SaveChangesAsync();

            return Ok("Admin Girdi");

        }



    }
}
