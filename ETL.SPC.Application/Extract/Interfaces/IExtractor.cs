namespace ETL.SPC.Application.Extract.Interfaces
{
    public interface IExtractor<T>
    {
        public Task<IEnumerable<T>> ExtractAsync();
    }
}