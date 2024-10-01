using Amazon.Rekognition;
using Amazon.Rekognition.Model;
using BiometricAuthenticationAPI.Controllers;
using BiometricAuthenticationAPI.Services.Interfaces;

namespace BiometricAuthenticationAPI.Services
{
    public class AwsFaceService(IConfiguration configuration, ILogger<AwsFaceService> logger) : IAwsFaceService
    {
        private readonly string _awsAccessKey = configuration["AwsFaceApi:AwsAccessKey"];
        private readonly string _awsSecretKey = configuration["AwsFaceApi:AwsSecretKey"];
        private readonly ILogger<AwsFaceService> _logger = logger;

        public async Task<CompareFacesResponse?> VerifyFacesAsync(CompareFacesRequest compareFacesRequest)
        {
            try
            {
                var awsCredentials = new Amazon.Runtime.BasicAWSCredentials(_awsAccessKey, _awsSecretKey);
                AmazonRekognitionClient rekognitionClient = new(awsCredentials, Amazon.RegionEndpoint.APSouth1);

                return await rekognitionClient.CompareFacesAsync(compareFacesRequest) ?? new CompareFacesResponse();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return null;
            }
        }
    }
}
