namespace ETL.SPC.Application.Load.Interfaces
{
    public interface ILoader<T>
    {
        Task LoadAsync(IEnumerable<T> items);
    }
}
