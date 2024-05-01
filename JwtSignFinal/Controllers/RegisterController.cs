using JwtSignFinal.Handler;
using JwtSignFinal.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using TokenHandler = JwtSignFinal.Handler.TokenHandler;

namespace JwtSignFinal.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RegisterController : ControllerBase
    {
        private readonly DataContext _datacontext;
        private readonly IConfiguration _configuration;
        private readonly MailHandler _mailHandler;

        public RegisterController(DataContext dataContext, IConfiguration configuration, MailHandler mailHandler)
        {
            _datacontext = dataContext;
            _configuration = configuration;
            _mailHandler = mailHandler;
        }

        [HttpPost("Register")]
        public async Task<IActionResult> Register([FromBody] ApiUsers newUser)
        {
            await _datacontext.Users.AddAsync(newUser);
            await _datacontext.SaveChangesAsync(); 

          
            Token token = TokenHandler.CreateToken(_configuration, newUser);
            newUser.Token = token.AccessToken;
            newUser.VerificationToken = token.AccessToken; 
            newUser.ETokenExpiration = token.Expiration;
            await _datacontext.SaveChangesAsync();

            
            EmailRequest emailRequest = new EmailRequest
            {
                To = newUser.Email,
                Subject = "Welcommen",
                Body = "Thanks for registering. Please verify your email by clicking here: [Verification Link]"
            };

           
             await _mailHandler.SendMail(_configuration, emailRequest,newUser);
            

            return Ok("Kayıt başarılı lütfen mailinizi kontrol edin");
        }
    }
}
