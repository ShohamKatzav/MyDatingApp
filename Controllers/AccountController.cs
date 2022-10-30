using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using API.Data;
using API.Entities;
using System.Security.Cryptography;
using Microsoft.EntityFrameworkCore;
using API.DTOs;
using API.Interfaces;

namespace API.Controllers
{
    public class AccountController : BaseApiController
    {
        private readonly DataContext _context;
        private readonly ITokenService _tokenService;
        public AccountController(DataContext context, ITokenService tokenService)
        {
            _tokenService = tokenService;
            _context = context;
        }
        [HttpPost("register")]
        public async Task<ActionResult<UserDto>> Register(RegisterAccountDto registerAccount)
        {
            if (await UserExist(registerAccount.Username)) return BadRequest("Username already exist");
            using var hmac = new HMACSHA512();

            var user = new AppUser
            {
                UserName = registerAccount.Username.ToLower(),
                PasswordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(registerAccount.Password)),
                PasswordSalt = hmac.Key
            };

            _context.Users.Add(user);

            await _context.SaveChangesAsync();

            return new UserDto{
                UserName = user.UserName,
                Token = _tokenService.CreateToken(user)
            };
        }

        [HttpPost("login")]
        public async Task<ActionResult<UserDto>> Login(LoginAccountDto loginAccount)
        {
            var user = await this._context.Users.SingleOrDefaultAsync(x => x.UserName == loginAccount.Username);
            if (user == null) return Unauthorized("Invalid username or password");

            using var hmac = new HMACSHA512(user.PasswordSalt);
            var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(loginAccount.Password));

            for (int i = 0; i < computedHash.Length; i++)
            {
                if (computedHash[i] != user.PasswordHash[i]) return Unauthorized("Invalid username or password");
            }
            
            return new UserDto{
                UserName = user.UserName,
                Token = _tokenService.CreateToken(user)
            };
        }

        private async Task<bool> UserExist(string username)
        {
            return await _context.Users.AnyAsync(x => x.UserName == username);
        }

    }
}