using System.Composition.Hosting;
using System.Reflection;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using QuizMaker.API.Middlewares;
using QuizMaker.Database;
using QuizMaker.Database.Extensions;
using QuizMaker.Domain.Bootstrap;
using QuizMaker.Domain.Features.QuizBuilder.Commands;
using QuizMaker.Shared.Base;
using SharpGrip.FluentValidation.AutoValidation.Mvc.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers()
    .AddNewtonsoftJson(options => options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore);
builder.Services.AddQuizDomain();

// Add Fluent Validators as dependencies
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<CreateQuizCommandValidator>(ServiceLifetime.Scoped, includeInternalTypes: true);

// Setup database context
builder.Services.AddDbContext<QuizMakerContext>(options =>
{
    options.SetDbOptions();
});

// Setup MEF
var basePath = Path.Combine(AppContext.BaseDirectory);
var assemblies = Directory.GetFiles(basePath, "QuizMaker.Plugins.*.dll")
    .Select(Assembly.LoadFrom)
    .ToList();
var configuration = new ContainerConfiguration().WithAssemblies(assemblies);
var mefContainer = configuration.CreateContainer();
builder.Services.AddSingleton(mefContainer.GetExports<Lazy<IFileExporter, FileExporterFormatMetadata>>());

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.EnvironmentName == "Local" || app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

//Middlewares
app.UseMiddleware<ExceptionMiddleware>();

app.MapControllers();


using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<QuizMakerContext>();
    await db.Database.MigrateAsync();
}


app.Run();