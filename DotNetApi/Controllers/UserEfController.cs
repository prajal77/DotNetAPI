using DotNetApi.Data;
using DotNetApi.Dtos;
using DotNetApi.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections;
using System.Data;

namespace DotNetApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserEfController : ControllerBase
    {
        DataContextEf _entityFramework;
        public UserEfController(IConfiguration config)
        {
            _entityFramework = new DataContextEf(config);
        }

    
        [HttpGet("GetUsers")]
        public IEnumerable<User> GetUsers()
        {

            IEnumerable<User> users = _entityFramework.Users.ToList<User>();
            return users;
        }
        [HttpGet("GetUsers/{userId}")]
        public User GetSingleUsers(int userId)
        {
            User? user = _entityFramework.Users.Where(u => u.UserId == userId).FirstOrDefault<User>();
       
            if (user != null)
            {
                return user;
            }
            throw new Exception("failed to Get User");
        }

        [HttpPut("EditUser")]

        public IActionResult EditUser(User user)
        {
            User? userDb = _entityFramework.Users.Where(u => u.UserId == user.UserId).FirstOrDefault<User>();

            if (userDb != null)
            {
                userDb.Active = user.Active;
                userDb.FirstName = user.FirstName;
                userDb.LastName = user.LastName;
                userDb.Email = user.Email;
                userDb.Gender = user.Gender;
                if (_entityFramework.SaveChanges() > 0)
                {
                    return Ok();
                }
                throw new Exception("Failed to updated user");
            }
            throw new Exception("Failed to Get User");

        }

        [HttpPost("AddUser")]
        public IActionResult AddUser(UserToAddDto user)
        {
            User userDb = new User();
           
            userDb.Active = user.Active;
            userDb.FirstName = user.FirstName;
            userDb.LastName = user.LastName;
            userDb.Email = user.Email;
            userDb.Gender = user.Gender;


            _entityFramework.Users.Add(userDb);
            if (_entityFramework.SaveChanges() > 0)
            {
                return Ok();
            }
            throw new Exception("Failed to Add user");
            
        }

        [HttpDelete("DeleteUser/{userId}")]
        public IActionResult DeleteUser(int userId)
        {
            User? userDb = _entityFramework.Users.Where(u => u.UserId == userId).FirstOrDefault<User>();

            if (userDb != null)
            {
             _entityFramework.Users.Remove(userDb);
                if (_entityFramework.SaveChanges() > 0)
                {
                    return Ok();
                }
                throw new Exception("Failed to Delete user");
            }
            throw new Exception("Failed to Get User");

        }

    }
}
