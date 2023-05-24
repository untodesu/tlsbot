// SPDX-License-Identifier: MPL-2.0
// Copyright (c) 2023, Kirill GPRB
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.
using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace TLS.Main
{
    public class ClientWrapperService : IHostedService
    {
        private readonly DiscordSocketClient m_client;
        private readonly IConfiguration m_config;

        public ClientWrapperService(DiscordSocketClient client, IConfiguration config)
        {
            m_client = client;
            m_config = config;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await m_client.LoginAsync(TokenType.Bot, m_config["client.token"]);
            await m_client.StartAsync();
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            await m_client.StopAsync();
            await m_client.LogoutAsync();
        }
    }
}
