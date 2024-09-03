using BiometricAuthenticationAPI.Data.Models;
using BiometricAuthenticationAPI.Data.Models.Response;

namespace BiometricAuthenticationAPI.Services.Interfaces
{
    public interface IUserIdentificationDataService
    {
        Task<IEnumerable<UserIdentificationData>> GetAllUserIdentificationData();
        Task<UserIdentificationData> GetUserIdentificationData(int Id);
        Task<int?> InsertUserIdentificationData(UserIdentificationData userIdentificationData);
        Task<int?> UpdateUserIdentificationData(int Id, UserIdentificationData userIdentificationData);
        Task<int?> DeleteUserIdentificationData(int Id);
        Task<DocumentValidateResponse> ValidateUserIdentificationData(DocumentDetailRequest documentDetailRequest);
    }
}
