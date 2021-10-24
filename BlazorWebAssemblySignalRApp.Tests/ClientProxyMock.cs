using Microsoft.AspNetCore.SignalR;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace BlazorWebAssemblySignalRApp.Tests
{
    public class ClientProxyMock : IClientProxy
    {

        public List<(string method, object?[] args)> _calls = new();

        public Task SendCoreAsync(string method, object?[] args, CancellationToken cancellationToken = default)
        {
            _calls.Add((method, args));
            return Task.CompletedTask;
        }

    }
}
