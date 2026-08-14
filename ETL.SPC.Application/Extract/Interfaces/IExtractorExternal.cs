namespace ETL.SPC.Application.Extract.Interfaces
{
    public interface IExtractorExternal<T>
    {
        public Task<IQueryable<T>> ExtractAsync();
    }
}