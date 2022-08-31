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

namespace API.Controllers
{
    public class AccountController : BaseApiController
    {
        private readonly DataContext _context;
        public AccountController(DataContext context)
        {
            _context = context;
        }
        [HttpPost("register")]
        public async Task<ActionResult<AppUser>> Register(UserInfo info)
        {
            using var hmac = new HMACSHA512();

            var user =  new AppUser {
                UserName = info.username,
                PasswordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(info.password)),
                PasswordSalt = hmac.Key
            };

            _context.Users.Add(user);

            await _context.SaveChangesAsync();

            return user;
        }
        
    }
    public class UserInfo
    {
        public string username { get; set; }
        public string password { get; set; }
    }
}