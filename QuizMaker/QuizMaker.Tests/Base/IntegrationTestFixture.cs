using Alba;
using Microsoft.Extensions.DependencyInjection;
using QuizMaker.Database;

namespace QuizMaker.Tests.Base;

public class IntegrationTestFixture : IAsyncLifetime
{
    public IAlbaHost Host = null!;
    public TestSeeder Seeder = null!;
    private IServiceScope _scope = null!;
    
    public async Task InitializeAsync()
    {
        Host = await AlbaHost.For<global::Program>();
        _scope = Host.Services.CreateScope();
        Seeder = new TestSeeder(_scope.ServiceProvider.GetRequiredService<QuizMakerContext>());
    }

    public async Task DisposeAsync()
    {
        _scope.Dispose();
        await Host.DisposeAsync();
    }
}