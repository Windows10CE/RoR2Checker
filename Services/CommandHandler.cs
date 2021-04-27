using System;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;

namespace RoR2Checker.Services
{
    public class CommandHandler 
    {
        IServiceProvider _provider;
        DiscordSocketClient _discord;
        CommandService _command;

        public CommandHandler(IServiceProvider provider, DiscordSocketClient discord, CommandService command) 
        {
            _provider = provider;
            _discord = discord;
            _command = command;

            _discord.MessageReceived += MessageReceiveAsync;
        }

        public async Task InitializeAsync() 
        {
            await _command.AddModulesAsync(typeof(CommandHandler).Assembly, _provider);
        }

        private async Task MessageReceiveAsync(SocketMessage msg) 
        {
            if (msg is SocketUserMessage usrMsg && usrMsg.Source == MessageSource.User) 
            {
                int argPos = 0;

                if (usrMsg.HasCharPrefix('^', ref argPos)) 
                {
                    var result = await _command.ExecuteAsync(new SocketCommandContext(_discord, usrMsg), argPos, _provider);
                    if (!result.IsSuccess)
                        await usrMsg.Channel.SendMessageAsync($"{result.Error}: {result.ErrorReason}");
                }
            }
        }
    }
}