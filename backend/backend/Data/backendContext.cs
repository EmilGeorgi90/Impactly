using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using backend.Model;

namespace backend.Data
{
    public class backendContext : DbContext
    {
        public backendContext (DbContextOptions<backendContext> options)
            : base(options)
        {
        }

        public DbSet<backend.Model.UserDTO> UserDTO { get; set; } = default!;

        public DbSet<backend.Model.Task> Task { get; set; } = default!;
    }
}
