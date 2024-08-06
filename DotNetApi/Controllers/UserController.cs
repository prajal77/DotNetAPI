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
    public class UserController : ControllerBase
    {
        DataContextDapper _dapper;
        public UserController(IConfiguration config)
        {
            _dapper = new DataContextDapper(config);
        }

        [HttpGet("GetUsers")]
        public IEnumerable<UserToAddDto> GetUsers()
        {
            string sql = @"
                        SELECT [UserId],
                            [FirstName],
                            [LastName],
                            [Email],
                            [Gender],
                            [Active]  
                        FROM TutorialAppSchema.Users";
            IEnumerable<UserToAddDto> users = _dapper.LoadData<UserToAddDto>(sql);
            return users;

        }
        [HttpGet("GetUsers/{userId}")]
        public UserToAddDto GetSingleUsers(int userId)
        {

            string sql = @"
                        SELECT [UserId],
                            [FirstName],
                            [LastName],
                            [Email],
                            [Gender],
                            [Active]  
                        FROM TutorialAppSchema.Users WHERE UserId = " + userId.ToString();
            UserToAddDto user = _dapper.LoadDataSingle<UserToAddDto>(sql);
            return user;

        }

        [HttpPut("EditUser")]

        public IActionResult EditUser(User user)
        {
            string sql = @"
            UPDATE TutorialAppSchema.Users
               SET [FirstName] = '" + user.FirstName +
                "',[LastName]='" + user.LastName +
                "',[Email]='" + user.Email +
                "',[Gender]='" + user.Gender +
                "',[Active]= '" + user.Active +
               "' WHERE UserId = " + user.UserId;

            if (_dapper.ExecutSql(sql))
            {

                return Ok();
            }
            throw new Exception("Failed to Update User");

        }

        [HttpPost("AddUser")]
        public IActionResult AddUser(UserToAddDto user)
        {
            string sql = @"INSERT INTO TutorialAppSchema.Users(
                        [FirstName],
                        [LastName],[Email],[Gender],[Active])
                        VALUES("+
                                "'"+user.FirstName+
                                "', '"+user.LastName+
                                "', '"+user.Email +
                                "', '"+user.Gender+
                                "', '" +user.Active +
                "')";
         
            if (_dapper.ExecutSql(sql))
            {
                return Ok();
            }
            throw new Exception("Failed to Add User");
        }

        [HttpDelete("DeleteUser/{userId}")]
        public IActionResult DeleteUser(int userId)
        {
            string sql = @"
                            DELETE FROM TutorialAppSchema.Users 
                               WHERE UserId =" + userId.ToString();
            if (_dapper.ExecutSql(sql))
            {
                return Ok();
            }
            throw new Exception("Failed to delete User");

        }

    }
}
