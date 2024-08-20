using DotNetApi.Data;
using DotNetApi.Dtos;
using DotNetApi.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections;
using System.Data;
using System.Reflection;

namespace DotNetApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserCompleteController : ControllerBase
    {
        DataContextDapper _dapper;
        public UserCompleteController(IConfiguration config)
        {
            _dapper = new DataContextDapper(config);
        }

        [HttpGet("GetUser/{userId}/{isActive}")]
        public IEnumerable<UserComplete> GetUsers(int userId ,bool isActive)
        {
            string sql = @"EXEC TutorialAppSchema.spUsers_Get";
            string parameter = "";

            if(userId != 0)
            {
                parameter += ", @UserId=" + userId.ToString();
            }
            if (isActive)
            {
                parameter += ", @Active=" + isActive.ToString();
            }
            else
            {
                parameter += ", @Active=" + isActive.ToString();
            }
            //substring help to start from index 1
            sql += parameter.Substring(1);//,parameter.Length);
            Console.WriteLine(sql);

            IEnumerable<UserComplete> users = _dapper.LoadData<UserComplete>(sql);
            return users;

        }
      

        [HttpPut("UpsertUser")]

        public IActionResult EditUser(UserComplete user)
        {
            string sql = @"
            EXEC TutorialAppSchema.spUser_Upsert 
                    @FirstName = '" + user.FirstName +
                "', @LastName='" + user.LastName +
                "', @Email='" + user.Email +
                "', Gender='" + user.Gender +
                "', @Active= '" + user.Active +
                "', @JobTitle= '" + user.JobTitle +
                "', @Department= '" + user.Department +
                "', @Salary= '" + user.Salary +
               "',  @UserId = " + user.UserId;

            if (_dapper.ExecutSql(sql))
            {

                return Ok();
            }
            throw new Exception("Failed to Update User");

        }

       
        [HttpDelete("DeleteUser/{userId}")]
        public IActionResult DeleteUser(int userId)
        {
            string sql = @"TutorialAppSchema.spUser_Delete @UserId = " + userId.ToString();
            if (_dapper.ExecutSql(sql))
            {
                return Ok();
            }
            throw new Exception("Failed to delete User");

        }
      


       
    }
}
