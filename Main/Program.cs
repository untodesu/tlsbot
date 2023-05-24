// SPDX-License-Identifier: MPL-2.0
// Copyright (c) 2023, Kirill GPRB
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace TLS.Main
{
    public static class Program
    {
        public static async Task Main(string[] args)
        {
            IHostBuilder builder = Host.CreateDefaultBuilder(args);

            builder.ConfigureHostConfiguration(config => {
                config.SetBasePath(Environment.CurrentDirectory);
                config.AddCommandLine(args);
                config.AddJsonFile("config.json", true);
            });

            builder.ConfigureServices(services => {
                services.AddSingleton<AuditDB>();
                services.AddSingleton<DiscordSocketClient>();
                services.AddSingleton<InteractionService>();
                services.AddHostedService<InteractionWrapperService>();
                services.AddHostedService<ClientWrapperService>();
            });

            IHost host = builder.Build();
            await host.RunAsync();
            host.Dispose();
        }
    }
}
