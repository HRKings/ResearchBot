using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;

namespace ResearchBot.Commands
{
    public class CommandHandler
    {
        // Initial constructors
        private readonly DiscordSocketClient _client;

        private readonly CommandService _service;

        /// <summary>
        /// The prefix used by the bot
        /// </summary>
        public char Prefix { get; protected set; }

        /// <summary>
        /// If the bot should log the error of commands in the console
        /// </summary>
        public bool MessageOnError { get; set; }

        public CommandHandler(DiscordSocketClient client, CommandService commands, char prefix)
        {
            this._client = client;
            this._service = commands;
            this.Prefix = prefix;
            this.MessageOnError = false;
        }

        /// <summary>
        /// This method initialize all the handlers and commands, it must be called in the main method of the bot after instantiating the DiscordSocketClient
        /// </summary>
        public async Task InstallCommandsAsync()
        {
            // This adds an event handler to the MessageRecceived and pass it to a command handler
            _client.MessageReceived += HandleCommandAsync;

            // Adds the modules from the main assembly and register them
            await _service.AddModulesAsync(assembly: Assembly.GetEntryAssembly(), services: null);
        }

        private async Task HandleCommandAsync(SocketMessage socketMessage)
        {
            // Don't process system messages
            SocketUserMessage message = socketMessage as SocketUserMessage;

            // Don't process null messages
            if (message == null)
            {
                return;
            }

            // Track where the prefix ends and the command begins
            int argumentPos = 0;

            if (message.Content == _client.CurrentUser.Mention)
            {
                await message.Channel.SendMessageAsync("Hey use the |help command to see my command list");
                return;
            }

            // Make sure that the commands wasn't issued by a bot and that it has the prefix
            if (!(message.HasCharPrefix(this.Prefix, ref argumentPos) || message.HasMentionPrefix(_client.CurrentUser, ref argumentPos) || message.Author.IsBot))
            {
                return;
            }

            if (message.Content == "|help")
            {
                await HelpAsync(message.Channel);
                return;
            }

            // Create a WebSocket based command context based on the message
            SocketCommandContext commandContext = new SocketCommandContext(_client, message);

            // The result is just an object stating that the command was executed successfully
            var result = await _service.ExecuteAsync(context: commandContext, argPos: argumentPos, services: null);

            // Optional method for giving the user an error message (disabled by default to prevent spam)
            if (this.MessageOnError && !result.IsSuccess)
            {
                //await commandContext.Channel.SendMessageAsync(result.ErrorReason);
                Console.WriteLine($"The command {message.ToString()} from {message.Author} , failed because: {result.ErrorReason}");
            }
        }

        public async Task HelpAsync(ISocketMessageChannel channel)
        {
            IEnumerable<CommandInfo> commands = _service.Commands;
            EmbedBuilder embedBuilder = new EmbedBuilder();

            foreach (CommandInfo command in commands)
            {
                // Get the command Summary attribute information
                string embedFieldText = command.Summary ?? "No description available\n";

                embedBuilder.AddField(command.Name, embedFieldText);
            }

            await channel.SendMessageAsync("Here's a list of commands and their description: ", false, embedBuilder.Build());
        }
    }
}