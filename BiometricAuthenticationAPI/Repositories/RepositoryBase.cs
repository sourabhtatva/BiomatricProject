using BiometricAuthenticationAPI.Data;
using BiometricAuthenticationAPI.Repositories.Interfaces;
using Dapper;
using System.Data;

namespace BiometricAuthenticationAPI.Repositories
{
    public abstract class RepositoryBase<T>(IDbConnectionFactory dbConnectionFactory) : IRepositoryBase<T> where T : class
    {
        protected IDbConnectionFactory _dbConnectionFactory { get; set; } = dbConnectionFactory;

        public async Task<IEnumerable<T>> GetAllAsync(string storedProcedure, object? parameters = null)
        {
            using (var connection = _dbConnectionFactory.CreateConnection())
            {
                return await connection.QueryAsync<T>(storedProcedure, parameters, commandType: CommandType.StoredProcedure);
            }
        }

        public async Task<T> GetAsync(string storedProcedure, object parameters)
        {
            using (var connection = _dbConnectionFactory.CreateConnection())
            {
                return await connection.QueryFirstOrDefaultAsync<T>(storedProcedure, parameters, commandType: CommandType.StoredProcedure);
            }
        }

        public async Task<int> AddAsync(string storedProcedure, object parameters)
        {
            using (var connection = _dbConnectionFactory.CreateConnection())
            {
                return await connection.ExecuteAsync(storedProcedure, parameters, commandType: CommandType.StoredProcedure);
            }
        }

        public async Task<int> UpdateAsync(string storedProcedure, object parameters)
        {
            using (var connection = _dbConnectionFactory.CreateConnection())
            {
                return await connection.ExecuteAsync(storedProcedure, parameters, commandType: CommandType.StoredProcedure);
            }
        }

        public async Task<int> DeleteAsync(string storedProcedure, object parameters)
        {
            using (var connection = _dbConnectionFactory.CreateConnection())
            {
                return await connection.ExecuteAsync(storedProcedure, parameters, commandType: CommandType.StoredProcedure);
            }
        }
    }

}
