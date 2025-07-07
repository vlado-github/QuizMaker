using Microsoft.AspNetCore.Mvc;
using QuizMaker.Database.Entities;
using QuizMaker.Domain.Base;
using QuizMaker.Domain.QuizFeature;
using QuizMaker.Domain.QuizFeature.Commands;

namespace QuizMaker.API.Controllers;

[ApiController]
[Route("[controller]")]
public class QuizController : ControllerBase
{
    private readonly ILogger<QuizController> _logger;
    private readonly IQuizQueryDispatcher _quizQueryDispatcher;
    private readonly IQuizCommandHandler _quizCommandHandler;

    public QuizController(ILogger<QuizController> logger, 
        IQuizQueryDispatcher quizQueryDispatcher,
        IQuizCommandHandler quizCommandHandler)
    {
        _logger = logger;
        _quizQueryDispatcher = quizQueryDispatcher;
        _quizCommandHandler = quizCommandHandler;
    }

    /// <summary>
    /// Fetch paginated quiz items. ItemsPerPage limit is 1000.
    /// </summary>
    /// <param name="query"></param>
    /// <returns></returns>
    [HttpGet]
    public async Task<IList<Quiz>> Get([FromQuery] PagedQueryParamBase query)
    {
        return await _quizQueryDispatcher.GetQuizzes(query);
    }
    
    /// <summary>
    /// Get quiz details with questions content.
    /// </summary>
    /// <param name="quizId"></param>
    /// <returns></returns>
    [HttpGet("{quizId}")]
    public async Task<Quiz> Get([FromRoute] long quizId)
    {
        return await _quizQueryDispatcher.GetQuiz(quizId);
    }
    
    /// <summary>
    /// Create a new quiz with questions.
    /// </summary>
    /// <param name="quizId"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task<long> Create(CreateQuizCommand command)
    {
        return await _quizCommandHandler.Handle(command);
    }
    
    /// <summary>
    /// Update an existing quiz with questions.
    /// </summary>
    /// <param name="quizId"></param>
    /// <returns></returns>
    [HttpPut]
    public async Task Update(UpdateQuizCommand command)
    {
        await _quizCommandHandler.Handle(command);
    }
    
    /// <summary>
    /// Delete an existing quiz.
    /// </summary>
    /// <param name="quizId"></param>
    /// <returns></returns>
    [HttpDelete("{quizId}")]
    public async Task Delete([FromRoute] long quizId)
    {
        await _quizCommandHandler.Handle(new DeleteQuizCommand(quizId));
    }
}