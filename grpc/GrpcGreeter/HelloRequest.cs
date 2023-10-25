namespace GrpcGreeter;

public partial class HelloRequest { 
    public string Name {
        get => $"{Firstname} {Lastname}";
    }
}