// SPDX-License-Identifier: MPL-2.0
// Copyright (c) 2023, Kirill GPRB
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.
using Discord;
using Discord.Interactions;
using Discord.WebSocket;

namespace TLS.Main
{
    [Group("tls", "The Linux Space")]
    public class AuditModule : InteractionModuleBase<SocketInteractionContext>
    {
        private readonly AuditDB m_db;

        public AuditModule(AuditDB db)
        {
            m_db = db;
            m_db.SaveAsync().GetAwaiter().GetResult();
        }

        private async Task AuditLogAsync(string message)
        {
            DateTimeOffset offset = new DateTimeOffset(DateTime.UtcNow);
            SocketGuild guild = Context.Client.GetGuild(m_db.GuildId);
            ArgumentNullException.ThrowIfNull(guild);
            SocketTextChannel channel = guild.GetTextChannel(m_db.ChannelId);
            ArgumentNullException.ThrowIfNull(channel);
            await channel.SendMessageAsync($"<t:{offset.ToUnixTimeSeconds()}> {Context.User.Mention} {message}");
        }

        [RequireBotPermission(GuildPermission.BanMembers)]
        [RequireUserPermission(GuildPermission.BanMembers)]
        [SlashCommand("ban", "Ban the specified user")]
        public async Task BanAsync(SocketGuildUser victim, string reason = "no reason")
        {
            // IntelliSense suririsingly (bollocks)
            // lacks both sensitivity and intelligence
            ArgumentNullException.ThrowIfNull(m_db.Warnings);

            if(Context.Guild.Id == m_db.GuildId) {
                await AuditLogAsync($"Banned {victim.Id} ({reason})");
                await RespondAsync($"Banned {victim.Mention} ({reason})");
                await victim.BanAsync(reason: reason);
                m_db.Warnings.Remove(victim.Id, out uint dummy);
            }
        }

        [RequireBotPermission(GuildPermission.BanMembers)]
        [RequireUserPermission(GuildPermission.BanMembers)]
        [SlashCommand("dewarn", "Remove one warning from a specified user")]
        public async Task DewarnAsync(SocketGuildUser victim)
        {
            // IntelliSense suririsingly (bollocks)
            // lacks both sensitivity and intelligence
            ArgumentNullException.ThrowIfNull(m_db.Warnings);

            if(Context.Guild.Id == m_db.GuildId) {
                uint count = m_db.Warnings.GetValueOrDefault(victim.Id, 0U);

                if(count != 0U) {
                    count -= 1U;
                    await AuditLogAsync($"Dewarned {victim.Id} [{count}/{m_db.MaxWarnings}]");
                    await RespondAsync($"Dewarned {victim.Mention} [{count}/{m_db.MaxWarnings}]");
                    m_db.Warnings[victim.Id] = count;
                }
            }
        }

        [RequireBotPermission(GuildPermission.BanMembers)]
        [RequireUserPermission(GuildPermission.BanMembers)]
        [SlashCommand("kick", "Kick the specified user")]
        public async Task KickAsync(SocketGuildUser victim, string reason = "no reason")
        {
            if(Context.Guild.Id == m_db.GuildId) {
                await AuditLogAsync($"Kicked {victim.Id} ({reason})");
                await RespondAsync($"Kicked {victim.Mention} ({reason})");
                await victim.KickAsync(reason: reason);
            }
        }

        [RequireBotPermission(GuildPermission.BanMembers)]
        [RequireUserPermission(GuildPermission.BanMembers)]
        [SlashCommand("warn", "Add one warning to a specified user")]
        public async Task WarnAsync(SocketGuildUser victim, string reason = "no reason")
        {
            // IntelliSense suririsingly (bollocks)
            // lacks both sensitivity and intelligence
            ArgumentNullException.ThrowIfNull(m_db.Warnings);

            if(Context.Guild.Id == m_db.GuildId) {
                uint count = m_db.Warnings.GetValueOrDefault(victim.Id, 0U) + 1U;

                if(count >= m_db.MaxWarnings) {
                    await AuditLogAsync($"Warned {victim.Id} ({reason}) [{count}/{m_db.MaxWarnings}]");
                    await AuditLogAsync($"Auto-banned {victim.Id} (warning count exceeded)");
                    await RespondAsync($"Auto-banned {victim.Mention} (warning count exceeded)");
                    await victim.BanAsync(reason: "warning count exceeded");
                    m_db.Warnings.Remove(victim.Id, out uint dummy);
                }
                else {
                    await AuditLogAsync($"Warned {victim.Id} ({reason}) [{count}/{m_db.MaxWarnings}]");
                    await RespondAsync($"Warned {victim.Mention} ({reason}) [{count}/{m_db.MaxWarnings}]");
                    m_db.Warnings[victim.Id] = count;
                }
            }
        }
    }
}
