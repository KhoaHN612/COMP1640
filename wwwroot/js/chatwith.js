"use strict";

{/* 
<div id="chat-box">
    <div class="message my-message p-3 mb-2">
    <p>This is my message. It's aligned to the right.</p>
    </div>
    <div class="message other-message p-3 mb-2">
    <p>This is someone else's message. It's aligned to the left.</p>
    </div> 
*/}

var connection = new signalR.HubConnectionBuilder().withUrl("/chatHub").build();

const chat = document.getElementById('chatBox');
var senderId = document.getElementById("senderId").value;
var receiverId = document.getElementById("receiverId").value;
var chatId = parseInt(document.getElementById("chatId").value);

//Disable the send button until connection is established.
document.getElementById("buttonSend").disabled = true;

connection.on("ReceiveMessageFrom", function (CchatId, CsenderId, Cmessage) {
    console.log(CchatId, CsenderId, Cmessage);
    if (CchatId = chatId){
        var p = document.createElement("p");
        p.textContent = Cmessage;
        let isCurrentUserMessage = CsenderId == senderId;
        var div = document.createElement("div");
        div.className = isCurrentUserMessage ? "message my-message p-3 mb-2" : "message other-message p-3 mb-2";
        div.appendChild(p);
        chat.appendChild(div);
    }
});

connection.start().then(function () {
    document.getElementById("buttonSend").disabled = false;
}).catch(function (err) {
    return console.error(err.toString());
});

// let sentAt = new Date();

document.getElementById("buttonSend").addEventListener("click", function (event) {
    var message = document.getElementById("messageInput").value;
    connection.invoke("SendMessageTo", chatId, senderId, receiverId, message).catch(function (err) {
        return console.error(err.toString());
    });
    event.preventDefault();
    document.getElementById("messageInput").value = "";
});