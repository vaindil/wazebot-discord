using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using WazeBotDiscord.Autoreplies;

namespace WazeBotDiscord
{
    public class Program
    {
        DiscordSocketClient client;
        CommandService commands;
        DependencyMap map;
        AutoreplyService autoreplyService;

        public static void Main(string[] args)
            => new Program().RunAsync().GetAwaiter().GetResult();

        public async Task RunAsync()
        {
            var token = Environment.GetEnvironmentVariable("DISCORD_API_TOKEN");
            if (token == null)
                throw new ArgumentNullException(nameof(token), "No Discord API token env var found");

            client = new DiscordSocketClient();

            var commandsConfig = new CommandServiceConfig
            {
                CaseSensitiveCommands = false
            };

            autoreplyService = new AutoreplyService();
            await autoreplyService.InitAutoreplyServiceAsync();

            commands = new CommandService(commandsConfig);
            map = new DependencyMap();
            map.Add(autoreplyService);
            await InstallCommands();

            client.Log += Log;

            await client.LoginAsync(TokenType.Bot, token);
            await client.StartAsync();

            client.Ready += async () =>
            {
                await client.SetGameAsync("with junction boxes");
            };

            client.MessageReceived += HandleAutoreply;

            await Task.Delay(-1);
        }

        async Task HandleAutoreply(SocketMessage msg)
        {
            await AutoreplyHandler.HandleAutoreplyAsync(msg, autoreplyService);
        }

        public async Task InstallCommands()
        {
            client.MessageReceived += HandleCommand;
            await commands.AddModulesAsync(Assembly.GetEntryAssembly());
        }

        public async Task HandleCommand(SocketMessage messageParam)
        {
            var message = messageParam as SocketUserMessage;
            if (message == null)
                return;

            int argPos = 0;
            if (!(message.HasCharPrefix('!', ref argPos) || message.HasMentionPrefix(client.CurrentUser, ref argPos)))
                return;

            var context = new CommandContext(client, message);
            var result = await commands.ExecuteAsync(context, argPos, map);
            if (!result.IsSuccess && result.Error == CommandError.UnmetPrecondition)
            {
                await context.Channel.SendMessageAsync(result.ErrorReason);
            }
        }

        Task Log(LogMessage msg)
        {
            Console.WriteLine(msg.ToString());
            return Task.CompletedTask;
        }
    }
}