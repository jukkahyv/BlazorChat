using BlazorWebAssemblySignalRApp.Server.Hubs;
using BlazorWebAssemblySignalRApp.Server.Repositories;
using BlazorWebAssemblySignalRApp.Shared;
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
        public async Task AddToGroup_Ok()
        {

            _contextMock.Setup(m => m.ConnectionId).Returns("1");

            var allClientsProxy = new ClientProxyMock();
            _clientsMock.Setup(m => m.All).Returns(allClientsProxy);

            var groupClientsProxy = new ClientProxyMock();
            _clientsMock.Setup(m => m.Group("MyGroup")).Returns(groupClientsProxy);

            await _hub.AddToGroup("Max", "MyGroup");

            _groups.GroupDTOs.Should().Contain(grp => grp.Name == "MyGroup")
                .Which.MemberCount.Should().Be(1, because: "member was added");
            _groupManager.ConnectionsInGroups.Should().ContainKey("1")
                .WhoseValue.Should().Contain("MyGroup");
            allClientsProxy.Calls.Should().ContainKey(MessageNames.RefreshGroups);
            groupClientsProxy.Calls.Should().ContainKey(MessageNames.ReceiveMessage)
                .WhoseValue.Should().Contain(c => c[0]!.ToString() == "Max" && c[1]!.ToString() == $"has joined the group MyGroup.");

        }

        [TestMethod]
        public async Task AddToGroup_GroupFull()
        {

            var groupName = "MyGroup";

            for (int i = 0; i < Constants.MaxMemberCount; ++i)
                _groups.TryJoinGroup(i.ToString(), groupName);

            var callerClientProxy = new ClientProxyMock();
            _clientsMock.Setup(m => m.Caller).Returns(callerClientProxy);

            await _hub.AddToGroup("Max", groupName);

            callerClientProxy.Calls.Should().ContainKey(MessageNames.ReceiveMessage)
                .WhoseValue.Should().Contain(c => c[0]!.ToString() == "System" && c[1]!.ToString() == "The group is full");

            _groupManager.ConnectionsInGroups.Should().BeEmpty(because: "member was not added because group is full");

        }

    }
}
