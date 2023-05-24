// SPDX-License-Identifier: MPL-2.0
// Copyright (c) 2023, Kirill GPRB
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.

using Microsoft.Extensions.Hosting;

namespace TLS.Main
{
    public class AuditService : BackgroundService
    {
        private readonly AuditDB m_db;

        public AuditService(AuditDB db)
        {
            m_db = db;
            m_db.SaveAsync().GetAwaiter().GetResult();
        }

        protected override async Task ExecuteAsync(CancellationToken token)
        {
            PeriodicTimer timer = new PeriodicTimer(TimeSpan.FromMinutes(5));
            while(await timer.WaitForNextTickAsync(token)) {
                await m_db.SaveAsync();
            }
        }
    }
}