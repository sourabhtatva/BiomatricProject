using BiometricAuthenticationAPI.Data.Models;

namespace BiometricAuthenticationAPI.Services.Interfaces
{
    public interface IRecognitionLogService
    {
        Task<IEnumerable<RecognitionLog>> GetAllRecognitionLogData();
        Task<RecognitionLog> GetRecognitionLogData(int Id);
        Task<int?> AddRecognitionLogData(RecognitionLog recognitionLogData);
        Task<int?> UpdateRecognitionLogData(int Id, RecognitionLog recognitionLogData);
        Task<int?> DeleteRecognitionLogData(int Id);
    }
}
