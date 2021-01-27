using ParkyAPI.Data;
using ParkyAPI.Models;
using ParkyAPI.Repository.IRepository;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;

namespace ParkyAPI.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly IConfiguration _configuration;
        private readonly ApplicationDbContext _db;

        public UserRepository(ApplicationDbContext db, IConfiguration configuration)
        {
            _db = db;
            _configuration = configuration;
        }

        public User Authenticate(string email, string password)
        {
            var user = _db.Users.SingleOrDefault(x => x.Email == email && x.Password == password);

            if(user == null)
            {
                return null;
            }

            var tokenHandler = new JwtSecurityTokenHandler();
            var securityKey = Encoding.UTF8.GetBytes(_configuration["Token:SecurityKey"]);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Role, user.Role)
                }),
                Expires = DateTime.Now.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(securityKey),SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            user.Token = tokenHandler.WriteToken(token);
            user.Password = "";
            user.ConfirmPassword = "";

            return user;
        }

        public bool IsUniqueUser(string email)
        {
            var user = _db.Users.SingleOrDefault(x => x.Email == email);

            if(user == null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public User Register(User user)
        {
            if(user == null)
            {
                return null;
            }

            User newUser = new User()
            {
                Email = user.Email,
                Password = user.Password,
                ConfirmPassword = user.ConfirmPassword,
                UserName = user.Email,
                Role = user.Role ?? "User"
            };

            _db.Users.Add(newUser);
            _db.SaveChanges();

            newUser.Password = "";
            newUser.ConfirmPassword = "";

            return newUser;

        }
    }
}
