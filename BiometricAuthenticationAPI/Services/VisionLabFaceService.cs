using BiometricAuthenticationAPI.Services.Interfaces;


namespace BiometricAuthenticationAPI.Services
{
    public class VisionLabFaceService : IVisionLabFaceService
    {
        private readonly FaceEngineWrapper _faceEngineWrapper = new FaceEngineWrapper();

        public VisionLabFaceService()
        {
            _faceEngineWrapper = new FaceEngineWrapper();
        }

        public dynamic GetDataDictionary(string action)
        {
            var response = _faceEngineWrapper.InitializeEngine(action);
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
