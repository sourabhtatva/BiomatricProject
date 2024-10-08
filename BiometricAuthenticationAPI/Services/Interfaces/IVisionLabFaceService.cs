namespace BiometricAuthenticationAPI.Services.Interfaces
{
    public interface IVisionLabFaceService
    {
        dynamic GetDataDictionary(string action);
        bool GetLicenseActivated();
        string GetDefaultPath();
        bool CheckFeatureId(int featureId);
    }
}
