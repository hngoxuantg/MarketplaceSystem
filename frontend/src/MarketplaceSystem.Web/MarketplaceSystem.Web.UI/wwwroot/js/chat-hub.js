// SignalR Chat Hub Connection
let chatHubConnection = null;
let currentUserId = null;

/**
 * Get JWT token from cookie
 */
function getAccessToken() {
    const cookies = document.cookie.split(';');
    for (let cookie of cookies) {
        const [name, value] = cookie.trim().split('=');
        if (name === 'accessToken') {
            return decodeURIComponent(value);
        }
    }
    return null;
}

/**
 * Initialize SignalR connection
 */
async function initializeChatHub(userId) {
    currentUserId = userId;

    const token = getAccessToken();
    
    if (!token) {
        console.error("❌ No access token found. User must be logged in.");
        updateConnectionStatus(false);
        return;
    }

    console.log("🔑 Token found, connecting to SignalR...");

    // Build SignalR connection - Send token via query string for SignalR
    chatHubConnection = new signalR.HubConnectionBuilder()
        .withUrl("https://localhost:7079/hubs/chat?access_token=" + encodeURIComponent(token), {
            transport: signalR.HttpTransportType.WebSockets | signalR.HttpTransportType.LongPolling,
            skipNegotiation: false
        })
        .withAutomaticReconnect([0, 2000, 5000, 10000, 30000])
        .configureLogging(signalR.LogLevel.Information)
        .build();

    // Setup event handlers
    setupHubEventHandlers();

    // Start connection
    try {
        await chatHubConnection.start();
        console.log("✅ SignalR Connected to ChatHub");
        
        // Update connection status
        updateConnectionStatus(true);
        
        // Load conversations immediately after connecting
        console.log("📥 Loading conversations via SignalR...");
        await getConversationsViaHub(1, 20);
        
    } catch (err) {
        console.error("❌ SignalR Connection Error:", err);
        updateConnectionStatus(false);
        
        // Retry after 5 seconds
        setTimeout(() => initializeChatHub(userId), 5000);
    }
}

/**
 * Setup SignalR event handlers
 */
function setupHubEventHandlers() {
    if (!chatHubConnection) return;

    // Receive new private message
    chatHubConnection.on("ReceivePrivateMessage", (message) => {
        console.log("📨 ReceivePrivateMessage event:", message);
        console.table({
            'Message ID': message.id,
            'Conversation ID': message.conversationId,
            'Sender ID': message.senderId,
            'Receiver ID': message.receiverId,
            'Content': message.content,
            'Type': message.type,
            'Sent At': message.sentAt
        });
        handleReceivedMessage(message);
    });

    // Receive messages list (from GetConversationMessages)
    chatHubConnection.on("ReceiveMessages", (messages) => {
        console.log("📋 ReceiveMessages event - Raw data:", messages);
        console.log("📋 Data type:", typeof messages);
        console.log("📋 Is array:", Array.isArray(messages));
        
        // Handle different response formats
        let messageArray = messages;
        
        // If it's an object with data property
        if (messages && typeof messages === 'object' && !Array.isArray(messages)) {
            if (messages.data) {
                messageArray = messages.data;
                console.log("📋 Extracted data array:", messageArray);
            } else if (messages.messages) {
                messageArray = messages.messages;
                console.log("📋 Extracted messages array:", messageArray);
            }
        }
        
        console.log("📋 Final messages array - Total:", messageArray?.length || 0);
        handleReceivedMessages(messageArray);
    });

    // Messages marked as read
    chatHubConnection.on("MessagesRead", (conversationId, readByUserId) => {
        console.log(`✓ MessagesRead event:`, {
            conversationId,
            readByUserId,
            'Is current user': readByUserId === currentUserId
        });
        handleMessagesRead(conversationId, readByUserId);
    });

    // Receive conversations list
    chatHubConnection.on("ReceiveConversations", (conversations) => {
        console.log("📋 ReceiveConversations event:", conversations);
        console.log("Conversations count:", conversations?.data?.length || 0);
        console.log("Total count:", conversations?.totalCount || 0);
        handleReceivedConversations(conversations);
    });

    // Connection closed
    chatHubConnection.onclose((error) => {
        console.warn("🔌 SignalR Connection Closed");
        if (error) {
            console.error("Error:", error);
        }
        updateConnectionStatus(false);
    });

    // Reconnecting
    chatHubConnection.onreconnecting((error) => {
        console.warn("🔄 SignalR Reconnecting...");
        if (error) {
            console.error("Reconnect reason:", error);
        }
        updateConnectionStatus(false);
    });

    // Reconnected
    chatHubConnection.onreconnected((connectionId) => {
        console.log("✅ SignalR Reconnected");
        console.log("New connection ID:", connectionId);
        updateConnectionStatus(true);
    });
}

