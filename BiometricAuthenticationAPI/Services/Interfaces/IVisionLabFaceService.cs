namespace BiometricAuthenticationAPI.Services.Interfaces
{
    public interface IVisionLabFaceService
    {
        //void InitializeAndDetectFaces();
        int DetectFaces(byte[] imageData);
        string GetDataDictionary();
        bool GetLicenseActivated();
        string GetDefaultPath();
        //LivenessResult CheckLiveness(byte[] imageData);
        //TrackResult TrackFaces(byte[] imageData);
    }
}
