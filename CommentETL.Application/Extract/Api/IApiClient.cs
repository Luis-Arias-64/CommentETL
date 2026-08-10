namespace CommentETL.Application.Extract.Api
{
    public interface IApiClient
    {
        Task<string> GetAsync(string endpoint);
    }
}