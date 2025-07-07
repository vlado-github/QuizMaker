using System.Net;
using FluentValidation.Results;
using QuizMaker.API.Utils;

namespace QuizMaker.API.Middlewares.CustomResponses;

public class CustomValidationErrorDetails : ErrorDetailsBase
{
    public CustomValidationErrorDetails(string message)
        : base(RfcConsts.RfcBadRequestType, (int)HttpStatusCode.BadRequest, message)
    {
    }

    public CustomValidationErrorDetails(string message, IEnumerable<ValidationFailure> errors)
        : base(RfcConsts.RfcBadRequestType, (int)HttpStatusCode.BadRequest, message)
    {
        Errors = errors.ToJson();
    }

    public string Errors { get; private set; }
}