using Microsoft.AspNetCore.SignalR;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace BlazorWebAssemblySignalRApp.Tests
{
    public class ClientProxyMock : IClientProxy
    {

        private readonly List<(string method, object?[] args)> _calls = new();

        public IReadOnlyDictionary<string, IReadOnlyCollection<object?[]>> Calls => _calls.GroupBy(c => c.method)
            .ToDictionary(c => c.Key, c => (IReadOnlyCollection<object?[]>)c.Select(c2 => c2.args).ToList());

        public Task SendCoreAsync(string method, object?[] args, CancellationToken cancellationToken = default)
        {
            _calls.Add((method, args));
            return Task.CompletedTask;
        }

    }
}
