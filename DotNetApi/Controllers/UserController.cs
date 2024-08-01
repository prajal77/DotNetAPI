﻿using DotNetApi.Data;
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

        [HttpGet("TestConnection")]
        public DateTime TestConnection()
        {
            return _dapper.LoadDataSingle<DateTime>("SELECT GETDATE()");
        }

        [HttpGet("GetUsers")]
        public IEnumerable<User> GetUsers()
        {
            string sql = @"
                        SELECT [UserId],
                            [FirstName],
                            [LastName],
                            [Email],
                            [Gender],
                            [Active]  
                        FROM TutorialAppSchema.Users";
            IEnumerable<User> users = _dapper.LoadData<User>(sql);
            return users;

        }
        [HttpGet("GetUsers/{userId}")]
        public User GetSingleUsers(int userId)
        {

            string sql = @"
                        SELECT [UserId],
                            [FirstName],
                            [LastName],
                            [Email],
                            [Gender],
                            [Active]  
                        FROM TutorialAppSchema.Users WHERE UserId = " + userId.ToString();
            User user = _dapper.LoadDataSingle<User>(sql);
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
        public IActionResult AddUser(User user)
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
            Console.WriteLine(sql);
         
            if (_dapper.ExecutSql(sql))
            {
                return Ok();
            }
            throw new Exception("Failed to Update User");
        }

    }
}
