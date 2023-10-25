using Grpc.Core;
using GrpcGreeter;

namespace GrpcGreeter.Services;

public class GreeterService : Greeter.GreeterBase
{
    private readonly ILogger<GreeterService> _logger;
    public GreeterService(ILogger<GreeterService> logger)
    {
        _logger = logger;
    }

    public override Task<HelloReply> SayHello(HelloRequest request, ServerCallContext context)
    {
        string name;
        if (request.Firstname != String.Empty || request.Lastname != String.Empty)
        {
            name = $"{request.Firstname} {request.Lastname}";
        }
        else
        {
            name = request.Name;
        }

        return Task.FromResult(new HelloReply
        {
            Message = "Hello " + name
        });
    }
}


