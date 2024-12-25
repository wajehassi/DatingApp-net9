using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using API.Entities;
using API.Interfaces;
using Microsoft.IdentityModel.Tokens;

namespace API.Services;

public class TokenService(IConfiguration config) : ITokenService
{
    public string CreateToken(AppUser user)
    {
        var tokenKey = config["TokenKey"] ?? throw new Exception("Cannot access TokenKey from app setting");
        if(tokenKey.Length < 64) throw new Exception("your key needs to be longer");
        var key =new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenKey));
        var claims = new List<Claim>{
            new(ClaimTypes.NameIdentifier , user.UserName)
        };
        var creds = new SigningCredentials(key,SecurityAlgorithms.HmacSha512Signature);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddDays(7),
            SigningCredentials = creds
        };
        var tokenHandller = new JwtSecurityTokenHandler();
        var token = tokenHandller.CreateToken(tokenDescriptor);

        return tokenHandller.WriteToken(token);
    }
}
