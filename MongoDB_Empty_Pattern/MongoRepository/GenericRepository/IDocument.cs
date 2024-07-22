using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.IdGenerators;

namespace MongoDB_Empty_Pattern.MongoRepository.GenericRepository
{
    public interface IDocument
    {
        string Id { get; set; }
    }
}
