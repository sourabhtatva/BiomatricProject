using BiometricAuthenticationAPI.Data.Models;

namespace BiometricAuthenticationAPI.Services.Interfaces
{
    public interface IRecognitionLogService
    {
        Task<int?> AddRecognitionLogData(RecognitionLog recognitionLogData);
    }
}
