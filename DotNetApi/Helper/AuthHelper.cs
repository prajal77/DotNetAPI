using DotNetApi.Data;
using DotNetApi.Dtos;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration.UserSecrets;
using Microsoft.IdentityModel.Tokens;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace DotNetApi.Helper
{
    public class AuthHelper
    {
        private readonly IConfiguration _config;
        private readonly DataContextDapper _dapper;

        public AuthHelper( IConfiguration config)
        {
            _dapper = new DataContextDapper(config);
            _config = config;
            
        }

        public byte[] GetPasswordHash(string password, byte[] passwordSalt)
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
        public string CreateToken(int userId)
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

        public bool SetPassword(UserForLoginDto userForSetPassword)
        {
            // Generate a random salt for the password
            byte[] passwordSalt = new byte[128 / 8];
            //generate random number
            using (RandomNumberGenerator rng = RandomNumberGenerator.Create())
            {
                rng.GetNonZeroBytes(passwordSalt);
            }
            byte[] passwordHash = GetPasswordHash(userForSetPassword.Password, passwordSalt);
            // SQL query to insert the new user into the database
            string sqlAddAuth = @"EXEC TutorialAppSchema.spRegistration_Upsert 
                                                            @Email = @EmailParam,
                                                            @PasswordHash = @PasswordHashParam, 
                                                            @PasswordSalt = @PasswordSaltParam ";

            // Create SQL parameters for the password hash and salt
            List<SqlParameter> sqlParameters = new List<SqlParameter>();

            SqlParameter emailParameter = new SqlParameter("@EmailParam", SqlDbType.VarChar);
            emailParameter.Value = userForSetPassword.Email;
            sqlParameters.Add(emailParameter);

            SqlParameter passwordHashParameter = new SqlParameter("@PasswordHashParam", SqlDbType.VarBinary);
            passwordHashParameter.Value = passwordHash;
            sqlParameters.Add(passwordHashParameter);

            SqlParameter passwordSaltParameter = new SqlParameter("@PasswordSaltParam", SqlDbType.VarBinary);
            passwordSaltParameter.Value = passwordSalt;
            sqlParameters.Add(passwordSaltParameter);



            // Execute the SQL query with parameters and return success if it works
            return _dapper.ExecutSqlWithParameter(sqlAddAuth, sqlParameters);

        }       

    }
}
