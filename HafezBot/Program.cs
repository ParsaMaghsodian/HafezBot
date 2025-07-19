
using HafezBot;
using Microsoft.Extensions.Configuration;


using var cts = new CancellationTokenSource();
var config = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false)
    .Build();
string botToken = config["BotToken"];
string apiUrl = config["ApiUrl"];

var bot = new HafezBotClass(botToken, apiUrl, cts.Token);
await bot.StartAsync();
Console.ReadLine();
cts.Cancel();
