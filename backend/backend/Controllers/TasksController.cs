using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using backend.Data;
using backend.Model;
using Microsoft.AspNetCore.Authorization;
using backend.Services;
using Supabase.Gotrue;
using System.Security.Cryptography;
using Microsoft.IdentityModel.Tokens;

namespace backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TasksController : ControllerBase
    {
        private readonly backendContext _context;
        private DbService<Model.Task> _dbService;

        public TasksController(backendContext context)
        {
            _dbService = new DbService<Model.Task>();
            _context = context;
        }

        [HttpPost]
        [Authorize]
        [Route("GetAll")]
        public ICollection<Model.Task> Get(int id)
        {
            return _dbService.GetAll($"SELECT * FROM \"TODO\" WHERE \"UserId\"={id}").GetAwaiter().GetResult();
        }

        [HttpGet]
        [Authorize]
        [Route("GetOne")]
        public Model.Task GetOne(int id)
        {
            return _dbService.GetOne($"SELECT * FROM \"TODO\" WHERE id={id}").GetAwaiter().GetResult();
        }

        [HttpPost]
        [Authorize]
        [Route("Add")]
        public ActionResult Add([FromBody] Model.Task task)
        {
             if(_dbService.ExecuteNonQuery($"INSERT INTO \"TODO\" (\"UserId\", \"Title\", \"Description\") VALUES ({task.UserId?.id}, '{task.Title}', '{task.Description}')").GetAwaiter().GetResult())
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
            Model.Task task = _dbService.GetOne($"SELECT * FROM \"TODO\" WHERE id={id}").GetAwaiter().GetResult();
            if (task == null)
            {
                return BadRequest();
            }
            if (!value.Description.IsNullOrEmpty())
            {
                task.Description = value.Description;
            }
            if (!value.Title.IsNullOrEmpty())
            {
                task.Title = value.Title;
            }
            if (_dbService.ExecuteNonQuery($"UPDATE \"TODO\" SET \"Title\"='{task.Title}', \"Description\"='{task.Description}' WHERE id={id}").GetAwaiter().GetResult())
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

        private bool TaskExists(int id)
        {
          return (_context.Task?.Any(e => e.id == id)).GetValueOrDefault();
        }
    }
}
