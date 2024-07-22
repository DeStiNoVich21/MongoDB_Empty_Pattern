namespace MongoDB_Empty_Pattern.MongoRepository.GenericRepository
{
    public class MongoCollectionAttribute : Attribute
    {
        public string CollectionName { get; set; }

        public MongoCollectionAttribute(string collectionName)
        {
            CollectionName = collectionName;
        }
    }
}
