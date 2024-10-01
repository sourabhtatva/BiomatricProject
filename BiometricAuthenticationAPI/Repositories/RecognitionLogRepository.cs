using BiometricAuthenticationAPI.Data;
using BiometricAuthenticationAPI.Data.Models;
using BiometricAuthenticationAPI.Repositories.Interfaces;

namespace BiometricAuthenticationAPI.Repositories
{
    public class RecognitionLogRepository : RepositoryBase<RecognitionLog>, IRecognitionLogRepository
    {
        public RecognitionLogRepository(IDbConnectionFactory dbConnectionFactory) : base(dbConnectionFactory)
        {
        }
    }
}
