using BiometricAuthenticationAPI.Data.Models;
using BiometricAuthenticationAPI.Data.Models.Response;
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
    public class FaceMatchingController(IFaceMatchService faceMatchService, ILogger<FaceMatchingController> logger) : ControllerBase
    {
        private readonly IFaceMatchService _faceMatchService = faceMatchService;
        private readonly ILogger<FaceMatchingController> _logger = logger;
        private readonly string _entityName = SystemConstants.FaceMatching.CONTROLLER_ENTITY;

        [HttpPost("match")]
        public async Task<IActionResult> MatchFacesAWS([FromBody] MatchFacesRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    throw new DataValidationException(ModelState);
                }

                FaceVerifyResponse? response = await _faceMatchService.MatchFace(request);

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
