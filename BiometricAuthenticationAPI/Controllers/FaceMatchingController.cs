using Amazon.Rekognition.Model;
using Amazon.Rekognition;
using BiometricAuthenticationAPI.Data.Models;
using BiometricAuthenticationAPI.Data.Models.Response;
using BiometricAuthenticationAPI.Helpers.Constants;
using BiometricAuthenticationAPI.Helpers.Extensions;
using BiometricAuthenticationAPI.Helpers.Utils;
using BiometricAuthenticationAPI.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BiometricAuthenticationAPI.Controllers
{
    // <summary>
    /// Face Matching api controller.
    /// </summary>
    [Route("api/[controller]")]
    public class FaceMatchingController(IAzureFaceService azureFaceService,IRecognitionLogService recognitionLogService, ILogger<FaceMatchingController> logger) : ControllerBase
    {
        private readonly IAzureFaceService _azureFaceService = azureFaceService;
        private readonly IRecognitionLogService _recognitionLogService = recognitionLogService;
        private readonly ILogger<FaceMatchingController> _logger = logger;
        private readonly string _entityName = SystemConstants.FaceMatching.CONTROLLER_ENTITY;

        //[HttpPost("match")]
        //public async Task<IActionResult> MatchFaces([FromBody] MatchFacesRequest request)
        //{
        //    try
        //    {
        //        if (!ModelState.IsValid)
        //        {
        //            throw new DataValidationException(ModelState);
        //        }

        //        string faceId1 = await _azureFaceService.DetectFaceAsync(request.ScannedImage);
        //        string faceId2 = await _azureFaceService.DetectFaceAsync(request.ClickedImage);

        //        if (faceId1 == null || faceId2 == null)
        //        {
        //            return BadRequest(Messages.FaceMatching.General.DetectFailureMessage);
        //        }

        //        var response = await _azureFaceService.VerifyFacesAsync(faceId1, faceId2);
        //        if(response == null)
        //        {
        //            return BadRequest(Messages.FaceMatching.General.VerifyFailureMessage);
        //        }

        //        RecognitionLog recognitionLog = new()
        //        {
        //            ConfidenceLevel = response.Confidence,
        //            RecognitionTime = DateTime.Now,
        //            Status = response.IsIdentical ? SystemConstants.General.SUCCESS : SystemConstants.General.FAILURE
        //        };
        //        await _recognitionLogService.AddRecognitionLogData(recognitionLog);
        //        return this.SuccessResult(response, Messages.FaceMatching.General.FaceMatchingMessage(_entityName));
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, ex.Message);
        //        throw;
        //    }
        //}

        [HttpPost("match")]
        public async Task<IActionResult> MatchFacesAWS([FromBody] MatchFacesRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    throw new DataValidationException(ModelState);
                }
                var awsCredentials = new Amazon.Runtime.BasicAWSCredentials(SystemConstants.Configuration.AWS_ACCESS_KEY, SystemConstants.Configuration.AWS_SECRET_KEY);
                AmazonRekognitionClient rekognitionClient = new(awsCredentials, Amazon.RegionEndpoint.USEast1);

                float similarityThreshold = SystemConstants.Configuration.AWS_THRESHOLD_VALUE;

                Image imageSource = new();
                imageSource.Bytes = new MemoryStream(request.ScannedImage);
                Image imageTarget = new();
                imageTarget.Bytes = new MemoryStream(request.ClickedImage);


                CompareFacesRequest compareFacesRequest = new CompareFacesRequest()
                {
                    SourceImage = imageSource,
                    TargetImage = imageTarget,
                    SimilarityThreshold = similarityThreshold
                };

                CompareFacesResponse compareFacesResponse = await rekognitionClient.CompareFacesAsync(compareFacesRequest);

                FaceVerifyResponse response = new FaceVerifyResponse()
                {
                    Confidence = compareFacesResponse.FaceMatches.Count > 1 ? compareFacesResponse.FaceMatches.First().Similarity : 0,
                    IsIdentical = compareFacesResponse.FaceMatches.Count > 1 ? true : false
                };

                Data.Models.RecognitionLog recognitionLog = new()
                {
                    ConfidenceLevel = response.Confidence,
                    RecognitionTime = DateTime.Now,
                    Status = response.IsIdentical ? SystemConstants.General.SUCCESS : SystemConstants.General.FAILURE
                };
                await _recognitionLogService.AddRecognitionLogData(recognitionLog);
                return this.SuccessResult(response, Messages.FaceMatching.General.FaceMatchingMessage(_entityName));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                throw;
            }
        }

    }
}
