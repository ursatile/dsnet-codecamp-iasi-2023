// See https://aka.ms/new-console-template for more information
using Grpc.Net.Client;
using GrpcGreeter;

Console.WriteLine("Running gRPC client...");
using var channel
    = GrpcChannel.ForAddress("https://localhost:7002");

var client = new Greeter.GreeterClient(channel);
Console.WriteLine("press a key to send a gRPC request...");
while (true)
{
    Console.ReadKey(true);
    var request = new HelloRequest
    {
        Firstname = "CodeCamp",
        Lastname = "Iasi"
    };
    var reply = await client.SayHelloAsync(request);
    Console.WriteLine(reply.Message);
}



