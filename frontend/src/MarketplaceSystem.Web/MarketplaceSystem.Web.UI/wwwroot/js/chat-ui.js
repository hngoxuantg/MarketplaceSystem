// Chat UI Management
let currentConversationId = null;
let currentReceiverId = null;
let currentReceiverName = null;
let oldestMessageId = null; // Track oldest message ID for loading older
let newestMessageId = null; // Track newest message ID for loading newer
let isLoadingMessages = false; // Prevent multiple simultaneous loads
let hasOlderMessages = true; // Track if there are older messages to load
let hasNewerMessages = false; // Track if there are newer messages to load
let currentLoadDirection = null; // Track which direction we're loading: 'older', 'newer', or 'initial'

/**
 * Initialize chat UI
 */
function initializeChat(userId) {
    currentUserId = userId;
    
    console.log('🚀 Initializing chat with userId:', userId);
    
    // Initialize SignalR
    initializeChatHub(userId);
    
    // Setup event listeners
    setupChatEventListeners();
    
    // Auto-resize textarea
    autoResizeTextarea();
    
    // Setup infinite scroll for loading older messages
    setupInfiniteScroll();
    
    // Note: handleUrlParameters() will be called AFTER conversations are loaded
    // to ensure we can find existing conversations
}

/**
 * Handle URL parameters to auto-open chat
 */
function handleUrlParameters() {
    const urlParams = new URLSearchParams(window.location.search);
    const userId = urlParams.get('userId');
    const userName = urlParams.get('userName');
    
    console.log('🔍 Checking URL params - userId:', userId, 'userName:', userName);
    
    if (!userId || !userName) {
        return; // No URL params, nothing to do
    }
    
    // Wait for DOM to be fully ready
    const checkAndOpenChat = () => {
        const chatPlaceholder = document.querySelector('.chat-placeholder');
        const chatContent = document.querySelector('.chat-content');
        
        if (!chatPlaceholder || !chatContent) {
            console.warn('⏳ Chat elements not ready, retrying...');
            setTimeout(checkAndOpenChat, 100);
            return;
        }
        
        console.log('✅ DOM ready, checking for existing conversation');
        
        // Find existing conversation with this user
        const conversationItem = document.querySelector(`[data-user-id="${userId}"]`);
        
        if (conversationItem) {
            console.log('📋 Found existing conversation, loading chat history');
            // Conversation exists, open it with full history
            const conversationId = parseInt(conversationItem.dataset.conversationId);
            openConversation(conversationId, parseInt(userId), decodeURIComponent(userName));
        } else {
            console.log('✨ No existing conversation, creating new one');
            // New conversation, prepare UI
            prepareNewConversation(parseInt(userId), decodeURIComponent(userName));
        }
    };
    
    // Start checking (small delay to ensure DOM is ready)
    setTimeout(checkAndOpenChat, 100);
}

/**
 * Prepare UI for new conversation
 */
function prepareNewConversation(receiverId, receiverName) {
    console.log('💬 Preparing new conversation with:', receiverName, '(ID:', receiverId, ')');
    
    currentConversationId = null; // No conversation yet
    currentReceiverId = receiverId;
    currentReceiverName = receiverName;
    
    try {
        // Update hidden inputs
        const receiverIdInput = document.getElementById('receiverId');
        const conversationIdInput = document.getElementById('currentConversationId');
        
        if (receiverIdInput) {
            receiverIdInput.value = receiverId;
        }
        
        if (conversationIdInput) {
            conversationIdInput.value = '';
        }
        
        // Update UI - show chat content, hide placeholder
        const chatPlaceholder = document.querySelector('.chat-placeholder');
        const chatContent = document.querySelector('.chat-content');
        
        if (chatPlaceholder && chatContent) {
            chatPlaceholder.style.display = 'none';
            chatContent.style.display = 'flex';
            console.log('✅ Chat UI switched to conversation mode');
        }
        
        // Update chat header
        const usernameEl = document.querySelector('.chat-username');
        const statusEl = document.querySelector('.chat-status');
        
        if (usernameEl) {
            usernameEl.textContent = receiverName;
        }
        
        if (statusEl) {
            statusEl.textContent = 'Bắt đầu cuộc trò chuyện';
        }
        
        // Clear messages
        const messagesList = document.getElementById('messagesList');
        if (messagesList) {
            messagesList.innerHTML = '<div class="no-messages">Bắt đầu cuộc trò chuyện mới</div>';
        }
        
        // Focus on message input
        const messageInput = document.getElementById('messageContent');
        if (messageInput) {
            setTimeout(() => messageInput.focus(), 100);
        }
        
        console.log('✅ New conversation prepared successfully');
        
    } catch (err) {
        console.error('❌ Error preparing new conversation:', err);
    }
}

