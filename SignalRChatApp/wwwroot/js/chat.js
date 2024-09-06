const connection = new signalR.HubConnectionBuilder().withUrl("/chatHub").build();
let isLoggedIn = false;
let currentUsername = '';

let navLinks = [
    { text: 'Group Chat', href: '/GroupChat' },
];

// Use jQuery's document ready function
$(document).ready(() => {
    checkAuthStatus();
});

function checkAuthStatus() {
    currentUsername = sessionStorage.getItem('username');
    isLoggedIn = !!currentUsername;
    updateNavbar();
    if (isLoggedIn) {
        startSignalRConnection();
    }
}

function updateNavbar() {
    // Use jQuery selectors
    const $navbarItems = $('#navbarItems');
    const $loginStatus = $('#loginStatus');
    const $chatInterface = $('#chatInterface');
    const $userForm = $('#userForm');

    if (isLoggedIn) {
        // Clear and update navbar items
        $navbarItems.empty().append('<li class="nav-item"><a class="nav-link text-dark" href="/">Home</a></li>');
        
        // Add chat room links
        $.each(navLinks, (index, link) => {
            $navbarItems.append(`<li class="nav-item"><a class="nav-link text-dark" href="${link.href}">${link.text}</a></li>`);
        });

        // Update login status
        $loginStatus.html(`
            <div class="d-flex align-items-center">
                <span class="nav-link text-dark me-3">Welcome, ${currentUsername}!</span>
                <button id="logoutButton" class="btn btn-link nav-link text-dark">Logout</button>
            </div>
        `);

        // Attach logout event listener
        $('#logoutButton').on('click', logout);

        // Show/hide elements
        $chatInterface.show();
        $userForm.hide();
    } else {
        // Remove chat room links
        $('#globalChatLink').parent().remove();

        // Update login status
        $loginStatus.html('<span class="nav-link text-dark">Not logged in</span>');

        // Show/hide elements
        $chatInterface.hide();
        $userForm.show();
    }
}

function startSignalRConnection() {
    if (connection.state === signalR.HubConnectionState.Disconnected) {
        console.log('Attempting to connect');
        connection.start()
            .then(() => {
                console.log("SignalR Connected");
                if (currentUsername) {
                    connection.invoke("UserConnected", currentUsername);
                }
            })  
            .catch(function (err) {
                console.error("SignalR Connection Error: " + err.toString());
                setTimeout(startSignalRConnection, 5000);
            });
    }
}

function register(username) {
    $.ajax({
        url: "/User/Register",
        method: "POST",
        data: { username: username },
        success: () => {
            alert("Registration successful!");
        },
        error: () => {
            alert("Registration failed. Username might already exist.");
        }
    });
}

function login(username) {
    $.ajax({
        url: "/User/Login",
        method: "POST",
        data: { username: username },
        dataType: 'json',
        success: (data) => {
            if (data.username) {
                sessionStorage.setItem('username', data.username);
                currentUsername = data.username;
                checkAuthStatus();
            } else {
                alert("Login failed. User not found.");
            }
        },
        error: () => {
            alert("Login failed. Please try again.");
        }
    });
}

function logout() {
    const username = sessionStorage.getItem('username');
    $.ajax({
        url: "/User/Logout",
        method: "POST",
        data: { username: username },
        success: function() {
            sessionStorage.removeItem('username');
            currentUsername = '';
            checkAuthStatus();
        },
        error: function(error) {
            console.error('Error:', error);
        }
    });
}

function displayMessage(message) {
    const $msgFeed = $('#msg_feed');
    $msgFeed.append(`
        <div class="mb-2">
            <strong>${message.user}</strong>
            <span class="text-muted small ms-2">${new Date(message.timestamp).toLocaleTimeString()}</span>
            <p class="mb-0">${message.message}</p>
        </div>
    `);
}