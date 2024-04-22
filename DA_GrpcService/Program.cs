using DA_Domain.Models;
using DA_Domain.Services;
using DA_GrpcService;
using DailyActivities.Data;
using DailyActivities.Models;
using Microsoft.AspNetCore.Connections;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Linq;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddGrpc();

var session = builder.Configuration.GetSection("Database");
var connectionString = session["ConnectionString"];
var databaseName = session["DatabaseName"];

builder.Services.AddScoped<MongoDBGenericDAO>(dao => new MongoDBGenericDAO(connectionString, databaseName));


var app = builder.Build();

// Configure the HTTP request pipeline.
app.MapGrpcService<CalcCaloricService>();

app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. " +
"To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

ListenForIntegrationEvents(app.Services);


app.Run();


static void ListenForIntegrationEvents(IServiceProvider serviceProvider)
{
    var factory = new ConnectionFactory() { HostName = "localhost", UserName = "guest", Password = "guest" };
    var connection = factory.CreateConnection();
    var channel = connection.CreateModel();
    var consumer = new EventingBasicConsumer(channel);

    consumer.Received += (model, ea) =>
    {
        using (var scope = serviceProvider.CreateScope())
        {
            try
            {
                var scopedServiceProvider = scope.ServiceProvider;
                var mongoDAO = scopedServiceProvider.GetRequiredService<MongoDBGenericDAO>();

                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                Console.WriteLine(" [x] Received {0}", message);
                dynamic data = JObject.Parse(message);
                var type = ea.RoutingKey;

                var cal = CaloriesCalculator.Calculator((decimal)data.Weight, (decimal)data.act.Met, (decimal)data.act.Duration);
                var calorie = new EnergySpent() { 
                    UserId = data.act.UserId,
                    FirstName = data.FirstName,
                    LastName = data.LastName,
                    Calorie = cal,
                    ExecutionDate = Convert.ToDateTime(data.act.ExecutionDate)
                };               

                var res = mongoDAO.Save<EnergySpent>("calories", calorie);
                channel.BasicAck(ea.DeliveryTag, false);
            }
            catch
            {
                channel.BasicNack(ea.DeliveryTag, false, false);
            }           

            
        }
    };

    channel.BasicConsume(queue: "postactivity",
                         autoAck: false, consumer: consumer);
}