using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LiveChat.Models;

namespace LiveChat.Services
{
    public interface IChatRoomService
    {
        Task<Guid> CreateRoom(string connectionId);
        Task<Guid> GetRoomWithConnectionId(string connectionId);
        Task SetRoomName(Guid roomId, string name);
        Task AddMessage(Guid roomId, MessageViewModel message);
        Task<IEnumerable<MessageViewModel>> GetChatHistory(Guid roomId);
        Task<Dictionary<Guid, ChatRoom>> GetAllRooms();
    }
}
