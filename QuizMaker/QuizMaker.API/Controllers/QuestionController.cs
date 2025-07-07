using Microsoft.AspNetCore.Mvc;
using QuizMaker.Database.Entities;
using QuizMaker.Domain.Base;
using QuizMaker.Domain.QuizFeature;

namespace QuizMaker.API.Controllers;

[ApiController]
[Route("[controller]")]
public class QuestionController
{
    private readonly ILogger<QuizController> _logger;
    private readonly IQuizQueryDispatcher _quizQueryDispatcher;

    public QuestionController(ILogger<QuizController> logger, 
        IQuizQueryDispatcher quizQueryDispatcher)
    {
        _logger = logger;
        _quizQueryDispatcher = quizQueryDispatcher;
    }

    /// <summary>
    /// Search and fetch paginated question items. ItemsPerPage limit is 1000.
    /// </summary>
    /// <param name="query"></param>
    /// <returns></returns>
    [HttpGet]
    public async Task<IList<Question>> Get([FromQuery] SearchPagedQueryParam query)
    {
        return await _quizQueryDispatcher.GetQuestions(query);
    }
}