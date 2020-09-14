using ResearchBot.Util;
using Discord;
using Discord.Commands;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using GoogleTranslateFreeApi;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using System;

namespace ResearchBot.Commands.Modules
{
    public class ScholarModule : ModuleBase<SocketCommandContext>
    {
        private Dictionary<string, int> TagCount;
        private Dictionary<string, List<ulong>> UserHistory;

        public JObject ReadJSON(string path)
        {
            using (StreamReader file = File.OpenText(path))
            // Reads the json from the file
            using (JsonTextReader reader = new JsonTextReader(file))
            {
                // Deserialize the JSON into the JObject
                return (JObject)JToken.ReadFrom(reader);
            }
        }

        public void IncreaseTagCoung(string tag)
        {
            if (TagCount == null)
            {
                TagCount = ReadJSON("tagCount.json").ToObject<Dictionary<string, int>>();
            }
            else
            {
                if (!TagCount.ContainsKey(tag))
                {
                    TagCount.Add(tag, 1);
                }
                else
                {
                    TagCount[tag] += 1;
                }

                string json = JsonConvert.SerializeObject(TagCount);
                File.WriteAllText("tagCount.json", json);
            }
        }

        public void AddUserHistory(string tag, ulong user)
        {
            if (UserHistory == null)
            {
                UserHistory = ReadJSON("userHistory.json").ToObject<Dictionary<string, List<ulong>>>();
            }
            else
            {
                if (!UserHistory.ContainsKey(tag))
                {
                    UserHistory.Add(tag, new List<ulong>());
                }
                UserHistory[tag].Add(user);

                string json = JsonConvert.SerializeObject(UserHistory);
                File.WriteAllText("userHistory.json", json);
            }
        }

        [Command("scholar")]
        [Summary("Sends a Google Scholar search link.")]
        public Task ScholarAsync([Remainder][Summary("The search query")] string query)
        {
            string mentions = String.Empty;
            List<long> userHis = new List<long>();

            foreach (string tag in query.Split(" "))
            {
                AddUserHistory(tag, Context.User.Id);

                if (UserHistory.ContainsKey(tag))
                {
                    foreach (long idTag in UserHistory[tag])
                    {
                        if (!userHis.Contains(idTag))
                            userHis.Add(idTag);
                    }
                }
            }

            foreach (long ids in userHis)
            {
                mentions += $"<@{ids}> ";
            }

            GoogleTranslator translator = new GoogleTranslator();

            TranslationResult portuguese = translator.TranslateLiteAsync(query, Language.English, Language.Portuguese).GetAwaiter().GetResult();
            TranslationResult english = translator.TranslateLiteAsync(query, Language.Portuguese, Language.English).GetAwaiter().GetResult();

            return ReplyAsync($"Pesquisa em portugues: https://scholar.google.com/scholar?q={portuguese.MergedTranslation.Replace("+", "%2B").Replace(' ', '+')} \nEnglish search: https://scholar.google.com/scholar?q={english.MergedTranslation.Replace("+", "%2B").Replace(' ', '+')}\n Outra pessoa que tambem pesquisou: {mentions}");
        }

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
        public Task GoogleAsync([Remainder][Summary("The thing to search for")] string search)
        {
            foreach (string tag in search.Split(" "))
            {
                IncreaseTagCoung(tag);
            }

            GoogleTranslator translator = new GoogleTranslator();

            TranslationResult portuguese = translator.TranslateLiteAsync(search, Language.English, Language.Portuguese).GetAwaiter().GetResult();
            TranslationResult english = translator.TranslateLiteAsync(search, Language.Portuguese, Language.English).GetAwaiter().GetResult();

            return ReplyAsync($"Pesquisa em portugues: https://www.google.com/search?q={portuguese.MergedTranslation.Replace("+", "%2B").Replace(' ', '+')} \nEnglish search: https://www.google.com/search?q={english.MergedTranslation.Replace("+", "%2B").Replace(' ', '+')}");
        }
    }
}