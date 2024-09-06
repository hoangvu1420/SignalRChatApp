$(document).ready(() => {
    startSignalRConnection();
    updateNavbar();
    fetchGlobalChat();

    $("#send_btn").on("click", sendMessage);
    $("#msg_input").on("keypress", function (event) {
        if (event.key === "Enter") {
            sendMessage(event);
        }
    });

    $("#registerButton").on("click", () => {
        const username = $("#usernameInput").val();
        register(username);
    });

    $("#loginButton").on("click", () => {
        const username = $("#usernameInput").val();
        login(username);
    });

    connection.on("ReceiveMessage", function (chatMessage) {
        console.log("Message received");
        displayMessage(chatMessage);
    });
});

function fetchGlobalChat() {
    $.ajax({
        url: `/GroupChat/GetGroupChatMessages?name=${encodeURIComponent('Global')}`,
        method: 'GET',
        dataType: 'json',
        success: (messages) => {
            const $msgFeed = $('#msg_feed');
            $msgFeed.empty();
            if (Array.isArray(messages) && messages.length > 0) {
                $.each(messages, (index, message) => displayMessage(message));
                console.log('Messages loaded:', messages);
            } else {
                console.log('No messages available');
            }
        },
        error: (error) => {
            console.error('Error fetching messages:', error);
            $('#msg_feed').html('<p>Error loading messages.</p>');
        }
    });
}

function sendMessage(event) {
    const message = $("#msg_input").val();
    connection.invoke("SendMessage", "Global", currentUsername, message).catch(function (err) {
        return console.error(err.toString());
    });
    $("#msg_input").val('');
    console.log("Message sent");
    event.preventDefault();
}