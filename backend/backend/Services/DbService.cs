using backend.Model;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Npgsql;
using System.ComponentModel;
using System.Data.Common;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;

namespace backend.Services
{
    internal class DbService<T> where T : IModel
    {
        private const int iterations = 350000;
        private HashAlgorithmName hashAlgorithm = HashAlgorithmName.SHA512;
        private static string host = Environment.GetEnvironmentVariable("SupabaseHost");
        private static string dbpass = Environment.GetEnvironmentVariable("SupabasePass");
        private static string port = Environment.GetEnvironmentVariable("supabasePort");
        private static string user = Environment.GetEnvironmentVariable("supabaseUser");
        private static string db = Environment.GetEnvironmentVariable("supabaseDb");

        private string cs = $"User Id={user};Password={dbpass};Server={host};Port={port};Database={db}";
        private NpgsqlDataSource dataSource;
        private const int keySize = 64;
        public DbService()
        {
            dataSource = NpgsqlDataSource.Create(cs);
        }
        private async Task<IModel> GetUser(string sql)
        {
            List<KeyValuePair<string, object>> keyValues = new List<KeyValuePair<string, object>>();
            IModel returnResult = Activator.CreateInstance(typeof(UserDTO), new object[] { }) as IModel ?? throw new ArgumentException();
            await using var cmd = dataSource.CreateCommand(sql);
            await using var reader = await cmd.ExecuteReaderAsync();
            int fieldCount = reader.FieldCount;
            while (reader.Read())
            {
                object[] fieldValues = new object[fieldCount];
                int instances = reader.GetValues(fieldValues);
                for (int fieldCounter = 0; fieldCounter < fieldCount; fieldCounter++)
                {
                    KeyValuePair<string, object> keyValue = new KeyValuePair<string, object>(reader.GetName(fieldCounter), fieldValues[fieldCounter]);
                    Console.WriteLine();
                    if (Convert.IsDBNull(fieldValues[fieldCounter]))
                        fieldValues[fieldCounter] = "NA";

                    keyValues.Add(keyValue);
                }
            }
            foreach (var item in keyValues)
            {
                PropertyInfo prop = returnResult?.GetType().GetProperty(item.Key) ?? throw new ArgumentException();
                var propType = returnResult?.GetType().GetProperty(item.Key)?.PropertyType;
                Console.WriteLine(item.Value.ToString());
                var converter = TypeDescriptor.GetConverter(propType ?? throw new ArgumentException());
                var convertedObject = converter.ConvertFromString(item.Value.ToString());
                prop.SetValue(returnResult, convertedObject, null);

            }
            return returnResult ?? throw new ArgumentException();
        }
        public async Task<T> ExecuteQuery(string sql)
        {
            List<KeyValuePair<string, object>> keyValues = new List<KeyValuePair<string, object>>();
            T returnResult = (T)Activator.CreateInstance(typeof(T), new object[] { });
            var cmd = dataSource.CreateCommand(sql);
            var reader = await cmd.ExecuteReaderAsync();
            if(!reader.HasRows)
            {
                throw new ArgumentException("No rows found");
            }
            int fieldCount = reader.FieldCount;
            while (reader.Read())
            {
                object[] fieldValues = new object[fieldCount];
                int instances = reader.GetValues(fieldValues);
                for (int fieldCounter = 0; fieldCounter < fieldCount; fieldCounter++)
                {
                    KeyValuePair<string, object> keyValue = new KeyValuePair<string, object>(reader.GetName(fieldCounter), fieldValues[fieldCounter]);
                    Console.WriteLine();
                    if (Convert.IsDBNull(fieldValues[fieldCounter]))
                        fieldValues[fieldCounter] = "NA";

                    keyValues.Add(keyValue);
                }
            }
            foreach (var item in keyValues)
            {
                PropertyInfo prop = returnResult?.GetType().GetProperty(item.Key) ?? throw new ArgumentException();
                if (item.Key == "UserId")
                {
                    UserDTO temp = (UserDTO)await GetUser($"SELECT * FROM \"UserDTO\" WHERE id={item.Value}");
                    prop.SetValue(returnResult, temp);
                }
                else
                {
                    var propType = returnResult.GetType().GetProperty(item.Key)?.PropertyType;
                    Console.WriteLine(item.Value.ToString());
                    var converter = TypeDescriptor.GetConverter(propType ?? throw new ArgumentException());
                    var convertedObject = converter.ConvertFromString(item.Value.ToString() ?? "");
                    prop.SetValue(returnResult, convertedObject, null);
                }
            }
            return returnResult ?? throw new ArgumentException();
        }
        public async Task<ICollection<T>> ExecuteQueries(string sql)
        {
            List<T> list = new List<T>();
            var cmd = dataSource.CreateCommand(sql);
            var reader = await cmd.ExecuteReaderAsync();
            int fieldCount = reader.FieldCount;
            while (reader.Read())
            {
                T returnResult = (T)Activator.CreateInstance(typeof(T), new object[] { });
                List<KeyValuePair<string, object>> keyValues = new List<KeyValuePair<string, object>>();
                object[] fieldValues = new object[fieldCount];
                int instances = reader.GetValues(fieldValues);
                for (int fieldCounter = 0; fieldCounter < fieldCount; fieldCounter++)
                {
                    KeyValuePair<string, object> keyValue = new KeyValuePair<string, object>(reader.GetName(fieldCounter), fieldValues[fieldCounter]);
                    Console.WriteLine();
                    if (Convert.IsDBNull(fieldValues[fieldCounter]))
                        fieldValues[fieldCounter] = "NA";

                    keyValues.Add(keyValue);
                }
                foreach (var item in keyValues)
                {
                    PropertyInfo prop = returnResult?.GetType().GetProperty(item.Key);
                    if (prop == null) continue;
                    if (item.Key == "UserId")
                    {
                        UserDTO temp = (UserDTO)await GetUser($"SELECT * FROM \"UserDTO\" WHERE id={item.Value}");
                        prop.SetValue(returnResult, temp);
                    }
                    else
                    {
                        var propType = returnResult.GetType().GetProperty(item.Key)?.PropertyType;
                        var converter = TypeDescriptor.GetConverter(propType ?? throw new ArgumentException());
                        var convertedObject = converter.ConvertFromString(item.Value.ToString() ?? "");
                        prop.SetValue(returnResult, convertedObject, null);
                    }
                }
                list.Add(returnResult ?? throw new ArgumentException());
            }
            return list;
        }
        public async Task<bool> ExecuteNonQuery(string sql)
        {
            try
            {
                var cmd = dataSource.CreateCommand(sql);
                var RecordsAffected = await cmd.ExecuteNonQueryAsync();
                if (RecordsAffected > 0)
                {
                    return true;
                }
                return false;
            }
            catch (DbException)
            {
                return false;
            }
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
