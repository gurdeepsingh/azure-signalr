﻿// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.SignalR.Protocol;
using Microsoft.Extensions.Logging.Abstractions;

namespace Microsoft.Azure.SignalR.Tests
{
    internal sealed class TestServiceConnectionContainer : ServiceConnectionContainerBase
    {
        public bool IsOffline { get; set; } = false;

        public bool MockOffline { get; set; } = false;

        public TestServiceConnectionContainer(List<IServiceConnection> serviceConnections, HubServiceEndpoint endpoint = null, AckHandler ackHandler = null, IServiceConnectionFactory factory = null)
            : base(factory, 0, endpoint, serviceConnections, ackHandler: ackHandler, logger: NullLogger.Instance)
        {
        }

        public List<IServiceConnection> Connections { get => FixedServiceConnections; }

        public void ShutdownForTest()
        {
            var prop = typeof(ServiceConnectionContainerBase).GetField("_terminated", BindingFlags.NonPublic | BindingFlags.Instance);
            prop.SetValue(this, true);
        }

        public override async Task OfflineAsync()
        {
            if (MockOffline)
            {
                await Task.Delay(100);
                IsOffline = true;
            } else
            {
                await base.OfflineAsync();
            }
        }

        public override Task HandlePingAsync(PingMessage pingMessage)
        {
            return Task.CompletedTask;
        }

        protected override Task OnConnectionComplete(IServiceConnection connection)
        {
            return Task.CompletedTask;
        }

        public Task OnConnectionCompleteForTestShutdown(IServiceConnection connection)
        {
            return base.OnConnectionComplete(connection);
        }
    }
}
