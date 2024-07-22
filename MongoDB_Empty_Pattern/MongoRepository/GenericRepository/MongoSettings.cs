namespace MongoDB_Empty_Pattern.MongoRepository.GenericRepository
{
    public class MongoSettings : IMongoSettings
    {
        public string DatabaseName { get; set; }
        public string ConnectionString { get; set; }
    }
}
