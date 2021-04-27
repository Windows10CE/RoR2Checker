using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;

namespace RoR2Checker.Commands 
{
    public class PingModule : ModuleBase<SocketCommandContext>
    {
        [Command("ping")]
        public async Task PingAsync() 
        {
            await ReplyAsync("Pong!");
        }
    }
}
