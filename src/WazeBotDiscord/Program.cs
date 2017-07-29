using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
using WazeBotDiscord.Autoreplies;
using WazeBotDiscord.Events;
using WazeBotDiscord.Glossary;
using WazeBotDiscord.Keywords;
using WazeBotDiscord.Lookup;
using WazeBotDiscord.MultiPermissionChannels;
using WazeBotDiscord.Twitter;

namespace WazeBotDiscord
{
    public class Program
    {
        DiscordSocketClient client;
        CommandService commands;
        IServiceProvider services;
        static HttpClient httpClient;
        bool isDev;

        public static void Main(string[] args)
            => new Program().RunAsync().GetAwaiter().GetResult();

        public async Task RunAsync()
        {
            isDev = !string.IsNullOrEmpty(Environment.GetEnvironmentVariable("WAZEBOT_ISDEV"));

            var token = Environment.GetEnvironmentVariable("DISCORD_API_TOKEN");
            if (token == null)
                throw new ArgumentNullException(nameof(token), "No Discord API token env var found");

            VerifyEnvironmentVariables();

            var clientConfig = new DiscordSocketConfig
            {
                LogLevel = isDev ? LogSeverity.Info : LogSeverity.Warning,
                AlwaysDownloadUsers = true
            };

            client = new DiscordSocketClient(clientConfig);
            client.Log += Log;

            var commandsConfig = new CommandServiceConfig
            {
                CaseSensitiveCommands = false
            };

            commands = new CommandService(commandsConfig);
            httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("WazeBotDiscord/1.0");

            var autoreplyService = new AutoreplyService();
            await autoreplyService.InitAutoreplyServiceAsync();

            var keywordService = new KeywordService();
            await keywordService.InitKeywordServiceAsync();

            var glossaryService = new GlossaryService(httpClient);
            await glossaryService.InitAsync();

            var lookupService = new LookupService(httpClient);
            await lookupService.InitAsync();

            var mpcService = new MultiPermissionChannelService();
            await mpcService.InitAsync();

            var serviceCollection = new ServiceCollection();
            serviceCollection.AddSingleton(commands);
            serviceCollection.AddSingleton(autoreplyService);
            serviceCollection.AddSingleton(keywordService);
            serviceCollection.AddSingleton(lookupService);
            serviceCollection.AddSingleton(glossaryService);
            serviceCollection.AddSingleton(mpcService);
            serviceCollection.AddSingleton(httpClient);

            client.Ready += async () => await client.SetGameAsync("with junction boxes");

            var twitterService = new TwitterService(client);
            serviceCollection.AddSingleton(twitterService);

            client.Connected += async () => await twitterService.InitTwitterServiceAsync();

            client.Disconnected += (ex) =>
            {
                twitterService.StopAllStreams();
                return Task.CompletedTask;
            };

            services = serviceCollection.BuildServiceProvider();

            client.MessageReceived += async (SocketMessage msg) =>
                await AutoreplyHandler.HandleAutoreplyAsync(msg, autoreplyService);

            client.MessageReceived += async (SocketMessage msg) =>
                await KeywordHandler.HandleKeywordAsync(msg, keywordService, client);

            client.UserJoined += async (SocketGuildUser user) => await UserJoinedRoleSyncEvent.SyncRoles(user, client);

            client.GuildMemberUpdated += async (SocketGuildUser unused, SocketGuildUser user) =>
                await UserRoleMpcEvent.GrantPermissionsAsync(user, client, mpcService);

            await InstallCommands();

            await client.LoginAsync(TokenType.Bot, token);
            await client.StartAsync();

            await Task.Delay(-1);
        }

        public async Task InstallCommands()
        {
            client.MessageReceived += HandleCommand;
            await commands.AddModulesAsync(Assembly.GetEntryAssembly());
        }

        public async Task HandleCommand(SocketMessage messageParam)
        {
            var message = messageParam as SocketUserMessage;
            if (message == null || message.Author.Id == client.CurrentUser.Id)
                return;

            if (isDev)
            {
                var appInfo = await client.GetApplicationInfoAsync();
                if (message.Author.Id != appInfo.Owner.Id)
                    return;
            }

            int argPos = 0;
            if (!(message.HasCharPrefix('!', ref argPos) || message.HasMentionPrefix(client.CurrentUser, ref argPos)))
                return;

            var context = new CommandContext(client, message);
            var result = await commands.ExecuteAsync(context, argPos, services);
            if (!result.IsSuccess && result.Error == CommandError.UnmetPrecondition)
            {
                await context.Channel.SendMessageAsync(result.ErrorReason);
            }
            else if (!result.IsSuccess && result.Error == CommandError.BadArgCount)
            {
                await context.Channel.SendMessageAsync($"{context.Message.Author.Mention}: You didn't specify the right " +
                    "parameters. If you're using a role command, you probably forgot to specify the user.");
            }
        }

        Task Log(LogMessage msg)
        {
            Console.WriteLine(msg.ToString());
            return Task.CompletedTask;
        }

        void VerifyEnvironmentVariables()
        {
            if (Environment.GetEnvironmentVariable("WAZEBOT_DB_CONNECTIONSTRING") == null)
                throw new ArgumentNullException("DB connection string env var not found", innerException: null);

            if (Environment.GetEnvironmentVariable("TWITTER_CONSUMER_KEY") == null)
                throw new ArgumentNullException("Twitter consumer key env var not found", innerException: null);

            if (Environment.GetEnvironmentVariable("TWITTER_CONSUMER_SECRET") == null)
                throw new ArgumentNullException("Twitter consumer secret env var not found", innerException: null);

            if (Environment.GetEnvironmentVariable("TWITTER_ACCESS_TOKEN") == null)
                throw new ArgumentNullException("Twitter access token env var not found", innerException: null);

            if (Environment.GetEnvironmentVariable("TWITTER_ACCESS_TOKEN_SECRET") == null)
                throw new ArgumentNullException("Twitter access token secret env var not found", innerException: null);
        }
    }
}
