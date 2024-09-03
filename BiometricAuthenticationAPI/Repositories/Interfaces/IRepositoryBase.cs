namespace BiometricAuthenticationAPI.Repositories.Interfaces
{
    public interface IRepositoryBase<T> where T : class
    {
        Task<IEnumerable<T>> GetAllAsync(string storedProcedure, object? parameters = null);
        Task<T> GetAsync(string storedProcedure, object parameters);
        Task<int> AddAsync(string storedProcedure, object parameters);
        Task<int> UpdateAsync(string storedProcedure, object parameters);
        Task<int> DeleteAsync(string storedProcedure, object parameters);
    }
}
