using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System.Threading.Tasks;
using WazeBotDiscord.Classes.Roles;
using WazeBotDiscord.Utilities;

namespace WazeBotDiscord.Modules
{
    public class RoleSyncModule : ModuleBase
    {
        [Command("cm", RunMode = RunMode.Async)]
        [Alias("countrymanager")]
        [RequireCmOrAbove]
        public async Task ToggleCm(IUser user)
        {
            if (IsSelf(user))
            {
                await ReplyAsync("You can't change this role for yourself.");
                return;
            }

            var msg = await ReplyAsync($"{user.Mention}: Just a moment...");

            var result = await RoleSyncHelpers.ToggleSyncedRolesAsync(user, CountryManager.Ids, Context);

            if (result == SyncedRoleStatus.Added)
            {
                await RoleSyncHelpers.RemoveSyncedRolesAsync((SocketGuildUser)user, LargeAreaManager.Ids, Context.Client);
                await RoleSyncHelpers.RemoveSyncedRolesAsync((SocketGuildUser)user, AreaManager.Ids, Context.Client);

                await msg.ModifyAsync(m => m.Content = $"{user.Mention}: Added CM, removed LAM and AM (if applicable).");
            }
            else if (result == SyncedRoleStatus.Removed)
            {
                await msg.ModifyAsync(m => m.Content = $"{user.Mention}: Removed CM.");
            }
        }

        [Command("sm", RunMode = RunMode.Async)]
        [Alias("statemanager")]
        [RequireSmOrAbove]
        public async Task ToggleSm(IUser user)
        {
            if (IsSelf(user))
            {
                await ReplyAsync("You can't change this role for yourself.");
                return;
            }

            var msg = await ReplyAsync($"{user.Mention}: Just a moment...");

            var result = await RoleSyncHelpers.ToggleSyncedRolesAsync(user, StateManager.Ids, Context);

            if (result == SyncedRoleStatus.Added)
            {
                await RoleSyncHelpers.RemoveSyncedRolesAsync((SocketGuildUser)user, LargeAreaManager.Ids, Context.Client);
                await RoleSyncHelpers.RemoveSyncedRolesAsync((SocketGuildUser)user, AreaManager.Ids, Context.Client);

                await msg.ModifyAsync(m => m.Content = $"{user.Mention}: Added SM, removed LAM and AM (if applicable).");
            }
            else if (result == SyncedRoleStatus.Removed)
            {
                await msg.ModifyAsync(m => m.Content = $"{user.Mention}: Removed SM.");
            }
        }

        [Command("lam", RunMode = RunMode.Async)]
        [Alias("largeareamanager")]
        [RequireSmOrAbove]
        public async Task ToggleLam(IUser user)
        {
            if (IsSelf(user))
            {
                await ReplyAsync("You can't change this role for yourself.");
                return;
            }

            var msg = await ReplyAsync($"{user.Mention}: Just a moment...");

            var result = await RoleSyncHelpers.ToggleSyncedRolesAsync(user, LargeAreaManager.Ids, Context);

            if (result == SyncedRoleStatus.Added)
            {
                await RoleSyncHelpers.RemoveSyncedRolesAsync((SocketGuildUser)user, StateManager.Ids, Context.Client);
                await RoleSyncHelpers.RemoveSyncedRolesAsync((SocketGuildUser)user, AreaManager.Ids, Context.Client);

                // some guilds have LAM and AM, some only have AM. have to do this to manage that.
                await RoleSyncHelpers.AddSyncedRolesAsync((SocketGuildUser)user, LargeAreaManager.Ids, Context.Client);

                await msg.ModifyAsync(m => m.Content = $"{user.Mention}: Added LAM, removed SM and AM (if applicable).");
            }
            else if (result == SyncedRoleStatus.Removed)
            {
                await msg.ModifyAsync(m => m.Content = $"{user.Mention}: Removed LAM.");
            }
        }

