using backend.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using backend.Services;
using Microsoft.AspNetCore.Cors;
using System.Text.Json;
using Newtonsoft.Json.Linq;


namespace backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors("AllowAllHeaders")]
    public class Auth : ControllerBase
    {
        private List<UserDTO> userDTOs = new List<UserDTO>();
        private readonly IConfiguration _config;
        private DbService<UserDTO> _dbService;
        public Auth(IConfiguration config)
        {
            _config = config;
            _dbService = new DbService<UserDTO>();
            userDTOs = _dbService.ExecuteQueries("SELECT * FROM \"UserDTO\"").GetAwaiter().GetResult().Cast<UserDTO>().ToList();

        }

        [AllowAnonymous]
        [HttpPost]
        public ActionResult Login([FromBody] UserLogin userLogin)
        {
            var user = Authenticate(userLogin);
            if (user != null)
            {
                var token = GenerateToken(user);
                string tempTokenHolder = @"{ ""accessToken"": """ + token + @""" }";
                var opt = new JsonSerializerOptions() { WriteIndented = true };
                var tokenJObject = JObject.Parse(tempTokenHolder);
                string userJson = JsonSerializer.Serialize(user, opt);
                var userJObject = JObject.Parse(userJson);
                userJObject.Merge(tokenJObject, new JsonMergeSettings
{
    // union array values together to avoid duplicates
    MergeArrayHandling = MergeArrayHandling.Union
});


                return Ok(userJObject.ToString());
            }

            return BadRequest("user not found");
        }

        // To generate token
        private string GenerateToken(UserDTO user)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"] ?? ""));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier,user.Username ?? ""),
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
        private UserDTO Authenticate(UserLogin userLogin)
        {
            return userDTOs.Find(user => user.Username == userLogin.Username && _dbService.VerifyPassword(userLogin.Password ?? "", user.Password ?? "", Convert.FromHexString(user.PasswordSalt ?? "")));
        }
    }
}
