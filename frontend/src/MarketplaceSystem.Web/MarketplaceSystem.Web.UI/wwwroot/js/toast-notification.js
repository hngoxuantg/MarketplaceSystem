// Toast Notification System
const ToastNotification = {
    container: null,

    init() {
        if (!this.container) {
            this.container = document.createElement('div');
            this.container.className = 'toast-container';
            document.body.appendChild(this.container);
        }
    },

    show(options) {
        this.init();

        const {
            type = 'info',
            title = '',
            message = '',
            duration = 5000,
            closable = true
        } = options;

        // Create toast element
        const toast = document.createElement('div');
        toast.className = `toast toast-${type}`;
        
        // Icon based on type
        const icons = {
            success: '<i class="fas fa-check-circle"></i>',
            error: '<i class="fas fa-times-circle"></i>',
            warning: '<i class="fas fa-exclamation-triangle"></i>',
            info: '<i class="fas fa-info-circle"></i>'
        };

        // Build toast HTML
        toast.innerHTML = `
            <div class="toast-icon">
                ${icons[type] || icons.info}
            </div>
            <div class="toast-content">
                ${title ? `<div class="toast-title">${title}</div>` : ''}
                <div class="toast-message">${message}</div>
            </div>
            ${closable ? '<button class="toast-close" aria-label="Close"><i class="fas fa-times"></i></button>' : ''}
            ${duration > 0 ? `<div class="toast-progress" style="color: ${this.getProgressColor(type)}"></div>` : ''}
        `;

        // Add to container
        this.container.appendChild(toast);

        // Show animation
        setTimeout(() => toast.classList.add('show'), 10);

        // Close button handler
        if (closable) {
            const closeBtn = toast.querySelector('.toast-close');
            closeBtn.addEventListener('click', () => this.remove(toast));
        }

        // Auto remove
        if (duration > 0) {
            setTimeout(() => this.remove(toast), duration);
        }

        return toast;
    },

    remove(toast) {
        toast.classList.remove('show');
        toast.classList.add('hide');
        setTimeout(() => {
            if (toast.parentElement) {
                toast.parentElement.removeChild(toast);
            }
        }, 400);
    },

    getProgressColor(type) {
        const colors = {
            success: '#10b981',
            error: '#ef4444',
            warning: '#f59e0b',
            info: '#3b82f6'
        };
        return colors[type] || colors.info;
    },

    // Shorthand methods
    success(message, title = 'Thành công!') {
        return this.show({ type: 'success', title, message });
    },

    error(message, title = 'Lỗi!') {
        return this.show({ type: 'error', title, message });
    },

    warning(message, title = 'Cảnh báo!') {
        return this.show({ type: 'warning', title, message });
    },

    info(message, title = 'Thông báo!') {
        return this.show({ type: 'info', title, message });
    }
};

// Make it global
window.Toast = ToastNotification;

// Auto-show toasts from TempData
document.addEventListener('DOMContentLoaded', function() {
    // Check for success message
    const successMessage = document.querySelector('[data-toast-success]');
    if (successMessage) {
        Toast.success(successMessage.getAttribute('data-toast-success'));
    }

    // Check for error message
    const errorMessage = document.querySelector('[data-toast-error]');
    if (errorMessage) {
        Toast.error(errorMessage.getAttribute('data-toast-error'));
    }

    // Check for warning message
    const warningMessage = document.querySelector('[data-toast-warning]');
    if (warningMessage) {
        Toast.warning(warningMessage.getAttribute('data-toast-warning'));
    }

    // Check for info message
    const infoMessage = document.querySelector('[data-toast-info]');
    if (infoMessage) {
        Toast.info(infoMessage.getAttribute('data-toast-info'));
    }
});
