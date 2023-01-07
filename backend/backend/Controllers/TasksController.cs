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

namespace backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TasksController : ControllerBase
    {
        private readonly backendContext _context;

        public TasksController(backendContext context)
        {
            _context = context;
        }

        // GET: Tasks
        [HttpGet]
        [Authorize]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/<Auth>/5
        [HttpGet("{id}")]
        [Authorize]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<Auth>
        [HttpPost]
        [Authorize]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/<Auth>/5
        [HttpPut("{id}")]
        [Authorize]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<Auth>/5
        [HttpDelete("{id}")]
        [Authorize]
        public void Delete(int id)
        {
        }

        private bool TaskExists(int id)
        {
          return (_context.Task?.Any(e => e.ID == id)).GetValueOrDefault();
        }
    }
}
