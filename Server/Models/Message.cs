using System.ComponentModel.DataAnnotations.Schema;

namespace BlazorWebAssemblySignalRApp.Server.Models
{
    public class Message
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Group { get; set; }
        public string MessageText { get; set; }
        public string User { get; set; }
        public DateTime Timestamp { get; } = DateTime.Now;
    }

}
