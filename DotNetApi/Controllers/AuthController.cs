using DotNetApi.Data;
using DotNetApi.Dtos;
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
        private readonly IConfiguration _config;

/*        public object SymetricSecuirtyKey { get; private set; }
*/
        public AuthController(IConfiguration config)
        {
            _dapper = new DataContextDapper(config);
            _config = config;
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
                    byte[] passwordHash = GetPasswordHash(userForRegistration.Password, passwordSalt);
                    // SQL query to insert the new user into the database
                    string sqlAddAuth = @"INSERT INTO TutorialAppSchema.Auth ([Email],
                                                [PasswordHash],
                                                [PasswordSalt] ) VALUES ('" + userForRegistration.Email +
                                                "', @PasswordHash, @PasswordSalt)";
                    Console.WriteLine(sqlAddAuth);
                    // Create SQL parameters for the password hash and salt
                    List<SqlParameter> sqlParameters = new List<SqlParameter>();

                    SqlParameter passwordSaltParameter = new SqlParameter("@PasswordSalt", SqlDbType.VarBinary);
                    passwordSaltParameter.Value = passwordSalt;

                    SqlParameter passwordHashParameter = new SqlParameter("@PasswordHash", SqlDbType.VarBinary);
                    passwordHashParameter.Value = passwordHash;

                    sqlParameters.Add(passwordSaltParameter);
                    sqlParameters.Add(passwordHashParameter);
                    // Execute the SQL query with parameters and return success if it works
                    if (_dapper.ExecutSqlWithParameter(sqlAddAuth, sqlParameters))
                    {
                        string sqlAddUser = @"INSERT INTO TutorialAppSchema.Users(
                        [FirstName],
                        [LastName],[Email],[Gender],[Active])
                        VALUES(" +
                        "'" + userForRegistration.FirstName +
                        "', '" + userForRegistration.LastName +
                        "', '" + userForRegistration.Email +
                        "', '" + userForRegistration.Gender +
                               "', 1)";
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
            byte[] passwordHash = GetPasswordHash(userForLogin.Password, userForConfirmation.PasswordSalt);
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
                {"token",CreateToken(userId) }
            });
        }

        [HttpGet("RefreshToken")]
        public string RefreshToken()
        {
            string sqlGetUserId = @"SELECT UserId FROM TutorialAppSchema.Users WHERE UserId = '" + User.FindFirst("userId")?.Value + "'";

            int userId = _dapper.LoadDataSingle<int>(sqlGetUserId);
            return CreateToken(userId);
        }
        private byte[] GetPasswordHash(string password, byte[] passwordSalt)
        {
            // Combine the salt with a password key from the configuration
            string passwordSaltPlusString = _config.GetSection("AppSettings.PasswordKey").Value +
                Convert.ToBase64String(passwordSalt);

            // Create a hash of the password using the salt
            return KeyDerivation.Pbkdf2(
                password: password,
                salt: Encoding.ASCII.GetBytes(passwordSaltPlusString),
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 100000,
                numBytesRequested: 256 / 8);
        }

        // Method to create a JWT token for a user
        private string CreateToken(int userId)
        {
            // Create an array of claims. A claim is a statement about the user (e.g., user ID).
            Claim[] claims = new Claim[]
            {
                new Claim("userId",userId.ToString())
                // Add a claim for the user ID
            };
            // Retrieve the token key from the configuration settings
            string? tokenKeyString = _config.GetSection("AppSettings:TokenKey").Value;

            // Create a symmetric security key using the token key string
            SymmetricSecurityKey tokenKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenKeyString != null ? tokenKeyString : ""));

            // Create signing credentials using the security key and the HMAC SHA-512 algorithm
            SigningCredentials credentials = new SigningCredentials(tokenKey, SecurityAlgorithms.HmacSha512Signature);

            // Describe the security token, including the claims, signing credentials, and expiration time
            SecurityTokenDescriptor descriptor = new SecurityTokenDescriptor()
            {
                Subject = new ClaimsIdentity(claims),
                SigningCredentials = credentials,
                Expires = DateTime.Now.AddDays(1)
                // Set the token to expire in 1 day
            };
            // Create a token handler to generate the token
            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
            
            // Create the security token using the descriptor
            SecurityToken token = tokenHandler.CreateToken(descriptor);

            // Write the token as a string and return it
            return tokenHandler.WriteToken(token);
        }
    }
}
