namespace CommentETL.Domain.Entities.Dimensions
{
    public class DimClient
    {
        public int ClientKey { get; set; }

        public int ClientID { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
        public int Age { get; set; }
        public string ClientType { get; set; } = string.Empty;
    }
}