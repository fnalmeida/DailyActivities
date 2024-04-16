using System.Text;
using RabbitMQ.Client;

var factory = new ConnectionFactory { HostName = "localhost", UserName = "guest", Password = "guest"};
using var connection = factory.CreateConnection();
using var channel = connection.CreateModel();

//channel.QueueDeclare(queue: "postactivity",
//                     durable: false,
//                     exclusive: false,
//                     autoDelete: false,
//                     arguments: null);

const string message = "Hello World!";
var body = Encoding.UTF8.GetBytes(message);

channel.BasicPublish(exchange: string.Empty,
                     routingKey: "postactivity",
                     basicProperties: null,
                     body: body);
Console.WriteLine($" [x] Sent {message}");

Console.WriteLine(" Press [enter] to exit.");
Console.ReadLine();