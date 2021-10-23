namespace BlazorWebAssemblySignalRApp.Server.Models
{
    public class Message
    {
        public string Group { get; init; }
        public string MessageText { get; init; }
        public string User { get; init; }
        public DateTime Timestamp { get; } = DateTime.Now;
    }

}
