var chatter = "Visitor";

//initialize and configure singnalR client

var connection = new signalR.HubConnectionBuilder().withUrl('/Livechat').build();

connection.on("ReceiveMessage", renderMessage);
connection.start();

var userInfoForm = document.getElementById("userInfoForm");
var chatForm = document.getElementById("ChatForm");
var messageBox = document.getElementById("MessageBox");
var userInfoBox = document.getElementById("userInfo");
var chatDialog = document.getElementById("chatDialog");
//tryConnecting for setInterval
var tryConnecting=0;
function ready() {
    userInfoForm.addEventListener("submit",
        function (e) {
            e.preventDefault();
            chatter = document.getElementById("nameBox").value;
            $('#myModal').modal({
                keyboard: false,
                backdrop: 'static'
            });
            $('#myModal').modal('show');
            startConnection();
            userInfoBox.style.display = "none";
            //use jquery for effect ;)

            $(chatDialog).slideDown();
          
        });
    chatForm.addEventListener("submit",
        function (e) {
            e.preventDefault();
            sendMessage(messageBox.value);
            messageBox.value = "";
        });
}
//Starting connection
function startConnection() {
    connection.start().then(onConnected).catch(function (err) {
        console.log(err);
    });
}

//This method invoke when user information complete
function onConnected() {
    console.log("create chatroom Start");
    clearInterval(tryConnecting);
    connection.invoke('SetNameForRoom', chatter);
    var messageTextBox = document.getElementById('MessageBox');
    messageTextBox.focus();
    $('#myModal').modal('hide');
    console.log("create chatroom End");

}

function onDisconnected() {
    alert("ReConnecting in 5 Second ...");
}

//This method invoke when user disconnected from the hub
connection.onclose(function () {
   // onDisconnected();
    console.log('ReConnecting in 5 Second ...');
    tryConnecting = setInterval(startConnection, 5000);
});



//This method generate message box in chat history
function renderMessage(senderName, text, sendAt) {
    var messageBody = '<div class="media border p-3 mt-3">' +
        '<img src = "/Images/img_avatar3.png" alt = "John Doe" class="mr-3 mt-3 rounded-circle" style = "width:60px;">' +
        '<div class="media-body">';
    var friendlyTime = moment(sendAt).format('H:mm');
    messageBody += '<h4>' + senderName + '<small><i class="pl-3">' + friendlyTime + '</i></small></h4>';
    messageBody += '<p>' + text + '</p>';
    messageBody += '</div></div>';
    var chatHistory = document.getElementById('chatHistory');
    chatHistory.insertAdjacentHTML('beforeend', messageBody);
    window.scrollBy(0, 70);

}


function sendMessage(text) {
    if (text && text.length) {
        connection.invoke('SendMessageAsync', chatter, text);
    }
}



document.addEventListener('DOMContentLoaded', ready);