/**
 * Handle quick message from sessionStorage
 */
function handleQuickMessage() {
    const urlParams = new URLSearchParams(window.location.search);
    const hasQuickMsg = urlParams.get('quickMsg');
    
    if (hasQuickMsg === 'true') {
        const quickMsgData = sessionStorage.getItem('quickMessage');
        
        if (quickMsgData) {
            try {
                const data = JSON.parse(quickMsgData);
                
                // Set message content
                document.getElementById('messageContent').value = data.message;
                
                // Clear sessionStorage
                sessionStorage.removeItem('quickMessage');
                
                // Auto send after a short delay (optional)
                setTimeout(() => {
                    const shouldAutoSend = confirm(`Gửi tin nhắn: "${data.message}"?`);
                    if (shouldAutoSend) {
                        sendMessage();
                    }
                }, 500);
                
            } catch (err) {
                console.error('Error parsing quick message:', err);
            }
        }
    }
}

/**
 * Setup UI event listeners
 */
function setupChatEventListeners() {
    // Click on conversation item
    document.querySelectorAll('.conversation-item').forEach(item => {
        item.addEventListener('click', function() {
            const conversationId = parseInt(this.dataset.conversationId);
            const userId = parseInt(this.dataset.userId);
            const userName = this.querySelector('.conversation-name').textContent;
            
            openConversation(conversationId, userId, userName);
        });
    });
    
    // Send message form
    const messageForm = document.getElementById('messageForm');
    if (messageForm) {
        messageForm.addEventListener('submit', function(e) {
            e.preventDefault();
            sendMessage();
        });
    }
    
    // Message input - send on Enter (not Shift+Enter)
    const messageContent = document.getElementById('messageContent');
    if (messageContent) {
        messageContent.addEventListener('keydown', function(e) {
            if (e.key === 'Enter' && !e.shiftKey) {
                e.preventDefault();
                sendMessage();
            }
        });
    }
    
    // Close chat button
    const closeChatBtn = document.getElementById('closeChatBtn');
    if (closeChatBtn) {
        closeChatBtn.addEventListener('click', function() {
            closeChat();
        });
    }
    
    // Search conversations
    const searchInput = document.getElementById('searchConversation');
    if (searchInput) {
        searchInput.addEventListener('input', function(e) {
            filterConversations(e.target.value);
        });
    }
    
    // Load more conversations
    const loadMoreBtn = document.getElementById('loadMoreConversations');
    if (loadMoreBtn) {
        loadMoreBtn.addEventListener('click', function() {
            const nextPage = parseInt(this.dataset.page);
            loadMoreConversations(nextPage);
        });
    }
}

/**
 * Open a conversation
 */
async function openConversation(conversationId, receiverId, receiverName) {
    currentConversationId = conversationId;
    currentReceiverId = receiverId;
    currentReceiverName = receiverName;
    
    // Reset message IDs and pagination flags for new conversation
    oldestMessageId = null;
    newestMessageId = null;
    hasOlderMessages = true;
    hasNewerMessages = false; // Initial load gets newest messages, so no newer ones
    currentLoadDirection = 'initial';
    
    // Update hidden inputs
    document.getElementById('receiverId').value = receiverId;
    document.getElementById('currentConversationId').value = conversationId;
    
    // Update UI - show chat content, hide placeholder
    document.querySelector('.chat-placeholder').style.display = 'none';
    document.querySelector('.chat-content').style.display = 'flex';
    
    // Update chat header
    document.querySelector('.chat-username').textContent = receiverName;
    
    // Highlight selected conversation
    document.querySelectorAll('.conversation-item').forEach(item => {
        item.classList.remove('active');
    });
    document.querySelector(`[data-conversation-id="${conversationId}"]`)?.classList.add('active');
    
    // Clear messages
    document.getElementById('messagesList').innerHTML = '';
    
    // Show loading
    showMessagesLoading(true);
    
    // Load messages via SignalR (initial load with null lastMessageId)
    await loadMessagesViaHub(conversationId, null, true);
    
    // Mark as read
    await markMessagesAsReadViaHub(conversationId);
    
    // Clear unread badge
    const conversationItem = document.querySelector(`[data-conversation-id="${conversationId}"]`);
    const unreadBadge = conversationItem?.querySelector('.unread-badge');
    if (unreadBadge) {
        unreadBadge.remove();
    }
}

