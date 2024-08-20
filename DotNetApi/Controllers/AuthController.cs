using DotNetApi.Data;
using DotNetApi.Dtos;
using DotNetApi.Helper;
using DotNetApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.IdentityModel.Tokens;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace DotNetApi.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly DataContextDapper _dapper;
        /*private readonly IConfiguration _config;*/
        private readonly AuthHelper _authHelper;
/*        public object SymetricSecuirtyKey { get; private set; }
*/
        public AuthController(IConfiguration config)
        {
            _dapper = new DataContextDapper(config);
            /*_config = config;*/
            _authHelper = new AuthHelper(config);
        }

        [AllowAnonymous]
        [HttpPost("Register")]

        public IActionResult Register(UserForRegistrationDto userForRegistration)
        {
            // Check if the password and password confirmation match
            if (userForRegistration.Password == userForRegistration.PasswordConfirm)
            {
                // SQL query to check if the user already exists in the database
                string sqlCheckUserExits = "SELECT Email FROM TutorialAppSchema.Auth WHERE Email = '" +
                    userForRegistration.Email + "'";
                Console.WriteLine(sqlCheckUserExits);
                // If no existing user is found, proceed with registration
                IEnumerable<string> existingUsers = _dapper.LoadData<string>(sqlCheckUserExits);
                if (existingUsers.Count() == 0)
                {
                    // Generate a random salt for the password
                    byte[] passwordSalt = new byte[128 / 8];
                    //generate random number
                    using (RandomNumberGenerator rng = RandomNumberGenerator.Create())
                    {
                        rng.GetNonZeroBytes(passwordSalt);
                    }
                    byte[] passwordHash = _authHelper.GetPasswordHash(userForRegistration.Password, passwordSalt);
                    // SQL query to insert the new user into the database
                    string sqlAddAuth = @"EXEC TutorialAppSchema.spRegistration_Upsert 
                                                            @Email = @EmailParam,
                                                            @PasswordHash = @PasswordHashParam, 
                                                            @PasswordSalt = @PasswordSaltParam ";

                    Console.WriteLine(sqlAddAuth);
                    // Create SQL parameters for the password hash and salt
                    List<SqlParameter> sqlParameters = new List<SqlParameter>();

                    SqlParameter emailParameter = new SqlParameter("@EmailParam", SqlDbType.VarChar);
                    emailParameter.Value = userForRegistration.Email;
                    sqlParameters.Add(emailParameter);

                    SqlParameter passwordHashParameter = new SqlParameter("@PasswordHashParam", SqlDbType.VarBinary);
                    passwordHashParameter.Value = passwordHash;
                    sqlParameters.Add(passwordHashParameter);

                    SqlParameter passwordSaltParameter = new SqlParameter("@PasswordSaltParam", SqlDbType.VarBinary);
                    passwordSaltParameter.Value = passwordSalt;
                    sqlParameters.Add(passwordSaltParameter);

                    

                    // Execute the SQL query with parameters and return success if it works
                    if (_dapper.ExecutSqlWithParameter(sqlAddAuth, sqlParameters))
                    {
                        string sqlAddUser = @"EXEC TutorialAppSchema.spUser_Upsert 
                                                @FirstName = '" + userForRegistration.FirstName +
                                            "', @LastName = '" + userForRegistration.LastName +
                                            "', @Email = '" + userForRegistration.Email +
                                            "', @Gender = '" + userForRegistration.Gender +
                                            "', @Active = 1" +
                                            ", @JobTitle = '" + userForRegistration.JobTitle +
                                            "', @Department = '" + userForRegistration.Department +
                                            "', @Salary = '" + userForRegistration.Salary + "'" ;
                        Console.WriteLine(sqlAddUser);

                        /*string sqlAddUser = @"INSERT INTO TutorialAppSchema.Users(
                        [FirstName],
                        [LastName],[Email],[Gender],[Active])
                        VALUES(" +
                        "'" + userForRegistration.FirstName +
                        "', '" + userForRegistration.LastName +
                        "', '" + userForRegistration.Email +
                        "', '" + userForRegistration.Gender +
                               "', 1)";*/
                        if (_dapper.ExecutSql(sqlAddUser))
                        {
                        return Ok();

                        }
                        throw new Exception("Failed to add user.");
                    }
                    throw new Exception("Failed to register user.");

                }
                throw new Exception("User with this email already exists!");
            }
            throw new Exception("Password do not match!");
        }

        [AllowAnonymous]
        [HttpPost("Login")]

        public IActionResult Login(UserForLoginDto userForLogin)
        {
            string sqlForHashAndSalt = @"SELECT 
                                            [PasswordHash],
                                            [PasswordSalt] FROM TutorialAppSchema.Auth WHERE Email = '" +
                                            userForLogin.Email + "'";
            UserForLoginConfirmationDto userForConfirmation = _dapper.LoadDataSingle<UserForLoginConfirmationDto>(sqlForHashAndSalt);
            byte[] passwordHash = _authHelper.GetPasswordHash(userForLogin.Password, userForConfirmation.PasswordSalt);
            // if (passwordHash == userForConfirmation.PasswordHash)  //won't work   

            for (int index = 0; index < passwordHash.Length; index++)
            {
                if (passwordHash[index] != userForConfirmation.PasswordHash[index])
                {
                    return StatusCode(401, "Incorrect Password!");
                }

            }
            string userSqlId = @"SELECT UserId FROM TutorialAppSchema.Users WHERE Email = '" + userForLogin.Email + "'";
            Console.WriteLine(userSqlId);

            int userId = _dapper.LoadDataSingle<int>(userSqlId);

            return Ok(new Dictionary<string, string>
            {
                {"token",_authHelper.CreateToken(userId) }
            });
        }

        [HttpGet("RefreshToken")]
        public string RefreshToken()
        {
            string sqlGetUserId = @"SELECT UserId FROM TutorialAppSchema.Users WHERE UserId = '" + User.FindFirst("userId")?.Value + "'";

            int userId = _dapper.LoadDataSingle<int>(sqlGetUserId);
            return _authHelper.CreateToken(userId);
        }
        
    }
}
