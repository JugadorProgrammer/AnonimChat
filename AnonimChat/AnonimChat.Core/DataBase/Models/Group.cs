
namespace AnonimChat.Core.DataBase.Models
{
    public class Group
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public User Owner { get; set; }
        public IEnumerable<User> Users { get; set; }
        public IEnumerable<Message>? Messages { get; set; }
    }
}
