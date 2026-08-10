namespace CommentETL.Application.Extract.Interfaces
{
    public interface IExtractor<T>
    {
        Task<IEnumerable<T>> ExtractAsync();
    }
}