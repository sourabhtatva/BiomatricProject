using BiometricAuthenticationAPI.Data;
using BiometricAuthenticationAPI.Data.Models.Response;
using BiometricAuthenticationAPI.Repositories.Interfaces;

namespace BiometricAuthenticationAPI.Repositories
{
    public class DocumentValidationResponseRepository : RepositoryBase<DocumentValidateResponse>, IDocumentValidationResponseRepository
    {
        public DocumentValidationResponseRepository(IDbConnectionFactory dbConnectionFactory) : base(dbConnectionFactory)
        {
        }
    }
}
