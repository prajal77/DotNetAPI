using AutoMapper;
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
        IMapper _mapper;
        public UserEfController(IConfiguration config)
        {
            _entityFramework = new DataContextEf(config);
            _mapper = new Mapper( new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<UserToAddDto, User>();

            }));
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
            /*User userDb = new User();*/
            User userDb = _mapper.Map<User>(user);
           
            /*userDb.Active = user.Active;
            userDb.FirstName = user.FirstName;
            userDb.LastName = user.LastName;
            userDb.Email = user.Email;
            userDb.Gender = user.Gender;*/


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

        ////UserSalary
        ///
        [HttpGet("UserSalaryGet")]
        public IEnumerable<UserSalary> Get()
        {
            IEnumerable<UserSalary> UserSalary = _entityFramework.UserSalary.ToList();
            return UserSalary;
        }

        [HttpGet("UserSalaryGetById/{userId}")]
        public UserSalary GetById(int userId)
        {
            UserSalary? userSalary = _entityFramework.UserSalary.Where(u => u.UserId == userId).FirstOrDefault<UserSalary>();

            if (userSalary != null)
            {

                return userSalary;
            }
            else
            {
                throw new Exception("UserSalary not found");
            }
        }

        [HttpPost("UserSalaryPost")]
        public IActionResult PostUserSalary(UserSalary userSalary)
        {
            _entityFramework.UserSalary.Add(userSalary);
            if (_entityFramework.SaveChanges() > 0)
            {
                return Ok();
            }
            throw new Exception("Adding UserSalary failed on save");
        }

        [HttpPut("UserSalaryUpdate")]

        public IActionResult UpdateSalary(UserSalary userSalary)
        {


            UserSalary? userSalaryToUpdate = _entityFramework.UserSalary.Where(u => u.UserId == userSalary.UserId).FirstOrDefault<UserSalary>();
            if (userSalaryToUpdate != null)
            {
                _mapper.Map(userSalary, userSalaryToUpdate);
                if (_entityFramework.SaveChanges() > 0)
                {
                    return Ok();
                }
                throw new Exception("Updating UserSalary failed on save");
            }
            else
            {

                throw new Exception("Failed to find UserSalary to update");
            }
        }

        [HttpDelete("userSalaryDelete{userId}")]
        public IActionResult UserSalaryDelete(int userId)
        {
            UserSalary? userSalary = _entityFramework.UserSalary.Where(u => u.UserId == userId).FirstOrDefault();

            if (userSalary != null)
            {
                _entityFramework.UserSalary.Remove(userSalary);
                if (_entityFramework.SaveChanges() > 0)
                {
                    return Ok();
                }
                else
                {
                    throw new Exception("Cannot delete the userSalary");
                }
            }
            throw new Exception("Cannot find the userSalary");

        }

        //JOb
        [HttpGet("UserJobInfo")]

        public IEnumerable<UserJobInfo> GetAllUserJobInfo()
        {
            IEnumerable<UserJobInfo> UserJobInfo = _entityFramework.UserJobInfo.ToList<UserJobInfo>();
            return UserJobInfo;
        }
        public IEnumerable<UserJobInfo> GetUserJobInfoEF(int userId)
        {
            return _entityFramework.UserJobInfo
                .Where(u => u.UserId == userId)
                .ToList();
        }

        [HttpPost("UserJobInfo")]
        public IActionResult PostUserJobInfoEf(UserJobInfo userForInsert)
        {
            _entityFramework.UserJobInfo.Add(userForInsert);
            if (_entityFramework.SaveChanges() > 0)
            {
                return Ok();
            }
            throw new Exception("Adding UserJobInfo failed on save");
        }


        [HttpPut("UserJobInfo")]
        public IActionResult PutUserJobInfoEf(UserJobInfo userForUpdate)
        {
            UserJobInfo? userToUpdate = _entityFramework.UserJobInfo
                .Where(u => u.UserId == userForUpdate.UserId)
                .FirstOrDefault();

            if (userToUpdate != null)
            {
                _mapper.Map(userForUpdate, userToUpdate);
                if (_entityFramework.SaveChanges() > 0)
                {
                    return Ok();
                }
                throw new Exception("Updating UserJobInfo failed on save");
            }
            throw new Exception("Failed to find UserJobInfo to Update");
        }


        [HttpDelete("UserJobInfo/{userId}")]
        public IActionResult DeleteUserJobInfoEf(int userId)
        {
            UserJobInfo? userToDelete = _entityFramework.UserJobInfo
                .Where(u => u.UserId == userId)
                .FirstOrDefault();

            if (userToDelete != null)
            {
                _entityFramework.UserJobInfo.Remove(userToDelete);
                if (_entityFramework.SaveChanges() > 0)
                {
                    return Ok();
                }
                throw new Exception("Deleting UserJobInfo failed on save");
            }
            throw new Exception("Failed to find UserJobInfo to delete");
        }

    }
}
