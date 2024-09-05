using BiometricAuthenticationAPI.Data.Models;
using BiometricAuthenticationAPI.Data.Models.Response;
using BiometricAuthenticationAPI.Helpers.Constants;
using BiometricAuthenticationAPI.Helpers.Enums;
using BiometricAuthenticationAPI.Helpers.Utils;
using BiometricAuthenticationAPI.Repositories.Interfaces;
using BiometricAuthenticationAPI.Services.Interfaces;
using Dapper;

namespace BiometricAuthenticationAPI.Services
{
    public class UserIdentificationDataService(IUserIdentificationDataRepository userIdentificationDataRepository, IUserIdentificationTypeService userIdentificationTypeService, IMemoryCacheService memoryCacheService) : IUserIdentificationDataService
    {
        private readonly IUserIdentificationDataRepository _userIdentificationDataRepository = userIdentificationDataRepository;
        private readonly IUserIdentificationTypeService _userIdentificationTypeService = userIdentificationTypeService;
        private readonly IMemoryCacheService _memoryCacheService = memoryCacheService;
        private readonly string _entityName = SystemConstants.UserIdentificationData.ENTITY;

        /// <summary>
        /// Get all user Id Data.
        /// </summary>
        /// <returns>List of user Id Data</returns>
        public async Task<IEnumerable<UserIdentificationData>> GetAllUserIdentificationData()
        {
            try
            {
                return await _userIdentificationDataRepository.GetAllAsync(DBConstants.UserIdentificationData.GET_USER_IDENTIFICATION_DATA, null);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return [];
            }
        }

        /// <summary>
        /// Get user Id Data by id.
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        /// <exception cref="DataNotFoundException"></exception>
        public async Task<UserIdentificationData> GetUserIdentificationData(int Id)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add(DBConstants.UserIdentificationData.ID, Id);
                return await _userIdentificationDataRepository.GetAsync(DBConstants.UserIdentificationData.GET_USER_IDENTIFICATION_DATA_BY_ID, parameters) ?? throw new DataNotFoundException(Messages.UserIdentificationData.General.NotFoundMessage(_entityName));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }

        /// <summary>
        /// Add User Id Data
        /// </summary>
        /// <param name="userData"></param>
        /// <returns>created user Id Data</returns>
        public async Task<int?> InsertUserIdentificationData(UserIdentificationData userData)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add(DBConstants.UserIdentificationData.FIRST_NAME, userData.FirstName);
                parameters.Add(DBConstants.UserIdentificationData.LAST_NAME, userData.LastName);
                parameters.Add(DBConstants.UserIdentificationData.EMAIL, userData.Email);
                parameters.Add(DBConstants.UserIdentificationData.PHONE_NUMBER, userData.PhoneNumber);
                parameters.Add(DBConstants.UserIdentificationData.USER_ID_NUMBER, userData.UserIdNumber);
                parameters.Add(DBConstants.UserIdentificationData.USER_ID_TYPE, userData.UserIdType);
                parameters.Add(DBConstants.UserIdentificationData.USER_IMAGE, userData.UserImage);
                parameters.Add(DBConstants.UserIdentificationData.IS_BLACKLIST_USER, userData.IsBlacklistUser);
                parameters.Add(DBConstants.UserIdentificationData.ADDRESS, userData.Address);
                parameters.Add(DBConstants.UserIdentificationData.CITY, userData.City);
                parameters.Add(DBConstants.UserIdentificationData.STATE, userData.State);
                parameters.Add(DBConstants.UserIdentificationData.ZIP_CODE, userData.ZipCode);

