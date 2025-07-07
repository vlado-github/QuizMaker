using System.Text;
using FluentValidation.Results;
using Newtonsoft.Json;

namespace QuizMaker.API.Utils;

public static class FluentValidationErrorsExtension
{
    public static string ToLogMessage(this IEnumerable<ValidationFailure> errors)
    {
        var message = new StringBuilder();
        foreach (var error in errors)
        {
            message.Append(error.ErrorMessage);
        }
        return message.ToString();
    }

    public static string ToJson(this IEnumerable<ValidationFailure> errors)
    {
        var result = new Dictionary<string, string[]>();
        foreach (var error in errors)
        {
            result.Add(error.PropertyName, new string[] { error.ErrorMessage });
        }
        return JsonConvert.SerializeObject(result);
    }
}