        [Command("am", RunMode = RunMode.Async)]
        [Alias("areamanager")]
        [RequireSmOrAbove]
        public async Task ToggleAm(IUser user)
        {
            if (IsSelf(user))
            {
                await ReplyAsync("You can't change this role for yourself.");
                return;
            }

            var msg = await ReplyAsync($"{user.Mention}: Just a moment...");

            var result = await RoleSyncHelpers.ToggleSyncedRolesAsync(user, AreaManager.Ids, Context);

            if (result == SyncedRoleStatus.Added)
            {
                await RoleSyncHelpers.RemoveSyncedRolesAsync((SocketGuildUser)user, StateManager.Ids, Context.Client);
                await RoleSyncHelpers.RemoveSyncedRolesAsync((SocketGuildUser)user, LargeAreaManager.Ids, Context.Client);

                // some guilds have LAM and AM, some only have AM. have to do this to manage that.
                await RoleSyncHelpers.AddSyncedRolesAsync((SocketGuildUser)user, AreaManager.Ids, Context.Client);

                await msg.ModifyAsync(m => m.Content = $"{user.Mention}: Added AM, removed SM and LAM (if applicable).");
            }
            else if (result == SyncedRoleStatus.Removed)
            {
                await msg.ModifyAsync(m => m.Content = $"{user.Mention}: Removed AM.");
            }
        }

        [Command("mentor", RunMode = RunMode.Async)]
        [RequireSmOrAbove]
        public async Task ToggleMentor(IUser user)
        {
            if (IsSelf(user))
            {
                await ReplyAsync("You can't change this role for yourself.");
                return;
            }

            var msg = await ReplyAsync($"{user.Mention}: Just a moment...");

            var result = await RoleSyncHelpers.ToggleSyncedRolesAsync(user, Mentor.Ids, Context);

            if (result == SyncedRoleStatus.Added)
                await msg.ModifyAsync(m => m.Content = $"{user.Mention}: Added mentor.");
            else if (result == SyncedRoleStatus.Removed)
                await msg.ModifyAsync(m => m.Content = $"{user.Mention}: Removed mentor.");
        }

        [Command("l6", RunMode = RunMode.Async)]
        [Alias("level6", "r6", "rank6")]
        [RequireChampInNationalGuild]
        public async Task ToggleL6(IUser user)
        {
            if (IsSelf(user))
            {
                await ReplyAsync("You can't change this role for yourself.");
                return;
            }

            var msg = await ReplyAsync($"{user.Mention}: Just a moment...");

            var result = await RoleSyncHelpers.ToggleSyncedRolesAsync(user, Level6.Ids, Context);

            if (result == SyncedRoleStatus.Added)
            {
                await RoleSyncHelpers.RemoveSyncedRolesAsync((SocketGuildUser)user, Level5.Ids, Context.Client);
                await RoleSyncHelpers.RemoveSyncedRolesAsync((SocketGuildUser)user, Level4.Ids, Context.Client);
                await RoleSyncHelpers.RemoveSyncedRolesAsync((SocketGuildUser)user, Level3.Ids, Context.Client);
                await RoleSyncHelpers.RemoveSyncedRolesAsync((SocketGuildUser)user, Level2.Ids, Context.Client);
                await RoleSyncHelpers.RemoveSyncedRolesAsync((SocketGuildUser)user, Level1.Ids, Context.Client);

                await msg.ModifyAsync(m => m.Content = $"{user.Mention}: Added L6, removed other level roles.");
            }
            else if (result == SyncedRoleStatus.Removed)
            {
                await msg.ModifyAsync(m => m.Content = $"{user.Mention}: Removed L6.");
            }
        }

