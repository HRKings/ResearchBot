using Discord;
using Discord.Commands;
using Discord.WebSocket;
using ResearchBot.Commands;
using ResearchBot.Util;
using System;
using System.Threading.Tasks;

namespace ResearchBot
{
    internal class ResearchBot
    {
        /// <summary>
        /// SocketClient is the type of connection the bot makes to Discord's API
        /// </summary>
        private DiscordSocketClient _client;

        private CommandHandler _commandHandler;
        private CommandService _commandService;

        /// <summary>
        /// The main method of any program, redirected to the asynchronous version
        /// </summary>
        private static void Main(string[] args) => new ResearchBot().MainAsync().GetAwaiter().GetResult();

        /// <summary>
        /// The async version of the main method
        /// </summary>
        /// <returns></returns>
        public async Task MainAsync()
        {
            // Instantiate Discord Socket Client and hook the logging method
            _client = new DiscordSocketClient();
            _client.Log += Log;

            // Pass the type of the program as bot and reads the token from a JSON file
            await _client.LoginAsync(TokenType.Bot, JsonWrapper.ReadJSON(@"E:\College\Repo\ResearchBot\ResearchBot\Resources\auth.json").Value<string>("token"));
            await _client.StartAsync();

            // Activate the commands and modules
            _commandService = new CommandService();
            _commandHandler = new CommandHandler(_client, _commandService, '|');
            _commandHandler.MessageOnError = true;

            await _commandHandler.InstallCommandsAsync();

            _client.Ready += OnReady;
            //_client.MessageReceived += OnMessageReceived;

            // Block this task until the bot is closed (prevent the main method from closing upon completing one task)
            await Task.Delay(-1);
        }

        private Task OnReady()
        {
            // Prints to the console all the servers the bot is connected to
            Log(LogSeverity.Info, $"Connected to these servers as '{_client.CurrentUser.Username}': ");

            foreach (var guild in _client.Guilds)
            {
                Log(LogSeverity.Info, $"- {guild.Name}");
            }

            _client.SetGameAsync("Helping with scientific researches", null, ActivityType.CustomStatus);
            Log(LogSeverity.Info, $"Actiity set to '{_client.Activity.Type} {_client.Activity.Name}'");

            return Task.CompletedTask;
        }

        /// <summary>
        /// The logging method of the bot
        /// </summary>
        /// <param name="msg">A discord log message</param>
        /// <returns>A completed task</returns>
        private static Task Log(LogMessage msg)
        {
            Console.WriteLine(msg.ToString());
            return Task.CompletedTask;
        }

        public static Task Log(LogSeverity logSeverity, string message, string source = "Debugging") => Log(new LogMessage(logSeverity, source, message));
    }
}