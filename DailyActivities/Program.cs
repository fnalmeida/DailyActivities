using Amazon.Runtime.SharedInterfaces;
using DailyActivities.Data;
using DailyActivities.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Abstractions;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.Resource;
using MiniValidation;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Core.Configuration;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddMicrosoftIdentityWebApi(builder.Configuration.GetSection("AzureAd"));
builder.Services.AddAuthorization();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//builder.Services.AddScoped<MongoDBGenericDAO>( dao => new MongoDBGenericDAO(builder.Configuration.GetSection("Database")));

var session = builder.Configuration.GetSection("Database");
var connectionString = session["ConnectionString"];
var databaseName = session["DatabaseName"];

builder.Services.AddScoped<MongoDBGenericDAO>(dao => new MongoDBGenericDAO(connectionString, databaseName));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
 
app.UseHttpsRedirection();



#region user
app.MapGet("/user", async (MongoDBGenericDAO mongoDAO) =>
{
    return mongoDAO.Find<User>("user", new BsonDocument("_", true));

})
.Produces(StatusCodes.Status200OK)
.Produces(StatusCodes.Status400BadRequest)
.WithName("getUsers")
.WithTags("mongoDB")
.WithOpenApi();

app.MapPost("/user", (MongoDBGenericDAO mongoDAO, User user) =>
{

    if (! MiniValidator.TryValidate(user, out var errors))
        return Results.ValidationProblem(errors);
    try
    {
        var res = mongoDAO.Save<User>("user", user);
        return Results.Ok(res);
    }
    catch (Exception ex) { return Results.Problem("Registro não inserido"); };

})
.ProducesValidationProblem()
.Produces(StatusCodes.Status200OK)
.Produces(StatusCodes.Status400BadRequest)
.WithName("insertUser")
.WithTags("mongoDB")
.WithOpenApi();

app.MapPut("/user/{id}", async (MongoDBGenericDAO mongoDAO, string id, User user) =>
{
    if (!MiniValidator.TryValidate(user, out var errors))
        return Results.ValidationProblem(errors);

    try
    {
        dynamic res = mongoDAO.Update<User>("user", id, user);
        return Results.Ok(user);
    }
    catch (Exception ex) { return Results.BadRequest(ex.Message); }

})
.ProducesValidationProblem()
.Produces(StatusCodes.Status200OK)
.Produces(StatusCodes.Status400BadRequest)
.WithName("alterUser")
.WithTags("mongoDB")
.WithOpenApi();

app.MapDelete("/user/harddelete/{id}", async (MongoDBGenericDAO mongoDB,  string id) =>
{
    try
    {
        var res = await mongoDB.HardDelete<User>("user", id);
        return Results.Ok(res);
    }
    catch (Exception ex) { return Results.BadRequest("Documento não excluído"); }

})
.ProducesValidationProblem()
.Produces(StatusCodes.Status200OK)
.Produces(StatusCodes.Status400BadRequest)
.WithName("hardDeleteUser")
.WithTags("mongoDB")
.WithOpenApi();

app.MapDelete("/user/softdelete/{id}", async (MongoDBGenericDAO mongoDB, string id) =>
{
    try
    {
        return Results.Ok(mongoDB.SoftDelete<User>("user", id));            
        
    }
    catch (Exception ex) { return Results.BadRequest(ex.Message); }

})
.ProducesValidationProblem()
.Produces(StatusCodes.Status200OK)
.Produces(StatusCodes.Status400BadRequest)
.WithName("softDeleteUser")
.WithTags("mongoDB")
.WithOpenApi();
#endregion


app.MapPost("/activity", async (MongoDBGenericDAO mongoDAO, Activity act) =>
{
    if (!MiniValidator.TryValidate(act, out var errors))
        return Results.ValidationProblem(errors);
    try
    {
        var res = mongoDAO.Save<Activity>("activity", act);

        var filter = new BsonDocument();
        filter.Add("_id", new BsonObjectId(new ObjectId(act.UserId)));
        var user = mongoDAO.Find<User>("user", filter).Result.FirstOrDefault();
        if (user != null)
        {
            var data = new { act, Weight = user.Weight, FirstName = user.FirstName, LastName = user.LastName};            
            RabbitMQClient.PublishToMessageQueue("postactivity", data);
        }

        return Results.Ok(res.Result);
    }
    catch (Exception ex) { return Results.Problem("Registro não inserido"); };

})
.ProducesValidationProblem()
.Produces(StatusCodes.Status200OK)
.Produces(StatusCodes.Status400BadRequest)
.WithName("insertActivity")
.WithTags("mongoDB")
.WithOpenApi();


app.MapGet("/activity", (MongoDBGenericDAO mongoDAO) =>
{
try
    {
        var res = mongoDAO.Find<VwActivity>("vw-activity", new BsonDocument("_", true));
        return Results.Ok(res);
    }
    catch (Exception ex) { return Results.Problem("Falha na consulta"); };

})
.ProducesValidationProblem()
.Produces(StatusCodes.Status200OK)
.Produces(StatusCodes.Status400BadRequest)
.WithName("getActivities")
.WithTags("mongoDB")
.WithOpenApi();



app.Run();


internal class DatabaseSettings
{
    public string ConnectionString { get; set; } = null!;

    public string DatabaseName { get; set; } = null!;
}

internal class RabbitMQClient
{
    public static void PublishToMessageQueue(string integrationEvent, object data)
    {
        // Ajustes a fazer : Reusar e fechar conexões e channel, e outro detalhes 
        var factory = new ConnectionFactory { HostName = "localhost", UserName = "guest", Password = "guest" };
        //factory.Uri = new Uri("amqp://guest:guest@localhost:15672/");
        var connection = factory.CreateConnection();
        var channel = connection.CreateModel();
        string json = JsonSerializer.Serialize(data);
        var body = Encoding.UTF8.GetBytes(json);

        channel.BasicPublish(exchange: "",
                                         routingKey: integrationEvent,
                                         basicProperties: null,
                                         body: body);
    }
}