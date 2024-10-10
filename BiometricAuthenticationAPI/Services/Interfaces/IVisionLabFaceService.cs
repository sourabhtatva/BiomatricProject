namespace BiometricAuthenticationAPI.Services.Interfaces
{
    public interface IVisionLabFaceService
    {
        dynamic ExecuteAction(string action);
        bool GetLicenseActivated();
        string GetDefaultPath();
        bool CheckFeatureId(int featureId);
    }
}
