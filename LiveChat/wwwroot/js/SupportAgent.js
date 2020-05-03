var activeRoomId = "";

var chatBox = document.getElementById("chatBox");
var roomBox = document.getElementById("roomsBox");
var supportAgentConnection = new signalR.HubConnectionBuilder().withUrl('/SupportAgent').build();
var chatConnection = new signalR.HubConnectionBuilder().withUrl('/Livechat').build();

//tryConnecting for setInterval
var tryConnecting = 0;
var tryConnectingSupport = 0;
supportAgentConnection.on("ShowActiveRooms", loadActiveRooms);
//chat
function startChatConnection() {
    chatConnection.start().then(onConnected).catch(function (err) {
        console.log(err);
    });
}
//agent
function startAgentConnection() {
    supportAgentConnection.start().then(agentOnConnected).catch(function (err) {
        console.log(err);
    });
}
//agent
function agentOnConnected() {
    clearInterval(tryConnectingSupport);
}
//chat
function onConnected() {
    clearInterval(tryConnecting);
}

chatConnection.onclose(function () {
    tryConnecting = setInterval(startChatConnection, 5000);
});

supportAgentConnection.onclose(function() {
    tryConnectingSupport = setInterval(startAgentConnection, 5000);
});
chatConnection.on("ReceiveMessage", appendMessages);
supportAgentConnection.on("ReceiveClientsMessages", getMessages);


function loadActiveRooms(rooms) {
    console.log("come");
    if (!rooms) return;

    var roomIds = Object.keys(rooms);

    if (!roomIds.length) {
        return;
    }

    //switch to other active rooms
    switchRoom(null);
    clearElement(roomBox);
    roomIds.forEach(function (id) {
        var roomInfo = rooms[id];
        console.log(roomInfo);
        if (roomInfo.name) {
            var roomButton = generateRoomButtons(id, roomInfo);
            roomBox.insertAdjacentHTML('beforeend', roomButton);
            console.log("end");
        }

        
    });
}

function switchRoom(roomId) {
    if (roomId === activeRoomId) {
        return;
    }

    if (activeRoomId) {
        chatConnection.invoke("SupportAgentLeaveRoom", activeRoomId);
    }

    activeRoomId = roomId;
    clearElement(chatBox);

    if (!roomId) {
        return;
    }

    chatConnection.invoke("SupportAgentJoinRoom", activeRoomId);
    supportAgentConnection.invoke("LoadMessageHistory", activeRoomId);

}


function clearElement(element) {
    element.innerHTML = "";
}

function generateRoomButtons(id, roomInfo) {
    var roomButton = '<a href="#" data-id=" ' +
        id +
        '"class="btn btn-block btn-info bg-info  list-group-item  text-white">' +
        roomInfo.name +
        '</a>';
       // '"class="list-group-item list-group-item-action list-group-item-light rounded-0">' +
       // '<div class="media"><h6 class="mb-0">' +
       // roomInfo.name +
       // '</div></div></div></a>';
    return roomButton;
}

function sendMessage(text) {
    if (text && text.length) {
        supportAgentConnection.invoke("SendMessageByAgent", activeRoomId, text);
    }
}

function getMessages(messages) {
    if (!messages) {
        return;
    }
    messages.forEach(function(message) {
        appendMessages(message.senderName, message.text, message.sendAt);
    });
}

function appendMessages(senderName, text, sendAt) {
    var messageBody = '<div class="media-body ml-3"><div class="bg-light rounded py-2 px-3 mb-2" > '
    +'<p class="text-small mb-0 text-muted">';
    messageBody += text;
    messageBody += '</p></div><p class="small text-muted">' +
        moment(sendAt).format('H:mm') + '</p></div>';
    
    chatBox.insertAdjacentHTML('beforeend', messageBody);   
    chatBox.scrollTop = chatBox.scrollHeight - chatBox.clientHeight;
}


roomBox.addEventListener("click",
    function(e) {
        setActiveRoomButton(e.target);
        var roomId = e.target.getAttribute("data-id");
        switchRoom(roomId);
    });
    
function setActiveRoomButton(e) {
    var allRoomsButton = roomBox.querySelectorAll("a.list-group-item");
    allRoomsButton.forEach(function(anchor) {
        if (anchor.classList.contains("active")) {
            anchor.classList.add("list-group-item-light");
        }
        anchor.classList.remove("active", "text-white");
    });
    e.classList.add("active", "text-white");
}



function ready() {
    startChatConnection();
    startAgentConnection();

    var agentChatForm = document.getElementById("agentChatForm");
    agentChatForm.addEventListener("submit",
        function(e) {
            e.preventDefault();

            var agentMessage = document.getElementById("agentMessage");
            sendMessage(agentMessage.value);
            agentMessage.value = "";
        });
}

document.addEventListener('DOMContentLoaded', ready);