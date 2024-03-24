<<<<<<< HEAD
using COMP1640.Controllers;
using Microsoft.AspNetCore.SignalR;
=======
﻿using Microsoft.AspNetCore.SignalR;
>>>>>>> a21ee76 (Create original chat version)

namespace COMP1640.Hubs
{
    public class ChatHub : Hub
    {
<<<<<<< HEAD
        private readonly ChatService _chatService;

        public ChatHub(ChatService chatService)
        {
            _chatService = chatService;
        }
=======
>>>>>>> a21ee76 (Create original chat version)
        public async Task SendMessage(string user, string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", user, message);
        }
<<<<<<< HEAD

        public async Task SendMessageTo(int chatId, string senderId, string receiverId, string message)
        {
            await _chatService.CreateMessageAsync(chatId, senderId, message);
            await Clients.All.SendAsync("ReceiveMessageFrom", chatId, senderId, message);
        }
=======
>>>>>>> a21ee76 (Create original chat version)
    }
}