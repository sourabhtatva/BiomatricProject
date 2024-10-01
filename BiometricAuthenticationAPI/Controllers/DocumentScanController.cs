using BiometricAuthenticationAPI.Data.Models.Common;
using BiometricAuthenticationAPI.Data.Models.Request;
using BiometricAuthenticationAPI.Helpers.Constants;
using BiometricAuthenticationAPI.Helpers.Extensions;
using BiometricAuthenticationAPI.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BiometricAuthenticationAPI.Controllers
{
    // <summary>
    /// Document Scan api controller.
    /// </summary>
    [Route("api/[controller]")]
    public class DocumentScanController(IUserIdentificationDataService userIdentificationDataService, ILogger<DocumentScanController> logger) : ControllerBase
    {
        private readonly IUserIdentificationDataService _userIdentificationDataService = userIdentificationDataService;
        private readonly ILogger<DocumentScanController> _logger = logger;
        private readonly string _entityName = SystemConstants.UserIdentificationData.CONTROLLER_ENTITY;

        /// <summary>
        /// Add User Identification Data api.
        /// </summary>
        /// <param name="documentDetail">Document detail data.</param>
        /// <returns>Document Validate Response Data object in response.</returns>
        [HttpPost("validate")]
        public async Task<ActionResult> ValidateDocument([FromBody] DocumentDetailRequest documentDetail)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    IList<ValidationDetails> errorInfo = ModelState.Select(ex => new ValidationDetails { InputName = ex.Key, ValidationMessage = ex.Value?.Errors.FirstOrDefault()?.ErrorMessage }).ToList();
                    return this.FailureResult(errorInfo, Messages.Common.ModelStateFailureMessage);
                }

                string? message = string.Empty;
                var response = await _userIdentificationDataService.ValidateUserIdentificationData(documentDetail);

                if(response != null && !response.IsValid && SystemConstants.General.Reasons.Contains(response.RejectReason))
                {
                    message = response.RejectReason;
                    return this.FailureResult(null, message);
                }

                return !Convert.ToBoolean(response?.IsValid) && response?.RejectReason == "GeneralError"
                    ?  this.FailureResult(null,Messages.UserIdentificationData.General.ValidateErrorMessage(_entityName))
                    :  this.SuccessResult(response, Messages.UserIdentificationData.General.ValidateMessage(_entityName));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return this.FailureResult(null, ex.Message);
            }
        }
    }
}
