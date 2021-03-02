using animalFinder.Service.Interface;
using System;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace animalFinder.Service
{
    public class JWTService : IJWTService
    {
        public string Generate(string userId, string issuer, string audience, string tokenBytes, int expireTime)
        {
            Claim[] claims = new Claim[]
                {
                    new Claim(ClaimTypes.PrimarySid, userId)
                };

            JwtSecurityToken token = new JwtSecurityToken
                (
                issuer: issuer,
                audience: audience,
                notBefore: DateTime.Now,
                expires: DateTime.Now.AddMinutes(expireTime),
                claims: claims,
                signingCredentials: new SigningCredentials
                    (
                        new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenBytes)),
                        SecurityAlgorithms.HmacSha512
                    )
                );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
