/**
 * Marketplace System - Main JavaScript File
 * Author: Marketplace Team
 * Version: 1.0.0
 */

(function () {
    'use strict';

    // ==========================================
    // Document Ready
    // ==========================================
    document.addEventListener('DOMContentLoaded', function () {
        initializeApp();
    });

    // ==========================================
    // Initialize Application
    // ==========================================
    function initializeApp() {
        initScrollReveal();
        initSmoothScroll();
        initHeaderScroll();
        initNumberCounter();
        initLazyLoading();
            initDarkMode();
    }

        // ==========================================
        // Dark Mode Toggle
        // ==========================================
        function initDarkMode() {
            const darkModeToggle = document.getElementById('darkModeToggle');
            if (!darkModeToggle) return;

                const themeIcon = document.querySelector('.theme-toggle__label i');
                const themeText = document.querySelector('.theme-toggle__label span');

            // Check for saved theme preference or default to light mode
            const currentTheme = localStorage.getItem('theme') || 'light';
            if (currentTheme === 'dark') {
                document.body.classList.add('dark-mode');
                darkModeToggle.checked = true;
                    if (themeIcon) {
                        themeIcon.className = 'fas fa-sun';
                    }
                    if (themeText) {
                        themeText.textContent = 'Giao diện sáng';
                    }
            }

            // Toggle dark mode
            darkModeToggle.addEventListener('change', function() {
                if (this.checked) {
                    document.body.classList.add('dark-mode');
                    localStorage.setItem('theme', 'dark');
                        if (themeIcon) {
                            themeIcon.className = 'fas fa-sun';
                        }
                        if (themeText) {
                            themeText.textContent = 'Giao diện sáng';
                        }
                } else {
                    document.body.classList.remove('dark-mode');
                    localStorage.setItem('theme', 'light');
                        if (themeIcon) {
                            themeIcon.className = 'fas fa-moon';
                        }
                        if (themeText) {
                            themeText.textContent = 'Giao diện tối';
                        }
                }
            });
        }

    // ==========================================
    // Scroll Reveal Animation
    // ==========================================
    function initScrollReveal() {
        const revealElements = document.querySelectorAll('.category-card, .product-card, .feature-card, .stat-item');

        const revealOnScroll = () => {
            revealElements.forEach((element) => {
                const elementTop = element.getBoundingClientRect().top;
                const elementVisible = 150;

                if (elementTop < window.innerHeight - elementVisible) {
                    element.classList.add('animate-on-scroll');
                }
            });
        };

        window.addEventListener('scroll', revealOnScroll);
        revealOnScroll(); // Initial check
    }

    // ==========================================
    // Smooth Scroll
    // ==========================================
    function initSmoothScroll() {
        document.querySelectorAll('a[href^="#"]').forEach(anchor => {
            anchor.addEventListener('click', function (e) {
                const href = this.getAttribute('href');
                if (href !== '#' && document.querySelector(href)) {
                    e.preventDefault();
                    const target = document.querySelector(href);
                    const headerOffset = 80;
                    const elementPosition = target.getBoundingClientRect().top;
                    const offsetPosition = elementPosition + window.pageYOffset - headerOffset;

                    window.scrollTo({
                        top: offsetPosition,
                        behavior: 'smooth'
                    });
                }
            });
        });
    }

    // ==========================================
    // Header Scroll Effect
    // ==========================================
    function initHeaderScroll() {
        const header = document.querySelector('.header');
        let lastScroll = 0;

        window.addEventListener('scroll', () => {
            const currentScroll = window.pageYOffset;

            if (currentScroll > 50) {
                header.classList.add('scrolled');
            } else {
                header.classList.remove('scrolled');
            }

            lastScroll = currentScroll;
        });
    }

    // ==========================================
    // Number Counter Animation
    // ==========================================
    function initNumberCounter() {
        const counters = document.querySelectorAll('.stat-number');
        const speed = 200;

        const countUp = (counter) => {
            const target = counter.innerText.replace(/[^0-9.]/g, '');
            const count = +target;
            const increment = count / speed;

            let current = 0;
            const updateCounter = () => {
                current += increment;
                if (current < count) {
                    counter.innerText = Math.ceil(current).toLocaleString();
                    requestAnimationFrame(updateCounter);
                } else {
                    counter.innerText = counter.dataset.originalText || counter.innerText;
                }
            };

            updateCounter();
        };

        const observerOptions = {
            threshold: 0.5,
            rootMargin: '0px'
        };

        const observer = new IntersectionObserver((entries) => {
            entries.forEach(entry => {
                if (entry.isIntersecting && !entry.target.dataset.counted) {
                    entry.target.dataset.originalText = entry.target.innerText;
                    countUp(entry.target);
                    entry.target.dataset.counted = 'true';
                }
            });
        }, observerOptions);

        counters.forEach(counter => observer.observe(counter));
    }

    // ==========================================
    // Lazy Loading Images
    // ==========================================
    function initLazyLoading() {
        const images = document.querySelectorAll('img[data-src]');

        const imageObserver = new IntersectionObserver((entries, observer) => {
            entries.forEach(entry => {
                if (entry.isIntersecting) {
                    const img = entry.target;
                    img.src = img.dataset.src;
                    img.removeAttribute('data-src');
                    observer.unobserve(img);
                }
            });
        });

        images.forEach(img => imageObserver.observe(img));
    }

    // ==========================================
    // Toast Notification
    // ==========================================
    window.showToast = function (message, type = 'info', duration = 3000) {
        const toast = document.createElement('div');
        toast.className = `toast toast-${type}`;
        toast.innerHTML = `
            <i class="fas fa-${getToastIcon(type)}"></i>
            <span>${message}</span>
        `;

        document.body.appendChild(toast);

        setTimeout(() => {
            toast.classList.add('show');
        }, 10);

        setTimeout(() => {
            toast.classList.remove('show');
            setTimeout(() => {
                toast.remove();
            }, 300);
        }, duration);
    };

    function getToastIcon(type) {
        const icons = {
            success: 'check-circle',
            error: 'exclamation-circle',
            warning: 'exclamation-triangle',
            info: 'info-circle'
        };
        return icons[type] || icons.info;
    }

    // ==========================================
    // Format Currency (VND)
    // ==========================================
    window.formatCurrency = function (amount) {
        return new Intl.NumberFormat('vi-VN', {
            style: 'currency',
            currency: 'VND'
        }).format(amount);
    };

    // ==========================================
    // Format Number
    // ==========================================
    window.formatNumber = function (number) {
        return new Intl.NumberFormat('vi-VN').format(number);
    };

    // ==========================================
    // Debounce Function
    // ==========================================
    window.debounce = function (func, wait) {
        let timeout;
        return function executedFunction(...args) {
            const later = () => {
                clearTimeout(timeout);
                func(...args);
            };
            clearTimeout(timeout);
            timeout = setTimeout(later, wait);
        };
    };

    // ==========================================
    // Form Validation Helper
    // ==========================================
    window.validateForm = function (formId) {
        const form = document.getElementById(formId);
        if (!form) return false;

        const inputs = form.querySelectorAll('input[required], select[required], textarea[required]');
        let isValid = true;

        inputs.forEach(input => {
            if (!input.value.trim()) {
                input.classList.add('is-invalid');
                isValid = false;
            } else {
                input.classList.remove('is-invalid');
            }

            input.addEventListener('input', function () {
                if (this.value.trim()) {
                    this.classList.remove('is-invalid');
                }
            });
        });

        return isValid;
    };

    // ==========================================
    // Loading Spinner
    // ==========================================
    window.showLoading = function () {
        const loader = document.createElement('div');
        loader.id = 'global-loader';
        loader.innerHTML = '<div class="loading-spinner"></div>';
        loader.style.cssText = `
            position: fixed;
            top: 0;
            left: 0;
            width: 100%;
            height: 100%;
            background: rgba(0, 0, 0, 0.5);
            display: flex;
            align-items: center;
            justify-content: center;
            z-index: 9999;
        `;
        document.body.appendChild(loader);
    };

    window.hideLoading = function () {
        const loader = document.getElementById('global-loader');
        if (loader) {
            loader.remove();
        }
    };

    // ==========================================
    // Local Storage Helper
    // ==========================================
    window.storage = {
        set: function (key, value) {
            try {
                localStorage.setItem(key, JSON.stringify(value));
                return true;
            } catch (e) {
                console.error('Error saving to localStorage:', e);
                return false;
            }
        },
        get: function (key) {
            try {
                const item = localStorage.getItem(key);
                return item ? JSON.parse(item) : null;
            } catch (e) {
                console.error('Error reading from localStorage:', e);
                return null;
            }
        },
        remove: function (key) {
            try {
                localStorage.removeItem(key);
                return true;
            } catch (e) {
                console.error('Error removing from localStorage:', e);
                return false;
            }
        },
        clear: function () {
            try {
                localStorage.clear();
                return true;
            } catch (e) {
                console.error('Error clearing localStorage:', e);
                return false;
            }
        }
    };

    // ==========================================
    // Console Welcome Message
    // ==========================================
    console.log('%c🎉 Welcome to Marketplace System!', 'color: #FF6B35; font-size: 20px; font-weight: bold;');
    console.log('%cVersion 1.0.0', 'color: #546E7A; font-size: 12px;');

})();
