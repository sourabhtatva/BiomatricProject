using BiometricAuthenticationAPI.Helpers.Constants;
using BiometricAuthenticationAPI.Helpers.Extensions;
using BiometricAuthenticationAPI.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BiometricAuthenticationAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestController(IVisionLabFaceService visionLabFaceService, ILogger<TestController> logger) : ControllerBase
    {
        private readonly IVisionLabFaceService _visionLabFaceService = visionLabFaceService;
        private readonly ILogger<TestController> _logger = logger;
        private readonly string _entityName = "Test";

        [HttpGet]
        public IActionResult CreateFaceDetectionBatchEndpoint()
        {
            try
            {
                //if (!ModelState.IsValid)
                //{
                //    throw new DataValidationException(ModelState);
                //}

                //FaceVerifyResponse? response = await _faceMatchService.MatchFace(request);
                //visionLabFaceService.InitializeAndDetectFaces();

                //byte[] bytes = new byte[1];

                //int response = _visionLabFaceService.DetectFaces(bytes);

                //string response = _visionLabFaceService.GetDataDictionary();

                //bool response = _visionLabFaceService.GetLicenseActivated();

                string response = _visionLabFaceService.GetDefaultPath();

                return this.SuccessResult(response, Messages.FaceMatching.General.FaceMatchingMessage(_entityName));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                throw;
            }
        }
    }
    }
}