                return await _userIdentificationDataRepository.AddAsync(DBConstants.UserIdentificationData.USER_IDENTIFICATION_DATA_INSERT, parameters);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }

        /// <summary>
        /// Update User Id Data.
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="userData"></param>
        /// <returns>updated user Id Data</returns>
        public async Task<int?> UpdateUserIdentificationData(int Id, UserIdentificationData userData)
        {
            try
            {
                var searchParams = new DynamicParameters();
                searchParams.Add(DBConstants.UserIdentificationData.ID, Id);
                UserIdentificationData userIdentificationData = await _userIdentificationDataRepository.GetAsync(DBConstants.UserIdentificationData.GET_USER_IDENTIFICATION_DATA_BY_ID, searchParams);
                if (userIdentificationData == null)
                {
                    throw new DataNotFoundException(Messages.UserIdentificationData.General.NotFoundMessage(_entityName));
                }

                var parameters = new DynamicParameters();
                parameters.Add(DBConstants.UserIdentificationData.FIRST_NAME, userData.FirstName);
                parameters.Add(DBConstants.UserIdentificationData.LAST_NAME, userData.LastName);
                parameters.Add(DBConstants.UserIdentificationData.EMAIL, userData.Email);
                parameters.Add(DBConstants.UserIdentificationData.PHONE_NUMBER, userData.PhoneNumber);
                parameters.Add(DBConstants.UserIdentificationData.USER_ID_NUMBER, userData.UserIdNumber);
                parameters.Add(DBConstants.UserIdentificationData.USER_ID_TYPE, userData.UserIdType);
                parameters.Add(DBConstants.UserIdentificationData.USER_IMAGE, userData.UserImage);
                parameters.Add(DBConstants.UserIdentificationData.IS_BLACKLIST_USER, userData.IsBlacklistUser);
                parameters.Add(DBConstants.UserIdentificationData.ADDRESS, userData.Address);
                parameters.Add(DBConstants.UserIdentificationData.CITY, userData.City);
                parameters.Add(DBConstants.UserIdentificationData.STATE, userData.State);
                parameters.Add(DBConstants.UserIdentificationData.ZIP_CODE, userData.ZipCode);

                return await _userIdentificationDataRepository.UpdateAsync(DBConstants.UserIdentificationData.USER_IDENTIFICATION_DATA_UPDATE, parameters);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }

        /// <summary>
        /// Delete User Id Data.
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        /// <exception cref="DataNotFoundException"></exception>
        public async Task<int?> DeleteUserIdentificationData(int Id)
        {
            try
            {
                var searchParams = new DynamicParameters();
                searchParams.Add(DBConstants.UserIdentificationData.ID, Id);
                UserIdentificationData userIdentificationData = await _userIdentificationDataRepository.GetAsync(DBConstants.UserIdentificationData.GET_USER_IDENTIFICATION_DATA_BY_ID, searchParams);
                if (userIdentificationData == null)
                {
                    throw new DataNotFoundException(Messages.UserIdentificationData.General.NotFoundMessage(_entityName));
                }
                var parameters = new DynamicParameters();
                parameters.Add(DBConstants.UserIdentificationData.ID, Id);
                return await _userIdentificationDataRepository.DeleteAsync(DBConstants.UserIdentificationData.USER_IDENTIFICATION_DATA_DELETE, parameters);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);  
                return null;
            }
        }

        /// <summary>
        /// Validate User Id Data.
        /// </summary>
        /// <param name="documentDetailRequest"></param>
        /// <returns></returns>
        /// <exception cref="DataNotFoundException"></exception>
        public async Task<DocumentValidateResponse> ValidateUserIdentificationData(DocumentDetailRequest documentDetailRequest)
        {
            try
            {
                DocumentValidateResponse documentValidateResponse = new()
                {
                    IsValid = true,
                    RejectReason = RejectReason.None
                };

                var userIdentificationData = await _userIdentificationDataRepository.GetAllAsync(DBConstants.UserIdentificationData.GET_USER_IDENTIFICATION_DATA) ?? throw new DataNotFoundException(Messages.UserIdentificationData.General.NotFoundMessage(_entityName)); ;

                var userIdentificationType = await _userIdentificationTypeService.GetAllUserIdentificationType() ?? throw new DataNotFoundException(Messages.UserIdentificationType.General.NotFoundMessage(_entityName));

                var userIdData = userIdentificationData.FirstOrDefault(i => i.UserIdNumber == documentDetailRequest.DocumentNumber);

                if(userIdData != null)
                {
                    _memoryCacheService.SetData("UserId", Convert.ToString(userIdData.Id));
                }

                if (!userIdentificationData.Any(i => i.UserIdNumber == documentDetailRequest.DocumentNumber))
                {
                    documentValidateResponse.IsValid = false;
                    documentValidateResponse.RejectReason = RejectReason.DocumentNotFound;
                    return documentValidateResponse;
                }
                if (!userIdentificationType.Any(i => i.Type == documentDetailRequest.DocumentType))
                {
                    documentValidateResponse.IsValid = false;
                    documentValidateResponse.RejectReason = RejectReason.InvalidDocumentType;
                    return documentValidateResponse;
                }
                if (userIdentificationData.Any(i => i.UserIdNumber == documentDetailRequest.DocumentNumber && i.IsBlacklistUser))
                {
                    documentValidateResponse.IsValid = false;
                    documentValidateResponse.RejectReason = RejectReason.PassengerIsBlackListed;
                    return documentValidateResponse;
                }

                return documentValidateResponse;
            }
            catch (Exception ex) 
            { 
                Console.WriteLine(ex.Message);
                DocumentValidateResponse documentValidateResponse = new()
                {
                    IsValid = false,
                    RejectReason = RejectReason.GeneralError
                };
                return documentValidateResponse;
            }
        }
    }
}
