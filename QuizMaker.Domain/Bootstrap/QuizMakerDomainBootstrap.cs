using Microsoft.Extensions.DependencyInjection;
using QuizMaker.Database;
using QuizMaker.Domain.Base;
using QuizMaker.Domain.Features.QuizBuilder;
using QuizMaker.Domain.Features.QuizExporter;

namespace QuizMaker.Domain.Bootstrap;

public static class QuizMakerDomainBootstrap
{
    public static void AddQuizDomain(this IServiceCollection services)
    {
        services.AddScoped<IUnitOfWork<QuizMakerContext>, UnitOfWork<QuizMakerContext>>();
        services.AddScoped<IQuizQueryDispatcher, QuizQueryDispatcher>();
        services.AddScoped<IQuizCommandHandler, QuizCommandHandler>();
        services.AddScoped<IExporterCommandHandler, ExporterCommandHandler>();

        // services.AddScoped<CompositionContainer>(sp =>
        // {
        //     var catalog = new AggregateCatalog();
        //     catalog.Catalogs.Add(new AssemblyCatalog(typeof(IFileExporter).Assembly));
        //     catalog.Catalogs.Add(new DirectoryCatalog(AppContext.BaseDirectory));
        //     var container = new CompositionContainer(catalog);
        //     container.ComposeParts();
        //     return container;
        // });
    }
}