using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ParkyAPI.Models;
using ParkyAPI.Models.DTOs;
using ParkyAPI.Repository.IRepository;

namespace ParkyAPI.Controllers
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [Authorize]
    public class UserController : Controller
    {
        private readonly IUserRepository _userRepository;

        public UserController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        [HttpPost("[action]")]
        [AllowAnonymous]
        public IActionResult Authenticate([FromBody] AuthenticateDTO model)
        {
            var user = _userRepository.Authenticate(model.Email, model.Password);
            if(user == null)
            {
                return BadRequest(new { message = "Username or password is incorrect!" });
            }

            return Ok(user);
        }

        [HttpPost("[action]")]
        [AllowAnonymous]
        public IActionResult Register([FromBody] User user)
        {
            var isUniqueUser = _userRepository.IsUniqueUser(user.Email);

            if (!isUniqueUser)
            {
                return BadRequest(new { message = "Username is already taken!" });
            }

            var registerUser = _userRepository.Register(user);

            if(registerUser == null)
            {
                return BadRequest(new { message = "Error while registering!" });
            }

            return Ok(new { message = "User registered successfully!" });
        }
    }
}