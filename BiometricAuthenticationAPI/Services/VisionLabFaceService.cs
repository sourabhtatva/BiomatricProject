using BiometricAuthenticationAPI.Helpers.Constants;
using BiometricAuthenticationAPI.Services.Interfaces;

namespace BiometricAuthenticationAPI.Services
{
    public class VisionLabFaceService : IVisionLabFaceService
    {
        private readonly FaceEngineWrapper _faceEngineWrapper;

        public VisionLabFaceService()
        {
            _faceEngineWrapper = new FaceEngineWrapper();
            _faceEngineWrapper.InitializeEngine();
        }

        public string GetDataDictionary()
        {
            var response = _faceEngineWrapper.GetDataDirectory();
            return response;
        }

        public bool GetLicenseActivated()
        {
            var response = _faceEngineWrapper.IsActivated();
            return response;
        }

        public string GetDefaultPath()
        {
            var response = _faceEngineWrapper.GetDefaultPath();
            return response;
        }

        public bool CheckFeatureId(int featureId)
        {
            var response = _faceEngineWrapper.CheckFeatureId(featureId);
            return response;
        }
    }
}
