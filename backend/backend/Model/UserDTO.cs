using Postgrest.Attributes;
using Supabase;

namespace backend.Model
{
    public class UserDTO : IModel
    {
        public int id { get; set; }

        public string? Username { get; set; }

        public string? Password { get; set; }

        public string? PasswordSalt { get; set; }

        public Roles Role { get; set; }

        public DateTime created_at { get; set; }
        public override string ToString()
        {
            return $"{Username} {Role}";
        }
    }
}
