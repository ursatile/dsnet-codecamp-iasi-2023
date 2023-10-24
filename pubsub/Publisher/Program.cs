using EasyNetQ;
using Messages;

const string AMQP = "amqps://efwzzuku:7CflikvF4dquXhPcz_lzIcGQJOUJM1Vd@strong-golden-bat.rmq4.cloudamqp.com/efwzzuku";
var bus = RabbitHutch.CreateBus(AMQP);
Console.WriteLine("Press any key to send a message...");
int number = 0;
while(true) {
    Console.ReadKey(true);
    var greeting = new Greeting {
        Message = "Hello!",
        SentAt = DateTimeOffset.Now,
        Number = ++number
    };
    bus.PubSub.Publish(greeting);
    Console.WriteLine(greeting);
}
