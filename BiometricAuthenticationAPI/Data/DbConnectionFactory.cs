using Microsoft.Data.SqlClient;
using System.Data;

namespace BiometricAuthenticationAPI.Data
{
    public class DbConnectionFactory(IConfiguration configuration) : IDbConnectionFactory
    {
        private readonly string _connectionString = configuration.GetConnectionString("DefaultConnection");

        public IDbConnection CreateConnection()
        {
            return new SqlConnection(_connectionString);
        }
    }
}
