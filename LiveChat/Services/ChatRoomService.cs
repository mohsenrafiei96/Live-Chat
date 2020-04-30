using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LiveChat.Models;

namespace LiveChat.Services
{
    public class ChatRoomService : IChatRoomService
    {
        private readonly Dictionary<Guid, ChatRoom> _roomsInfo = new Dictionary<Guid, ChatRoom>();
        //this dictionary works as database
        private readonly Dictionary<Guid, List<MessageViewModel>> _db = new Dictionary<Guid, List<MessageViewModel>>();
        public Task<Guid> CreateRoom(string connectionId)
        {
            var id = Guid.NewGuid();
            _roomsInfo[id] = new ChatRoom()
            {
                OwnerConnectionId = connectionId
            };
            return Task.FromResult(id);
        }

        public Task<Guid> GetRoomWithConnectionId(string connectionId)
        {
            var room = _roomsInfo.FirstOrDefault(p => p.Value.OwnerConnectionId == connectionId);
            if (room.Key == Guid.Empty)
            {
                throw new ArgumentException("room is null");
            }

            return Task.FromResult(room.Key);
        }

        public Task SetRoomName(Guid roomId, string name)
        {
            if (!_roomsInfo.ContainsKey(roomId))
            {
                throw new Exception("invalid room id");
            }

            _roomsInfo[roomId].Name = name;
            return Task.CompletedTask;
        }

        public Task AddMessage(Guid roomId, MessageViewModel message)
        {
            if (!_db.ContainsKey(roomId))
            {
                _db[roomId] = new List<MessageViewModel>();
            }
            _db[roomId].Add(message);

            return Task.CompletedTask;
        }

        public Task<IEnumerable<MessageViewModel>> GetChatHistory(Guid roomId)
        {
            _db.TryGetValue(roomId, out var messages);
            messages = messages ?? new List<MessageViewModel>();
            var sortedMessage = messages.OrderBy(p => p.SendAt).AsEnumerable();
            return Task.FromResult(sortedMessage);
        }

        public Task<Dictionary<Guid, ChatRoom>> GetAllRooms()
        {
            return Task.FromResult(_roomsInfo);
        }
    }
}