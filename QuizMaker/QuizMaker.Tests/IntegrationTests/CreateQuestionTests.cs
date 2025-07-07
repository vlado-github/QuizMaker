using Alba;
using QuizMaker.Domain.Dtos;
using QuizMaker.Domain.QuizFeature.Commands;
using QuizMaker.Tests.Base;

namespace QuizMaker.Tests.IntegrationTests;

public class CreateQuestionTests :  IClassFixture<IntegrationTestFixture>
{
    private readonly IAlbaHost _host;
    private readonly TestSeeder _seeder;
    
    public CreateQuestionTests(IntegrationTestFixture fixture)
    {
        _host = fixture.Host;
        _seeder = fixture.Seeder;
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
    }
}