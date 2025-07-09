using System.Net;

namespace QuizMaker.API.Middlewares.CustomResponses;

public class ServerErrorDetails : ErrorDetailsBase
{
    public ServerErrorDetails(string message)
        : base(RfcConsts.RfcInternalServerType, (int)HttpStatusCode.InternalServerError, message)
    {

    }
}