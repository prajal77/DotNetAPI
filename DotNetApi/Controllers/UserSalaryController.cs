using DotNetApi.Data;
using DotNetApi.Dtos;
using DotNetApi.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DotNetApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserSalaryController : ControllerBase
    {
        DataContextDapper _dapper;
        public UserSalaryController(IConfiguration config)
        {
            _dapper = new DataContextDapper(config);
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
                                 WHERE UserId =" + userId.ToString() ;
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
                                SET  [Salary] ='" + userSalary.Salary+ 
                                "', [AvgSalary] ='" + userSalary.AvgSalary + "' WHERE UserId =" +userSalary.UserId;
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

    }
}
