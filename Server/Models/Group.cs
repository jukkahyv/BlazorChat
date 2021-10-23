using BlazorWebAssemblySignalRApp.Shared;

namespace BlazorWebAssemblySignalRApp.Server.Models
{
    public class Group
    {
        public string Name { get; init; }
        public HashSet<string> Members { get; } = new HashSet<string>();

        public GroupDTO ToDTO() => new GroupDTO { Name = Name, MemberCount = Members.Count };
    }

}
