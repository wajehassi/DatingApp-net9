using System;
using System.Security.Cryptography;
using System.Text;
using API.Data;
using API.DTOs;
using API.Entities;
using API.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;

public class AccountController (DataContext context , ITokenService tokenService) :BaseApiController
{
    [HttpPost("register")]  // account/register

    public async Task<ActionResult<UserDto>> Register (RegisterDto registerDto)
    {
            if(await UserExists(registerDto.Username)) return BadRequest("Username is taken");

        using var hmac = new HMACSHA512();

        var user =new AppUser 
        {
            UserName = registerDto.Username.ToLower(),
            PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDto.Password)),
            PasswordSalt = hmac.Key
        };

        context.Users.Add(user);
        await context.SaveChangesAsync();

        return new UserDto{
            Username = user.UserName,
            Token = tokenService.CreateToken(user)
        };

    }

    [HttpPost("login")] 
    public async Task<ActionResult<UserDto>> Login(LoginDto loginDto){
       var  User = await context.Users.FirstOrDefaultAsync(x=>x.UserName == loginDto.Username.ToLower());
       if(User == null) return Unauthorized("invalid username");

       var hmac = new HMACSHA512(User.PasswordSalt);
       var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDto.Password));

        for (int i = 0; i < computedHash.Length; i++)
        {
            if(computedHash[i] != User.PasswordHash[i]) return Unauthorized("invalid Password");
        }
        return new UserDto{
            Username = User.UserName,
            Token = tokenService.CreateToken(User)
        };
            
    }

    private async Task<bool> UserExists(string username)
    {
        return await context.Users.AnyAsync(x=> x.UserName.ToLower() == username.ToLower());
    }


}
