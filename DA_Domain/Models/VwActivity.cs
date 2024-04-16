using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System.ComponentModel.DataAnnotations;

namespace DailyActivities.Models
{
    
    public class VwActivity
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }
      
        [BsonElement("description")]
        public string? Description { get; set; }

        [BsonElement("executionDate")]
        public DateTime ExecutionDate { get; set; }

        [BsonElement("duration")]
        public int Duration { get; set; }

        [BsonElement("firstName")]
        public string? FirstName { get; set; }

        [BsonElement("lastName")]
        public string LastName { get; set; } = string.Empty;

    }
}
