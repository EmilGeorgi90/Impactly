using backend.Model;
using Npgsql;
using System.Data;
using System.Text;

namespace backend.Services
{
    internal class DbService<T> where T : IModel
    {
        string cs = "User Id=postgres;Password=qkDpwwamqQw888Pn;Server=db.tnluydrgjyfcygqufyoz.supabase.co;Port=5432;Database=postgres";
        private NpgsqlDataSource dataSource;
        public DbService() 
        {
            dataSource = NpgsqlDataSource.Create(cs);
        }
        public async Task<ICollection<IModel>> GetAll(string sql)
        {
            List<IModel> list = new List<IModel>();
            var cmd = dataSource.CreateCommand(sql);
            var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                UserDTO temp = new UserDTO();
                temp.ID = reader.GetInt16(0);
                temp.Username = reader.GetString(2);
                temp.Password = reader.GetString(3);
                temp.Salt = Encoding.Default.GetBytes(reader.GetString(4));
                temp.Role = (Roles)reader.GetInt32(5);
                list.Add(temp);
            }
            return list;
        }
    }
}
