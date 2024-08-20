using Dapper;
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
            string sql = @"
            EXEC TutorialAppSchema.spUser_Upsert 
                    @FirstName= @FirstNameParameter, 
                    @LastName= @LastNameParameter,
                    @Email= @EmailParameter, 
                    @Gender= @GenderParameter, 
                    @Active= @ActiveParameter,
                    @JobTitle= @JobTitleParameter,
                    @Department= @DepartmentParameter,
                    @Salary= @SalaryParameter, 
                    @UserId= @UserIdParameter";
            Console.WriteLine(sql);

            DynamicParameters sqlParameters = new DynamicParameters();
            sqlParameters.Add("@FirstNameParameter", user.FirstName, DbType.String);
            sqlParameters.Add("@LastNameParameter", user.LastName, DbType.String);
            sqlParameters.Add("@EmailParameter", user.Email, DbType.String);
            sqlParameters.Add("@GenderParameter", user.Gender, DbType.String);
            sqlParameters.Add("@ActiveParameter", user.Active, DbType.Boolean);
            sqlParameters.Add("@JobTitleParameter", user.JobTitle, DbType.String);
            sqlParameters.Add("@DepartmentParameter", user.Department, DbType.String);
            sqlParameters.Add("@SalaryParameter", user.Salary, DbType.Decimal);
            sqlParameters.Add("@UserIdParameter", user.UserId, DbType.Int32);


            if (_dapper.ExecutSqlWithParameter(sql,sqlParameters))
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
