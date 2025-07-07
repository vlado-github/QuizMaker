using Microsoft.Extensions.DependencyInjection;
using QuizMaker.Database;
using QuizMaker.Domain.Base;
using QuizMaker.Domain.QuizFeature;

namespace QuizMaker.Domain.Boostrap;

public static class QuizMakerDomainBootstrap
{
    public static void AddQuizDomain(this IServiceCollection services)
    {
        services.AddScoped<IUnitOfWork<QuizMakerContext>, UnitOfWork<QuizMakerContext>>();
        services.AddScoped<IQuizQueryDispatcher, QuizQueryDispatcher>();
        services.AddScoped<IQuizCommandHandler, QuizCommandHandler>();
    }
}