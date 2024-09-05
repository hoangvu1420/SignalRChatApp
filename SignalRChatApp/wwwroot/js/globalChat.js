document.addEventListener('DOMContentLoaded', (event) => {
    startSignalRConnection();

    updateNavbar();

    fetchGlobalChat();

    document.getElementById("send_btn").addEventListener("click", sendMessage);
    document.getElementById("msg_input").addEventListener("keypress", function (event) {
        if (event.key === "Enter") {
            sendMessage(event);
        }
    });

    document.getElementById("registerButton").addEventListener("click", (event) => {
        const username = document.getElementById("usernameInput").value;
        register(username);
    });

    document.getElementById("loginButton").addEventListener("click", (event) => {
        const username = document.getElementById("usernameInput").value;
        login(username);
    });

    connection.on("ReceiveMessage", function (chatMessage) {
        console.log("Message received");
        displayMessage(chatMessage);
    });

    function fetchGlobalChat() {
        fetch(`/GroupChat/GetGroupChatMessages?name=${encodeURIComponent('Global')}`)
            .then(response => response.json())
            .then(messages => {
                const msgFeed = document.getElementById('msg_feed');
                msgFeed.innerHTML = '';
                if (Array.isArray(messages) && messages.length > 0) {
                    messages.forEach(message => displayMessage(message));
                    console.log('Messages loaded:', messages);
                } else {
                    console.log('No messages available');
                }
            })
            .catch(error => {
                console.error('Error fetching messages:', error);
                document.getElementById('msg_feed').innerHTML = '<p>Error loading messages.</p>';
            });
    }
});

function sendMessage(event) {
    const message = document.getElementById("msg_input").value;
    connection.invoke("SendMessage", "Global", currentUsername, message).catch(function (err) {
        return console.error(err.toString());
    });
    document.getElementById("msg_input").value = '';
    console.log("Message sent");
    event.preventDefault();
}