using BiometricAuthenticationAPI.Data.Models;
using BiometricAuthenticationAPI.Data;
using BiometricAuthenticationAPI.Repositories.Interfaces;

namespace BiometricAuthenticationAPI.Repositories
{
    public class FaceDataRepository : RepositoryBase<FaceData>, IFaceDataRepository
    {
        public FaceDataRepository(IDbConnectionFactory dbConnectionFactory) : base(dbConnectionFactory)
        {
        }
    }
}
