using Alba;
using Microsoft.EntityFrameworkCore;
using QuizMaker.Database;
using QuizMaker.Database.Entities;
using QuizMaker.Tests.Base;

namespace QuizMaker.Tests.IntegrationTests;

public class DeleteQuestionTests :  IClassFixture<IntegrationTestFixture>
{
    private readonly IAlbaHost _host;
    private readonly TestSeeder _seeder;
    private readonly QuizMakerContext _dbContext;
    
    public DeleteQuestionTests(IntegrationTestFixture fixture)
    {
        _host = fixture.Host;
        _seeder = fixture.Seeder;
        _dbContext = fixture.DatabaseContext;
    }
    
    [Fact]
    public async Task When_User_Deletes_Quiz_Should_Succeed()
    {
        //Arrange
        _seeder.Include<Quiz>().Include<Question>();
        var quiz = _seeder.GetItems<Quiz>().Last();
        var question = _seeder.GetItems<Question>().Last();
        
        //Act
        await _host.Scenario(config =>
        {
            config.Delete.Url($"/quiz/{quiz.Id}");
            config.StatusCodeShouldBeOk();
        });
        
        //Assert
        var actualQuiz = _dbContext.Quizzes
            .AsNoTracking()
            .SingleOrDefault(x => x.Id == quiz.Id);
        Assert.Null(actualQuiz);
        
        var actualQuestion= _dbContext.Questions
            .AsNoTracking()
            .SingleOrDefault(x => x.Id == question.Id);
        Assert.NotNull(actualQuestion);
    }
}