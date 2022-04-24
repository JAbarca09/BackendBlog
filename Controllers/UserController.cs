using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Backend_Blog.Models;
using Backend_Blog.Models.DTO;
using Backend_Blog.Services;
using Microsoft.AspNetCore.Mvc;

namespace Backend_Blog.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {
        private readonly UserService _data;
        public UserController(UserService dataFromService)
        {
            _data = dataFromService;
        }

        [HttpGet("userByUsername/{username}")]
        public UserIdDTO GetUserIdDTOByUsername(string username)
        {
            return _data.GetUserIdDTOByUsername(username);
        }



        //Add a user
        [HttpPost("AddUsers")]
        public bool AddUser(CreateAccountDTO UserToAdd)
        {
            return _data.AddUser(UserToAdd);
        }

        [HttpPost("Login")]
        public IActionResult Login([FromBody] LoginDTO User)
        {
            return _data.Login(User);
        }
        //Update User Account

        [HttpPost("UpdateUser")]

        public bool UpdateUser(UserModel userToUpdate)
        {
            return _data.UpdateUser(userToUpdate);
        }

        [HttpPost("UpdateUser/{id}/{username}")]
        public bool UpdateUser(int id, string username)
        {
            return _data.UpdateUsername(id,username);
        }

        
        //Delete User Account
        [HttpPost("DeleteUser/{userToDelete}")]

        public bool DeleteUser(string userToDelete)
        {
            return _data.DeleteUser(userToDelete);
        }


    }
}