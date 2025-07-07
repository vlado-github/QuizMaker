using Mapster;
using Microsoft.EntityFrameworkCore;
using QuizMaker.Database;
using QuizMaker.Database.Entities;
using QuizMaker.Domain.Base;
using QuizMaker.Domain.Exceptions;
using QuizMaker.Domain.QuizFeature.Commands;

namespace QuizMaker.Domain.QuizFeature;

public interface IQuizCommandHandler
{
    Task<long> Handle(CreateQuizCommand command);
    Task Handle(UpdateQuizCommand command);
    Task Handle(DeleteQuizCommand command);
}

internal class QuizCommandHandler : IQuizCommandHandler
{
    private readonly IUnitOfWork<QuizMakerContext> _unitOfWork;
    
    public QuizCommandHandler(IUnitOfWork<QuizMakerContext> unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }
    
    public async Task<long> Handle(CreateQuizCommand command)
    {
        var quiz = command.Adapt<Quiz>();
        await _unitOfWork.ExecuteAsync(async (dbContext) =>
        {
            var questions = command.Questions.Where(x => x.Id is null or 0).Adapt<IList<Question>>();
            foreach (var commandQuestion in command.Questions.Where(x => x.Id.HasValue).ToList())
            {
                // case: question added from the pool
                var question = dbContext.Questions.SingleOrDefault(x => x.Id == commandQuestion.Id);
                if (question == null)
                {
                    throw new RecordNotFoundException(commandQuestion.Id.Value, typeof(Question));
                }
                questions.Add(question);
            }
            quiz.Questions = questions;
            await dbContext.Quizzes.AddAsync(quiz);
        });
        
        return quiz.Id;
    }
    
    public async Task Handle(UpdateQuizCommand command)
    {
        await _unitOfWork.ExecuteAsync(async (dbContext) =>
        {
            var quiz = await dbContext.Quizzes
                .Include(x => x.Questions)
                .SingleOrDefaultAsync(x => x.Id == command.Id);
            if (quiz == null)
            {
                throw new RecordNotFoundException(command.Id, typeof(Quiz));
            }

            quiz.Name = command.Name;

            if (command.Questions == null || !command.Questions.Any())
            {
                quiz.Questions.Clear();
                return;
            }
            
            var addedAndUpdatedQuestions = command.Questions.Where(x => x.Id is null or 0).Adapt<List<Question>>();
            foreach (var commandQuestion in command.Questions.Where(x => x.Id.HasValue).ToList())
            {
                var existingQuestion = quiz.Questions.SingleOrDefault(x => x.Id == commandQuestion.Id);
                
                // case: question not contained in a quiz, added from the pool
                if (existingQuestion == null)
                {
                    var question = dbContext.Questions.SingleOrDefault(x => x.Id == commandQuestion.Id);
                    if (question == null)
                    {
                        throw new RecordNotFoundException(commandQuestion.Id.Value, typeof(Question));
                    }
                    addedAndUpdatedQuestions.Add(question);
                    continue;
                }

                // case: question contained in a quiz, updated
                existingQuestion.QuestionPhrase = commandQuestion.QuestionPhrase;
                existingQuestion.CorrectAnswer = commandQuestion.CorrectAnswer;
                addedAndUpdatedQuestions.Add(existingQuestion);
            }
            
            quiz.Questions.Clear();
            quiz.Questions = addedAndUpdatedQuestions;
        });
    }

    public async Task Handle(DeleteQuizCommand command)
    {
        await _unitOfWork.ExecuteAsync(async (dbContext) =>
        {
            var quiz = await dbContext.Quizzes.SingleOrDefaultAsync(x => x.Id == command.QuizId);
            if (quiz != null)
            {
                dbContext.Remove(quiz);
            }
        });
    }
}