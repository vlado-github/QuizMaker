using Alba;
using Mapster;
using Microsoft.EntityFrameworkCore;
using QuizMaker.Database;
using QuizMaker.Database.Entities;
using QuizMaker.Domain.Dtos;
using QuizMaker.Domain.Features.QuizBuilder.Commands;
using QuizMaker.Tests.Base;

namespace QuizMaker.Tests.IntegrationTests;

public class UpdateQuestionTests :  IClassFixture<IntegrationTestFixture>
{
    private readonly IAlbaHost _host;
    private readonly TestSeeder _seeder;
    private readonly QuizMakerContext _dbContext;
    
    public UpdateQuestionTests(IntegrationTestFixture fixture)
    {
        _host = fixture.Host;
        _seeder = fixture.Seeder;
        _dbContext = fixture.DatabaseContext;
    }
    
    [Fact]
    public async Task When_User_Updates_Quiz_Name_Should_Succeed()
    {
        //Arrange
        _seeder.Include<Quiz>(1).Include<Question>(3);
        var quiz = _seeder.GetItems<Quiz>().First();
        var command = new UpdateQuizCommand(
            quiz.Id, 
            "TestQuizUpdated", 
            quiz.Questions.Select(x => x.Adapt<QuestionDto>()).ToList());
        
        //Act
        await _host.Scenario(config =>
        {
            config.Put.Json(command).ToUrl("/quiz");
            config.StatusCodeShouldBeOk();
        });

        //Assert
        var actualQuiz = _dbContext.Quizzes
            .AsNoTracking()
            .Include(x => x.Questions)
            .SingleOrDefault(x => x.Id == command.Id);
        Assert.NotNull(actualQuiz);
        Assert.NotNull(actualQuiz.Questions);
        Assert.NotEmpty(actualQuiz.Questions);
        Assert.Equal(actualQuiz.Name, command.Name);
        Assert.Equal(actualQuiz.Questions.Count, command.Questions.Count);
        foreach (var question in actualQuiz.Questions)
        {
            Assert.Contains(command.Questions, q => q.QuestionPhrase == question.QuestionPhrase);
            Assert.Contains(command.Questions, q => q.CorrectAnswer == question.CorrectAnswer);
        }
    }
    
    
    [Fact]
    public async Task When_User_Updates_Quiz_AddsFromQuestionPool_Should_Succeed()
    {
        //Arrange
        var numberOfQuestionsPerQuiz = 5;
        _seeder.Include<Quiz>(2).Include<Question>(numberOfQuestionsPerQuiz);
        var quiz = _seeder.GetItems<Quiz>().Last();
        var questionFromPool = _seeder
            .GetItems<Question>()
            .First(x => !x.Quizzes.Select(q => q.Id).Contains(quiz.Id));

        var commandQuestions = quiz.Questions.Select(x => x.Adapt<QuestionDto>()).ToList();
        commandQuestions.Add(new QuestionDto()
        {
            Id = questionFromPool.Id
        });
        var command = new UpdateQuizCommand(quiz.Id, "TestQuiz", commandQuestions);
        
        //Act
        await _host.Scenario(config =>
        {
            config.Put.Json(command).ToUrl("/quiz");
            config.StatusCodeShouldBeOk();
        });

        //Assert
        var actualQuiz = _dbContext.Quizzes
            .AsNoTracking()
            .Include(x => x.Questions)
            .SingleOrDefault(x => x.Id == command.Id);
        Assert.NotNull(actualQuiz);
        Assert.NotNull(actualQuiz.Questions);
        Assert.NotEmpty(actualQuiz.Questions);
        Assert.Equal(numberOfQuestionsPerQuiz + 1, actualQuiz.Questions.Count);
        foreach (var question in actualQuiz.Questions)
        {
            if (question.Id == questionFromPool.Id)
            {
                Assert.Contains(question.QuestionPhrase, questionFromPool.QuestionPhrase);
                Assert.Contains(question.CorrectAnswer, questionFromPool.CorrectAnswer);
            }
            else
            {
                Assert.Contains(question.QuestionPhrase,
                    command.Questions.Select(x => x.QuestionPhrase));
                Assert.Contains(question.CorrectAnswer,
                    command.Questions.Select(x => x.CorrectAnswer));
            }
        }
    }
    
