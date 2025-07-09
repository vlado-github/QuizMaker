using Mapster;
using Microsoft.EntityFrameworkCore;
using QuizMaker.Database;
using QuizMaker.Database.Entities;
using QuizMaker.Domain.Base;
using QuizMaker.Domain.Exceptions;
using QuizMaker.Domain.Features.QuizBuilder.QueryResults;

namespace QuizMaker.Domain.Features.QuizBuilder;

public interface IQuizQueryDispatcher
{
    Task<IList<QuizSimpleResult>> GetQuizzes(PagedQueryParamBase query);
    Task<Quiz> GetQuiz(long quizId);
    Task<IList<Question>> GetQuestions(SearchPagedQueryParam query);
}

public class QuizQueryDispatcher : IQuizQueryDispatcher
{
    private readonly QuizMakerContext _dbContext;
    
    public QuizQueryDispatcher(QuizMakerContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IList<QuizSimpleResult>> GetQuizzes(PagedQueryParamBase query)
    {
        return await _dbContext.Quizzes
            .Skip(query.Skip)
            .Take(query.Take)
            .ProjectToType<QuizSimpleResult>()
            .ToListAsync();
    }

    public async Task<Quiz> GetQuiz(long quizId)
    {
        var quiz = await _dbContext.Quizzes
            .Include(x => x.Questions)
            .SingleOrDefaultAsync(x => x.Id == quizId);
        if (quiz == null)
        {
            throw new RecordNotFoundException(quizId, typeof(Quiz));
        }

        return quiz;
    }

    public async Task<IList<Question>> GetQuestions(SearchPagedQueryParam query)
    {
        if (string.IsNullOrEmpty(query.Search))
        {
            return await _dbContext.Questions
                .Skip(query.Skip)
                .Take(query.Take)
                .ToListAsync();
        }
        return await _dbContext.Questions
            .Where(x => x.QuestionPhrase.Contains(query.Search))
            .Skip(query.Skip)
            .Take(query.Take)
            .ToListAsync();
    }
}