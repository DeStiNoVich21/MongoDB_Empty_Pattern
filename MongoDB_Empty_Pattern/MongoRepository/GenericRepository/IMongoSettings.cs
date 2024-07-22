namespace MongoDB_Empty_Pattern.MongoRepository.GenericRepository
{
    public interface IMongoSettings
    {
        string DatabaseName { get; set; }
        string ConnectionString { get; set; }
    }
}
