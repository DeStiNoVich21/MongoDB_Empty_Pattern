using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System.Text.Json.Serialization;
using MongoDB_Empty_Pattern.MongoRepository.GenericRepository;


namespace MongoDB_Empty_Pattern.Models
{
    public class Licenses : IDocument
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]

        public string Id { get; set; }
        public string mac_address { get; set; }
        public string licences_key { get; set; }
        public string licences_date { get; set; }
        public string BIN { get; set; } = string.Empty;
        public string Org { get; set; } = string.Empty;
        public string status { get; set; } = "True";
        public DateTime expire_date { get; set; } = new DateTime(2025, 1, 1);
    }
}
