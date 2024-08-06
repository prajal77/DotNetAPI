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
        //DataContextEf _entityFramework;
        IUserRepository _userRepository;
        IMapper _mapper;
        public UserEfController( IUserRepository userRepository)
        {
            _mapper = new Mapper(new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<UserToAddDto, User>();

            }));
            _userRepository = userRepository;
        }


        [HttpGet("GetUsers")]
        public IEnumerable<User> GetUsers()
        {

            IEnumerable<User> users = _userRepository.GetUsers();
            return users;
        }
        [HttpGet("GetUsers/{userId}")]
        public User GetSingleUsers(int userId)
        {
            return _userRepository.GetSingleUsers(userId);
        }


        [HttpPut("EditUser")]

        public IActionResult EditUser(User user)
        {
            /*User? userDb = _entityFramework.Users.Where(u => u.UserId == user.UserId).FirstOrDefault<User>();*/
            User? userDb = _userRepository.GetSingleUsers(user.UserId);


            if (userDb != null)
            {
                userDb.Active = user.Active;
                userDb.FirstName = user.FirstName;
                userDb.LastName = user.LastName;
                userDb.Email = user.Email;
                userDb.Gender = user.Gender;
                if (_userRepository.SaveChanges())
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


            _userRepository.AddEntity<User>(userDb);
            if (_userRepository.SaveChanges())
            {
                return Ok();
            }
            throw new Exception("Failed to Add user");

        }

        [HttpDelete("DeleteUser/{userId}")]
        public IActionResult DeleteUser(int userId)
        {
            User? userDb = _userRepository.GetSingleUsers(userId);

            if (userDb != null)
            {
                _userRepository.RemoveEntity<User>(userDb);
                if (_userRepository.SaveChanges())
                {
                    return Ok();
                }
                throw new Exception("Failed to Delete user");
            }
            throw new Exception("Failed to Get User");

        }

        ////UserSalary
        ///
        [HttpGet("UserSalaryGetById/{userId}")]
        public UserSalary GetById(int userId)
        {
            UserSalary? userSalary = _userRepository.GetSingleUsersSalary(userId);

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
            _userRepository.AddEntity<UserSalary>(userSalary);
            if (_userRepository.SaveChanges())
            {
                return Ok();
            }
            throw new Exception("Adding UserSalary failed on save");
        }

        [HttpPut("UserSalaryUpdate")]

        public IActionResult UpdateSalary(UserSalary userSalary)
        {


            UserSalary? userSalaryToUpdate = _userRepository.GetSingleUsersSalary(userSalary.UserId);
            if (userSalaryToUpdate != null)
            {
                _mapper.Map(userSalary, userSalaryToUpdate);
                if (_userRepository.SaveChanges())
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
            UserSalary? userSalary = _userRepository.GetSingleUsersSalary(userId);

            if (userSalary != null)
            {
                _userRepository.RemoveEntity<UserSalary>(userSalary);
                if (_userRepository.SaveChanges())
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
        [HttpGet("UserJobInfo{userId}")]
        public UserJobInfo GetUserJobInfoEF(int userId)
        {
            return _userRepository.GetSingleUsersJobInfo(userId);
        }

        [HttpPost("UserJobInfo")]
        public IActionResult PostUserJobInfoEf(UserJobInfo userForInsert)
        {
            _userRepository.AddEntity<UserJobInfo>(userForInsert);
            if (_userRepository.SaveChanges())
            {
                return Ok();
            }
            throw new Exception("Adding UserJobInfo failed on save");
        }


        [HttpPut("UserJobInfo")]
        public IActionResult PutUserJobInfoEf(UserJobInfo userForUpdate)
        {
            UserJobInfo? userToUpdate = _userRepository.GetSingleUsersJobInfo(userForUpdate.UserId);
            if (userToUpdate != null)
            {
                _mapper.Map(userForUpdate, userToUpdate);
                if (_userRepository.SaveChanges())
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
            UserJobInfo? userToDelete = _userRepository.GetSingleUsersJobInfo(userId);

            if (userToDelete != null)
            {
                _userRepository.RemoveEntity<UserJobInfo>(userToDelete);
                if (_userRepository.SaveChanges())
                {
                    return Ok();
                }
                throw new Exception("Deleting UserJobInfo failed on save");
            }
            throw new Exception("Failed to find UserJobInfo to delete");
        }


    }
}
