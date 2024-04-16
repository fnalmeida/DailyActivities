using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DA_Domain.Models
{
    public class EnergySpent
    {
        public EnergySpent() { }

        //public EnergySpent(string userId, string firstName, string lastName, decimal? calorie, DateTime executionDate)
        //{
        //    UserId = userId;
        //    FirstName = firstName;
        //    LastName = lastName;
        //    Calorie = calorie;
        //    ExecutionDate = executionDate;
        //}   

        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonRepresentation(BsonType.ObjectId)]
        [BsonElement("user_Id")]
        public  string UserId { get; set; }

        [BsonElement("firstname")]
        public string FirstName { get; set; }

        [Required]
        [BsonElement("lastName")]
        public string LastName { get; set; }
        
        [Required]
        [BsonElement("calorie")]
        public decimal? Calorie { get; set; }
        
        [Required]
        [BsonElement("executionDate")]
        public DateTime ExecutionDate { get; set; }
    }
}
