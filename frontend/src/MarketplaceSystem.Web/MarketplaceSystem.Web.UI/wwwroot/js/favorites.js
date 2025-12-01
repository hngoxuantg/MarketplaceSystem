/**
 * Favorites Management
 * Quản lý danh sách sản phẩm yêu thích trong localStorage
 */

(function() {
    'use strict';

    // Storage key
    const FAVORITES_KEY = 'favoriteProducts';

    // Favorites Manager
    window.FavoritesManager = {
        /**
         * Get all favorites
         * @returns {Array<number>} Array of product IDs
         */
        getAll: function() {
            return window.storage.get(FAVORITES_KEY) || [];
        },

        /**
         * Add product to favorites
         * @param {number} productId - Product ID to add
         * @returns {boolean} Success status
         */
        add: function(productId) {
            const favorites = this.getAll();
            if (!favorites.includes(productId)) {
                favorites.push(productId);
                return window.storage.set(FAVORITES_KEY, favorites);
            }
            return false;
        },

        /**
         * Remove product from favorites
         * @param {number} productId - Product ID to remove
         * @returns {boolean} Success status
         */
        remove: function(productId) {
            const favorites = this.getAll();
            const index = favorites.indexOf(productId);
            if (index > -1) {
                favorites.splice(index, 1);
                return window.storage.set(FAVORITES_KEY, favorites);
            }
            return false;
        },

        /**
         * Check if product is in favorites
         * @param {number} productId - Product ID to check
         * @returns {boolean} Is favorite
         */
        isFavorite: function(productId) {
            return this.getAll().includes(productId);
        },

        /**
         * Toggle favorite status
         * @param {number} productId - Product ID to toggle
         * @returns {boolean} New favorite status (true = added, false = removed)
         */
        toggle: function(productId) {
            if (this.isFavorite(productId)) {
                this.remove(productId);
                return false;
            } else {
                this.add(productId);
                return true;
            }
        },

        /**
         * Clear all favorites
         * @returns {boolean} Success status
         */
        clear: function() {
            return window.storage.remove(FAVORITES_KEY);
        },

        /**
         * Get favorites count
         * @returns {number} Number of favorites
         */
        count: function() {
            return this.getAll().length;
        },

        /**
         * Sync UI elements with favorites state
         * @param {string} selector - CSS selector for favorite buttons
         */
        syncUI: function(selector = '.btn-favorite') {
            const favorites = this.getAll();
            const buttons = document.querySelectorAll(selector);
            
            buttons.forEach(btn => {
                // Try to get product ID from onclick attribute
                const onclickAttr = btn.getAttribute('onclick');
                const match = onclickAttr?.match(/\d+/);
                
                if (match) {
                    const productId = parseInt(match[0]);
                    if (favorites.includes(productId)) {
                        btn.classList.add('active');
                        const icon = btn.querySelector('i');
                        if (icon) {
                            icon.classList.remove('far', 'bi-heart');
                            icon.classList.add('fas', 'bi-heart-fill');
                        }
                    }
                }
            });
        }
    };

    // Auto-sync on page load
    window.addEventListener('DOMContentLoaded', function() {
        window.FavoritesManager.syncUI();
    });

})();
