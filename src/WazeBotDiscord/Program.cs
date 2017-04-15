using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
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
        static bool isDev;

        public static void Main(string[] args)
            => new Program().RunAsync().GetAwaiter().GetResult();

        public async Task RunAsync()
        {
            isDev = !string.IsNullOrEmpty(Environment.GetEnvironmentVariable("WAZEBOT_ISDEV"));

            var token = Environment.GetEnvironmentVariable("DISCORD_API_TOKEN");
            if (token == null)
                throw new ArgumentNullException(nameof(token), "No Discord API token env var found");

            var dbConnString = Environment.GetEnvironmentVariable("WAZEBOT_DB_CONNECTIONSTRING");
            if (dbConnString == null)
                throw new ArgumentNullException(nameof(dbConnString), "No DB connection string env var found");

            var clientConfig = new DiscordSocketConfig
            {
                LogLevel = isDev ? LogSeverity.Info : LogSeverity.Warning
            };

            client = new DiscordSocketClient(clientConfig);
            client.Log += Log;

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