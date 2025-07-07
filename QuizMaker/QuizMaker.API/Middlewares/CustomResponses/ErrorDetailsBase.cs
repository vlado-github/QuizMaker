using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace QuizMaker.API.Middlewares.CustomResponses;

public class ErrorDetailsBase
{
    public ErrorDetailsBase(string type, int status, string title)
    {
        Type = type;
        Status = status;
        Title = title;
    }

    public string Type { get; private set; }
    public int Status { get; private set; }
    public string Title { get; private set; }

    public override string ToString()
    {
        var serializerSettings = new JsonSerializerSettings();
        serializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
        return JsonConvert.SerializeObject(this, serializerSettings);
    }
}