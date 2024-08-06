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
        [HttpGet("UserSalaryGetAll")]
        public IEnumerable<UserSalary> GetUserSalaryAll()
        {
            string sql = @"Select * From TutorialAppSchema.UserSalary ";
            IEnumerable<UserSalary> UserSalary = _dapper.LoadData<UserSalary>(sql);
            return UserSalary;
        }

        [HttpGet("User/{userId}")]
        public UserSalary GetUserSalaryById(int userId)
        {
            string sql = @"SELECT * FROM TutorialAppSchema.UserSalary
                                 WHERE UserId =" + userId.ToString();
            UserSalary userSalary = _dapper.LoadDataSingle<UserSalary>(sql);
            return userSalary;
        }

        [HttpPost("AddUserSalary")]
        public IActionResult AddUserSalary(UserSalary userSalary)
        {
            string sql = @"INSERT INTO TutorialAppSchema.UserSalary(
                                     [UserId],[Salary],[AvgSalary]) 
                                VALUES(" + " '" + userSalary.UserId + "', '" + userSalary.Salary +
                            "','" + userSalary.AvgSalary +
                            "')";

            if (_dapper.ExecutSql(sql))
            {
                return Ok();
            }
            throw new Exception("User Salary cannot be added");

        }

        /*   [HttpPost("AddUserSalary")]
           public IActionResult AddUserSalary(UserSalaryAddDto userSalary)
           {
               string sql = @"INSERT INTO TutorialAppSchema.UserSalary(
                                    [Salary],[AvgSalary]) 
                               VALUES(" + " '" + userSalary.Salary +
                               "','" + userSalary.AvgSalary +
                               "')";

               if (_dapper.ExecutSql(sql))
               {
                   return Ok();
               }
               throw new Exception("User Salary cannot be added");

           }*/

        [HttpPut("EditUserSalary")]
        public IActionResult EditUserSalary(UserSalary userSalary)
        {

            string sql = @"Update FROM TutorialAppSchema.UserSalary
                                SET  [Salary] ='" + userSalary.Salary +
                                "', [AvgSalary] ='" + userSalary.AvgSalary + "' WHERE UserId =" + userSalary.UserId;
            if (_dapper.ExecutSql(sql))
            {
                return Ok();
            }
            throw new Exception("UserSalary cannot be updated");
        }

        [HttpDelete("DeleteUserSalary")]

        public IActionResult DeleteUserSalary(int userId)
        {
            string sql = @"DELETE FROM TutorialAppSchema.UserSalary WHERE UserID =" + userId.ToString();
            if (_dapper.ExecutSql(sql))
            {
                return Ok();
            }
            throw new Exception("Failed to delete UserSalary");
        }

        //UserJobInfo
        [HttpGet("UserJobInfo/{userId}")]
        public IEnumerable<UserJobInfo> GetUserJobInfo(int userId)
        {
            return _dapper.LoadData<UserJobInfo>(@"
            SELECT  UserJobInfo.UserId
                    , UserJobInfo.JobTitle
                    , UserJobInfo.Department
            FROM  TutorialAppSchema.UserJobInfo
                WHERE UserId = " + userId.ToString());
        }

        [HttpPost("UserJobInfo")]
        public IActionResult PostUserJobInfo(UserJobInfo userJobInfoForInsert)
        {
            string sql = @"
            INSERT INTO TutorialAppSchema.UserJobInfo (
                UserId,
                Department,
                JobTitle
            ) VALUES (" + userJobInfoForInsert.UserId
                    + ", '" + userJobInfoForInsert.Department
                    + "', '" + userJobInfoForInsert.JobTitle
                    + "')";

            if (_dapper.ExecutSql(sql))
            {
                return Ok(userJobInfoForInsert);
            }
            throw new Exception("Adding User Job Info failed on save");
        }

        [HttpPut("UserJobInfo")]
        public IActionResult PutUserJobInfo(UserJobInfo userJobInfoForUpdate)
        {
            string sql = "UPDATE TutorialAppSchema.UserJobInfo SET Department='"
                + userJobInfoForUpdate.Department
                + "', JobTitle='"
                + userJobInfoForUpdate.JobTitle
                + "' WHERE UserId=" + userJobInfoForUpdate.UserId.ToString();

            if (_dapper.ExecutSql(sql))
            {
                return Ok(userJobInfoForUpdate);
            }
            throw new Exception("Updating User Job Info failed on save");
        }

        // [HttpDelete("UserJobInfo/{userId}")]
        // public IActionResult DeleteUserJobInfo(int userId)
        // {
        //     string sql = "DELETE FROM TutorialAppSchema.UserJobInfo  WHERE UserId=" + userId;

        //     if (_dapper.ExecuteSql(sql))
        //     {
        //         return Ok();
        //     }
        //     throw new Exception("Deleting User Job Info failed on save");
        // }

        [HttpDelete("UserJobInfo/{userId}")]
        public IActionResult DeleteUserJobInfo(int userId)
        {
            string sql = @"
            DELETE FROM TutorialAppSchema.UserJobInfo 
                WHERE UserId = " + userId.ToString();

            Console.WriteLine(sql);

            if (_dapper.ExecutSql(sql))
            {
                return Ok();
            }

            throw new Exception("Failed to Delete User");
        }
    }
}
