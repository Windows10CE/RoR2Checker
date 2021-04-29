using System;
using System.Linq;
using System.Threading.Tasks;
using Discord.Commands;
using Discord.WebSocket;

namespace RoR2Checker.Modules.Preconditions
{
    public class ThunderstoreModeratorAttribute : PreconditionAttribute
    {
        public override Task<PreconditionResult> CheckPermissionsAsync(ICommandContext context, CommandInfo command, IServiceProvider services)
        {
            if (context.User is SocketGuildUser user)
            {
                return user.Roles.Any(x => x.Id == 562759923100942367)
                    ? Task.FromResult(PreconditionResult.FromSuccess())
                    : Task.FromResult(PreconditionResult.FromError("Only moderators can use this command"));
            }
            return Task.FromResult(PreconditionResult.FromError("This command must be used in a guild"));
        }
    }
}
