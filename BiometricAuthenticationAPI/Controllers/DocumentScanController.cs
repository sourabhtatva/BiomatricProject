using BiometricAuthenticationAPI.Data.Models;
using BiometricAuthenticationAPI.Helpers.Constants;
using BiometricAuthenticationAPI.Helpers.Extensions;
using BiometricAuthenticationAPI.Helpers.Utils;
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
        /// <param name="userIdentificationData">User Identification data.</param>
        /// <returns>Created user Identification Data object in response.</returns>
        /// <exception cref="DataValidationException">Show model validation errors.</exception>
        [HttpPost]
        public async Task<ActionResult> InsertUserIdentificationData([FromBody] UserIdentificationData userIdentificationData)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    throw new DataValidationException(ModelState);
                }

                var response = await _userIdentificationDataService.InsertUserIdentificationData(userIdentificationData);

                return response == null
                    ? this.FailureResult(null, Messages.UserIdentificationData.General.AddError(_entityName))
                    : this.SuccessResult(response, Messages.UserIdentificationData.General.AddMessage(_entityName));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                throw;
            }
        }

        /// <summary>
        /// Add User Identification Data api.
        /// </summary>
        /// <param name="documentDetail">Document detail data.</param>
        /// <returns>Document Validate Response Data object in response.</returns>
        /// <exception cref="DataValidationException">Show model validation errors.</exception>
        [HttpPost("validate")]
        public async Task<ActionResult> ValidateDocument([FromBody] DocumentDetailRequest documentDetail)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    throw new DataValidationException(ModelState);
                }

                string message = string.Empty;
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
                throw;
            }
        }
    }
}
