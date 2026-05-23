using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Json;

namespace PrimeMarket.Authentication
{
    public class JwtProvider(IOptions<JwtOptions> options) : IJwtProvider
    {
        private readonly JwtOptions _JwtOptions = options.Value;

        public (string Token, int expiresIn) GenerateJwtToken(ApplicationUser user, IEnumerable<string> roles)
        {
            Claim[] claims = [
                new(JwtRegisteredClaimNames.Sub, user.Id),
                new(JwtRegisteredClaimNames.Email, user.Email!),
                new(JwtRegisteredClaimNames.FamilyName, user.LastName),
                new(JwtRegisteredClaimNames.GivenName, user.FirstName),
                new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new(nameof(roles), JsonSerializer.Serialize(roles), JsonClaimValueTypes.JsonArray)
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
