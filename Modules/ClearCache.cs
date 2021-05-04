using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Discord.Commands;
using RoR2Checker.Modules.Preconditions;

namespace RoR2Checker.Modules
{
    public class ClearCacheModule : ModuleBase<SocketCommandContext>
    {
        [Command("clearcache")]
        [RequireOwner(Group = "Auth")]
        [ThunderstoreModerator(Group = "Auth")]
        public async Task ClearCache()
        {
            foreach (var file in Directory.GetFiles("Temp", "*").Concat(Directory.GetFiles("Cache", "*")))
                File.Delete(file);

            await ReplyAsync("Cleared cache directories");

            return;
        }
    }
}
