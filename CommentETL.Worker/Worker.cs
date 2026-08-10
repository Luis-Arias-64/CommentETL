using CommentETL.Application.Extract.Api;
using CommentETL.Application.Extract.Csv;
using CommentETL.Application.Extract.Database;
using CommentETL.Domain.Base;

namespace CommentETL.Worker;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;

    private readonly DbCommentExtractor _dbExtractor;
    private readonly ApiCommentExtractor _apiExtractor;
    private readonly SocialCommentExtractor _socialExtractor;
    private readonly SurveyExtractor _surveyExtractor;
    private readonly WebReviewExtractor _webReviewExtractor;
    private readonly ClientExtractor _clientExtractor;
    private readonly ProductExtractor _productExtractor;
    private readonly SourceExtractor _sourceExtractor;

    public Worker(
        ILogger<Worker> logger,
        DbCommentExtractor dbExtractor,
        ApiCommentExtractor apiExtractor,
        SocialCommentExtractor socialExtractor,
        SurveyExtractor surveyExtractor,
        WebReviewExtractor webReviewExtractor,
        ClientExtractor clientExtractor,
        ProductExtractor productExtractor,
        SourceExtractor sourceExtractor)
    {
        _logger = logger;
        _dbExtractor = dbExtractor;
        _apiExtractor = apiExtractor;
        _socialExtractor = socialExtractor;
        _surveyExtractor = surveyExtractor;
        _webReviewExtractor = webReviewExtractor;
        _clientExtractor = clientExtractor;
        _productExtractor = productExtractor;
        _sourceExtractor = sourceExtractor;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Iniciando fase de extracción (E)...");

        // Las 6 fuentes se extraen en paralelo: DB relacional, API y 4 CSV.
        var dbTask = _dbExtractor.ExtractAsync();
        var apiTask = _apiExtractor.ExtractAsync();
        var socialTask = _socialExtractor.ExtractAsync();
        var surveyTask = _surveyExtractor.ExtractAsync();
        var webReviewTask = _webReviewExtractor.ExtractAsync();
        var clientTask = _clientExtractor.ExtractAsync();
        var productTask = _productExtractor.ExtractAsync();
        var sourceTask = _sourceExtractor.ExtractAsync();

        await Task.WhenAll(
            dbTask, apiTask, socialTask, surveyTask, webReviewTask,
            clientTask, productTask, sourceTask);

        // Comentarios: se combinan todos en una sola colección cruda (CommentRaw),
        // sin importar de qué fuente vinieron. La deduplicación/validación es del Transform.
        var comentarios = new List<CommentRaw>()
            .Concat(dbTask.Result)
            .Concat(apiTask.Result)
            .Concat(socialTask.Result)
            .Concat(surveyTask.Result)
            .Concat(webReviewTask.Result)
            .ToList();

        var clientes = clientTask.Result.ToList();
        var productos = productTask.Result.ToList();
        var fuentes = sourceTask.Result.ToList();

        _logger.LogInformation("Extracción finalizada:");
        _logger.LogInformation(" - Comentarios (DB + API + CSV): {Count}", comentarios.Count);
        _logger.LogInformation("   - DB:            {Count}", dbTask.Result.Count());
        _logger.LogInformation("   - API:           {Count}", apiTask.Result.Count());
        _logger.LogInformation("   - Social CSV:    {Count}", socialTask.Result.Count());
        _logger.LogInformation("   - Surveys CSV:   {Count}", surveyTask.Result.Count());
        _logger.LogInformation("   - WebReviews CSV:{Count}", webReviewTask.Result.Count());
        _logger.LogInformation(" - Clientes:  {Count}", clientes.Count);
        _logger.LogInformation(" - Productos: {Count}", productos.Count);
        _logger.LogInformation(" - Fuentes:   {Count}", fuentes.Count);

        // Solo se implementa Extract en esta etapa; Transform y Load quedan pendientes.
    }
}
