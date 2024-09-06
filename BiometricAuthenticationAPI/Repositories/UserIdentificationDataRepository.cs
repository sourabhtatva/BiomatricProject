using BiometricAuthenticationAPI.Data;
using BiometricAuthenticationAPI.Data.Models;
using BiometricAuthenticationAPI.Repositories.Interfaces;

namespace BiometricAuthenticationAPI.Repositories
{

    public class UserIdentificationDataRepository : RepositoryBase<UserIdentificationData>, IUserIdentificationDataRepository
    {
        public UserIdentificationDataRepository(IDbConnectionFactory dbConnectionFactory) : base(dbConnectionFactory)
        {
        }
    }
}
