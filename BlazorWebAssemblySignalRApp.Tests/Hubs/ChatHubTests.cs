using BlazorWebAssemblySignalRApp.Server.Hubs;
using BlazorWebAssemblySignalRApp.Server.Repositories;
using FluentAssertions;
using Microsoft.AspNetCore.SignalR;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Threading.Tasks;

namespace BlazorWebAssemblySignalRApp.Tests.Hubs
{

    /// <summary>
    /// Tests for <see cref="ChatHub"/>.
    /// </summary>
    [TestClass]
    public class ChatHubTests
    {

        public ChatHubTests()
        {
            _hub = new ChatHub(_messages, _groups);
            _hub.Clients = _clientsMock.Object;
            _hub.Groups = _groupManager;
            _hub.Context = _contextMock.Object;
        }

        private readonly Mock<IHubCallerClients> _clientsMock = new();
        private readonly Mock<HubCallerContext> _contextMock = new();
        private readonly ChatHub _hub;
        private readonly GroupRepository _groups = new();
        private readonly GroupManagerMock _groupManager = new();
        private readonly InMemoryMessageRepository _messages = new();

        [TestMethod]
        public async Task AddToGroup()
        {

            _contextMock.Setup(m => m.ConnectionId).Returns("1");
            var clientProxyMock = new ClientProxyMock();

            _clientsMock.Setup(m => m.All).Returns(clientProxyMock);

            var groupClientsProxy = new ClientProxyMock();
            _clientsMock.Setup(m => m.Group("MyGroup")).Returns(groupClientsProxy);

            await _hub.AddToGroup("Max", "MyGroup");

            _groups.GroupDTOs.Should().Contain(grp => grp.Name == "MyGroup").Which.MemberCount.Should().Be(1);
            _groupManager.ConnectionsInGroups.Should().ContainKey("1").WhoseValue.Should().Contain("MyGroup");

        }

    }
}
