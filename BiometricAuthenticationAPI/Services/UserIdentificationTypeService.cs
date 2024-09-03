using BiometricAuthenticationAPI.Data.Models;
using BiometricAuthenticationAPI.Helpers.Constants;
using BiometricAuthenticationAPI.Helpers.Utils;
using BiometricAuthenticationAPI.Repositories.Interfaces;
using BiometricAuthenticationAPI.Services.Interfaces;
using Dapper;

namespace BiometricAuthenticationAPI.Services
{
    public class UserIdentificationTypeService(IUserIdentificationTypeRepository userIdentificationTypeRepository) : IUserIdentificationTypeService
    {
        private readonly IUserIdentificationTypeRepository _userIdentificationTypeRepository = userIdentificationTypeRepository;
        private readonly string _entityName = SystemConstants.UserIdentificationType.ENTITY;

        /// <summary>
        /// Get all user Type Data.
        /// </summary>
        /// <returns>List of user Type Data</returns>
        public async Task<IEnumerable<UserIdentificationType>> GetAllUserIdentificationType()
        {
            try
            {
                return await _userIdentificationTypeRepository.GetAllAsync(DBConstants.UserIdentificationType.GET_USER_IDENTIFICATION_TYPE, null);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return [];
            }
        }

        /// <summary>
        /// Get user Type Data by id.
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        /// <exception cref="DataNotFoundException"></exception>
        public async Task<UserIdentificationType> GetUserIdentificationType(int Id)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add(DBConstants.UserIdentificationType.ID, Id);
                return await _userIdentificationTypeRepository.GetAsync(DBConstants.UserIdentificationType.GET_USER_IDENTIFICATION_TYPE_BY_ID, parameters) ?? throw new DataNotFoundException(Messages.UserIdentificationType.General.NotFoundMessage(_entityName));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }

        /// <summary>
        /// Get user Type Data by type.
        /// </summary>
        /// <param name="Type"></param>
        /// <returns></returns>
        /// <exception cref="DataNotFoundException"></exception>
        public async Task<UserIdentificationType> GetUserIdentificationTypeByType(string Type)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add(DBConstants.UserIdentificationType.DOCUMENT_TYPE, Type);
                return await _userIdentificationTypeRepository.GetAsync(DBConstants.UserIdentificationType.GET_USER_IDENTIFICATION_TYPE_BY_TYPE, parameters) ?? throw new DataNotFoundException(Messages.UserIdentificationType.General.NotFoundMessage(_entityName));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }
    }
}
