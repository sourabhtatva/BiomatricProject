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
        public int DetectFaces(byte[] image)
        {
            _faceEngineWrapper.DetectFaces(image, out int faceCount);
            return faceCount;
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

        //public void InitializeAndDetectFaces()
        //{
        //    int initResult = FaceEngineWrapper.InitializeFaceEngine();
        //    if (initResult == 0)
        //    {
        //        Console.WriteLine("FaceEngine initialized successfully.");
        //    }
        //    else
        //    {
        //        Console.WriteLine("Failed to initialize FaceEngine.");
        //        return;
        //    }

        //    // Prepare to detect faces
        //    IntPtr imagePtr = IntPtr.Zero; // Load your image pointer here
        //    int faceCount = 0;

        //    // Allocate an array to hold the face data (assuming maximum of 10 faces for demo purposes)
        //    FaceEngineWrapper.FaceData[] faces = new FaceEngineWrapper.FaceData[10];

        //    // Call the DetectFaces function
        //    int detectResult = FaceEngineWrapper.DetectFaces(imagePtr, faces, ref faceCount);

        //    if (detectResult == 0)
        //    {
        //        Console.WriteLine($"Detected {faceCount} faces.");
        //        foreach (var face in faces)
        //        {
        //            Console.WriteLine($"Face: X={face.X}, Y={face.Y}, Width={face.Width}, Height={face.Height}");
        //        }
        //    }
        //    else
        //    {
        //        Console.WriteLine("Failed to detect faces.");
        //    }
        //}
    }
}
