﻿// See https://aka.ms/new-console-template for more information

using Engine;

Console.WriteLine("What is your name?");
var username = Console.ReadLine();

var bot = new TravelBot.TravelBot();
var engine = new ChatEngine(new InMemoryChatContextStorage());
while (true)
{
    var input = Console.ReadLine();
    var output = await engine.Perform(bot, input, username);
    Console.WriteLine(output.Text);
}