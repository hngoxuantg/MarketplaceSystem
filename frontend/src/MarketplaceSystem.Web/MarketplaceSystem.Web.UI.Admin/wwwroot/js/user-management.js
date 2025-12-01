// User Management JavaScript
(function () {
    'use strict';

    // Initialize when DOM is ready
    document.addEventListener('DOMContentLoaded', function () {
        initializeLockUserModal();
        initializeUnlockUserModal();
    });

    // Lock User Modal
    function initializeLockUserModal() {
        const lockButtons = document.querySelectorAll('.btn-lock-user');
        const lockModal = new bootstrap.Modal(document.getElementById('lockUserModal'));
        const lockUserIdInput = document.getElementById('lockUserId');
        const lockUserNameSpan = document.getElementById('lockUserName');
        const lockUntilDateInput = document.getElementById('lockUntilDate');
        const confirmLockBtn = document.getElementById('confirmLockBtn');

        lockButtons.forEach(button => {
            button.addEventListener('click', function () {
                const userId = this.getAttribute('data-user-id');
                const userName = this.getAttribute('data-user-name');

                lockUserIdInput.value = userId;
                lockUserNameSpan.textContent = userName;

                // Set minimum date to now
                const now = new Date();
                now.setMinutes(now.getMinutes() - now.getTimezoneOffset());
                lockUntilDateInput.min = now.toISOString().slice(0, 16);

                // Set default to 7 days from now
                const defaultDate = new Date();
                defaultDate.setDate(defaultDate.getDate() + 7);
                defaultDate.setMinutes(defaultDate.getMinutes() - defaultDate.getTimezoneOffset());
                lockUntilDateInput.value = defaultDate.toISOString().slice(0, 16);

                lockModal.show();
            });
        });

        if (confirmLockBtn) {
            confirmLockBtn.addEventListener('click', function () {
                const userId = lockUserIdInput.value;
                const untilDate = lockUntilDateInput.value;

                if (!untilDate) {
                    showAlert('Vui lòng chọn thời gian khóa!', 'warning');
                    return;
                }

                const untilDateTime = new Date(untilDate);
                if (untilDateTime <= new Date()) {
                    showAlert('Thời gian khóa phải sau thời gian hiện tại!', 'warning');
                    return;
                }

                lockUser(userId, untilDateTime);
                lockModal.hide();
            });
        }
    }

    // Unlock User Modal
    function initializeUnlockUserModal() {
        const unlockButtons = document.querySelectorAll('.btn-unlock-user');
        const unlockModal = new bootstrap.Modal(document.getElementById('unlockUserModal'));
        const unlockUserIdInput = document.getElementById('unlockUserId');
        const unlockUserNameSpan = document.getElementById('unlockUserName');
        const confirmUnlockBtn = document.getElementById('confirmUnlockBtn');

        unlockButtons.forEach(button => {
            button.addEventListener('click', function () {
                const userId = this.getAttribute('data-user-id');
                const userName = this.getAttribute('data-user-name');

                unlockUserIdInput.value = userId;
                unlockUserNameSpan.textContent = userName;

                unlockModal.show();
            });
        });

        if (confirmUnlockBtn) {
            confirmUnlockBtn.addEventListener('click', function () {
                const userId = unlockUserIdInput.value;
                unlockUser(userId);
                unlockModal.hide();
            });
        }
    }

    // Lock User API Call
    function lockUser(userId, untilDate) {
        const loadingBtn = showLoading('Đang khóa người dùng...');

        fetch('/User/Lock', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
            },
            body: JSON.stringify({
                id: parseInt(userId),
                until: untilDate.toISOString()
            })
        })
            .then(response => response.json())
            .then(data => {
                hideLoading(loadingBtn);
                if (data.success) {
                    showAlert(data.message || 'Khóa người dùng thành công!', 'success');
                    setTimeout(() => {
                        location.reload();
                    }, 1500);
                } else {
                    showAlert(data.message || 'Không thể khóa người dùng!', 'danger');
                }
            })
            .catch(error => {
                hideLoading(loadingBtn);
                console.error('Error:', error);
                showAlert('Đã xảy ra lỗi khi khóa người dùng!', 'danger');
            });
    }

    // Unlock User API Call
    function unlockUser(userId) {
        const loadingBtn = showLoading('Đang mở khóa người dùng...');

        fetch('/User/Unlock', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
            },
            body: JSON.stringify({
                id: parseInt(userId)
            })
        })
            .then(response => response.json())
            .then(data => {
                hideLoading(loadingBtn);
                if (data.success) {
                    showAlert(data.message || 'Mở khóa người dùng thành công!', 'success');
                    setTimeout(() => {
                        location.reload();
                    }, 1500);
                } else {
                    showAlert(data.message || 'Không thể mở khóa người dùng!', 'danger');
                }
            })
            .catch(error => {
                hideLoading(loadingBtn);
                console.error('Error:', error);
                showAlert('Đã xảy ra lỗi khi mở khóa người dùng!', 'danger');
            });
    }

    // Show Alert
    function showAlert(message, type) {
        const alertDiv = document.createElement('div');
        alertDiv.className = `alert alert-${type} alert-dismissible fade show position-fixed top-0 start-50 translate-middle-x mt-3`;
        alertDiv.style.zIndex = '9999';
        alertDiv.innerHTML = `
            ${message}
            <button type="button" class="btn-close" data-bs-dismiss="alert"></button>
        `;
        document.body.appendChild(alertDiv);

        setTimeout(() => {
            alertDiv.remove();
        }, 5000);
    }

    // Show Loading
    function showLoading(message) {
        const loadingDiv = document.createElement('div');
        loadingDiv.className = 'position-fixed top-0 start-0 w-100 h-100 d-flex align-items-center justify-content-center';
        loadingDiv.style.zIndex = '10000';
        loadingDiv.style.backgroundColor = 'rgba(0,0,0,0.5)';
        loadingDiv.innerHTML = `
            <div class="text-center">
                <div class="spinner-border text-light" role="status">
                    <span class="visually-hidden">Loading...</span>
                </div>
                <p class="text-light mt-2">${message}</p>
            </div>
        `;
        document.body.appendChild(loadingDiv);
        return loadingDiv;
    }

    // Hide Loading
    function hideLoading(loadingDiv) {
        if (loadingDiv && loadingDiv.parentNode) {
            loadingDiv.parentNode.removeChild(loadingDiv);
        }
    }

})();
