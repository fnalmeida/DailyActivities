﻿
using Grpc.Net.Client;
using GrpcService1;

using var channel = GrpcChannel.ForAddress("https://localhost:7116");
var client = new Greeter.GreeterClient(channel);

var reply = await client.SayHelloAsync(new HelloRequest { Name = "fabricio" });
Console.WriteLine("Greeting: " + reply.Message);

Console.WriteLine("Shutting down");
Console.WriteLine("Press any key to exit...");
Console.ReadKey();