using BlazorWebAssemblySignalRApp.Server.Models;
using BlazorWebAssemblySignalRApp.Shared;

namespace BlazorWebAssemblySignalRApp.Server.Repositories
{

    /// <summary>
    /// Manages list of groups and group memberships.
    /// This service is singleton.
    /// </summary>
    public interface IGroupRepository
    {
        List<GroupDTO> GroupDTOs { get; }
        /// <summary>
        /// Removes user from all groups.
        /// </summary>
        /// <param name="user">User name, for example "Max".</param>
        void LeaveGroups(string user);
        void LeaveGroup(string user, string groupName);
        bool TryJoinGroup(string user, string groupName);
    }

    public class GroupRepository : IGroupRepository
    {

        private readonly Dictionary<string, Group> _groups = new();

        public List<GroupDTO> GroupDTOs => _groups.Values.Select(g => g.ToDTO()).ToList();

        public void LeaveGroup(string user, string groupName)
        {
            if (_groups.TryGetValue(groupName, out var group))
            {
                group.Members.Remove(user);
            }
        }

        public void LeaveGroups(string user)
        {
            foreach (var grp in _groups.Values)
            {
                grp.Members.Remove(user);
            }
        }

        public bool TryJoinGroup(string user, string groupName)
        {

            if (!_groups.TryGetValue(groupName, out var group))
            {
                group = new Group { Name = groupName };
                _groups.Add(groupName, group);
            }
            else if (group.Members.Count >= Constants.MaxMemberCount)
            {
                return false;
            }

            group.Members.Add(user);
            return true;

        }
    }
}
