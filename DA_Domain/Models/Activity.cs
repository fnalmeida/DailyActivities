using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace DailyActivities.Models
{
    public class Activity
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [Required]
        [BsonRepresentation(BsonType.ObjectId)]
        [BsonElement("user_Id")]
        public string? UserId { get; set; }  

        [Required]
        [BsonElement("description")]
        public string? Description { get; set; }

        [Required]
        [BsonElement("executionDate")]
        public DateTime ExecutionDate { get; set; }

        [Required]
        [BsonElement("duration")]
        [Description("Duração (min)")]
        public int Duration { get; set; }

        [Required]
        [BsonElement("met")]
        public decimal Met { get; set; }

        [Required]
        [BsonElement("createdAt")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [BsonElement("updateAt")]
        public DateTime? UpdateAt { get; set; }

        [Required]
        [BsonElement("deleted")]
        public bool Deleted { get; set; } = false;


    }
}
