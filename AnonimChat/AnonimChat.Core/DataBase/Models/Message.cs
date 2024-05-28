
namespace AnonimChat.Core.DataBase.Models
{
    public class Message
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public string? ImagePath { get; set; }
        public DateTime FirstCtreateTime { get; set; }

        public Group Group { get; set; }
        public User? Author { get; set; }
        public Message? QuotedMessage { get; set; }
    }
}
