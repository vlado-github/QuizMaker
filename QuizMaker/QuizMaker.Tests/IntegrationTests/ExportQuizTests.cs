using System.Text;
using Alba;
using QuizMaker.Database;
using QuizMaker.Database.Entities;
using QuizMaker.Domain.Features.QuizExporter.Commands;
using QuizMaker.Tests.Base;

namespace QuizMaker.Tests.IntegrationTests;

public class ExportQuizTests : IClassFixture<IntegrationTestFixture>
{
    private readonly IAlbaHost _host;
    private readonly TestSeeder _seeder;
    private readonly QuizMakerContext _dbContext;
    
    public ExportQuizTests(IntegrationTestFixture fixture)
    {
        _host = fixture.Host;
        _seeder = fixture.Seeder;
        _dbContext = fixture.DatabaseContext;
    }
    
    [Fact]
    public async Task When_User_Exports_Quiz_AsCsv_Should_Succeed()
    {
        //Arrange
        _seeder.Include<Quiz>().Include<Question>();
        var quizId = _seeder.GetItems<Quiz>().Last().Id;
        var question = _seeder.GetItems<Question>().Last();
        var command = new ExportQuizCommand()
        {
            Id = quizId,
            FileType = "text/csv"
        };
        
        //Act
        var result = await _host.Scenario(config =>
        {
            config.Post.Json(command).ToUrl("/quiz/export");
            config.StatusCodeShouldBeOk();
            config.ContentShouldContain($"{nameof(question.Id)},{nameof(question.QuestionPhrase)}");
            config.ContentShouldContain($"{question.Id},{question.QuestionPhrase}");
        });
        
        //Assert
        Assert.Equal("text/csv", result.Context.Response.Headers.ContentType);
        var contentDisposition = result.Context.Response.Headers.ContentDisposition;
        Assert.NotEmpty(contentDisposition);
        Assert.Contains(".csv", contentDisposition.ToString());
    }
}