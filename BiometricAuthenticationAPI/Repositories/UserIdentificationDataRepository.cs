using BiometricAuthenticationAPI.Data;
using BiometricAuthenticationAPI.Data.Models;
using BiometricAuthenticationAPI.Repositories.Interfaces;
using System.Net;

namespace BiometricAuthenticationAPI.Repositories
{

    public class UserIdentificationDataRepository : RepositoryBase<UserIdentificationData>, IUserIdentificationDataRepository
    {
        public UserIdentificationDataRepository(IDbConnectionFactory dbConnectionFactory) : base(dbConnectionFactory)
        {
        }
    }
}
