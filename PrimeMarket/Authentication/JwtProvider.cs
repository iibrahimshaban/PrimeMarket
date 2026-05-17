using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace PrimeMarket.Authentication
{
    public class JwtProvider(IOptions<JwtOptions> options) : IJwtProvider
    {
        private readonly JwtOptions _JwtOptions = options.Value;

        public (string Token, int expiresIn) GenerateJwtToken(ApplicationUser user)
        {
            Claim[] claims = [
                new(JwtRegisteredClaimNames.Sub, user.Id),
                new(JwtRegisteredClaimNames.Email, user.Email!),
                new(JwtRegisteredClaimNames.FamilyName, user.LastName),
                new(JwtRegisteredClaimNames.GivenName, user.FirstName),
                new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            ];

            var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_JwtOptions.Key));

            var signingCredintials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _JwtOptions.Issuer,
                audience: _JwtOptions.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(_JwtOptions.ExpiryMinutes),
                signingCredentials: signingCredintials
            );

            return (token: new JwtSecurityTokenHandler().WriteToken(token), 
                    expiresIn: _JwtOptions.ExpiryMinutes * 60);
        }
    }
}
