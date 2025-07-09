using Alba;
using Microsoft.Extensions.DependencyInjection;
using QuizMaker.Database;

namespace QuizMaker.Tests.Base;

public class IntegrationTestFixture : IAsyncLifetime
{
    public IAlbaHost Host = null!;
    public TestSeeder Seeder = null!;
    public QuizMakerContext DatabaseContext = null!;
    private IServiceScope _scope = null!;
    
    public async Task InitializeAsync()
    {
        Host = await AlbaHost.For<global::Program>();
        _scope = Host.Services.CreateScope();
        DatabaseContext = _scope.ServiceProvider.GetRequiredService<QuizMakerContext>();
        Seeder = new TestSeeder(DatabaseContext);
    }

    public async Task DisposeAsync()
    {
        Seeder.Dispose();
        await DatabaseContext.DisposeAsync();
        _scope.Dispose();
        await Host.DisposeAsync();
    }
}