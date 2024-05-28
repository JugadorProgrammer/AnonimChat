using AnonimChat.Core.DataBase.Models;

namespace AnonimChat.Core.DataBase.Interfaces
{
    /// <summary>
    /// Удаление - Remove
    /// Добавление сущности в другую сущность - Add
    /// Создание новой сущности - Create
    /// Обновить сущность - Update
    /// </summary>
    public interface IDataBaseService
    {
        #region User
        Task<IEnumerable<User>?> GetAllUsersAsync();

        Task<User?> GetUserAsync(int userId);
        Task<User?> GetUserAsync(string name, string password);

        Task UpdateUserAsync(User user);
        Task<User> CreateUserAsync(User user);
        Task RemoveUserAsync(int userId);
        #endregion


        #region Group
        Task<IEnumerable<Group>?> GetGroupsAsync(int userId);
        Task<IEnumerable<Group>?> GetGroupsAsync(string groupName);

        Task<Group> CreateGroupAsync(Group group);
        Task AddUserToGropAsync(int userId, int groupId);
        Task RemoveUserFromGroupAsync(int userId, int groupId);
        Task RemoveGroupAsync(int groupId);
        #endregion


        #region Message
        Task<IEnumerable<Message>?> GetAllGroupMessagesAsync(int groupId, string messageText = "");
        Task<Message> CreateMessageAsync(Message message);
        Task RemoveMessageAsync(int messageId);
        Task UpdateMessage(Message message);
        #endregion


        #region Settings
        Task<Settings> UpdateSettingsAsync(Settings settings);
        #endregion
    }
}