/**
 * Send message
 */
async function sendMessage() {
    const content = document.getElementById('messageContent').value.trim();
    const receiverId = parseInt(document.getElementById('receiverId').value);
    
    if (!content) {
        return;
    }
    
    if (!receiverId) {
        showToast('Vui lòng chọn người nhận', 'error');
        return;
    }
    
    // Disable send button
    const sendBtn = document.getElementById('sendMessageBtn');
    sendBtn.disabled = true;
    
    try {
        // ✅ Optimistic UI: Display message immediately
        const tempMessage = {
            id: Date.now(), // Temporary ID
            senderId: currentUserId,
            receiverId: receiverId,
            conversationId: currentConversationId,
            content: content,
            type: 'Text',
            sentAt: new Date().toISOString(),
            isOwnMessage: true
        };
        
        // Remove "no messages" placeholder if exists
        const noMessages = document.querySelector('.no-messages');
        if (noMessages) {
            noMessages.remove();
        }
        
        // Display message immediately
        appendMessage(tempMessage);
        scrollToBottom();
        
        // Clear input right away for better UX
        document.getElementById('messageContent').value = '';
        
        // Send via SignalR
        const success = await sendMessageViaHub(receiverId, content);
        
        if (!success) {
            showToast('Gửi tin nhắn thất bại', 'error');
            // Optionally: Remove the optimistically added message
            // document.querySelector(`[data-message-id="${tempMessage.id}"]`)?.remove();
        }
        
    } catch (err) {
        console.error('Error sending message:', err);
        showToast('Có lỗi xảy ra', 'error');
    } finally {
        sendBtn.disabled = false;
        document.getElementById('messageContent').focus();
    }
}

/**
 * Handle received message from SignalR
 */
function handleReceivedMessage(message) {
    console.log('📨 Received message:', message);
    console.log('🔍 Current state:', {
        currentConversationId,
        currentReceiverId,
        currentUserId,
        messageSenderId: message.senderId,
        messageReceiverId: message.receiverId,
        messageConversationId: message.conversationId
    });
    
    // If this is our sent message and we didn't have a conversationId yet, set it now
    if (!currentConversationId && message.senderId === currentUserId) {
        console.log('✅ Setting conversationId from our sent message');
        currentConversationId = message.conversationId;
        document.getElementById('currentConversationId').value = message.conversationId;
        
        // Update status text
        const statusEl = document.querySelector('.chat-status');
        if (statusEl) {
            statusEl.textContent = 'Đang hoạt động';
        }
        
        // Remove "no messages" placeholder if exists
        const noMessages = document.querySelector('.no-messages');
        if (noMessages) {
            noMessages.remove();
        }
    }
    
    // Determine if this message belongs to the currently open chat
    const isSameConversation = message.conversationId && message.conversationId === currentConversationId;
    const isMessageFromCurrentReceiver = message.senderId === currentReceiverId && message.receiverId === currentUserId;
    const isMessageToCurrentReceiver = message.senderId === currentUserId && message.receiverId === currentReceiverId;
    
    console.log('🔍 Message checks:', {
        isSameConversation,
        isMessageFromCurrentReceiver,
        isMessageToCurrentReceiver,
        shouldDisplay: isSameConversation || isMessageFromCurrentReceiver || isMessageToCurrentReceiver
    });
    
    // ✅ Show message if it belongs to current chat
    if (isSameConversation || isMessageFromCurrentReceiver || isMessageToCurrentReceiver) {
        console.log('✅ Message belongs to current chat, displaying...');
        
        // Update conversationId if we didn't have it
        if (!currentConversationId) {
            console.log('✅ Setting conversationId for new conversation');
            currentConversationId = message.conversationId;
            document.getElementById('currentConversationId').value = message.conversationId;
            
            // Update status text
            const statusEl = document.querySelector('.chat-status');
            if (statusEl) {
                statusEl.textContent = 'Đang hoạt động';
            }
        }
        
        // Remove "no messages" placeholder if exists
        const noMessages = document.querySelector('.no-messages');
        if (noMessages) {
            noMessages.remove();
        }
        
        // ✅ Add message to current chat window
        appendMessage(message);
        scrollToBottom();
        
        // Mark as read if we're actively viewing this conversation and it's not our message
        if (currentConversationId && message.senderId !== currentUserId) {
            console.log('📖 Marking messages as read');
            markMessagesAsReadViaHub(currentConversationId);
        }
        
        // Update conversation list item
        updateConversationLastMessage(message);
    } else {
        console.log('❌ Message NOT for current chat, updating conversation list only');
        // Message is NOT for current conversation
        // Update conversation list and show notification
        updateConversationLastMessage(message);
        
        // Show notification for new message from other users
        if (message.senderId !== currentUserId) {
            showMessageNotification(message);
        }
    }
}

