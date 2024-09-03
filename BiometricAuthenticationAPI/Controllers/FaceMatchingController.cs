using BiometricAuthenticationAPI.Data.Models;
using BiometricAuthenticationAPI.Helpers.Constants;
using BiometricAuthenticationAPI.Helpers.Extensions;
using BiometricAuthenticationAPI.Helpers.Utils;
using BiometricAuthenticationAPI.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BiometricAuthenticationAPI.Controllers
{
    // <summary>
    /// Face Matching api controller.
    /// </summary>
    [Route("api/[controller]")]
    public class FaceMatchingController(IAzureFaceService azureFaceService,IRecognitionLogService recognitionLogService, ILogger<FaceMatchingController> logger) : ControllerBase
    {
        private readonly IAzureFaceService _azureFaceService = azureFaceService;
        private readonly IRecognitionLogService _recognitionLogService = recognitionLogService;
        private readonly ILogger<FaceMatchingController> _logger = logger;
        private readonly string _entityName = SystemConstants.FaceMatching.CONTROLLER_ENTITY;

        [HttpPost("match")]
        public async Task<IActionResult> MatchFaces([FromBody] MatchFacesRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    throw new DataValidationException(ModelState);
                }

                string faceId1 = await _azureFaceService.DetectFaceAsync(request.ScannedImage);
                string faceId2 = await _azureFaceService.DetectFaceAsync(request.ClickedImage);

                if (faceId1 == null || faceId2 == null)
                {
                    return BadRequest(Messages.FaceMatching.General.DetectFailureMessage);
                }

                var response = await _azureFaceService.VerifyFacesAsync(faceId1, faceId2);
                if(response == null)
                {
                    return BadRequest(Messages.FaceMatching.General.VerifyFailureMessage);
                }

                RecognitionLog recognitionLog = new()
                {
                    ConfidenceLevel = response.Confidence,
                    RecognitionTime = DateTime.Now,
                    Status = response.IsIdentical ? SystemConstants.General.SUCCESS : SystemConstants.General.FAILURE
                };
                await _recognitionLogService.AddRecognitionLogData(recognitionLog);
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
