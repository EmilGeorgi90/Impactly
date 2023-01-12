using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using backend.Services;
using Microsoft.IdentityModel.Tokens;

namespace backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TasksController : ControllerBase
    {
        private DbService<Model.Task> _dbService;

        public TasksController()
        {
            _dbService = new DbService<Model.Task>();
        }

        [HttpPost]
        [Authorize]
        [Route("GetAll")]
        public ICollection<Model.Task> Get(int id)
        {
            return _dbService.ExecuteQueries($"SELECT * FROM \"TODO\" WHERE \"UserId\"={id}").GetAwaiter().GetResult();
        }

        [HttpGet]
        [Authorize]
        [Route("GetOne")]
        public Model.Task GetOne(int id)
        {
            return _dbService.ExecuteQuery($"SELECT * FROM \"TODO\" WHERE id={id}").GetAwaiter().GetResult();
        }

        [HttpPost]
        [Authorize]
        [Route("Add")]
        public ActionResult Add(int id, [FromBody] Model.Task task)
        {
             if(_dbService.ExecuteNonQuery($"INSERT INTO \"TODO\" (\"UserId\", \"Title\") VALUES ({id}, '{task.Title}')").GetAwaiter().GetResult())
            {
                return Ok();
            }
            return BadRequest();
        }

        // PUT api/<Auth>/5
        [HttpPost]
        [Authorize]
        [Route("Update")]
        public ActionResult Update(int id, [FromBody] Model.Task value)
        {
            Model.Task task = _dbService.ExecuteQuery($"SELECT * FROM \"TODO\" WHERE id={id}").GetAwaiter().GetResult();
            if (task == null)
            {
                return BadRequest();
            }
            if (!value.Title.IsNullOrEmpty())
            {
                task.Title = value.Title;
            }
            if (_dbService.ExecuteNonQuery($"UPDATE \"TODO\" SET \"Title\"='{task.Title}' WHERE id={id}").GetAwaiter().GetResult())
            {
                return Ok();
            }
            return BadRequest();
        }

        // DELETE api/<Auth>/5
        [HttpPost]
        [Authorize]
        [Route("Delete")]
        public ActionResult Delete(int id)
        {
            if (_dbService.ExecuteNonQuery($"DELETE FROM \"TODO\" WHERE id={id}").GetAwaiter().GetResult())
            {
                return Ok();
            }
            return BadRequest();
        }
    }
}
