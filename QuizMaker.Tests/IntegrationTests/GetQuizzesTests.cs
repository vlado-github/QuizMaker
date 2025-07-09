using System.Net;
using Alba;
using Microsoft.AspNetCore.Mvc;
using QuizMaker.Database.Entities;
using QuizMaker.Domain.Features.QuizBuilder.QueryResults;
using QuizMaker.Tests.Base;

namespace QuizMaker.Tests.IntegrationTests;


public class GetQuizzesTests :  IClassFixture<IntegrationTestFixture>
{
    private readonly IAlbaHost _host;
    private readonly TestSeeder _seeder;
    
    public GetQuizzesTests(IntegrationTestFixture fixture)
    {
        _host = fixture.Host;
        _seeder = fixture.Seeder;
    }
    
    [Fact]
    public async Task When_User_Lists_Available_Quizzes_Should_Succeed()
    {
        //Arrange
        var numberOfQuizzes = 3;
        _seeder.Include<Quiz>(numberOfQuizzes);
        
        //Act
        var result = await _host.Scenario(config =>
        {
            config.Get.Url("/quiz")
                .QueryString("page", "1")
                .QueryString("itemsPerPage", "1000");
            config.StatusCodeShouldBeOk();
        });

        //Assert
        var actualQuizzes = await result.ReadAsJsonAsync<IList<QuizSimpleResult>>();
        foreach (var seededQuiz in _seeder.GetItems<Quiz>())
        {
            Assert.Contains(actualQuizzes, item => item.Name == seededQuiz.Name);
        }
    }
    
    [Fact]
    public async Task When_User_Lists_Available_Quizzes_Per_Page_Should_Succeed()
    {
        //Arrange
        var itemsPerPage = 5;
        var numberOfQuizzes = 10;
        _seeder.Include<Quiz>(numberOfQuizzes);
        
        //Act
        var result = await _host.Scenario(config =>
        {
            config.Get.Url("/quiz")
                .QueryString("page", "2")
                .QueryString("itemsPerPage", $"{itemsPerPage}");
            config.StatusCodeShouldBeOk();
        });

        //Assert
        var actualQuizzes = await result.ReadAsJsonAsync<IList<QuizSimpleResult>>();
        Assert.Equal(actualQuizzes.Count, itemsPerPage);
    }
    
    [Fact]
    public async Task When_User_Lists_Available_Quizzes_And_Exceeds_ItemsPerPageLimit_Should_Fail()
    {
        //Arrange
        var numberOfQuizzes = 3;
        _seeder.Include<Quiz>(numberOfQuizzes);
        
        //Act
        var result = await _host.Scenario(config =>
        {
            config.Get.Url("/quiz")
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