using BiometricAuthenticationAPI.Helpers.Constants;
using BiometricAuthenticationAPI.Helpers.Extensions;
using BiometricAuthenticationAPI.Services.Interfaces;
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
        public IActionResult TestEndPoint()
        {
            try
            {

                //bool response = _visionLabFaceService.CheckFeatureId(1);

                //bool response = _visionLabFaceService.GetLicenseActivated();

                //string response = _visionLabFaceService.GetDefaultPath();

                //string response = _visionLabFaceService.ExecuteAction("GetDataDirectory");

                //string response = _visionLabFaceService.ExecuteAction("FaceDetection");

                //string response = _visionLabFaceService.ExecuteAction("CrowdEstimator");

                //string response = _visionLabFaceService.ExecuteAction("GlassEstimator");

                //string response = _visionLabFaceService.ExecuteAction("MedicalMaskEstimator");

                //string response = _visionLabFaceService.ExecuteAction("PPEEstimator");

                string response = _visionLabFaceService.ExecuteAction("ProcessingImage", "");

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
