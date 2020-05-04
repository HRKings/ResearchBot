using ResearchBot.Util;
using Discord;
using Discord.Commands;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace ResearchBot.Commands.Modules
{
    public class BasicModule : ModuleBase<SocketCommandContext>
    {
        [Command("say")]
        [Summary("Echoes a message.")]
        public Task SayAsync([Remainder] [Summary("The text to echo")] string echo) => ReplyAsync(echo);

        [Command("sayd")]
        [Summary("Echoes a detailed message.")]
        public async Task SaysAsync([Remainder] [Summary("The text to echo")] string echo)
        {
            foreach (var tag in Context.Message.Tags)
            {
                await ReplyAsync($"{tag.Type.ToString()} {tag.Key.ToString()} {tag.Value.ToString()}");

                if (tag.Type == TagType.Emoji)
                    await ReplyAsync(Emote.Parse($"<:{(tag.Value as Emote).Name}:{tag.Key.ToString()}>").Url);
            }
        }

        [Command("choose")]
        [Summary("Give it options and it will choose")]
        public Task ChooseAsync([Summary("The options to choose")] params string[] options) => ReplyAsync(Utils.Choose(options));
    }
}