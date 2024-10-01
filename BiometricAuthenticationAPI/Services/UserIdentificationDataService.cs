using BiometricAuthenticationAPI.Data.Models.Request;
using BiometricAuthenticationAPI.Data.Models.Response;
using BiometricAuthenticationAPI.Helpers.Constants;
using BiometricAuthenticationAPI.Helpers.Utils;
using BiometricAuthenticationAPI.Repositories.Interfaces;
using BiometricAuthenticationAPI.Services.Interfaces;
using Dapper;
using System.Data;

namespace BiometricAuthenticationAPI.Services
{
    public class UserIdentificationDataService(IDocumentValidationResponseRepository documentValidationResponseRepository, IMemoryCacheService memoryCacheService, ILogger<UserIdentificationDataService> logger) : IUserIdentificationDataService
    {
        private readonly IDocumentValidationResponseRepository _documentValidationResponseRepository = documentValidationResponseRepository;
        private readonly IMemoryCacheService _memoryCacheService = memoryCacheService;
        private readonly ILogger<UserIdentificationDataService> _logger = logger;
        private readonly string _entityName = SystemConstants.UserIdentificationData.ENTITY;

        /// <summary>
        /// Validate User Id Data.
        /// </summary>
        /// <param name="documentDetailRequest"></param>
        /// <returns></returns>
        /// <exception cref="DataNotFoundException"></exception>
        public async Task<DocumentValidateResponse?> ValidateUserIdentificationData(DocumentDetailRequest documentDetailRequest)
        {
            DocumentValidateResponse documentValidateResponse = new();
            try
            {
                documentDetailRequest.DocumentNumber = CommonHelper.DecryptString(documentDetailRequest.DocumentNumber);

                var parameters = new DynamicParameters();
                parameters.Add(DBConstants.DocumentValidationResponse.DOCUMENT_NUMBER, documentDetailRequest.DocumentNumber, DbType.String, ParameterDirection.Input);

                documentValidateResponse = await _documentValidationResponseRepository.GetAsync(DBConstants.DocumentValidationResponse.DOCUMENT_VALIDATION_SP, parameters);
                
                if (documentValidateResponse?.UserId != null)
                {
                    _memoryCacheService.SetData(SystemConstants.General.USER_ID, Convert.ToString(documentValidateResponse?.UserId ?? 0));
                }

                return documentValidateResponse;
            }
            catch (Exception ex) 
            {
                _logger.LogError(ex.Message);

                documentValidateResponse.IsValid = false;
                documentValidateResponse.RejectReason = SystemConstants.General.ERROR_REJECT_REASON;
                documentValidateResponse.ErrorMessage = ex.Message;

                return documentValidateResponse;
            }
        }
    }
}
