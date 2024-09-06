using BiometricAuthenticationAPI.Data.Models;
using BiometricAuthenticationAPI.Data.Models.Response;

namespace BiometricAuthenticationAPI.Services.Interfaces
{
    public interface IFaceMatchService
    {
        Task<FaceVerifyResponse?> MatchFace(MatchFacesRequest matchFacesRequest);
    }
}
