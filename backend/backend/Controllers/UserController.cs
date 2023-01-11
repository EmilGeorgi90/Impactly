using backend.Model;
using backend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private DbService<UserDTO> _dbService;
        private readonly IConfiguration _config;
        private const int keySize = 64;
        public UserController(IConfiguration config)
        {
            _config = config;
            _dbService = new DbService<UserDTO>();
        }
        [Authorize]
        [Route("GetOne")]
        [HttpPost]
        public ActionResult GetUser(int id)
        {
            UserDTO userDTO = (UserDTO)_dbService.GetTodo($"SELECT * FROM \"UserDTO\" WHERE id={id}").GetAwaiter().GetResult();
            if (userDTO == null)
            {
                return BadRequest();
            }
            return Ok(userDTO);
        }
        [Route("Add")]
        [HttpPost]
        public ActionResult AddUser([FromBody] UserLogin user)
        {
            byte[] passHash = new byte[keySize];
            string password = _dbService.HashPasword(user.Password ?? "", out passHash);
            string passSalt = Convert.ToHexString(passHash);
            if (_dbService.ExecuteNonQuery($"INSERT INTO \"UserDTO\" (\"Username\", \"Password\", \"PasswordSalt\", \"Role\") values ('{user.Username}', '{password}', '{passSalt}', '0')").GetAwaiter().GetResult())
            {
                return Ok();
            }
            return BadRequest();
        }
        [Route("Delete")]
        [HttpPost]
        public ActionResult DeleteUser(int id)
        {
            if (_dbService.ExecuteNonQuery($"DELETE FROM \"UserDTO\" WHERE id={id}").GetAwaiter().GetResult())
            {
                return Ok();
            }
            return BadRequest();
        }
        [Route("Update")]
        [HttpPost]
        public ActionResult Update(int id, [FromBody] UserDTO user)
        {
            UserDTO userDTO = _dbService.GetTodo($"SELECT * FROM \"UserDTO\" WHERE id={id}").GetAwaiter().GetResult();
            if (userDTO == null)
            {
                return BadRequest();
            }
            if (!user.Username.IsNullOrEmpty())
            {
                userDTO.Username = user.Username;
            }
            if (!user.Password.IsNullOrEmpty())
            {
                byte[] passHash = new byte[keySize];
                string password = _dbService.HashPasword(user.Password ?? "", out passHash);
                string passSalt = Convert.ToHexString(passHash);
                userDTO.Password = password;
                userDTO.PasswordSalt = passSalt;
            }
            if (!user.Role.ToString().IsNullOrEmpty())
            {
                userDTO.Role = user.Role;
            }
            if (_dbService.ExecuteNonQuery($"UPDATE \"UserDTO\" SET \"Username\"='{userDTO.Username}', \"Password\"='{userDTO.Password}', \"PasswordSalt\"='{userDTO.PasswordSalt}', \"Role\"='{(int)userDTO.Role}' WHERE id={id}").GetAwaiter().GetResult())
            {
                return Ok();
            }
            return BadRequest();
        }
    }
}
