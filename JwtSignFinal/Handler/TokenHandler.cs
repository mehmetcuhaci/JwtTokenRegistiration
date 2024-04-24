using JwtSignFinal.Models;
using Microsoft.IdentityModel.Tokens;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace JwtSignFinal.Handler
{
    public class TokenHandler
    {
        public static Token CreateToken(IConfiguration configuration, ApiUsers newUser)
        {
            
            SymmetricSecurityKey securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Token:SecurityKey"]));
            SigningCredentials credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            
            var claims = new List<Claim> {
                new Claim("Id", newUser.Id.ToString()), 
                new Claim(ClaimTypes.Role, newUser.Role) 
            };

            
            var tokenExpirationMinutes = Convert.ToInt16(configuration["Token:Expiration"]);
            var token = new JwtSecurityToken(
                issuer: configuration["Token:Issuer"],
                audience: configuration["Token:Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(tokenExpirationMinutes),
                signingCredentials: credentials
            );

            
            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
            return new Token
            {
                AccessToken = tokenHandler.WriteToken(token),
                Expiration = DateTime.Now.AddMinutes(tokenExpirationMinutes)
            };
        }
    }
}
