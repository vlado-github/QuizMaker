using Alba;
using Microsoft.EntityFrameworkCore;
using QuizMaker.Database;
using QuizMaker.Database.Entities;
using QuizMaker.Domain.Dtos;
using QuizMaker.Domain.Features.QuizBuilder.Commands;
using QuizMaker.Tests.Base;

namespace QuizMaker.Tests.IntegrationTests;

public class CreateQuestionTests :  IClassFixture<IntegrationTestFixture>
{
    private readonly IAlbaHost _host;
    private readonly TestSeeder _seeder;
    private readonly QuizMakerContext _dbContext;
    
    public CreateQuestionTests(IntegrationTestFixture fixture)
    {
        _host = fixture.Host;
        _seeder = fixture.Seeder;
        _dbContext = fixture.DatabaseContext;
    }
    
    [Fact]
    public async Task When_User_Creates_Quiz_Should_Succeed()
    {
        //Arrange
        var command = new CreateQuizCommand("TestQuiz", new List<QuestionDto>()
        {
            new QuestionDto()
            {
                QuestionPhrase = "test question 01",
                CorrectAnswer = "test answer 01"
            },
            new QuestionDto()
            {
                QuestionPhrase = "test question 02",
                CorrectAnswer = "test answer 02"
            }
        });
        
        //Act
        var result = await _host.Scenario(config =>
        {
            config.Post.Json(command).ToUrl("/quiz");
            config.StatusCodeShouldBeOk();
        });
        
        //Assert
        var id = await result.ReadAsJsonAsync<long>();
        Assert.NotEqual(0, id);
        _seeder.AddItemForCleanup<Quiz>(id);

        var actualQuiz = _dbContext.Quizzes
            .AsNoTracking()
            .Include(x => x.Questions)
            .SingleOrDefault(x => x.Id == id);
        Assert.NotNull(actualQuiz);
        Assert.NotNull(actualQuiz.Questions);
        Assert.NotEmpty(actualQuiz.Questions);
        Assert.Equal(actualQuiz.Questions.Count, command.Questions.Count);
        foreach (var question in actualQuiz.Questions)
        {
            Assert.Contains(command.Questions, q => q.QuestionPhrase == question.QuestionPhrase);
            Assert.Contains(command.Questions, q => q.CorrectAnswer == question.CorrectAnswer);
        }
    }
    
    
    [Fact]
    public async Task When_User_Creates_Quiz_FromQuestionPool_Should_Succeed()
    {
        //Arrange
        _seeder.Include<Quiz>(1).Include<Question>(5);
        var questionFromPool = _seeder.GetItems<Question>().Last();
        var command = new CreateQuizCommand("TestQuiz", new List<QuestionDto>()
        {
            new QuestionDto()
            {
                QuestionPhrase = "test question 01",
                CorrectAnswer = "test answer 01"
            },
            new QuestionDto()
            {
                Id = questionFromPool.Id
            }
        });
        
        //Act
        var result = await _host.Scenario(config =>
        {
            config.Post.Json(command).ToUrl("/quiz");
            config.StatusCodeShouldBeOk();
        });

        //Assert
        var id = await result.ReadAsJsonAsync<long>();
        Assert.NotEqual(0, id);
        _seeder.AddItemForCleanup<Quiz>(id);

        var actualQuiz = _dbContext.Quizzes
            .AsNoTracking()
            .Include(x => x.Questions)
            .SingleOrDefault(x => x.Id == id);
        Assert.NotNull(actualQuiz);
        Assert.NotNull(actualQuiz.Questions);
        Assert.NotEmpty(actualQuiz.Questions);
        Assert.Equal(actualQuiz.Questions.Count, command.Questions.Count);
        Assert.Contains(actualQuiz.Questions, q => q.QuestionPhrase == questionFromPool.QuestionPhrase);
        Assert.Contains(actualQuiz.Questions, q => q.CorrectAnswer == questionFromPool.CorrectAnswer);
        Assert.Contains(actualQuiz.Questions, q => q.QuestionPhrase == command.Questions.Single(x => x.Id == null).QuestionPhrase);
        Assert.Contains(actualQuiz.Questions, q => q.CorrectAnswer == command.Questions.Single(x => x.Id == null).CorrectAnswer);
    }
}