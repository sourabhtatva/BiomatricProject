using BiometricAuthenticationAPI.Data.Models.Response;

namespace BiometricAuthenticationAPI.Services.Interfaces
{
    public interface IAzureFaceService
    {
        Task<string?> DetectFaceAsync(byte[] image);

        Task<FaceVerifyResponse?> VerifyFacesAsync(string faceId1, string faceId2);
    }
}
