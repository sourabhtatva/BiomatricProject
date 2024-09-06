using Amazon.Rekognition;
using Amazon.Rekognition.Model;
using BiometricAuthenticationAPI.Data.Models.Response;
using BiometricAuthenticationAPI.Helpers.Constants;
using BiometricAuthenticationAPI.Services.Interfaces;

namespace BiometricAuthenticationAPI.Services
{
    public class AwsFaceService : IAwsFaceService
    {
        public async Task<CompareFacesResponse?> VerifyFacesAsync(CompareFacesRequest compareFacesRequest)
        {
            try
            {
                var awsCredentials = new Amazon.Runtime.BasicAWSCredentials(SystemConstants.Configuration.AWS_ACCESS_KEY, SystemConstants.Configuration.AWS_SECRET_KEY);
                AmazonRekognitionClient rekognitionClient = new(awsCredentials, Amazon.RegionEndpoint.APSouth1);

                return await rekognitionClient.CompareFacesAsync(compareFacesRequest) ?? new CompareFacesResponse();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }
    }
}
