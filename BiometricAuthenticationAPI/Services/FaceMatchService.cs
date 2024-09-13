using Amazon.Rekognition.Model;
using BiometricAuthenticationAPI.Data.Models;
using BiometricAuthenticationAPI.Data.Models.Response;
using BiometricAuthenticationAPI.Helpers.Constants;
using BiometricAuthenticationAPI.Helpers.Utils;
using BiometricAuthenticationAPI.Services.Interfaces;

namespace BiometricAuthenticationAPI.Services
{
    public class FaceMatchService(IAwsFaceService awsFaceService, IRecognitionLogService recognitionLogService, IMemoryCacheService memoryCacheService) : IFaceMatchService
    {
        private readonly IAwsFaceService _awsFaceService = awsFaceService;
        private readonly IRecognitionLogService _recognitionLogService = recognitionLogService;
        private readonly IMemoryCacheService _memoryCacheService = memoryCacheService;

        public async Task<FaceVerifyResponse?> MatchFace(MatchFacesRequest matchFacesRequest)
        {
            try
            {
                FaceVerifyResponse response = new FaceVerifyResponse();

                matchFacesRequest.ScannedImage = CommonHelper.DecryptByteArray(matchFacesRequest.ScannedImage);
                matchFacesRequest.ClickedImage = CommonHelper.DecryptByteArray(matchFacesRequest.ClickedImage);

                Image imageSource = new();
                imageSource.Bytes = CommonHelper.ByteArrayToMemoryStream(matchFacesRequest.ScannedImage);
                Image imageTarget = new();
                imageTarget.Bytes = CommonHelper.ByteArrayToMemoryStream(matchFacesRequest.ClickedImage);

                CompareFacesRequest compareFacesRequest = new CompareFacesRequest()
                {
                    SourceImage = imageSource,
                    TargetImage = imageTarget,
                    SimilarityThreshold = SystemConstants.Configuration.AWS_THRESHOLD_VALUE
                };

                CompareFacesResponse? compareFacesResponse = await _awsFaceService.VerifyFacesAsync(compareFacesRequest);

                if (compareFacesResponse != null && compareFacesResponse.FaceMatches.Count > 0)
                {
                    response.Confidence = compareFacesResponse.FaceMatches.First().Face.Confidence;
                    response.Similarity = compareFacesResponse.FaceMatches.First().Similarity;
                    response.IsIdentical = compareFacesResponse.FaceMatches.Count > 0;
                }
                else if (compareFacesResponse == null)
                {
                    response.Confidence = 0.0F;
                    response.Similarity = 0.0F;
                    response.IsIdentical = false;
                    response.ApiFailedStatus = true;
                }
                else
                {
                    response.Confidence = 0.0F;
                    response.Similarity = 0.0F;
                    response.IsIdentical = false;
                }

                int userId = Convert.ToInt32(_memoryCacheService.GetData("UserId"));

                _memoryCacheService.RemoveData("UserId");

                RecognitionLog recognitionLog = new()
                {
                    ConfidenceLevel = response.Confidence,
                    SimilarityLevel = response.Similarity,
                    UserId = userId,
                    RecognitionTime = DateTime.Now,
                    Status = response.IsIdentical ? SystemConstants.General.SUCCESS : SystemConstants.General.FAILURE
                };

                await _recognitionLogService.AddRecognitionLogData(recognitionLog);

                return response;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }
    }
}
