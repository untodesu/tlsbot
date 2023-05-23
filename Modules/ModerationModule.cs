// SPDX-License-Identifier: MPL-2.0
// Copyright (c) 2023, Kirill GPRB
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.
using Discord;
using Discord.Interactions;
using Discord.WebSocket;

namespace TLS.Modules
{
    public class ModerationModule : InteractionModuleBase<SocketInteractionContext>
    {
        [SlashCommand("ban", "Ban the specified user")]
        [RequireBotPermission(GuildPermission.BanMembers)]
        [RequireUserPermission(GuildPermission.BanMembers)]
        public async Task BanAsync(SocketGuildUser victim, string? reason = null)
        {
            // UNDONE: database with guild settings like audit log channel
            await RespondAsync(String.Format("Banned {0} ({1})", victim.Mention, (reason ?? "no reason")));
            await victim.BanAsync(reason: reason);
        }

        [SlashCommand("kick", "Kick the specified user")]
        [RequireBotPermission(GuildPermission.KickMembers)]
        [RequireUserPermission(GuildPermission.KickMembers)]
        public async Task KickAsync(SocketGuildUser victim, string? reason = null)
        {
            // UNDONE: database with guild settings like audit log channel
            await RespondAsync(String.Format("Kicked {0} ({1})", victim.Mention, (reason ?? "no reason")));
            await victim.KickAsync(reason: reason);
        }

        [SlashCommand("silence", "Shut the specified user up")]
        [RequireBotPermission(GuildPermission.ModerateMembers)]
        [RequireUserPermission(GuildPermission.ModerateMembers)]
        public async Task BananAsync(SocketGuildUser victim, int minutes = 5)
        {
            minutes = Math.Max(1, minutes);
            await RespondAsync(String.Format("{0} got silenced for {1} {2}", victim.Mention, ((minutes > 1) ? "minutes" : "minute")));
            await victim.SetTimeOutAsync(new TimeSpan(0, minutes, 0));
        }
    }
}
