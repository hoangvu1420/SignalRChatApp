const connection = new signalR.HubConnectionBuilder().withUrl("/chatHub").build();
let isLoggedIn = false;
let currentUsername = '';

let navLinks = [
    { text: 'Group Chat', href: '/GroupChat' },
]

document.addEventListener('DOMContentLoaded', () => {
    const logoutButton = document.getElementById('logoutButton');
    if (logoutButton) {
        logoutButton.addEventListener('click', logout);
    }
    checkAuthStatus();
});

function checkAuthStatus() {
    fetch('/User/CheckAuth')
        .then(response => response.json())
        .then(data => {
            isLoggedIn = data.isAuthenticated;
            currentUsername = data.username || '';
            updateNavbar();
            if (isLoggedIn) {
                startSignalRConnection();
            }
        })
        .catch(error => console.error('Error checking auth status:', error));
}

function updateNavbar() {
    const navbarItems = document.getElementById('navbarItems');
    const loginStatus = document.getElementById('loginStatus');
    const chatInterface = document.getElementById('chatInterface');
    const userForm = document.getElementById('userForm');

    if (isLoggedIn) {
        // Remove all existing navbar items except the home link
        if (navbarItems) {
            navbarItems.innerHTML = '';
            const homeLink = document.createElement('li');
            homeLink.className = 'nav-item';
            const homeA = document.createElement('a');
            homeA.className = 'nav-link text-dark';
            homeA.href = '/';
            homeA.textContent = 'Home';
            homeLink.appendChild(homeA);
            navbarItems.appendChild(homeLink);
        }

        // Add chat room links
        if (navbarItems) {
            navLinks.forEach(link => {
                const li = document.createElement('li');
                li.className = 'nav-item';
                const a = document.createElement('a');
                a.className = 'nav-link text-dark';
                a.href = link.href;
                a.textContent = link.text;
                li.appendChild(a);
                navbarItems.appendChild(li);
            });
        }

        // Update login status
        if (loginStatus) {
            loginStatus.innerHTML = `
                <div class="d-flex align-items-center">
                    <span class="nav-link text-dark me-3">Welcome, ${currentUsername}!</span>
                    <button id="logoutButton" class="btn btn-link nav-link text-dark">Logout</button>
                </div>
            `;

            // Reattach logout event listener
            const logoutButton = document.getElementById('logoutButton');
            if (logoutButton) {
                logoutButton.addEventListener('click', logout);
            }
        }

        // Show chat interface
        if (chatInterface) {
            chatInterface.style.display = 'block';
        }

        // Hide user form
        if (userForm) {
            userForm.style.display = 'none';
        }
    } else {
        // Remove chat room links
        if (navbarItems) {
            const globalChatLink = document.getElementById('globalChatLink');
            if (globalChatLink) {
                globalChatLink.parentElement.remove();
            }
        }

        // Update login status
        if (loginStatus) {
            loginStatus.innerHTML = '<span class="nav-link text-dark">Not logged in</span>';
        }

        // Hide chat interface
        if (chatInterface) {
            chatInterface.style.display = 'none';
        }

        // Show user form
        if (userForm) {
            userForm.style.display = 'block';
        }
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

function displayMessage(message) {
    const li = document.createElement("div");
    li.classList.add("mb-2");

    const strong = document.createElement("strong");
    strong.textContent = message.user;

    const timestamp = document.createElement("span");
    timestamp.classList.add("text-muted", "small", "ms-2");
    timestamp.textContent = new Date(message.timestamp).toLocaleTimeString();

    const p = document.createElement("p");
    p.classList.add("mb-0");
    p.textContent = message.message;

    li.appendChild(strong);
    li.appendChild(timestamp);
    li.appendChild(p);

    document.getElementById("msg_feed").appendChild(li);
}

function logout() {
    fetch('/User/Logout', { method: 'POST' })
        .then(response => {
            if (response.ok) {
                checkAuthStatus();
            }
        });
}