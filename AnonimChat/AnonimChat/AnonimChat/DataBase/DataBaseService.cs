using AnonimChat.Core.DataBase.Interfaces;
using AnonimChat.Core.DataBase.Models;
using Dapper;
using Npgsql;
using System.Data;

namespace AnonimChat.DataBase
{
    public class DataBaseService : IDataBaseService
    {
        #region private values
        private readonly string _connectionString;
        #endregion

        public DataBaseService(string connectionString)
        {
            _connectionString = connectionString;
        }

        #region User
        public async Task<IEnumerable<User>?> GetAllUsersAsync()
        {
            using (IDbConnection db = new NpgsqlConnection(_connectionString))
            {
                return await db.QueryAsync<User>("SELECT * FROM \"User\"");
            }
        }

        public async Task<User?> GetUserAsync(int userId)
        {
            using (IDbConnection db = new NpgsqlConnection(_connectionString))
            {
                return (await db.QueryAsync<User>("SELECT * FROM \"User\" WHERE \"Id\"=@userId", new { userId })).FirstOrDefault();
            }
        }
        public async Task<User?> GetUserAsync(string name, string password)
        {
            using (IDbConnection db = new NpgsqlConnection(_connectionString))
            {
                return (await db.QueryAsync<User>("SELECT * FROM \"User\" WHERE \"Name\"=@name AND \"Password\"=@password", new { name, password })).FirstOrDefault();
            }
        }

        public async Task UpdateUserAsync(User user)
        {
            using (IDbConnection db = new NpgsqlConnection(_connectionString))
            {
                await db.QueryAsync("UPDATE \"User\" SET \"Name\"=@Name AND \"Password\"=@Password AND \"Email\"=@Email AND \"IsActive\"=@IsActive AND \"PhoneNumber\"=@PhoneNumber WHERE \"Id\"=@Id", new { user });
            }
        }
        public async Task<User> CreateUserAsync(User user)
        {
            using (IDbConnection db = new NpgsqlConnection(_connectionString))
            {
                user.Id = (await db.QueryAsync<int>("INSERT INTO \"User\" (\"Name\", \"Email\",\"Password\") VALUES(@Name, @Email, @Password); SELECT CAST(SCOPE_IDENTITY() as int)", user)).FirstOrDefault();
                return user;
            }
        }
        public async Task RemoveUserAsync(int userId)
        {
            using (IDbConnection db = new NpgsqlConnection(_connectionString))
            {
                await db.QueryAsync("DELETE FROM \"User\" WHERE \"Id\"=@userId", new { userId });
            }
        }
        #endregion


        #region Group
        public async Task<IEnumerable<Group>?> GetGroupsAsync(int userId)
        {
            using (IDbConnection db = new NpgsqlConnection(_connectionString))
            {
                return await db.QueryAsync<Group>("SELECT \"g\".\"Id\", \"g\".\"Name\", \"g\".\"Owner_Id\" FROM \"Group\" AS \"g\" JOIN \"User_Group\" AS \"ug\" ON \"ug\".\"Group_Id\" = \"g\".\"Id\" JOIN \"User\" AS \"u\" ON \"ug\".\"User_Id\" = \"u\".\"Id\" WHERE \"u\".\"Id\"=@userId", new { userId });
            }
        }

        public async Task<IEnumerable<Group>?> GetGroupsAsync(string groupName)
        {
            using (IDbConnection db = new NpgsqlConnection(_connectionString))
            {
                return await db.QueryAsync<Group>("SELECT * FROM \"Group\" WHERE LOWER(\"Name\") LIKE LOWER('%@groupName%') ", new { groupName });
            }
        }

        public async Task<Group> CreateGroupAsync(Group group)
        {
            using (IDbConnection db = new NpgsqlConnection(_connectionString))
            {
                // `@Owner.Id` - may be not work
                group.Id = (await db.QueryAsync<int>("INSERT INTO \"Group\" (\"Name\",\"Owner_Id\") VALUES(@Name, @Owner.Id); SELECT CAST(SCOPE_IDENTITY() as int)", new { group })).FirstOrDefault();
                return group;
            }
        }

