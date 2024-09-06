using Amazon.Rekognition.Model;

namespace BiometricAuthenticationAPI.Services.Interfaces
{
    public interface IAwsFaceService
    {
        Task<CompareFacesResponse?> VerifyFacesAsync(CompareFacesRequest compareFacesRequest);
    }
}
