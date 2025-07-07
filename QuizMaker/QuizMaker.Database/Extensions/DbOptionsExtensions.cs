using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace QuizMaker.Database.Extensions;

public static class DbOptionsExtensions
{
    public static void SetDbOptions(this DbContextOptionsBuilder optionsBuilder)
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
            .AddJsonFile($"appsettings.json", optional: true)
            .Build();
        var connectionString = configuration.GetConnectionString("DefaultConnection");
        optionsBuilder.UseNpgsql(connectionString).UseSnakeCaseNamingConvention();
    }
}