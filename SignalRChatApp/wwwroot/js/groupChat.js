let currentGroupChat = "Global";
let groupChats = [];

document.addEventListener('DOMContentLoaded', (event) => {
    startSignalRConnection();

    document.getElementById("send_btn").addEventListener("click", sendMessage);
    document.getElementById("msg_input").addEventListener("keypress", function(event) {
        if (event.key === "Enter") {
            sendMessage(event);
        }
    });

    document.getElementById("create-group-btn").addEventListener("click", function (event) {
        $('#createGroupModal').modal('show');
    });

    document.getElementById("confirmCreateGroup").addEventListener("click", function (event) {
        const groupName = document.getElementById("groupNameInput").value.trim();
        if (groupName) {
            connection.invoke("CreateChatRoom", groupName).catch(function (err) {
                return console.error(err.toString());
            });
            $('#createGroupModal').modal('hide');
            document.getElementById("groupNameInput").value = '';
        }
    });

    fetchGroupChats();
});

connection.on("ReceiveMessage", function (chatMessage) {
    console.log("Message received");
    displayMessage(chatMessage);
    updateMessageCount(chatMessage.roomName);
});

function fetchGroupChats() {
    fetch('/GroupChat/GetGroupChats')
        .then(response => response.json())
        .then(data => {
            groupChats = data;
            console.log("Group chats received:", groupChats);
            populateGroupList(groupChats);
        })
        .catch(error => console.error('Error:', error));
}

function populateGroupList(groups) {
    const groupList = document.getElementById('group-list');
    groupList.innerHTML = '';
    groups.forEach(group => {
        const li = document.createElement('li');
        const validId = 'group-item-' + group.name.replace(/[^a-zA-Z0-9-_]/g, '_');
        li.id = validId;
        li.className = 'list-group-item d-flex justify-content-between align-items-center';
        li.innerHTML = `
            <div>
                <span class="fw-bold">${group.name}</span>
            </div>
            <span id="group-message-count-${validId}" class="badge bg-danger rounded-pill">${group.messageCount}</span>
        `;
        li.addEventListener('click', () => joinRoom(group.name));
        groupList.appendChild(li);
    });
}

function updateMessageCount(groupName) {
    const validId = 'group-item-' + groupName.replace(/[^a-zA-Z0-9-_]/g, '_');
    const groupItem = document.getElementById(validId);
    if (groupItem) {
        var messageCount = groupItem.querySelector('#group-message-count-' + validId);
        if (messageCount) {
            var count = parseInt(messageCount.textContent);
            count++;
            messageCount.textContent = count;
        }
    }
}

function joinRoom(roomName) {
    connection.invoke("JoinRoom", roomName, currentUsername).catch(function (err) {
        return console.error(err.toString());
    });
    currentGroupChat = roomName;
    document.getElementById('group-name').textContent = roomName;
    fetchGroupChat(roomName);
    const group = groupChats.find(g => g.name === roomName);
    const latestMessageTime = new Date(group.lastMessageTime);
    var groupLatestActivity = document.getElementById('group-latest-activity');
    if (latestMessageTime.getTime() === new Date(0).getTime()) {
        groupLatestActivity.textContent = 'None';
    } else {
        groupLatestActivity.textContent = latestMessageTime.toLocaleString();
    }
    
    updatePillColor(roomName);
}

function fetchGroupChat(roomName) {
    fetch(`/GroupChat/GetGroupChatMessages?name=${encodeURIComponent(roomName)}`)
        .then(response => response.json())
        .then(messages => {
            const msgFeed = document.getElementById('msg_feed');
            msgFeed.innerHTML = '';
            if (Array.isArray(messages) && messages.length > 0) {
                console.log('Messages loaded:', messages);
                messages.forEach(message => displayMessage(message));
                const validId = 'group-item-' + roomName.replace(/[^a-zA-Z0-9-_]/g, '_');
                const groupItem = document.getElementById(validId);
                if (groupItem) {
                    var messageCount = groupItem.querySelector('#group-message-count-' + validId);
                    if (messageCount) {
                        messageCount.textContent = '0';
                    }
                }
                messages.forEach(message => updateMessageCount(message.roomName));
            } else {
                console.log('No messages available');
            }
        })
        .catch(error => {
            console.error('Error fetching messages:', error);
        });
}

connection.on("UpdateGroupList", function (updatedGroups) {
    groupChats = updatedGroups;
    populateGroupList(groupChats);
});

function sendMessage(event) {
    const message = document.getElementById("msg_input").value;
    connection.invoke("SendMessage", currentGroupChat, currentUsername, message).catch(function (err) {
        return console.error(err.toString());
    });
    document.getElementById("msg_input").value = '';
    console.log("Message sent");
    event.preventDefault();
}

function updatePillColor(roomName) {
    const validId = 'group-item-' + roomName.replace(/[^a-zA-Z0-9-_]/g, '_');
    const groupItem = document.getElementById(validId);
    if (groupItem) {
        const pill = groupItem.querySelector('.badge');
        if (pill) {
            pill.classList.remove('bg-danger');
            pill.classList.add('bg-success');
        }
    }
}