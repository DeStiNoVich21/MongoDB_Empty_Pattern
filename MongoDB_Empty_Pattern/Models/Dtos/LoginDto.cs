using MongoDB.Bson.Serialization.Attributes;

namespace MongoDB_Empty_Pattern.Models.Dtos
{
    public class LoginDto
    {
        [BsonElement("username")]
        public string username { get; set; } = string.Empty;
        [BsonElement("password")]
        public string password { get; set; } = string.Empty;
    }
}
