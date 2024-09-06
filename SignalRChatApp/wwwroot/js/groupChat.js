let currentGroupChat = "Global";
let groupChats = [];

$(document).ready(() => {
    startSignalRConnection();

    $("#send_btn").on("click", sendMessage);
    $("#msg_input").on("keypress", function(event) {
        if (event.key === "Enter") {
            sendMessage(event);
        }
    });

    $("#create-group-btn").on("click", function () {
        $('#createGroupModal').modal('show');
    });

    $("#confirmCreateGroup").on("click", function () {
        const groupName = $("#groupNameInput").val().trim();
        if (groupName) {
            connection.invoke("CreateChatRoom", groupName).catch(function (err) {
                return console.error(err.toString());
            });
            $('#createGroupModal').modal('hide');
            $("#groupNameInput").val('');
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
    $.ajax({
        url: '/GroupChat/GetGroupChats',
        method: 'GET',
        dataType: 'json',
        success: (data) => {
            groupChats = data;
            console.log("Group chats received:", groupChats);
            populateGroupList(groupChats);
        },
        error: (error) => {
            console.error('Error:', error);
        }
    });
}

function populateGroupList(groups) {
    const $groupList = $('#group-list');
    $groupList.empty();
    $.each(groups, function(index, group) {
        const validId = 'group-item-' + group.name.replace(/[^a-zA-Z0-9-_]/g, '_');
        const $li = $('<li>', {
            id: validId,
            class: 'list-group-item d-flex justify-content-between align-items-center'
        }).html(`
            <div>
                <span class="fw-bold">${group.name}</span>
            </div>
            <span id="group-message-count-${validId}" class="badge bg-danger rounded-pill">${group.messageCount}</span>
        `);
        $li.on('click', () => joinRoom(group.name));
        $groupList.append($li);
    });
}

function updateMessageCount(groupName) {
    const validId = 'group-item-' + groupName.replace(/[^a-zA-Z0-9-_]/g, '_');
    const $groupItem = $(`#${validId}`);
    if ($groupItem.length) {
        const $messageCount = $groupItem.find(`#group-message-count-${validId}`);
        if ($messageCount.length) {
            let count = parseInt($messageCount.text());
            count++;
            $messageCount.text(count);
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
    $.ajax({
        url: `/GroupChat/GetGroupChatMessages?name=${encodeURIComponent(roomName)}`,
        method: 'GET',
        dataType: 'json',
        success: (messages) => {
            const $msgFeed = $('#msg_feed');
            $msgFeed.empty();
            if (Array.isArray(messages) && messages.length > 0) {
                console.log('Messages loaded:', messages);
                $.each(messages, function(index, message) {
                    displayMessage(message);
                });
                const validId = 'group-item-' + roomName.replace(/[^a-zA-Z0-9-_]/g, '_');
                const $groupItem = $(`#${validId}`);
                if ($groupItem.length) {
                    const $messageCount = $groupItem.find(`#group-message-count-${validId}`);
                    if ($messageCount.length) {
                        $messageCount.text('0');
                    }
                }
                $.each(messages, function(index, message) {
                    updateMessageCount(message.roomName);
                });
            } else {
                console.log('No messages available');
            }
        },
        error: (error) => {
            console.error('Error fetching messages:', error);
        }
    });
}

connection.on("UpdateGroupList", function (updatedGroups) {
    groupChats = updatedGroups;
    populateGroupList(groupChats);
});

function sendMessage(event) {
    const message = $("#msg_input").val();
    connection.invoke("SendMessage", currentGroupChat, currentUsername, message).catch(function (err) {
        return console.error(err.toString());
    });
    $("#msg_input").val('');
    console.log("Message sent");
    event.preventDefault();
}

function updatePillColor(roomName) {
    const validId = 'group-item-' + roomName.replace(/[^a-zA-Z0-9-_]/g, '_');
    const $groupItem = $(`#${validId}`);
    if ($groupItem.length) {
        const $pill = $groupItem.find('.badge');
        if ($pill.length) {
            $pill.removeClass('bg-danger').addClass('bg-success');
        }
    }
}