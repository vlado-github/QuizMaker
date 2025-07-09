using System.Net;
using Alba;
using QuizMaker.API.Middlewares.CustomResponses;
using QuizMaker.Database.Entities;
using QuizMaker.Tests.Base;

namespace QuizMaker.Tests.IntegrationTests;

public class GetQuizTests :  IClassFixture<IntegrationTestFixture>
{
    private readonly IAlbaHost _host;
    private readonly TestSeeder _seeder;
    
    public GetQuizTests(IntegrationTestFixture fixture)
    {
        _host = fixture.Host;
        _seeder = fixture.Seeder;
    }
    
    [Fact]
    public async Task When_User_Fetches_Available_Quiz_Should_Succeed()
    {
        //Arrange
        var numberOfItems = 3;
        _seeder.Include<Quiz>(1).Include<Question>(numberOfItems);
        
        //Act
        var result = await _host.Scenario(config =>
        {
            config.Get.Url($"/quiz/{_seeder.GetItems<Quiz>().Last().Id}");
            config.StatusCodeShouldBeOk();
        });

        //Assert
        var expectedQuiz = _seeder.GetItems<Quiz>().First();
        var actualQuiz = await result.ReadAsJsonAsync<Quiz>();
        Assert.Equal(actualQuiz.Name, expectedQuiz.Name);
        Assert.Equal(actualQuiz.Questions.Count, expectedQuiz.Questions.Count);
        foreach (var expectedQuestion in expectedQuiz.Questions)
        {
            Assert.Contains(expectedQuestion.QuestionPhrase, actualQuiz.Questions.Select(x => x.QuestionPhrase));
            Assert.Contains(expectedQuestion.CorrectAnswer, actualQuiz.Questions.Select(x => x.CorrectAnswer));
        }
    }
    
    [Fact]
    public async Task When_User_Fetches_NonExisting_Quiz_Should_Fail()
    {
        //Act & Assert
        await _host.Scenario(config =>
        {
            config.Get.Url($"/quiz/0");
            config.StatusCodeShouldBe(HttpStatusCode.NotFound);
        });
    }
}