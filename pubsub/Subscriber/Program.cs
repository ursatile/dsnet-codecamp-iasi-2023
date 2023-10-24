using EasyNetQ;
using Messages;

const string AMQP = "amqps://efwzzuku:7CflikvF4dquXhPcz_lzIcGQJOUJM1Vd@strong-golden-bat.rmq4.cloudamqp.com/efwzzuku";
var bus = RabbitHutch.CreateBus(AMQP);
var subscriptionId = "subscriber";
await bus.PubSub.SubscribeAsync<Greeting>(subscriptionId,
    Console.WriteLine
);

Console.WriteLine("Listening for messages... press Ctrl-C to quit");
Console.ReadKey(true);