    [Fact]
    public async Task When_User_Updates_Quiz_AddsNewQuestion_Should_Succeed()
    {
        //Arrange
        var numberOfQuestionsPerQuiz = 5;
        _seeder.Include<Quiz>(1).Include<Question>(numberOfQuestionsPerQuiz);
        var quiz = _seeder.GetItems<Quiz>().Last();
        var newQuestion = new QuestionDto()
        {
            QuestionPhrase = "new question - phrase",
            CorrectAnswer = "new question - answer"
        };
        var commandQuestions = quiz.Questions.Select(x => x.Adapt<QuestionDto>()).ToList();
        commandQuestions.Add(newQuestion);
        var command = new UpdateQuizCommand(quiz.Id, "TestQuiz", commandQuestions);
        
        //Act
        await _host.Scenario(config =>
        {
            config.Put.Json(command).ToUrl("/quiz");
            config.StatusCodeShouldBeOk();
        });

        //Assert
        var actualQuiz = _dbContext.Quizzes
            .AsNoTracking()
            .Include(x => x.Questions)
            .SingleOrDefault(x => x.Id == command.Id);
        Assert.NotNull(actualQuiz);
        Assert.NotNull(actualQuiz.Questions);
        Assert.NotEmpty(actualQuiz.Questions);
        Assert.Equal(numberOfQuestionsPerQuiz + 1, actualQuiz.Questions.Count);
        foreach (var question in actualQuiz.Questions)
        {
            Assert.Contains(question.QuestionPhrase,
                command.Questions.Select(x => x.QuestionPhrase));
            Assert.Contains(question.CorrectAnswer,
                command.Questions.Select(x => x.CorrectAnswer));
        }
    }
    
    [Fact]
    public async Task When_User_Updates_Quiz_UpdatesExistingQuestion_Should_Succeed()
    {
        //Arrange
        var numberOfQuestionsPerQuiz = 5;
        _seeder.Include<Quiz>(1).Include<Question>(numberOfQuestionsPerQuiz);
        var quiz = _seeder.GetItems<Quiz>().First();

        var updatedPhrase = "updated phrase";
        var updatedAnswer = "update answer";
        var commandQuestions = quiz.Questions.Select(x => x.Adapt<QuestionDto>()).ToList();
        commandQuestions.First().QuestionPhrase = updatedPhrase;
        commandQuestions.First().CorrectAnswer = updatedAnswer;
        var command = new UpdateQuizCommand(quiz.Id, "TestQuiz", commandQuestions);
        
        //Act
        await _host.Scenario(config =>
        {
            config.Put.Json(command).ToUrl("/quiz");
            config.StatusCodeShouldBeOk();
        });

        //Assert
        var actualQuiz = _dbContext.Quizzes
            .AsNoTracking()
            .Include(x => x.Questions)
            .SingleOrDefault(x => x.Id == command.Id);
        Assert.NotNull(actualQuiz);
        Assert.NotNull(actualQuiz.Questions);
        Assert.NotEmpty(actualQuiz.Questions);
        Assert.Equal(numberOfQuestionsPerQuiz, actualQuiz.Questions.Count);
       
        Assert.Contains(updatedPhrase, actualQuiz.Questions.Select(x => x.QuestionPhrase));
        Assert.Contains(updatedAnswer, actualQuiz.Questions.Select(x => x.CorrectAnswer));
    }
    
    [Fact]
    public async Task When_User_Updates_Quiz_RemovesExistingQuestion_Should_Succeed()
    {
        //Arrange
        var numberOfQuestionsPerQuiz = 5;
        _seeder.Include<Quiz>(1).Include<Question>(numberOfQuestionsPerQuiz);
        var quiz = _seeder.GetItems<Quiz>().First();
        
        var commandQuestions = quiz.Questions
            .Select(x => x.Adapt<QuestionDto>())
            .Take(numberOfQuestionsPerQuiz - 1)
            .ToList();
        var command = new UpdateQuizCommand(quiz.Id, "TestQuiz", commandQuestions);
        
        //Act
        await _host.Scenario(config =>
        {
            config.Put.Json(command).ToUrl("/quiz");
            config.StatusCodeShouldBeOk();
        });

        //Assert
        var actualQuiz = _dbContext.Quizzes
            .AsNoTracking()
            .Include(x => x.Questions)
            .SingleOrDefault(x => x.Id == command.Id);
        Assert.NotNull(actualQuiz);
        Assert.NotNull(actualQuiz.Questions);
        Assert.NotEmpty(actualQuiz.Questions);
        Assert.Equal(numberOfQuestionsPerQuiz - 1, actualQuiz.Questions.Count);
    }
}