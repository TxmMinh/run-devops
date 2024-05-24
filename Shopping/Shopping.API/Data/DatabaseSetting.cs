namespace Shopping.API.Data
{
    public class DatabaseSettings
    {
        public string ConnectionString { get; set; }
        public string DatabaseName { get; set; }
        public string ProductCollectionName { get; set; }
        public string CartCollectionName { get; set; }
    }
}
