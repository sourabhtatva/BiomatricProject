using System.Data;

namespace BiometricAuthenticationAPI.Data
{
    public interface IDbConnectionFactory
    {
        IDbConnection CreateConnection();
    }
}
