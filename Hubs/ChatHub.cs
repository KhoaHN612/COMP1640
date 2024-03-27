using COMP1640.Controllers;
using Microsoft.AspNetCore.SignalR;

namespace COMP1640.Hubs
{
    public class ChatHub : Hub
    {
        private readonly ChatService _chatService;

        public ChatHub(ChatService chatService)
        {
            _chatService = chatService;
        }
        public async Task SendMessage(string user, string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", user, message);
        }

        public async Task SendMessageTo(int chatId, string senderId, string receiverId, string message)
        {
            await _chatService.CreateMessageAsync(chatId, senderId, message);
            await Clients.All.SendAsync("ReceiveMessageFrom", chatId, senderId, message);
        }
    }
}