using System;
using System.Threading.Tasks;
using LiveChat.Models;
using LiveChat.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;

namespace LiveChat.Hubs
{
    [Authorize]
    public class SupportAgentHub : Hub
    {
        private readonly IChatRoomService _chatRoomService;
        private readonly IHubContext<LiveChatHub> _chatHubContext;
        public SupportAgentHub(IChatRoomService chatRoomService, IHubContext<LiveChatHub> chatHubContext)
        {
            _chatRoomService = chatRoomService;
            _chatHubContext = chatHubContext;
        }

        public override async Task OnConnectedAsync()
        {
            await Clients.Caller.SendAsync("ShowActiveRooms", await _chatRoomService.GetAllRooms());
            await base.OnConnectedAsync();
        }

        public async Task SendMessageByAgent(Guid roomId, string text)
        {
            var message = new MessageViewModel
            {
                SendAt = DateTime.Now,
                SenderName = Context.User.Identity.Name,
                Text = text
            };
            await _chatRoomService.AddMessage(roomId, message);
            await _chatHubContext.Clients.Group(roomId.ToString()).SendAsync("ReceiveMessage",
                message.SenderName, message.Text, message.SendAt);
        }

        public async Task LoadMessageHistory(Guid roomId)
        {
            var history = await _chatRoomService.GetChatHistory(roomId);
            await Clients.Caller.SendAsync("ReceiveClientsMessages", history);
        }
    }
}