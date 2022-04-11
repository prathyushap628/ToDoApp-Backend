
using ToDoApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using ToDoApp.Utilities;
using ToDoApp.Repositories;

namespace JwtApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private IConfiguration _config;
        private readonly IUsersRepository _user;

        public LoginController(IConfiguration config, IUsersRepository user)
        {
            _config = config;
            _user = user;
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Login([FromBody] Users users)
        {
            var user = await Authenticate(users);


            if (user != null)
            {
                var token = Generate(user);
                return Ok(token);
            }

            return NotFound("User not found");
        }

        private string Generate(Users users)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, users.UserName),
                new Claim(ClaimTypes.SerialNumber, users.UserId.ToString()),
               // new Claim(ClaimTypes.Email, users.Email)
            };

            var token = new JwtSecurityToken(_config["Jwt:Issuer"],
              _config["Jwt:Audience"],
              claims,
              expires: DateTime.Now.AddMinutes(15),
              signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private async Task<Users> Authenticate(Users users)
        {
            // var currentUser = UserConstants.Users.FirstOrDefault(o => o.Username.ToLower() == users.Username.ToLower() && o.Password == userLogin.Password);
            //  var query = $@"SELECT * FROM {TableNames.users} WHERE user_id = @UserId";
            var currentUser = await _user.GetByUserName(users.UserName);
            if (currentUser != null)
            {
                return currentUser;
            }

            return null;
        }
    }
}

