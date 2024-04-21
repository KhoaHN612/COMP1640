"use strict";

var connection = new signalR.HubConnectionBuilder().withUrl("/chatHub").build();

const chat = document.getElementById('chatBox');
var senderId = document.getElementById("senderId").value;
var receiverId = document.getElementById("receiverId").value;
var chatId = parseInt(document.getElementById("chatId").value);

var messageWrapper = document.getElementById('chatBox');

// Cuộn xuống cuối cùng của khu vực tin nhắn
function scrollToBottom() {
  messageWrapper.scrollTop = messageWrapper.scrollHeight;
}
document.addEventListener("DOMContentLoaded", function() {
    scrollToBottom();
});

function formatDateTimeNow() {
    const date = new Date();
    const day = date.getDate();
    const month = date.getMonth() + 1; // Tháng bắt đầu từ 0, cần phải cộng 1
    const year = date.getFullYear();
    let hours = date.getHours();
    const minutes = date.getMinutes();
    const seconds = date.getSeconds();

    const amOrPm = hours >= 12 ? 'PM' : 'AM';

    hours = hours % 12 || 12;

    const formattedMinutes = String(minutes).padStart(2, '0');
    const formattedSeconds = String(seconds).padStart(2, '0');

    const formattedDateTime = `${month}/${day}/${year} ${hours}:${formattedMinutes}:${formattedSeconds} ${amOrPm}`;

    return formattedDateTime;
}

//Disable the send button until connection is established.
document.getElementById("buttonSend").disabled = true;

connection.on("ReceiveMessageFrom", function (CchatId, CsenderId, Cmessage) {
    console.log(CchatId, CsenderId, Cmessage);

    if (CchatId = chatId){
        let isCurrentUserMessage = CsenderId == senderId;

        let className = isCurrentUserMessage ? "right-chat" : "left-chat";

        let html = `
        <div class="col-md-12 chat-main chat-sidebar ${className}">
            <div id="chat-messages">
                <ul class="discussion-list">
                    <li class="author-block ${className}">
                        <article class="paragraph-wrapper">
                            <div class="inner">
                                <div class="rating-individual" data-score="5"></div>
                                <p>
                                    ${Cmessage}
                                </p>
                            </div>
                            <div class="comment-controls">
                                <span>${formatDateTimeNow()}</span>
                            </div>
                        </article>
                    </li>
                </ul>
            </div>
        </div>
        `;


        chat.innerHTML += html;
        scrollToBottom();
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