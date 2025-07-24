using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace JWTGenerator
{
    internal class Program
    {
        static void Main()
        {
            string secretKey = "legoSecret1234567890111213141516";

            var tokenHandler = new JwtSecurityTokenHandler();

            var key = Encoding.ASCII.GetBytes(secretKey);

            var claims = new[]
        {
            new Claim("LastName", "Trenea"),                
            new Claim("FirstName", "Andrei"),
            new Claim("Role", "User"),                 
            new Claim("Role", "Admin"),
            new Claim("Email", "ceva@gmail.com"),  
        };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Issuer = "legoIssuer",
                Audience = "legoAudience",
                Expires = DateTime.UtcNow.AddDays(1), 
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature
                )
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            string jwt = tokenHandler.WriteToken(token);
        }
    }
}
