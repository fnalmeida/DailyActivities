// See https://aka.ms/new-console-template for more information
using Grpc.Net.Client;

Console.WriteLine("Hello, World!");
using var channel = GrpcChannel.ForAddress("https://localhost:7119");
var client = new UserGrpc.UserGrpcClient(channel);
var reply =  client.ListUsers(null, null);
Console.WriteLine("Greeting: " + reply.Message);
Console.WriteLine("Press any key to exit...");
Console.ReadKey();