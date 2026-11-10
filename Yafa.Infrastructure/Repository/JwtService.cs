using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Yafa.Application.Common.Interfaces;
using Yafa.Domain.Dtos;

namespace Yafa.Infrastructure.Repository
{
    public class JwtService : IJwtService
    {
        private readonly IConfiguration _configuration;

        public JwtService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string GenerateToken(UserClaims userClaims, bool rememberme)
        {
            var key = GetSigningKey();
            var tokenHandler = new JwtSecurityTokenHandler();


            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userClaims.Id),
                new Claim(ClaimTypes.Email, userClaims.Email),
                new Claim(ClaimTypes.Name, userClaims.Name),
                new Claim(ClaimTypes.Role, userClaims.Roles),
                new Claim("RolesId", userClaims.RolesId),
                new Claim("ProfilePic", userClaims.ProfileImagePath),
                new Claim("ThumbPic", userClaims.ThumbImagePath)
            };

            var expiry = rememberme
                ? DateTime.UtcNow.AddDays(6)
                : DateTime.UtcNow.AddDays(1);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = expiry,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature)
            };

            return BuildToken(claims, expiry, key);
        }

        public string GenerateScalarToken(string userId, string role)
        {
            var key = GetSigningKey();

            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, userId),
                new(ClaimTypes.Role,           role),
                new("token_type",              "scalar_portal") // scope hint
            };

            return BuildToken(claims, DateTime.UtcNow.AddHours(2), key);
        }

        public ClaimsPrincipal? ValidateToken(string token)
        {
            try
            {
                var key = GetSigningKey();
                var tokenHandler = new JwtSecurityTokenHandler();


                var principal = tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ClockSkew = TimeSpan.Zero
                }, out _);

                return principal;
            }
            catch
            {
                return null;
            }
        }


        private byte[] GetSigningKey()
        {
            var secret = _configuration["ApiSettings:Secret"]
                ?? throw new InvalidOperationException("JWT secret not configured.");

            // FIX: ASCII was stripping bytes > 127 — use UTF8 to preserve full entropy
            return Encoding.UTF8.GetBytes(secret);
        }

        private static string BuildToken(IEnumerable<Claim> claims, DateTime expires, byte[] key)
        {
            var handler = new JwtSecurityTokenHandler();
            var descriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = expires,
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature)
            };
            return handler.WriteToken(handler.CreateToken(descriptor));
        }
    }
}
