using System.Net;

namespace QuizMaker.API.Middlewares.CustomResponses;

public class NotFoundErrorDetails : ErrorDetailsBase
{
    public NotFoundErrorDetails(string message)
        : base(RfcConsts.RfcNotFoundType, (int)HttpStatusCode.NotFound, message)
    {

    }
}