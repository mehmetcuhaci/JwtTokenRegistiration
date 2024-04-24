using Azure.Core;
using JwtSignFinal.Models;
using System;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using JwtSignFinal.Models;

namespace JwtSignFinal.Handler
{
    public class MailHandler
    {
        private readonly DataContext _dataContext;

        public MailHandler(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public async Task<string> SendMail(IConfiguration configuration, EmailRequest request,ApiUsers newUser) // userId parametresi eklendi
        {
            var smtpServer = configuration["SmtpService:Server"];
            var smtpPort = configuration["SmtpService:Port"];
            var smtpUser = configuration["SmtpService:Mail"];
            var smtpPassword = configuration["SmtpService:Passw"];
            var smtpFromMail = smtpUser;

            if (string.IsNullOrWhiteSpace(smtpServer) || string.IsNullOrWhiteSpace(smtpPort) ||
                string.IsNullOrWhiteSpace(smtpUser) || string.IsNullOrWhiteSpace(smtpPassword))
            {
                return "Configuration for SMTP server is incomplete.";
            }

            try
            {
                using (var smtpClient = new SmtpClient(smtpServer, int.Parse(smtpPort)))
                {
                    smtpClient.Credentials = new NetworkCredential(smtpUser, smtpPassword);
                    smtpClient.EnableSsl = true;

                    var token = TokenHandler.CreateToken(configuration, newUser); // userId bilgisi kullanılarak token oluşturuluyor
                    newUser.VerificationToken = token.AccessToken; // Kullanıcı nesnesi güncelleniyor
                    newUser.ETokenExpiration = token.Expiration;
                    _dataContext.SaveChanges(); // Veritabanı güncellemesi yapılıyor

                    var verificationLink = $"https://localhost:7018/api/verification/verify-email?token={token.AccessToken}";
                    var body = $"Please verify your email by clicking on the following link: {verificationLink}";


                    var mailMessage = new MailMessage
                    {
                        From = new MailAddress(smtpFromMail),
                        Subject = request.Subject,
                        Body = body,
                        IsBodyHtml = true
                    };
                    mailMessage.To.Add(request.To);
                    await smtpClient.SendMailAsync(mailMessage);

                    return "Mail sent successfully with verification link.";
                }
            }
            catch (Exception ex)
            {
                return $"Failed to send mail: {ex.Message}";
            }
        }

    }
}
