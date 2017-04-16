using Discord;
using Discord.WebSocket;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Tweetinvi;
using Tweetinvi.Models;
using Tweetinvi.Streaming;

namespace WazeBotDiscord.Twitter
{
    public class TwitterService
    {
        readonly DiscordSocketClient _client;
        List<IFilteredStream> _streams;

        public TwitterService(DiscordSocketClient client)
        {
            _client = client;
            _streams = new List<IFilteredStream>();
        }

        public async Task InitTwitterServiceAsync()
        {
            var consumerKey = Environment.GetEnvironmentVariable("TWITTER_CONSUMER_KEY");
            var consumerSecret = Environment.GetEnvironmentVariable("TWITTER_CONSUMER_SECRET");
            var accessToken = Environment.GetEnvironmentVariable("TWITTER_ACCESS_TOKEN");
            var accessTokenSecret = Environment.GetEnvironmentVariable("TWITTER_ACCESS_TOKEN_SECRET");

            Auth.SetUserCredentials(consumerKey, consumerSecret, accessToken, accessTokenSecret);

            List<TwitterToCheck> twittersToCheck;

            using (var db = new WbContext())
            {
                twittersToCheck = await db.TwittersToCheck.ToListAsync();
            }

            foreach (var toCheck in twittersToCheck)
            {
                StartStream(toCheck);
            }
        }

        public void StartStream(TwitterToCheck toCheck)
        {
            var channel = _client.GetChannel(toCheck.DiscordChannelId) as SocketTextChannel;
            var stream = Stream.CreateFilteredStream();

            stream.AddFollow(toCheck.UserId);
            foreach (var w in toCheck.RequiredKeywords)
            {
                stream.AddTrack(w);
            }

            stream.MatchingTweetReceived += async (sender, args) =>
            {
                var embed = CreateEmbed(args.Tweet);
                await channel.SendMessageAsync("", embed: embed);
            };

            _streams.Add(stream);

            stream.StartStreamMatchingAllConditionsAsync();
        }

        public void StopAllStreams()
        {
            foreach (var s in _streams)
            {
                s.StopStream();
            }

            _streams.Clear();
        }

        Embed CreateEmbed(ITweet tweet)
        {
            var embed = new EmbedBuilder();

            var author = new EmbedAuthorBuilder
            {
                Name = tweet.CreatedBy.ScreenName + " (" + tweet.CreatedBy.Name + ")",
                Url = tweet.CreatedBy.Url,
                IconUrl = tweet.CreatedBy.ProfileImageUrlHttps
            };

            var footer = new EmbedFooterBuilder
            {
                Text = "Posted on " +
                    tweet.CreatedAt.ToString("MMM d, yyyy") +
                    " at " +
                    tweet.CreatedAt.ToString("H:mm") +
                    " UTC"
            };

            embed.Title = "Go to tweet";
            embed.Description = WebUtility.HtmlDecode(tweet.Text);
            embed.Url = tweet.Url;
            embed.Color = new Color(29, 161, 242);
            embed.Author = author;
            embed.Footer = footer;

            return embed.Build();
        }
    }
}
