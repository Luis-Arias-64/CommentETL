namespace CommentETL.Domain.Entities.Dimensions
{
    public class DimProduct
    {
        public int ProductKey { get; set; }

        public int ProductID { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string CategoryName { get; set; } = string.Empty;
    }
}