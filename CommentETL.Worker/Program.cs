using CommentETL.Application.Extract.Api;
using CommentETL.Application.Extract.Csv;
using CommentETL.Application.Extract.Database;
using CommentETL.Infraestructure.Api;
using CommentETL.Infraestructure.Database;
using CommentETL.Worker;

var builder = Host.CreateApplicationBuilder(args);

var configuration = builder.Configuration;

// --- Base de datos relacional de origen (PostgreSQL) ---
builder.Services.AddSingleton<IDbConnectionFactory>(_ =>
    new PostgresConnectionFactory(configuration.GetConnectionString("SourceDb")!));

builder.Services.AddSingleton<DbCommentExtractor>();

// --- API externa de comentarios ---
builder.Services.AddHttpClient<IApiClient, HttpApiClient>(client =>
{
    client.BaseAddress = new Uri(configuration["Api:BaseUrl"]!);
});

builder.Services.AddSingleton<ApiCommentExtractor>();

// --- Archivos CSV ---
// Cada extractor recibe su propia ruta desde appsettings (CsvPaths).
builder.Services.AddSingleton(_ =>
    new ClientExtractor(configuration["CsvPaths:Clients"]!));

builder.Services.AddSingleton(_ =>
    new ProductExtractor(configuration["CsvPaths:Products"]!));

builder.Services.AddSingleton(_ =>
    new SourceExtractor(configuration["CsvPaths:Sources"]!));

builder.Services.AddSingleton(_ =>
    new SocialCommentExtractor(configuration["CsvPaths:SocialComments"]!));

builder.Services.AddSingleton(_ =>
    new SurveyExtractor(configuration["CsvPaths:Surveys"]!));

builder.Services.AddSingleton(_ =>
    new WebReviewExtractor(configuration["CsvPaths:WebReviews"]!));

builder.Services.AddHostedService<Worker>();

var host = builder.Build();
host.Run();