/**
 * Handle received messages list
 */
function handleReceivedMessages(messages) {
    showMessagesLoading(false);
    isLoadingMessages = false;
    
    const messagesList = document.getElementById('messagesList');
    const messagesContainer = document.getElementById('messagesContainer');
    
    // If no messages returned
    if (!messages || messages.length === 0) {
        console.log('📋 No messages received');
        
        // Only show "no messages" if this is initial load
        if (currentLoadDirection === 'initial' || (!oldestMessageId && !newestMessageId)) {
            messagesList.innerHTML = '<div class="no-messages">Chưa có tin nhắn</div>';
        }
        
        // Mark that there are no more messages in this direction
        if (currentLoadDirection === 'older') {
            hasOlderMessages = false;
        } else if (currentLoadDirection === 'newer') {
            hasNewerMessages = false;
        }
        
        currentLoadDirection = null;
        return;
    }
    
    // Sort messages by time (oldest first)
    const sortedMessages = [...messages].sort((a, b) => 
        new Date(a.sentAt) - new Date(b.sentAt)
    );
    
    console.log('📋 Received', messages.length, 'messages, direction:', currentLoadDirection);
    
    // Handle based on load direction
    if (currentLoadDirection === 'initial') {
        // Initial load - clear and render all
        console.log('🔄 Initial load - clearing and rendering');
        messagesList.innerHTML = '';
        
        sortedMessages.forEach(message => {
            appendMessage(message, false);
        });
        
        // Set initial IDs
        oldestMessageId = sortedMessages[0].id;
        newestMessageId = sortedMessages[sortedMessages.length - 1].id;
        
        // Check if there might be more messages
        hasOlderMessages = messages.length >= 25;
        hasNewerMessages = false; // Initial load gets newest messages
        
        scrollToBottom();
        
    } else if (currentLoadDirection === 'older') {
        // Loading older messages - prepend to top
        console.log('⬆️ Prepending older messages to top');
        
        // Store current scroll position
        const scrollHeightBefore = messagesContainer.scrollHeight;
        const scrollTopBefore = messagesContainer.scrollTop;
        
        // Prepend messages in reverse order (newest of the old batch first)
        for (let i = sortedMessages.length - 1; i >= 0; i--) {
            prependMessage(sortedMessages[i]);
        }
        
        // Update oldest message ID
        oldestMessageId = sortedMessages[0].id;
        
        // Check if there are more older messages
        hasOlderMessages = messages.length >= 25;
        
        // Restore scroll position (keep user at same visual location)
        setTimeout(() => {
            const scrollHeightAfter = messagesContainer.scrollHeight;
            messagesContainer.scrollTop = scrollTopBefore + (scrollHeightAfter - scrollHeightBefore);
        }, 10);
        
    } else if (currentLoadDirection === 'newer') {
        // Loading newer messages - append to bottom
        console.log('⬇️ Appending newer messages to bottom');
        
        sortedMessages.forEach(message => {
            appendMessage(message, false);
        });
        
        // Update newest message ID
        newestMessageId = sortedMessages[sortedMessages.length - 1].id;
        
        // Check if there are more newer messages
        hasNewerMessages = messages.length >= 25;
        
        scrollToBottom();
    }
    
    console.log('📌 Message range:', oldestMessageId, '->', newestMessageId);
    console.log('📊 Pagination state - hasOlder:', hasOlderMessages, 'hasNewer:', hasNewerMessages);
    
    currentLoadDirection = null;
}

