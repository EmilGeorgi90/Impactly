using backend.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Security.Cryptography;
using backend.Services;

namespace backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class Auth : ControllerBase
    {
        private const int keySize = 64;
        private const int iterations = 350000;
        private HashAlgorithmName hashAlgorithm = HashAlgorithmName.SHA512;
        private List<UserDTO> userDTOs = new List<UserDTO>();
        private readonly IConfiguration _config;
        private DbService<UserDTO> _dbService;
        public Auth(IConfiguration config)
        {
            _config = config;
            _dbService = new DbService<UserDTO>();
            userDTOs = (List<UserDTO>)_dbService.GetAll("SELECT * FROM \"UserDTO\"").GetAwaiter().GetResult();
        }

        [AllowAnonymous]
        [HttpPost]
        public ActionResult Login([FromBody] UserLogin userLogin)
        {
            var user = Authenticate(userLogin).GetAwaiter().GetResult();
            if (user != null)
            {
                var token = GenerateToken(user);
                return Ok(token);
            }

            return BadRequest("user not found");
        }

        // To generate token
        private string GenerateToken(UserDTO user)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier,user.Username),
                new Claim(ClaimTypes.Role,user.Role.ToString())
            };
            var token = new JwtSecurityToken(_config["Jwt:Issuer"],
                _config["Jwt:Audience"],
                claims,
                expires: DateTime.Now.AddMinutes(15),
                signingCredentials: credentials);


            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        //To authenticate user
        private async Task<UserDTO> Authenticate(UserLogin userLogin)
        {
            return userDTOs.Find(user => user.Username == userLogin.Username && VerifyPassword(userLogin.Password, user.Password, user.Salt));
        }
        public string HashPasword(string password, out byte[] salt)
        {
            salt = RandomNumberGenerator.GetBytes(keySize);
            var hash = Rfc2898DeriveBytes.Pbkdf2(
                Encoding.UTF8.GetBytes(password),
                salt,
                iterations,
                hashAlgorithm,
                keySize);
            return Convert.ToHexString(hash);
        }
        bool VerifyPassword(string password, string hash, byte[] salt)
        {
            var hashToCompare = Rfc2898DeriveBytes.Pbkdf2(password, salt, iterations, hashAlgorithm, keySize);
            return hashToCompare.SequenceEqual(Convert.FromHexString(hash));
        }
    }
}
