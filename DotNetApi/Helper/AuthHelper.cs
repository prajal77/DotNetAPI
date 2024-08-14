using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace DotNetApi.Helper
{
    public class AuthHelper
    {
        private readonly IConfiguration _config;
        public AuthHelper( IConfiguration config)
        {
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
    }
}
