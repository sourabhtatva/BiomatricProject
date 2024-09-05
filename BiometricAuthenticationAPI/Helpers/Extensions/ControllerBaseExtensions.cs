using BiometricAuthenticationAPI.Data.Models.Common;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace BiometricAuthenticationAPI.Helpers.Extensions
{
    /// <summary>
    /// Controller base extension to setup result response.
    /// </summary>
    public static class ControllerBaseExtensions
    {
        public static ObjectResult SuccessResult(this ControllerBase controller, object? data, string? message = null)
        {
            var response = new BaseResponse
            {
                Data = data,
                Code = (int)HttpStatusCode.OK,
                IsSuccess = true,
                Message = message ?? string.Empty,
            };

            return controller.StatusCode((int)HttpStatusCode.OK, response);
        }
    }
}
