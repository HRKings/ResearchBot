using ResearchBot.Util;
using Discord;
using Discord.Commands;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace ResearchBot.Commands.Modules
{
    public class ScholarModule : ModuleBase<SocketCommandContext>
    {
        [Command("scholar")]
        [Summary("Sends a Google Scholar search link.")]
        public Task ScholarAsync([Remainder] [Summary("The search query")] string query) => ReplyAsync($"https://scholar.google.com/scholar?q={query.Replace(' ', '+')}");

        [Command("suggestion")]
        [Summary("Sends a random programming project to the user")]
        public Task SuggestionAsync()
        {
            EmbedBuilder suggest = new EmbedBuilder();

            JObject json = JsonWrapper.JsonChooseRandomProject(@"E:\College\Repo\ResearchBot\ResearchBot\Resources\projectSuggestion.json");

            suggest.Title = json.Value<string>("name");
            suggest.Description = json.Value<string>("description");

            return ReplyAsync("A new project to try can be:", embed: suggest.Build());
        }

        [Command("google")]
        [Summary("Google it !")]
        public Task GoogleAsync([Remainder] [Summary("The thing to search for")] string search) => ReplyAsync($"https://www.google.com/search?q={search.Replace(' ', '+')}");
    }
}