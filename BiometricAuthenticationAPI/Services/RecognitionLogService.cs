using BiometricAuthenticationAPI.Data.Models;
using BiometricAuthenticationAPI.Helpers.Constants;
using BiometricAuthenticationAPI.Helpers.Utils;
using BiometricAuthenticationAPI.Repositories.Interfaces;
using BiometricAuthenticationAPI.Services.Interfaces;
using Dapper;

namespace BiometricAuthenticationAPI.Services
{
    public class RecognitionLogService(IRecognitionLogRepository recognitionLogRepository) : IRecognitionLogService
    {
        private readonly IRecognitionLogRepository _recognitionLogRepository = recognitionLogRepository;
        private readonly string _entityName = SystemConstants.RecognitionLog.ENTITY;

        /// <summary>
        /// Get all Recognition Log Data.
        /// </summary>
        /// <returns>List of Recognition Log Data</returns>
        public async Task<IEnumerable<RecognitionLog>> GetAllRecognitionLogData()
        {
            try
            {
                return await _recognitionLogRepository.GetAllAsync(DBConstants.RecognitionLog.GET_RECOGNITION_LOG_DATA, null);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return [];
            }
        }

        /// <summary>
        /// Get Recognition Log Data by id.
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        /// <exception cref="DataNotFoundException"></exception>
        public async Task<RecognitionLog> GetRecognitionLogData(int Id)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add(DBConstants.RecognitionLog.ID, Id);
                return await _recognitionLogRepository.GetAsync(DBConstants.RecognitionLog.GET_RECOGNITION_LOG_DATA_BY_ID, parameters) ?? throw new DataNotFoundException(Messages.UserIdentificationData.General.NotFoundMessage(_entityName));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }

        }

        /// <summary>
        /// Add Recognition Log Data
        /// </summary>
        /// <param name="recognitionLogData"></param>
        /// <returns>created user Face Data</returns>
        public async Task<int?> AddRecognitionLogData(RecognitionLog recognitionLogData)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add(DBConstants.RecognitionLog.USER_ID, recognitionLogData.UserId);
                parameters.Add(DBConstants.RecognitionLog.RECOGNITION_TIME, recognitionLogData.RecognitionTime);
                parameters.Add(DBConstants.RecognitionLog.CONFIDENCE_LEVEL, recognitionLogData.ConfidenceLevel);
                parameters.Add(DBConstants.RecognitionLog.STATUS, recognitionLogData.Status);

                return await _recognitionLogRepository.AddAsync(DBConstants.RecognitionLog.RECOGNITION_LOG_INSERT, parameters);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }

        /// <summary>
        /// Update Recognition Log Data.
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="recognitionLogData"></param>
        /// <returns>updated user Id Data</returns>
        public async Task<int?> UpdateRecognitionLogData(int Id, RecognitionLog recognitionLogData)
        {
            try
            {
                var searchParams = new DynamicParameters();
                searchParams.Add(DBConstants.RecognitionLog.ID, Id);
                RecognitionLog recognitionLog = await _recognitionLogRepository.GetAsync(DBConstants.RecognitionLog.GET_RECOGNITION_LOG_DATA_BY_ID, searchParams);
                if (recognitionLog == null)
                {
                    throw new DataNotFoundException(Messages.RecognitionLog.General.NotFoundMessage(_entityName));
                }

                var parameters = new DynamicParameters();
                parameters.Add(DBConstants.RecognitionLog.USER_ID, recognitionLogData.UserId);
                parameters.Add(DBConstants.RecognitionLog.RECOGNITION_TIME, recognitionLogData.RecognitionTime);
                parameters.Add(DBConstants.RecognitionLog.CONFIDENCE_LEVEL, recognitionLogData.ConfidenceLevel);
                parameters.Add(DBConstants.RecognitionLog.STATUS, recognitionLogData.Status);

                return await _recognitionLogRepository.UpdateAsync(DBConstants.RecognitionLog.RECOGNITION_LOG_UPDATE, parameters);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
            
        }

        /// <summary>
        /// Delete Recognition Log Data.
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        /// <exception cref="DataNotFoundException"></exception>
        public async Task<int?> DeleteRecognitionLogData(int Id)
        {
            try
            {
                var searchParams = new DynamicParameters();
                searchParams.Add(DBConstants.RecognitionLog.ID, Id);
                RecognitionLog recognitionLog = await _recognitionLogRepository.GetAsync(DBConstants.RecognitionLog.GET_RECOGNITION_LOG_DATA_BY_ID, searchParams);
                if (recognitionLog == null)
                {
                    throw new DataNotFoundException(Messages.RecognitionLog.General.NotFoundMessage(_entityName));
                }
                var parameters = new DynamicParameters();
                parameters.Add(DBConstants.RecognitionLog.ID, Id);
                return await _recognitionLogRepository.DeleteAsync(DBConstants.RecognitionLog.RECOGNITION_LOG_DELETE, parameters);
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
            
        }
    }
}
