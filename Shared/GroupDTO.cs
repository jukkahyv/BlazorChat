
namespace BlazorWebAssemblySignalRApp.Shared
{

    /// <summary>
    /// Group DTO, for passing to client.
    /// </summary>
    public class GroupDTO
    {
        public string Name { get; init; }
        public int MemberCount { get; init; }
    }
}