        public async Task AddUserToGropAsync(int userId, int groupId)
        {
            using (IDbConnection db = new NpgsqlConnection(_connectionString))
            {
                await db.QueryAsync<Group>("INSERT INTO \"User_Group\"(\"User_Id\",\"Group_Id\") VALUES(@groupId,@userId)", new { groupId, userId });
            }
        }

        public async Task RemoveUserFromGroupAsync(int userId, int groupId)
        {
            using (IDbConnection db = new NpgsqlConnection(_connectionString))
            {
                await db.QueryAsync<Group>("DELETE FROM \"User_Group\" WHERE \"User_Id\"= @userId AND \"Group_Id\" = @groupId", new { userId, groupId });
            }
        }

        public async Task RemoveGroupAsync(int groupId)
        {
            using (IDbConnection db = new NpgsqlConnection(_connectionString))
            {
                await db.QueryAsync<Group>("DELETE FROM \"Group\" WHERE \"Id\"=@groupId", new { groupId });
            }
        }
        #endregion


        #region Message
        public async Task<IEnumerable<Message>?> GetAllGroupMessagesAsync(int groupId, string messageText = "")
        {
            using (IDbConnection db = new NpgsqlConnection(_connectionString))
            {
                return await db.QueryAsync<Message, User, Group, Message, Message>
                    ("SELECT * FROM \"Message\" JOIN \"Group\" ON \"Message\".\"Group_Id\" = @groupId AND \"Group\".\"Id\" = \"Message\".\"Group_Id\" JOIN \"User\" ON \"User\".\"Id\" = \"Message\".\"Author_Id\" LEFT JOIN \"Message\" AS \"QuotedMessage\" ON \"QuotedMessage\".\"Id\" = \"Message\".\"QuotedMessage_Id\" WHERE LOWER(\"Message\".\"Text\") LIKE LOWER(@messageText)",
                    (message, user, group, quotedMessage) =>
                    {
                        message.Author = user;
                        message.Group = group;
                        message.QuotedMessage = quotedMessage;
                        return message;
                    }, new { groupId, messageText = "%" + messageText + "%" });
            }
        }

        public async Task<Message> CreateMessageAsync(Message message)
        {
            using (IDbConnection db = new NpgsqlConnection(_connectionString))
            {
                //`@Group.Id,@User.Id` - may be not work
                message.Id = (await db.QueryAsync<int>("INSERT INTO \"Message\" (\"Text\", \"ImagePath\",\"FirstCreateTime\",\"Group_Id\",\"Author_Id\",\"QuotedMessage_Id\") VALUES(@Text, @ImagePath, @Password,@FirstCreateTime ,@Group.Id,@Author.Id,@QuotedMessage.Id); SELECT CAST(SCOPE_IDENTITY() as int)", message)).FirstOrDefault();
                return message;
            }
        }

        public async Task RemoveMessageAsync(int messageId)
        {
            using (IDbConnection db = new NpgsqlConnection(_connectionString))
            {
                await db.QueryAsync("DELETE FROM \"Message\" WHERE \"Id\"=@messageId", new { messageId });
            }
        }

        public async Task UpdateMessage(Message message)
        {
            using (IDbConnection db = new NpgsqlConnection(_connectionString))
            {
                await db.QueryAsync("UPDATE \"Message\" SET \"Text\"=@Text AND \"ImagePath\"=@ImagePath AND \"FirstCreateTime\"=@FirstCreateTime AND \"IsActive\"=@IsActive WHERE \"Id\"=@Id", new { message });
            }
        }
        #endregion


        #region Settings
        public async Task<Settings> UpdateSettingsAsync(Settings settings)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
