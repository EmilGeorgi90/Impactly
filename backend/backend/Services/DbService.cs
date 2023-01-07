using backend.Model;
using Npgsql;
using System.Data;
using System.Security.Cryptography;
using System.Text;

namespace backend.Services
{
    internal class DbService<T> where T : IModel
    {
        private const int iterations = 350000;
        private HashAlgorithmName hashAlgorithm = HashAlgorithmName.SHA512;
        string cs = "User Id=postgres;Password=qkDpwwamqQw888Pn;Server=db.tnluydrgjyfcygqufyoz.supabase.co;Port=5432;Database=postgres";
        private NpgsqlDataSource dataSource;
        private const int keySize = 64;
        public DbService()
        {
            dataSource = NpgsqlDataSource.Create(cs);
        }
        public async Task<IModel> GetOne(string sql)
        {
            var cmd = dataSource.CreateCommand(sql);
            var reader = await cmd.ExecuteReaderAsync();
            UserDTO temp = new UserDTO();
            temp.ID = reader.GetInt16(0);
            temp.Username = reader.GetString(2);
            temp.Password = reader.GetString(3);
            temp.Salt = reader.GetString(4);
            temp.Role = (Roles)reader.GetInt32(5);
            return temp;
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
                temp.Salt = reader.GetString(4);
                temp.Role = (Roles)reader.GetInt32(5);
                list.Add(temp);
            }
            return list;
        }
        public async Task<bool> Insert(string sql)
        {
            var cmd = dataSource.CreateCommand(sql);
            var RecordsAffected = await cmd.ExecuteNonQueryAsync();
            if (RecordsAffected > 0)
            {
                return true;
            }
            return false;
        }
        public async Task<bool> Delete(string sql)
        {
            var cmd = dataSource.CreateCommand(sql);
            var RecordsAffected = await cmd.ExecuteNonQueryAsync();
            if (RecordsAffected > 0)
            {
                return true;
            }
            return false;
        }
        public async Task<bool> Update(string sql)
        {
            var cmd = dataSource.CreateCommand(sql);
            var RecordsAffected = await cmd.ExecuteNonQueryAsync();
            if (RecordsAffected > 0)
            {
                return true;
            }
            return false;
        }
        public string HashPasword(string password, out byte[] salt)
        {
            salt = RandomNumberGenerator.GetBytes(keySize);
            var hash = Rfc2898DeriveBytes.Pbkdf2(
                Encoding.UTF8.GetBytes(password),
            salt,
                iterations,
                hashAlgorithm,
                keySize);
            return Convert.ToHexString(hash);
        }
        public bool VerifyPassword(string password, string hash, byte[] salt)
        {
            var hashToCompare = Rfc2898DeriveBytes.Pbkdf2(password, salt, iterations, hashAlgorithm, keySize);
            return hashToCompare.SequenceEqual(Convert.FromHexString(hash));
        }
    }
}
