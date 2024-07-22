using Microsoft.VisualBasic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Microsoft.AspNetCore.Components.Web;
using System.Reflection.Metadata;
using MongoDB_Empty_Pattern.MongoRepository.GenericRepository;

namespace MongoDB_Empty_Pattern.Models
{
    public class Users : IDocument
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]

        public string Id { get; set; }

        [BsonElement("username")]
        public string username { get; set; } = string.Empty;


        [BsonElement("passwordhash")]
        public byte[] PasswordHash { get; set; }

        [BsonElement("passwordsalt")]
        public byte[] PasswordSalt { get; set; }

      

    }
}