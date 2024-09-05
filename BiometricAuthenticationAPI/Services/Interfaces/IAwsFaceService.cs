using Amazon.Rekognition.Model;
using BiometricAuthenticationAPI.Data.Models.Response;

namespace BiometricAuthenticationAPI.Services.Interfaces
{
    public interface IAwsFaceService
    {
        Task<CompareFacesResponse?> VerifyFacesAsync(CompareFacesRequest compareFacesRequest);
    }
}
