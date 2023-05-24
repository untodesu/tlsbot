// SPDX-License-Identifier: MPL-2.0
// Copyright (c) 2023, Kirill GPRB
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.
using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using System.Reflection;

namespace TLS.Main
{
    public class InteractionWrapperService : IHostedService
    {
        private readonly DiscordSocketClient m_client;
        private readonly InteractionService m_interactions;
        private readonly IServiceProvider m_services;
        private readonly IConfiguration m_config;

        public InteractionWrapperService(DiscordSocketClient client, InteractionService interactions, IServiceProvider services, IConfiguration config)
        {
            m_client = client;
            m_interactions = interactions;
            m_services = services;
            m_config = config;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            m_client.Ready += OnClientReadyAsync;
            m_client.InteractionCreated += OnClientInteractionAsync;
            await m_interactions.AddModulesAsync(Assembly.GetEntryAssembly(), m_services);
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            m_interactions.Dispose();
            return Task.CompletedTask;
        }

        private async Task OnClientReadyAsync()
        {
            // We only work in one guilds which id is set in audit.guild
            await m_interactions.RegisterCommandsToGuildAsync(m_config.GetValue<ulong>("audit.guild"), true);
        }

        private async Task OnClientInteractionAsync(SocketInteraction interaction)
        {
            try {
                SocketInteractionContext context = new SocketInteractionContext(m_client, interaction);
                await m_interactions.ExecuteCommandAsync(context, m_services);
            }
            catch {
                if(interaction.Type == InteractionType.ApplicationCommand) {
                    await interaction.GetOriginalResponseAsync().ContinueWith(async msg => await msg.Result.DeleteAsync());
                }
            }
        }
    }
}
