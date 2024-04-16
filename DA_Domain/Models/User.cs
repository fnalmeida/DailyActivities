using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations;

namespace DailyActivities.Models
{
    public class User
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [Required]
        [MinLength(3)]
        [BsonElement("firstName")]
        public string? FirstName { get; set; }

        [Required]
        [MinLength(3)]
        [BsonElement("lastName")]
        public string LastName { get; set; } = string.Empty;

        [Required]
        [BsonElement("weight")]
        public Decimal? Weight { get; set; }

        [Required]
        [BsonElement("createdAt")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [BsonElement("updateAt")]
        public DateTime? UpdateAt { get; set; }

        [Required]
        [BsonElement("deleted")]
        public bool Deleted { get; set; } = false;

    }
}
