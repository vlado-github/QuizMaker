using System.Net;
using Alba;
using Microsoft.AspNetCore.Mvc;
using QuizMaker.Database.Entities;
using QuizMaker.Tests.Base;

namespace QuizMaker.Tests.IntegrationTests;


public class GetQuestionsTests :  IClassFixture<IntegrationTestFixture>
{
    private readonly IAlbaHost _host;
    private readonly TestSeeder _seeder;
    
    public GetQuestionsTests(IntegrationTestFixture fixture)
    {
        _host = fixture.Host;
        _seeder = fixture.Seeder;
    }
    
    [Fact]
    public async Task When_User_Lists_Available_Questions_Should_Succeed()
    {
        //Arrange
        var numberOfQuestions = 3;
        _seeder
            .Include<Quiz>(1)
            .Include<Question>(numberOfQuestions);
        
        //Act
        var result = await _host.Scenario(config =>
        {
            config.Get.Url("/question")
                .QueryString("page", "1")
                .QueryString("itemsPerPage", "1000");
            config.StatusCodeShouldBeOk();
        });

        //Assert
        var actualQuizzes = await result.ReadAsJsonAsync<IList<Question>>();
        foreach (var seededQuiz in _seeder.GetItems<Question>())
        {
            Assert.Contains(actualQuizzes, item => item.CorrectAnswer == seededQuiz.CorrectAnswer);
            Assert.Contains(actualQuizzes, item => item.QuestionPhrase == seededQuiz.QuestionPhrase);
        }
    }
    
    [Fact]
    public async Task When_User_Searches_Available_Questions_Should_Succeed()
    {
        //Arrange
        var numberOfQuestions = 3;
        var prefix = Guid.NewGuid().ToString();
        _seeder
            .Include<Quiz>(1)
            .Include<Question>(numberOfQuestions, prefix);
        
        //Act
        var result = await _host.Scenario(config =>
        {
            config.Get.Url("/question")
                .QueryString("search", prefix)
                .QueryString("page", "1")
                .QueryString("itemsPerPage", "1000");
            config.StatusCodeShouldBeOk();
        });

        //Assert
        var actualQuizzes = await result.ReadAsJsonAsync<IList<Question>>();
        Assert.Equal(actualQuizzes.Count, numberOfQuestions);
        foreach (var seededQuiz in _seeder.GetItems<Question>())
        {
            Assert.Contains(actualQuizzes, item => item.CorrectAnswer == seededQuiz.CorrectAnswer);
            Assert.Contains(actualQuizzes, item => item.QuestionPhrase == seededQuiz.QuestionPhrase);
        }
    }
    
    [Fact]
    public async Task When_User_Lists_Available_Questions_Per_Page_Should_Succeed()
    {
        //Arrange
        var itemsPerPage = 5;
        var numberOfQuestions = 10;
        _seeder
            .Include<Quiz>(1)
            .Include<Question>(numberOfQuestions);
        
        //Act
        var result = await _host.Scenario(config =>
        {
            config.Get.Url("/question")
                .QueryString("page", "2")
                .QueryString("itemsPerPage", $"{itemsPerPage}");
            config.StatusCodeShouldBeOk();
        });

        //Assert
        var actualQuizzes = await result.ReadAsJsonAsync<IList<Quiz>>();
        Assert.Equal(actualQuizzes.Count, itemsPerPage);
    }
    
    [Fact]
    public async Task When_User_Lists_Available_Questions_And_Exceeds_ItemsPerPageLimit_Should_Fail()
    {
        //Arrange
        var numberOfQuestions = 3;
        _seeder
            .Include<Quiz>(1)
            .Include<Question>(numberOfQuestions);
        
        //Act
        var result = await _host.Scenario(config =>
        {
            config.Get.Url("/question")
                .QueryString("page", "1")
                .QueryString("itemsPerPage", "10000");
            config.StatusCodeShouldBe(HttpStatusCode.BadRequest);
        });

        //Assert
        var details = result.ReadAsJson<ValidationProblemDetails>();
        Assert.Collection(details.Errors, 
            (error) => Assert.Contains("'Items Per Page' must be less than or equal to '1000'.", error.Value));
    }
}