/**
 * Handle messages marked as read
 */
function handleMessagesRead(conversationId, readByUserId) {
    if (conversationId === currentConversationId && readByUserId !== currentUserId) {
        // Update read status visually if needed
        console.log('Messages read by other user');
    }
}

/**
 * Handle received conversations
 */
function handleReceivedConversations(conversations) {
    console.log('📋 Received conversations, updating UI');
    
    // Update conversations list UI
    const conversationsList = document.getElementById('conversationsList');
    conversationsList.innerHTML = '';
    
    if (!conversations || conversations.data.length === 0) {
        conversationsList.innerHTML = `
            <div class="empty-conversations">
                <i class="bi bi-chat-dots"></i>
                <p>Chưa có cuộc trò chuyện nào</p>
            </div>
        `;
    } else {
        conversations.data.forEach(conv => {
            const conversationHtml = createConversationItemHtml(conv);
            conversationsList.insertAdjacentHTML('beforeend', conversationHtml);
        });
    }
    
    // Re-attach event listeners
    setupChatEventListeners();
    
    // ✅ NOW handle URL parameters after conversations are loaded
    handleUrlParameters();
    
    // Handle quick message from product detail
    handleQuickMessage();
}

/**
 * Append message to messages list
 */
function appendMessage(message, shouldScroll = true) {
    const messagesList = document.getElementById('messagesList');
    const isOwnMessage = message.senderId === currentUserId;
    
    const messageHtml = `
        <div class="message ${isOwnMessage ? 'message-sent' : 'message-received'}" data-message-id="${message.id}">
            <div class="message-content">
                ${message.attachmentUrl ? `<img src="${message.attachmentUrl}" alt="Image" class="message-image" />` : ''}
                ${message.content ? `<p>${escapeHtml(message.content)}</p>` : ''}
                <span class="message-time">${formatMessageTime(message.sentAt)}</span>
            </div>
        </div>
    `;
    
    messagesList.insertAdjacentHTML('beforeend', messageHtml);
    
    if (shouldScroll) {
        scrollToBottom();
    }
}

/**
 * Prepend message to messages list (for older messages)
 */
function prependMessage(message) {
    const messagesList = document.getElementById('messagesList');
    const isOwnMessage = message.senderId === currentUserId;
    
    const messageHtml = `
        <div class="message ${isOwnMessage ? 'message-sent' : 'message-received'}" data-message-id="${message.id}">
            <div class="message-content">
                ${message.attachmentUrl ? `<img src="${message.attachmentUrl}" alt="Image" class="message-image" />` : ''}
                ${message.content ? `<p>${escapeHtml(message.content)}</p>` : ''}
                <span class="message-time">${formatMessageTime(message.sentAt)}</span>
            </div>
        </div>
    `;
    
    messagesList.insertAdjacentHTML('afterbegin', messageHtml);
}

/**
 * Create conversation item HTML
 */
