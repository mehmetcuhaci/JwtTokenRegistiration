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
            await _datacontext.SaveChangesAsync(); // Save the user to database

            // Generate a token after successful registration
            Token token = TokenHandler.CreateToken(_configuration, newUser);
            newUser.Token = token.AccessToken;
            newUser.VerificationToken = token.AccessToken; // Update the verification token
            newUser.ETokenExpiration = token.Expiration; // Set token expiration
            await _datacontext.SaveChangesAsync(); // Save changes

            // Prepare the email request
            EmailRequest emailRequest = new EmailRequest
            {
                To = newUser.Email,
                Subject = "Welcome to our service!",
                Body = "Thanks for registering. Please verify your email by clicking here: [Verification Link]"
            };

            // Send the email
            string emailResult = await _mailHandler.SendMail(_configuration, emailRequest,newUser);
            if (!emailResult.StartsWith("Mail Gönderildi")) // Check if the mail was sent successfully
            {
                throw new Exception(emailResult); // If mail sending failed, throw an exception
            }

            return Ok("Registration successful.");
        }
    }
}
