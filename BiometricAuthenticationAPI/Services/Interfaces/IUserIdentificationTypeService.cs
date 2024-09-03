using BiometricAuthenticationAPI.Data.Models;

namespace BiometricAuthenticationAPI.Services.Interfaces
{
    public interface IUserIdentificationTypeService
    {
        Task<IEnumerable<UserIdentificationType>> GetAllUserIdentificationType();
        Task<UserIdentificationType> GetUserIdentificationType(int Id);
        Task<UserIdentificationType> GetUserIdentificationTypeByType(string Type);
    }
}