function createConversationItemHtml(conversation) {
    const avatarHtml = conversation.userAvatar 
        ? `<img src="${conversation.userAvatar}" alt="${conversation.userName}">`
        : `<div class="avatar-placeholder">${conversation.userName.substring(0, 1).toUpperCase()}</div>`;
    
    const lastMessageHtml = conversation.lastMessageType === 'Image'
        ? '<i class="bi bi-image"></i> <span>Hình ảnh</span>'
        : escapeHtml(conversation.lastMessage || '');
    
    const unreadBadgeHtml = conversation.unreadCount > 0
        ? `<span class="unread-badge">${conversation.unreadCount}</span>`
        : '';
    
    return `
        <div class="conversation-item" data-conversation-id="${conversation.conversationId}" data-user-id="${conversation.userId}">
            <div class="conversation-avatar">
                ${avatarHtml}
                ${conversation.isOnline ? '<span class="online-indicator"></span>' : ''}
            </div>
            <div class="conversation-info">
                <div class="conversation-header-info">
                    <h6 class="conversation-name">${escapeHtml(conversation.userName)}</h6>
                    <span class="conversation-time">${conversation.timeAgo}</span>
                </div>
                <div class="conversation-preview">
                    <p class="last-message">${lastMessageHtml}</p>
                    ${unreadBadgeHtml}
                </div>
            </div>
        </div>
    `;
}

/**
 * Update conversation last message
 */
function updateConversationLastMessage(message) {
    const conversationItem = document.querySelector(`[data-conversation-id="${message.conversationId}"]`);
    if (!conversationItem) return;
    
    // Update last message text
    const lastMessageEl = conversationItem.querySelector('.last-message');
    if (lastMessageEl) {
        if (message.type === 'Image') {
            lastMessageEl.innerHTML = '<i class="bi bi-image"></i> <span>Hình ảnh</span>';
        } else {
            lastMessageEl.textContent = message.content;
        }
    }
    
    // Add unread badge if not current conversation
    if (message.conversationId !== currentConversationId && message.senderId !== currentUserId) {
        let unreadBadge = conversationItem.querySelector('.unread-badge');
        if (!unreadBadge) {
            unreadBadge = document.createElement('span');
            unreadBadge.className = 'unread-badge';
            unreadBadge.textContent = '1';
            conversationItem.querySelector('.conversation-preview').appendChild(unreadBadge);
        } else {
            const currentCount = parseInt(unreadBadge.textContent) || 0;
            unreadBadge.textContent = currentCount + 1;
        }
    }
    
    // Move conversation to top
    const conversationsList = document.getElementById('conversationsList');
    conversationsList.insertBefore(conversationItem, conversationsList.firstChild);
}

/**
 * Close chat
 */
function closeChat() {
    currentConversationId = null;
    currentReceiverId = null;
    currentReceiverName = null;
    
    document.querySelector('.chat-content').style.display = 'none';
    document.querySelector('.chat-placeholder').style.display = 'flex';
    
    // Clear active state
    document.querySelectorAll('.conversation-item').forEach(item => {
        item.classList.remove('active');
    });
}

/**
 * Filter conversations by search term
 */
function filterConversations(searchTerm) {
    const conversations = document.querySelectorAll('.conversation-item');
    const term = searchTerm.toLowerCase();
    
    conversations.forEach(conv => {
        const name = conv.querySelector('.conversation-name').textContent.toLowerCase();
        if (name.includes(term)) {
            conv.style.display = 'flex';
        } else {
            conv.style.display = 'none';
        }
    });
}

/**
 * Load more conversations
 */
async function loadMoreConversations(pageNumber) {
    const loadMoreBtn = document.getElementById('loadMoreConversations');
    loadMoreBtn.disabled = true;
    loadMoreBtn.textContent = 'Đang tải...';
    
    await getConversationsViaHub(pageNumber, 12);
    
    loadMoreBtn.disabled = false;
    loadMoreBtn.textContent = 'Tải thêm';
}

/**
 * Show/hide messages loading indicator
 */
function showMessagesLoading(show) {
    const loadingEl = document.querySelector('.messages-loading');
    if (loadingEl) {
        loadingEl.style.display = show ? 'flex' : 'none';
    }
}

/**
 * Setup infinite scroll for messages
 */
