﻿namespace Messages;

public class Greeting {
    public string Message {get;set; }
    public DateTimeOffset SentAt { get; set; }
    public Number Number { get; set; }
    public override string ToString() 
        => $"{Number} at {SentAt}: {Message}";
}
