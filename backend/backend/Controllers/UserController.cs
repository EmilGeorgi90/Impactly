using backend.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Security.Claims;

namespace backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        [HttpGet]
        [Route("Admins")]
        [Authorize(Roles = "Admin")]
        public IActionResult AdminEndPoint()
        {
            var currentUser = GetCurrentUser();
            return Ok($"Hi you are an {currentUser.Role}");
        }
        private UserDTO GetCurrentUser()
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            if (identity != null)
            {
                var userClaims = identity.Claims;
                return new UserDTO
                {
                    Username = userClaims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value,
                    Role = (Roles)int.Parse(userClaims.FirstOrDefault(x => x.Type == ClaimTypes.Role)?.Value)
                };
            }
            return null;
        }
    }
}
