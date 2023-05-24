// SPDX-License-Identifier: MPL-2.0
// Copyright (c) 2023, Kirill GPRB
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.
using System.Collections.Concurrent;
using System.Text.Json;
using Microsoft.Extensions.Configuration;

namespace TLS.Main
{
    public class AuditDB
    {
        // A path to the uhhh... database?
        public const string WarnsPath = "warns.json";

        public readonly ulong GuildId;
        public readonly ulong ChannelId;
        public readonly uint MaxWarnings;
        public readonly ConcurrentDictionary<ulong, uint>? Warnings;

        public AuditDB(IConfiguration config)
        {
            GuildId = config.GetValue<ulong>("audit.guild");
            ChannelId = config.GetValue<ulong>("audit.channel");
            MaxWarnings = config.GetValue<uint>("audit.maxwarns");

            try {
                using FileStream stream = File.OpenRead("warns.json");
                Warnings = JsonSerializer.Deserialize<ConcurrentDictionary<ulong, uint>>(stream);
                ArgumentNullException.ThrowIfNull(Warnings);
                stream.Close();
            }
            catch {
                Warnings = new ConcurrentDictionary<ulong, uint>();
                using FileStream stream = File.OpenWrite("warns.json");
                JsonSerializer.Serialize(stream, Warnings);
                stream.Close();
            }
        }

        public async Task SaveAsync()
        {
            using FileStream stream = File.OpenWrite("warns.json");
            await JsonSerializer.SerializeAsync(stream, Warnings);
            stream.Close();
        }
    }
}