        [Command("l5", RunMode = RunMode.Async)]
        [Alias("level5", "r5", "rank5")]
        [RequireSmOrAbove]
        public async Task ToggleL5(IUser user)
        {
            if (IsSelf(user))
            {
                await ReplyAsync("You can't change this role for yourself.");
                return;
            }

            var msg = await ReplyAsync($"{user.Mention}: Just a moment...");

            var result = await RoleSyncHelpers.ToggleSyncedRolesAsync(user, Level5.Ids, Context);

            if (result == SyncedRoleStatus.Added)
            {
                await RoleSyncHelpers.RemoveSyncedRolesAsync((SocketGuildUser)user, Level6.Ids, Context.Client);
                await RoleSyncHelpers.RemoveSyncedRolesAsync((SocketGuildUser)user, Level4.Ids, Context.Client);
                await RoleSyncHelpers.RemoveSyncedRolesAsync((SocketGuildUser)user, Level3.Ids, Context.Client);
                await RoleSyncHelpers.RemoveSyncedRolesAsync((SocketGuildUser)user, Level2.Ids, Context.Client);
                await RoleSyncHelpers.RemoveSyncedRolesAsync((SocketGuildUser)user, Level1.Ids, Context.Client);

                await msg.ModifyAsync(m => m.Content = $"{user.Mention}: Added L5, removed other level roles.");
            }
            else if (result == SyncedRoleStatus.Removed)
            {
                await msg.ModifyAsync(m => m.Content = $"{user.Mention}: Removed L5.");
            }
        }

        [Command("l4", RunMode = RunMode.Async)]
        [Alias("level4", "r4", "rank4")]
        [RequireSmOrAbove]
        public async Task ToggleL4(IUser user)
        {
            if (IsSelf(user))
            {
                await ReplyAsync("You can't change this role for yourself.");
                return;
            }

            var msg = await ReplyAsync($"{user.Mention}: Just a moment...");

            var result = await RoleSyncHelpers.ToggleSyncedRolesAsync(user, Level4.Ids, Context);

            if (result == SyncedRoleStatus.Added)
            {
                await RoleSyncHelpers.RemoveSyncedRolesAsync((SocketGuildUser)user, Level6.Ids, Context.Client);
                await RoleSyncHelpers.RemoveSyncedRolesAsync((SocketGuildUser)user, Level5.Ids, Context.Client);
                await RoleSyncHelpers.RemoveSyncedRolesAsync((SocketGuildUser)user, Level3.Ids, Context.Client);
                await RoleSyncHelpers.RemoveSyncedRolesAsync((SocketGuildUser)user, Level2.Ids, Context.Client);
                await RoleSyncHelpers.RemoveSyncedRolesAsync((SocketGuildUser)user, Level1.Ids, Context.Client);

                await msg.ModifyAsync(m => m.Content = $"{user.Mention}: Added L4, removed other level roles.");
            }
            else if (result == SyncedRoleStatus.Removed)
            {
                await msg.ModifyAsync(m => m.Content = $"{user.Mention}: Removed L4.");
            }
        }

        [Command("l3", RunMode = RunMode.Async)]
        [Alias("level3", "r3", "rank3")]
        public async Task ToggleL3User()
        {
            await ToggleL3(Context.Message.Author);
        }

        [Command("l3", RunMode = RunMode.Async)]
        [Alias("level3", "r3", "rank3")]
        [RequireSmOrAbove]
        public async Task ToggleL3(IUser user)
        {
            var msg = await ReplyAsync($"{user.Mention}: Just a moment...");

            var result = await RoleSyncHelpers.ToggleSyncedRolesAsync(user, Level3.Ids, Context);

            if (result == SyncedRoleStatus.Added)
            {
                await RoleSyncHelpers.RemoveSyncedRolesAsync((SocketGuildUser)user, Level6.Ids, Context.Client);
                await RoleSyncHelpers.RemoveSyncedRolesAsync((SocketGuildUser)user, Level5.Ids, Context.Client);
                await RoleSyncHelpers.RemoveSyncedRolesAsync((SocketGuildUser)user, Level4.Ids, Context.Client);
                await RoleSyncHelpers.RemoveSyncedRolesAsync((SocketGuildUser)user, Level2.Ids, Context.Client);
                await RoleSyncHelpers.RemoveSyncedRolesAsync((SocketGuildUser)user, Level1.Ids, Context.Client);

                await msg.ModifyAsync(m => m.Content = $"{user.Mention}: Added L3, removed other level roles.");
            }
            else if (result == SyncedRoleStatus.Removed)
            {
                await msg.ModifyAsync(m => m.Content = $"{user.Mention}: Removed L3.");
            }
        }

