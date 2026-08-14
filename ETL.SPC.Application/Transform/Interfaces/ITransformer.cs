namespace ETL.SPC.Application.Transform.Interfaces
{
    public interface ITransformer<TIn, TOut>
    {
        IEnumerable<TOut> Transform(IEnumerable<TIn> input);
    }
}
