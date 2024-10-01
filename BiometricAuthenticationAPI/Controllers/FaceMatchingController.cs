using BiometricAuthenticationAPI.Data.Models.Common;
using BiometricAuthenticationAPI.Data.Models.Request;
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

        /// <summary>
        /// Match Face API endpoint.
        /// </summary>
        /// <param name="request">Match Face Request Data.</param>
        /// <returns>Match Face response.</returns>
        [HttpPost("match")]
        public async Task<IActionResult> MatchFacesAWS([FromBody] MatchFacesRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    IList<ValidationDetails> errorInfo = ModelState.Select(ex => new ValidationDetails { InputName = ex.Key, ValidationMessage = ex.Value?.Errors.FirstOrDefault()?.ErrorMessage }).ToList();
                    return this.FailureResult(errorInfo, Messages.Common.ModelStateFailureMessage);
                }

                FaceVerifyResponse? response = await _faceMatchService.MatchFace(request);

                return this.SuccessResult(response, Messages.FaceMatching.General.FaceMatchingMessage(_entityName));                                
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return this.FailureResult(null, ex.Message);
            }
        }
    }
}
