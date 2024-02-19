"use strict";


var connection = new signalR.HubConnectionBuilder().withUrl("/Chat").build();

document.getElementById("form-submit").disabled = true;

connection.on("GroupMessage", function (uid, username, message, date, picture) {

    let container = document.createElement('div');

    let imgContainer = document.createElement('div');
    let img = document.createElement('img');
    if (picture != null)
        img.src = "data:image;base64," + picture;

    let when = document.createElement('span');
    var dt = new Date(date);
    var data = dt.toLocaleTimeString();

    let text = document.createElement('p');
    text.innerHTML = message;

    if (uid == userId) { 
        container.className = "chat-container darker";
        imgContainer.className = "chat-img-div-r";
        when.className = "chat-time-left";
        when.innerHTML = data;
        when.innerHTML += " ";
        when.innerHTML += username;
    } else {
        container.className = "chat-container";
        imgContainer.className = "chat-img-div-l";
        when.className = "chat-time-right";
        when.innerHTML = data;
        when.innerHTML += " ";
        when.innerHTML += username;
    }

    imgContainer.appendChild(img);
    container.appendChild(imgContainer);
    container.appendChild(text);
    container.appendChild(when);
    document.getElementById('chat-fixed-container').appendChild(container);

    chatScrollToBottom();
});


connection.start().then(function () {
    document.getElementById("form-submit").disabled = false;
    connection.invoke('JoinGroup', routeId);
    e.preventDefault();
}).catch(function (err) {
    return console.error(err.toString());
});

document.getElementById("form-submit").addEventListener("click", function (event) {
    var message = document.getElementById("message").value;
    connection.invoke("SendGroupMessage", routeId, userId, message).catch(function (err) {
        return console.error(err.toString());
    });
    event.preventDefault();
});

