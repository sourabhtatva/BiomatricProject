namespace BiometricAuthenticationAPI.Services.Interfaces
{
    public interface IVisionLabFaceService
    {
        dynamic ExecuteAction(string action, string? base64String);
        bool GetLicenseActivated();
        string GetDefaultPath();
        bool CheckFeatureId(int featureId);
    }
}
