using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace PrimeMarket.Authentication
{
    public class JwtProvider : IJwtProvider
    {
        public (string Token, int expiresIn) GenerateJwtToken(ApplicationUser user)
        {
            Claim[] claims = [
                new(JwtRegisteredClaimNames.Sub, user.Id),
                new(JwtRegisteredClaimNames.Email, user.Email!),
                new(JwtRegisteredClaimNames.FamilyName, user.LastName),
                new(JwtRegisteredClaimNames.GivenName, user.FirstName),
                new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            ];

            var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("|dpk+&]Y4xaqVCJd3fC+0Q)scm?cqcoMVWk^XXp7@Yn\r\n\r\n"));

            var signingCredintials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);

            var expiresIn = 30;

            var token = new JwtSecurityToken(
                issuer: "PrimeMarket",
                audience: "PrimeMarket users",
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(expiresIn),
                signingCredentials: signingCredintials
            );

            return (token: new JwtSecurityTokenHandler().WriteToken(token), expiresIn: expiresIn * 60);
        }
    }
}