/**
 * Send private message via SignalR
 */
async function sendMessageViaHub(receiverId, content) {
    if (!chatHubConnection || chatHubConnection.state !== signalR.HubConnectionState.Connected) {
        console.error("❌ SignalR not connected. State:", chatHubConnection?.state);
        return false;
    }

    try {
        const request = {
            receiverId: receiverId,
            content: content || null
        };

        console.log("📤 Sending message via SignalR:");
        console.table({
            'Receiver ID': receiverId,
            'Content': content || '(no text)'
        });

        await chatHubConnection.invoke("SendPrivateMessage", request);
        console.log("✅ Message sent successfully via SignalR");
        return true;
        
    } catch (err) {
        console.error("❌ Error sending message via SignalR:");
        console.error(err);
        return false;
    }
}

/**
 * Load conversation messages via SignalR
 */
async function loadMessagesViaHub(conversationId, lastMessageId = null, loadOlder = true) {
    if (!chatHubConnection || chatHubConnection.state !== signalR.HubConnectionState.Connected) {
        console.error("❌ SignalR not connected");
        return false;
    }

    try {
        console.log("📥 Loading messages via SignalR:");
        console.table({
            'Conversation ID': conversationId,
            'Last Message ID': lastMessageId || 'N/A',
            'Load Older': loadOlder
        });

        await chatHubConnection.invoke("GetConversationMessages", conversationId, lastMessageId, loadOlder);
        console.log("✅ Message load request sent via SignalR");
        return true;
        
    } catch (err) {
        console.error("❌ Error loading messages via SignalR:");
        console.error(err);
        return false;
    }
}

/**
 * Mark messages as read via SignalR
 */
async function markMessagesAsReadViaHub(conversationId) {
    if (!chatHubConnection || chatHubConnection.state !== signalR.HubConnectionState.Connected) {
        console.error("❌ SignalR not connected");
        return false;
    }

    try {
        console.log(`✓ Marking messages as read - Conversation ID: ${conversationId}`);
        await chatHubConnection.invoke("MarkMessagesAsRead", conversationId);
        console.log("✅ Mark as read request sent via SignalR");
        return true;
        
    } catch (err) {
        console.error("❌ Error marking messages as read:");
        console.error(err);
        return false;
    }
}

/**
 * Get conversations via SignalR
 */
async function getConversationsViaHub(pageNumber = 1, pageSize = 12) {
    if (!chatHubConnection || chatHubConnection.state !== signalR.HubConnectionState.Connected) {
        console.error("❌ SignalR not connected");
        return false;
    }

    try {
        const request = {
            pageNumber: pageNumber,
            pageSize: pageSize
        };

        console.log("📥 Getting conversations via SignalR:");
        console.table({
            'Page Number': pageNumber,
            'Page Size': pageSize
        });

        await chatHubConnection.invoke("GetConversations", request);
        console.log("✅ Conversations request sent via SignalR");
        return true;
        
    } catch (err) {
        console.error("❌ Error getting conversations:");
        console.error(err);
        return false;
    }
}

/**
 * Update connection status indicator
 */
function updateConnectionStatus(isConnected) {
    const statusIndicator = document.querySelector('.connection-status');
    if (statusIndicator) {
        if (isConnected) {
            statusIndicator.classList.remove('disconnected');
            statusIndicator.classList.add('connected');
            statusIndicator.textContent = 'Đã kết nối';
        } else {
            statusIndicator.classList.remove('connected');
            statusIndicator.classList.add('disconnected');
            statusIndicator.textContent = 'Mất kết nối';
        }
    }
}

/**
 * Disconnect from SignalR hub
 */
async function disconnectChatHub() {
    if (chatHubConnection) {
        try {
            await chatHubConnection.stop();
            console.log("🔌 SignalR Disconnected");
        } catch (err) {
            console.error("❌ Error disconnecting:", err);
        }
    }
}

// Disconnect when page unloads
window.addEventListener('beforeunload', () => {
    disconnectChatHub();
});