function setupInfiniteScroll() {
    const messagesContainer = document.getElementById('messagesContainer');
    if (!messagesContainer) return;
    
    let scrollTimeout;
    
    messagesContainer.addEventListener('scroll', function() {
        // Debounce scroll events
        clearTimeout(scrollTimeout);
        scrollTimeout = setTimeout(() => {
            if (isLoadingMessages || !currentConversationId) return;
            
            const scrollTop = messagesContainer.scrollTop;
            const scrollHeight = messagesContainer.scrollHeight;
            const clientHeight = messagesContainer.clientHeight;
            
            // Scrolled to top - load older messages
            if (scrollTop < 100 && oldestMessageId && hasOlderMessages) {
                console.log('⬆️ Scrolled to top, loading older messages...');
                console.log('🔍 Current oldestMessageId:', oldestMessageId);
                loadOlderMessages();
            }
            
            // Scrolled to bottom - load newer messages (only if flag says there are more)
            else if (scrollTop + clientHeight >= scrollHeight - 100 && newestMessageId && hasNewerMessages) {
                console.log('⬇️ At bottom, loading newer messages...');
                console.log('🔍 Current newestMessageId:', newestMessageId);
                loadNewerMessages();
            }
        }, 200);
    });
}

/**
 * Load older messages (scroll up)
 */
async function loadOlderMessages() {
    if (!currentConversationId || !oldestMessageId || isLoadingMessages) return;
    
    isLoadingMessages = true;
    currentLoadDirection = 'older';
    showMessagesLoading(true);
    
    console.log('📥 Loading older messages, lastMessageId:', oldestMessageId);
    await loadMessagesViaHub(currentConversationId, oldestMessageId, true);
}

/**
 * Load newer messages (scroll down)
 */
async function loadNewerMessages() {
    if (!currentConversationId || !newestMessageId || isLoadingMessages) return;
    
    isLoadingMessages = true;
    currentLoadDirection = 'newer';
    showMessagesLoading(true);
    
    console.log('📥 Loading newer messages, lastMessageId:', newestMessageId);
    await loadMessagesViaHub(currentConversationId, newestMessageId, false);
}

/**
 * Scroll messages to bottom
 */
function scrollToBottom() {
    const messagesContainer = document.getElementById('messagesContainer');
    if (messagesContainer) {
        messagesContainer.scrollTop = messagesContainer.scrollHeight;
    }
}

/**
 * Auto-resize textarea as user types
 */
function autoResizeTextarea() {
    const textarea = document.getElementById('messageContent');
    if (!textarea) return;
    
    textarea.addEventListener('input', function() {
        this.style.height = 'auto';
        this.style.height = Math.min(this.scrollHeight, 120) + 'px';
    });
}

/**
 * Format message time
 */
function formatMessageTime(dateString) {
    const date = new Date(dateString);
    const now = new Date();
    const diff = now - date;
    
    // Less than 1 minute
    if (diff < 60000) {
        return 'Vừa xong';
    }
    
    // Less than 1 hour
    if (diff < 3600000) {
        const minutes = Math.floor(diff / 60000);
        return `${minutes} phút trước`;
    }
    
    // Less than 1 day
    if (diff < 86400000) {
        const hours = Math.floor(diff / 3600000);
        return `${hours} giờ trước`;
    }
    
    // Same year
    if (date.getFullYear() === now.getFullYear()) {
        return date.toLocaleDateString('vi-VN', { day: '2-digit', month: '2-digit' });
    }
    
    // Different year
    return date.toLocaleDateString('vi-VN', { day: '2-digit', month: '2-digit', year: 'numeric' });
}

/**
 * Escape HTML to prevent XSS
 */
function escapeHtml(text) {
    if (!text) return '';
    const div = document.createElement('div');
    div.textContent = text;
    return div.innerHTML;
}

/**
 * Show message notification
 */
function showMessageNotification(message) {
    // Use existing toast notification or browser notification
    if (typeof showToast === 'function') {
        showToast(`Tin nhắn mới từ ${message.senderName || 'người dùng'}`, 'info');
    }
    
    // Browser notification (if permission granted)
    if ('Notification' in window && Notification.permission === 'granted') {
        new Notification('Tin nhắn mới', {
            body: message.content || 'Bạn có tin nhắn mới',
            icon: '/images/chat-icon.png'
        });
    }
}

/**
 * Show toast message (using existing toast system)
 */
function showToast(message, type = 'info') {
    // Integrate with existing toast notification system
    if (typeof showToastNotification === 'function') {
        showToastNotification(message, type);
    } else {
        console.log(`[${type.toUpperCase()}] ${message}`);
    }
}
