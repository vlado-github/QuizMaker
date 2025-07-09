using System.Net;

namespace QuizMaker.API.Middlewares.CustomResponses;

public class UnauthorizedErrorDetails : ErrorDetailsBase
{
    public UnauthorizedErrorDetails(string message)
        : base(RfcConsts.RfcUnauthorizedType, (int)HttpStatusCode.Unauthorized, message)
    {

    }
}