        [Command("l2", RunMode = RunMode.Async)]
        [Alias("level2", "r2", "rank2")]
        public async Task ToggleL2User()
        {
            await ToggleL2(Context.Message.Author);
        }

        [Command("l2", RunMode = RunMode.Async)]
        [Alias("level2", "r2", "rank2")]
        [RequireSmOrAbove]
        public async Task ToggleL2(IUser user)
        {
            var msg = await ReplyAsync($"{user.Mention}: Just a moment...");

            var result = await RoleSyncHelpers.ToggleSyncedRolesAsync(user, Level2.Ids, Context);

            if (result == SyncedRoleStatus.Added)
            {
                await RoleSyncHelpers.RemoveSyncedRolesAsync((SocketGuildUser)user, Level6.Ids, Context.Client);
                await RoleSyncHelpers.RemoveSyncedRolesAsync((SocketGuildUser)user, Level5.Ids, Context.Client);
                await RoleSyncHelpers.RemoveSyncedRolesAsync((SocketGuildUser)user, Level4.Ids, Context.Client);
                await RoleSyncHelpers.RemoveSyncedRolesAsync((SocketGuildUser)user, Level3.Ids, Context.Client);
                await RoleSyncHelpers.RemoveSyncedRolesAsync((SocketGuildUser)user, Level1.Ids, Context.Client);

                await msg.ModifyAsync(m => m.Content = $"{user.Mention}: Added L2, removed other level roles.");
            }
            else if (result == SyncedRoleStatus.Removed)
            {
                await msg.ModifyAsync(m => m.Content = $"{user.Mention}: Removed L2.");
            }
        }

        [Command("l1", RunMode = RunMode.Async)]
        [Alias("level1", "r1", "rank1")]
        public async Task ToggleL1User()
        {
            await ToggleL1(Context.Message.Author);
        }

        [Command("l1", RunMode = RunMode.Async)]
        [Alias("level1", "r1", "rank1")]
        [RequireSmOrAbove]
        public async Task ToggleL1(IUser user)
        {
            var msg = await ReplyAsync($"{user.Mention}: Just a moment...");

            var result = await RoleSyncHelpers.ToggleSyncedRolesAsync(user, Level1.Ids, Context);

            if (result == SyncedRoleStatus.Added)
            {
                await RoleSyncHelpers.RemoveSyncedRolesAsync((SocketGuildUser)user, Level6.Ids, Context.Client);
                await RoleSyncHelpers.RemoveSyncedRolesAsync((SocketGuildUser)user, Level5.Ids, Context.Client);
                await RoleSyncHelpers.RemoveSyncedRolesAsync((SocketGuildUser)user, Level4.Ids, Context.Client);
                await RoleSyncHelpers.RemoveSyncedRolesAsync((SocketGuildUser)user, Level3.Ids, Context.Client);
                await RoleSyncHelpers.RemoveSyncedRolesAsync((SocketGuildUser)user, Level2.Ids, Context.Client);

                await msg.ModifyAsync(m => m.Content = $"{user.Mention}: Added L1, removed other level roles.");
            }
            else if (result == SyncedRoleStatus.Removed)
            {
                await msg.ModifyAsync(m => m.Content = $"{user.Mention}: Removed L1.");
            }
        }

        bool IsSelf(IUser target)
        {
            return Context.Message.Author == target && Context.Message.Author.Id != 132714099241910273;
        }
    }
}
