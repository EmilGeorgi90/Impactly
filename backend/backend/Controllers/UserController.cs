using backend.Model;
using backend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

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
        [Authorize("Admin")]
        [Route("GetOne")]
        [HttpPost]
        public ActionResult GetUser([FromBody] UserLogin user)
        {
            UserDTO userDTO = (UserDTO)_dbService.GetOne($"SELECT * FROM \"UserDTO\" WHERE id={user.ID}").GetAwaiter().GetResult();
            if (userDTO == null)
            {
                return BadRequest();
            }
            return Ok(userDTO);
        }
        [Authorize("Admin")]
        [Route("Add")]
        [HttpPost]
        public ActionResult AddUser([FromBody] UserLogin user)
        {
            byte[] passHash = new byte[keySize];
            string password = _dbService.HashPasword(user.Password, out passHash);
            string passSalt = Convert.ToHexString(passHash);
            if (_dbService.Insert($"INSERT INTO \"UserDTO\" (\"Username\", \"Password\", \"PasswordSalt\", \"Role\") values ('{user.Username}', '{password}', '{passSalt}', '0')").GetAwaiter().GetResult())
            {
                return Ok();
            }
            return BadRequest();
        }
        [Authorize("Admin")]
        [Route("Delete")]
        [HttpPost]
        public ActionResult DeleteUser(int id)
        {
            if (_dbService.Delete($"DELETE FROM \"UserDTO\" WHERE id={id}").GetAwaiter().GetResult())
            {
                return Ok();
            }
            return BadRequest();
        }
        [Authorize("Admin")]
        [Route("Update")]
        [HttpPost]
        public ActionResult UpdateUser([FromBody] UserLogin user)
        {
            UserDTO userDTO = (UserDTO)_dbService.GetOne($"SELECT * FROM \"UserDTO\" WHERE id={user.ID}").GetAwaiter().GetResult();
            if(userDTO == null)
            {
                return BadRequest();
            }

            byte[] passHash = new byte[keySize];
            string password = _dbService.HashPasword(user.Password, out passHash);
            string passSalt = Convert.ToHexString(passHash);
            if (_dbService.Update($"UPDATE \"UserDTO\" SET \"Username\"={user.Username} \"Password\", \"PasswordSalt\", \"Role\") values ('{user.Username}', '{password}', '{passSalt}', '0')").GetAwaiter().GetResult())
            {
                return Ok();
            }
            return BadRequest();
        }
    }
}
