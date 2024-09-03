using BiometricAuthenticationAPI.Data.Models.Common;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Net;

namespace BiometricAuthenticationAPI.Helpers.Utils
{
    /// <summary>
    /// Data validation exception.
    /// </summary>
    public class DataValidationException : Exception
    {
        public HttpStatusCode StatusCode { get; private set; }
        public IList<ValidationDetails> Error { get; private set; }

        public DataValidationException(ModelStateDictionary excpetion, string message = "") : base(message)
        {
            StatusCode = HttpStatusCode.BadRequest;
            Error = excpetion.Select(ex => new ValidationDetails { InputName = ex.Key, ValidationMessage = ex.Value?.Errors.FirstOrDefault()?.ErrorMessage }).ToList();
        }
    }

    /// <summary>
    /// Data not found exception.
    /// </summary>
    public class DataNotFoundException(string message) : Exception(message)
    {
        public HttpStatusCode StatusCode { get; private set; } = HttpStatusCode.NoContent;
        public string Message { get; private set; } = message;
    }

    /// <summary>
    /// Data conflict exception.
    /// </summary>
    public class DataConflictException(string message) : Exception(message)
    {
        public HttpStatusCode StatusCode { get; private set; } = HttpStatusCode.Conflict;
        public string Message { get; private set; } = message;
    }
}
