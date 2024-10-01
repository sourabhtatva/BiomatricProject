using BiometricAuthenticationAPI.Data.Models;
using BiometricAuthenticationAPI.Helpers.Constants;
using BiometricAuthenticationAPI.Repositories.Interfaces;
using BiometricAuthenticationAPI.Services.Interfaces;
using Dapper;

namespace BiometricAuthenticationAPI.Services
{
    public class RecognitionLogService(IRecognitionLogRepository recognitionLogRepository, ILogger<RecognitionLogService> logger) : IRecognitionLogService
    {
        private readonly IRecognitionLogRepository _recognitionLogRepository = recognitionLogRepository;
        private readonly ILogger<RecognitionLogService> _logger = logger;
        private readonly string _entityName = SystemConstants.RecognitionLog.ENTITY;

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
                parameters.Add(DBConstants.RecognitionLog.SIMILARITY_LEVEL, recognitionLogData.SimilarityLevel);
                parameters.Add(DBConstants.RecognitionLog.STATUS, recognitionLogData.Status);

                return await _recognitionLogRepository.AddAsync(DBConstants.RecognitionLog.RECOGNITION_LOG_INSERT, parameters);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return null;
            }
        }
    }
}
