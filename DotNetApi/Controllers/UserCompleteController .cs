using Dapper;
using DotNetApi.Data;
using DotNetApi.Dtos;
using DotNetApi.Helper;
using DotNetApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections;
using System.Data;
using System.Reflection;

namespace DotNetApi.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class UserCompleteController : ControllerBase
    {
        private readonly DataContextDapper _dapper;
        private readonly ReusableSql _reusableSql;
        public UserCompleteController(IConfiguration config)
        {
            _dapper = new DataContextDapper(config);
            _reusableSql = new ReusableSql(config);
        }

        [HttpGet("GetUser/{userId}/{isActive}")]
        public IEnumerable<UserComplete> GetUsers(int userId ,bool isActive)
        {
            string sql = @"EXEC TutorialAppSchema.spUsers_Get";
            string stringParameter = "";
            DynamicParameters sqlParameters = new DynamicParameters();

            if(userId != 0)
            {
                //parameter += ", @UserId=" + userId.ToString();
                stringParameter += ", @UserId= @UserIdParameter";
                sqlParameters.Add("@UserIdParameter", userId, DbType.Int32);
            }
            if (isActive)
            {
                stringParameter += ", @Active= @ActiveParam";
                sqlParameters.Add("@ActiveParam",isActive,DbType.Boolean);
            }
            /*else
            {
                stringParameter += ", @Active= @ActiveParam";
                sqlParameters.Add("@ActiveParam", isActive, DbType.Boolean);
            }*/
            if(stringParameter.Length > 0)
            {
            //substring help to start from index 1
                sql += stringParameter.Substring(1);//,parameter.Length);
            }
            Console.WriteLine(sql);

            IEnumerable<UserComplete> users = _dapper.LoadDataParameters<UserComplete>(sql,sqlParameters);
            return users;

        }
      

        [HttpPut("UpsertUser")]

        public IActionResult EditUser(UserComplete user)
        {
           
            if (_reusableSql.UpsertUser(user))
            {
                return Ok();
            }
            throw new Exception("Failed to Update User");

        }

       
        [HttpDelete("DeleteUser/{userId}")]
        public IActionResult DeleteUser(int userId)
        {
            string sql = @"TutorialAppSchema.spUser_Delete @UserId = @UserIdParam";
            DynamicParameters sqlParameters = new DynamicParameters();
            sqlParameters.Add("@UserIdParam", userId, DbType.Int32);
            if (_dapper.ExecutSqlWithParameter(sql,sqlParameters))
            {
                return Ok();
            }
            throw new Exception("Failed to delete User");
        }
    }
}
