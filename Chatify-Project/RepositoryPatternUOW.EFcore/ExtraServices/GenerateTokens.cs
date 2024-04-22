using Microsoft.IdentityModel.Tokens;
using RepositoryPattern.Core.Interfaces;
using RepositoryPattern.Core.OptionPattern;
using RepositoryPatternUOW.Core.Models;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace RepositoryPattern.EFcore.ExtraServices
{
    public class GenerateTokens(JwtOptions jwtOptions) : IGenerateTokens
    {
        
        public string GenerateToken(User user)
        {
            var claims = new List<Claim>() {
            new(JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString()),
            new(JwtRegisteredClaimNames.Name,user.FirstName),
            new(JwtRegisteredClaimNames.FamilyName,user.LastName),
            new(JwtRegisteredClaimNames.UniqueName,user.UserName),
            new(JwtRegisteredClaimNames.NameId,user.Id.ToString()),
            new(JwtRegisteredClaimNames.Email,user.Email)
            };
            var issuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.IssuerSigningKey));
            var signingCredentials = new SigningCredentials(issuerSigningKey, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
                claims:claims,
                issuer: jwtOptions.Issuer,
                audience: jwtOptions.Audience,
                expires: DateTime.Now.AddMinutes(jwtOptions.ExpiresAfter),
                signingCredentials: signingCredentials

                );
            return new JwtSecurityTokenHandler().WriteToken(token);

        }

        
        public string GenerateToken()
        {
            var randBytes = RandomNumberGenerator.GetBytes(32);
            return Convert.ToBase64String(randBytes);
        }
    }
}
