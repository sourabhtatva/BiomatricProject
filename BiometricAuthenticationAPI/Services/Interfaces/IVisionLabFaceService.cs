namespace BiometricAuthenticationAPI.Services.Interfaces
{
    public interface IVisionLabFaceService
    {
        string GetDataDictionary();
        bool GetLicenseActivated();
        string GetDefaultPath();
        bool CheckFeatureId(int featureId);
    }
}
