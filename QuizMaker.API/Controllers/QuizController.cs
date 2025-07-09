using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using QuizMaker.Database.Entities;
using QuizMaker.Domain.Base;
using QuizMaker.Domain.Features.QuizBuilder;
using QuizMaker.Domain.Features.QuizBuilder.Commands;
using QuizMaker.Domain.Features.QuizBuilder.QueryResults;
using QuizMaker.Domain.Features.QuizExporter;
using QuizMaker.Domain.Features.QuizExporter.Commands;
using QuizMaker.Shared;

namespace QuizMaker.API.Controllers;

[ApiController]
[Route("[controller]")]
public class QuizController : ControllerBase
{
    private readonly ILogger<QuizController> _logger;
    private readonly IQuizQueryDispatcher _quizQueryDispatcher;
    private readonly IQuizCommandHandler _quizCommandHandler;
    private readonly IExporterCommandHandler _exporterCommandHandler;
    private readonly IExporterQueryDispatcher _exporterQueryDispatcher;

    public QuizController(ILogger<QuizController> logger, 
        IQuizQueryDispatcher quizQueryDispatcher,
        IQuizCommandHandler quizCommandHandler,
        IExporterCommandHandler exporterCommandHandler,
        IExporterQueryDispatcher exporterQueryDispatcher)
    {
        _logger = logger;
        _quizQueryDispatcher = quizQueryDispatcher;
        _quizCommandHandler = quizCommandHandler;
        _exporterCommandHandler = exporterCommandHandler;
        _exporterQueryDispatcher = exporterQueryDispatcher;
    }

    /// <summary>
    /// Fetch paginated quiz items. ItemsPerPage limit is 1000.
    /// </summary>
    /// <param name="query"></param>
    /// <returns></returns>
    [HttpGet]
    public async Task<IList<QuizSimpleResult>> Get([FromQuery] PagedQueryParamBase query)
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

    /// <summary>
    /// Exports quiz questions to a file. Designated file format is required (e.g. text/csv).
    /// </summary>
    /// <param name="command"></param>
    /// <returns></returns>
    [HttpPost("export")]
    public async Task<FileResult> Export(ExportQuizCommand command)
    {
        var fileType = command.FileType.ToLower();
        var stream = await _exporterCommandHandler.Handle(command);
        var provider = new FileExtensionContentTypeProvider();
        var extensions = provider.Mappings
            .Where(kvp => kvp.Value.Equals(fileType, StringComparison.OrdinalIgnoreCase))
            .Select(kvp => kvp.Key)
            .ToList();
        return File(stream, fileType, $"result.{extensions.First()}");
    }
    
    /// <summary>
    /// Supported export file formats.
    /// </summary>
    /// <returns></returns>
    [HttpGet("export/supported")]
    public IList<string> Export()
    {
        return _exporterQueryDispatcher.GetSupportedExportFormats();
    }
}