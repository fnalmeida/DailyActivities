using DA_Domain.Models;
using DA_GrpcService.Utils;
using DailyActivities.Data;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using MongoDB.Bson;


namespace DA_GrpcService
{
    public class CalcCaloricService : CaloriesGrpc.CaloriesGrpcBase
    {
        private readonly ILogger<CalcCaloricService> _logger;
        private readonly MongoDBGenericDAO _mongoDB;

        public CalcCaloricService(ILogger<CalcCaloricService> logger, MongoDBGenericDAO mongoDAO)
        {
            _logger = logger;
            _mongoDB = mongoDAO;
        }

        public override async Task<CaloriesOut> ListCalories(Empty request, ServerCallContext context)
        {
            var filter = new BsonDocument();
            //  filter.Add("_id", new BsonObjectId(new ObjectId(request.UserID)));

            var data =  await _mongoDB.Find <EnergySpent>("calories", new BsonDocument("_", true));
            var reply = new CaloriesOut();
            reply.EnergySpents.AddRange(data.Select(x => new EnergySpentOutType
            {
                Id = x.Id,
                FirstName = x.FirstName,
                LastName = x.LastName,
                UserId = x.UserId,
                ExecutionDate = Timestamp.FromDateTime(x.ExecutionDate),
                Calorie = DecimalConverter.ConvertToDecimalValue(x.Calorie.Value ?? ) ,

            }
                    
            ).ToList());
            return reply;
        }


    }
}


    
