using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using JwtSignFinal.Models;
using System.Net.Mail;
using System.Net;


namespace JwtSignFinal.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MailController : ControllerBase
    {
        private readonly DataContext _dataContext;

        public MailController(DataContext dataContext)
        {
            _dataContext = dataContext;
        }


        [HttpPost("Verify")]
        public IActionResult VerifyEmail([FromBody]EmailRequest request) {
            try
            {
                using(var smtpClient=new SmtpClient("smtp-mail.outlook.com", 587))
                {
                    smtpClient.Credentials = new NetworkCredential("mehmet17014@hotmail.com", "021302135i");
                    smtpClient.EnableSsl = true;

                    var mailMessage = new MailMessage
                    {
                        From = new MailAddress("mehmet17014@hotmail.com"),
                        Subject = request.Subject,
                        Body = request.Body,
                        IsBodyHtml = true
                    };
                    mailMessage.To.Add(request.To);

                    smtpClient.Send(mailMessage);

                    var user = _dataContext.Users.FirstOrDefault(u => u.Email == request.To);
                    if (user != null)
                    {
                        user.IsEmailVerified = 1;
                        _dataContext.SaveChanges();
                    }

                }
                return Ok("Mail Gönderildi Kullanıcı doğrulandı");
            }
            catch (Exception ex)
            {

                return BadRequest("Başarız gönderilemedi "+ex.Message);
            }
            
        }
    }
}
