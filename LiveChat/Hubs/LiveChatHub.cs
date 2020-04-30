using System;
using System.Threading.Tasks;
using LiveChat.Models;
using LiveChat.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace LiveChat.Hubs
{
    public class LiveChatHub:Hub
    {
        private readonly IChatRoomService _chatRoomService;
        private readonly IHubContext<SupportAgentHub> _supportHubContext;

        public LiveChatHub(IChatRoomService chatRoomService, IHubContext<SupportAgentHub> supportHubContext)
        {
            _chatRoomService = chatRoomService;
            _supportHubContext = supportHubContext;
        }

        public override async Task OnConnectedAsync()
        {
            if (Context.User.Identity.IsAuthenticated)
            {
                await base.OnConnectedAsync();
                return;
            }

            var roomId = await _chatRoomService.CreateRoom(Context.ConnectionId);
            await Groups.AddToGroupAsync(Context.ConnectionId, roomId.ToString()); 
            await base.OnConnectedAsync();
        }

        public async Task SendMessageAsync(string name, string text)
        {
            var roomId = await _chatRoomService.GetRoomWithConnectionId(Context.ConnectionId);
            var message = new MessageViewModel
            {
                SenderName = name,
                Text = text,
                SendAt = DateTime.Now
            };
            //here you can save the message to database

            //await Clients.All.SendAsync("ReciveMessage", message.SenderName, 
            //    message.Text, message.SendAt);

            await _chatRoomService.AddMessage(roomId, message);
            //ReciveMessage is the method that clients must call 
            await Clients.Group(roomId.ToString()).SendAsync("ReceiveMessage", message.SenderName,
                message.Text, message.SendAt);
        }

        public async Task SetNameForRoom(string visitorName)
        {
            var name = $"{visitorName} from web app";
            var roomId = await _chatRoomService.GetRoomWithConnectionId(Context.ConnectionId);
            await _chatRoomService.SetRoomName(roomId, name);
            await _supportHubContext.Clients.All.SendAsync("ShowActiveRooms", await _chatRoomService.GetAllRooms());
        }

        [Authorize]
        public async Task SupportAgentJoinRoom(Guid roomId)
        {
            if (roomId==Guid.Empty)
            {
                throw new ArgumentException("invalid room id");
            }

            await Groups.AddToGroupAsync(Context.ConnectionId, roomId.ToString());
        }
        [Authorize]
        public async Task SupportAgentLeaveRoom(Guid roomId)
        {
            if (roomId == Guid.Empty)
            {
                throw new ArgumentException("invalid room id");
            }

            await Groups.RemoveFromGroupAsync(Context.ConnectionId, roomId.ToString());
        }
    }
}