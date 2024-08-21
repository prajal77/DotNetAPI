using AutoMapper;
using Dapper;
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
        //public object SymetricSecuirtyKey { get; private set; }

        private readonly ReusableSql _reusableSql;
        private readonly IMapper _mapper;

        public AuthController(IConfiguration config)
        {
            _dapper = new DataContextDapper(config);
            /*_config = config;*/
            _authHelper = new AuthHelper(config);
            _reusableSql = new ReusableSql(config);
            _mapper = new Mapper(new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<UserForRegistrationDto, UserComplete>();
            }));
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
                    UserForLoginDto userForSetPassword= new UserForLoginDto()
                    {
                        Email = userForRegistration.Email,
                        Password = userForRegistration.Password,
                    };

                    if (_authHelper.SetPassword(userForSetPassword))
                    {
                        UserComplete userComplete = _mapper.Map<UserComplete>(userForRegistration);
                        userComplete.Active = true;
                        if (_reusableSql.Equals(userComplete))
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

        [HttpPut("ResetPassword")]
        public IActionResult ResetPassword(UserForLoginDto userForSetPassword)
        {
            if (_authHelper.SetPassword(userForSetPassword))
            {
                return Ok();
            }
            throw new Exception("Failed to update password! ");
        }

        [AllowAnonymous]
        [HttpPost("Login")]

        public IActionResult Login(UserForLoginDto userForLogin)
        {
            string sqlForEmail = @"EXEC TutorialAppSchema.spLoginConfirmation_Get @Email = @EmailParam";

            DynamicParameters sqlParameters = new DynamicParameters();

           /* SqlParameter emailParameter = new SqlParameter("@EmailParam", SqlDbType.VarChar);
            emailParameter.Value = userForLogin.Email;*/
            sqlParameters.Add("@EmailParam",userForLogin.Email,DbType.String);

            UserForLoginConfirmationDto userForConfirmation = _dapper.LoadDataSingleParameters<UserForLoginConfirmationDto>(sqlForEmail, sqlParameters);
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
