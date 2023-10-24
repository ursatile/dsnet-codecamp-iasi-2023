using System.Runtime.CompilerServices;
using EasyNetQ;
using Messages;

const string AMQP = "amqps://efwzzuku:7CflikvF4dquXhPcz_lzIcGQJOUJM1Vd@strong-golden-bat.rmq4.cloudamqp.com/efwzzuku";
var bus = RabbitHutch.CreateBus(AMQP);
var subscriptionId = "dylan-framework-laptop";
await bus.PubSub.SubscribeAsync<Greeting>(
    subscriptionId,
    Handle
);

Console.WriteLine("Listening for messages... press Ctrl-C to quit");
Console.ReadKey(true);

void Handle(Greeting greeting) {
    if (greeting.Number % 4 == 0) {
        throw new Exception("Greetings divisible by 4 are not allowed");        
    }
    Console.WriteLine(greeting);
}