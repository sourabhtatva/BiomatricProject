using BiometricAuthenticationAPI.Data;
using BiometricAuthenticationAPI.Data.Models;
using BiometricAuthenticationAPI.Repositories.Interfaces;

namespace BiometricAuthenticationAPI.Repositories
{
    public class UserIdentificationTypeRepository : RepositoryBase<UserIdentificationType>, IUserIdentificationTypeRepository
    {
        public UserIdentificationTypeRepository(IDbConnectionFactory dbConnectionFactory) : base(dbConnectionFactory)
        {
        }
    }
}
