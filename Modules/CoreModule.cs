// SPDX-License-Identifier: MPL-2.0
// Copyright (c) 2023, Kirill GPRB
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.
using Discord.Interactions;
using System.Globalization;

namespace TLS.Modules
{
    public class CoreModule : InteractionModuleBase<SocketInteractionContext>
    {
        [SlashCommand("echo", "Say some text as the bot")]
        public async Task EchoAsync(string text)
        {
            await RespondAsync(text);
        }

        [SlashCommand("ping", "Check the bot status")]
        public async Task PingAsync()
        {
            await RespondAsync(String.Format("Pong! This took {0} ms!", Context.Client.Latency));
        }

        [SlashCommand("time", "Display system (host) time")]
        public async Task TimeAsync()
        {
            await RespondAsync(String.Format(CultureInfo.InvariantCulture, "System time: {0}", DateTime.UtcNow));
        }
    }
}
