using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using RoR2Checker.Services;

namespace RoR2Checker
{
    public static class Program
    {
        public static async Task Main(string[] args)
        {
            System.AppDomain.CurrentDomain.ProcessExit += (sender, args) => {
                if (Directory.Exists("Temp"))
                    Directory.Delete("Temp");
            };

            using (var services = GetServices())
            {
                if (!Directory.Exists("Temp"))
                    Directory.CreateDirectory("Temp");

                var client = services.GetRequiredService<DiscordSocketClient>();

                client.Log += LogAsync;

                await client.LoginAsync(TokenType.Bot, Environment.GetEnvironmentVariable("CHECKER_TOKEN"));
                await client.StartAsync();

                await services.GetRequiredService<CommandHandler>().InitializeAsync();

                await Task.Delay(-1);
            }
        }

        private static Task LogAsync(LogMessage log) 
        {
            Console.WriteLine($"{log.Severity}: {log.Message}");

            return Task.CompletedTask;
        }

        private static ServiceProvider GetServices() 
        {
            return new ServiceCollection()
                .AddSingleton<DiscordSocketClient>()
                .AddSingleton<CommandService>()
                .AddSingleton<CommandHandler>()
                .BuildServiceProvider();
        }
    }
}
