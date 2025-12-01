// Category Index JavaScript
(function () {
    'use strict';

    // Current page state
    let currentPage = 1;
    let currentPageSize = 10;
    let currentSearchTerm = '';
    let currentIsActive = null;

    // Initialize on document ready
    document.addEventListener('DOMContentLoaded', function () {
        initializeEventHandlers();
    });

    // Initialize all event handlers
    function initializeEventHandlers() {
        // Filter form submit
        const filterForm = document.getElementById('filterForm');
        if (filterForm) {
            filterForm.addEventListener('submit', function (e) {
                e.preventDefault();
                currentPage = 1; // Reset to first page
                loadCategories();
            });
        }

        // Reset filter button
        const btnResetFilter = document.getElementById('btnResetFilter');
        if (btnResetFilter) {
            btnResetFilter.addEventListener('click', function () {
                resetFilters();
            });
        }

        // Page size change
        const pageSizeSelect = document.getElementById('pageSize');
        if (pageSizeSelect) {
            pageSizeSelect.addEventListener('change', function () {
                currentPage = 1; // Reset to first page
                loadCategories();
            });
        }

        // Save category button
        const btnSaveCategory = document.getElementById('btnSaveCategory');
        if (btnSaveCategory) {
            btnSaveCategory.addEventListener('click', function () {
                saveCategory();
            });
        }

        // Modal reset when closed
        const addCategoryModal = document.getElementById('addCategoryModal');
        if (addCategoryModal) {
            addCategoryModal.addEventListener('hidden.bs.modal', function () {
                resetCategoryForm();
            });
        }
    }

    // Load categories with current filters
    function loadCategories() {
        const searchTerm = document.getElementById('searchTerm')?.value || '';
        const isActive = document.getElementById('isActive')?.value || '';
        const pageSize = document.getElementById('pageSize')?.value || 10;

        currentSearchTerm = searchTerm;
        currentIsActive = isActive === '' ? null : isActive === 'true';
        currentPageSize = parseInt(pageSize);

        showLoading();

        const params = new URLSearchParams({
            pageNumber: currentPage,
            pageSize: currentPageSize
        });

        if (currentSearchTerm) {
            params.append('searchTerm', currentSearchTerm);
        }

        if (currentIsActive !== null) {
            params.append('isActive', currentIsActive);
        }

        fetch(`/Category/GetCategoriesPartial?${params.toString()}`)
            .then(response => {
                if (!response.ok) {
                    throw new Error('Network response was not ok');
                }
                return response.text();
            })
            .then(html => {
                const container = document.getElementById('categoriesTableContainer');
                if (container) {
                    container.innerHTML = html;
                    container.classList.add('fade-in');
                }
                hideLoading();
            })
            .catch(error => {
                console.error('Error loading categories:', error);
                showToast('Lỗi tải dữ liệu', 'Không thể tải danh sách danh mục. Vui lòng thử lại.', 'error');
                hideLoading();
            });
    }

    // Load specific page
    window.loadPage = function (pageNumber) {
        currentPage = pageNumber;
        loadCategories();
    };

    // Reset filters
    function resetFilters() {
        document.getElementById('searchTerm').value = '';
        document.getElementById('isActive').value = '';
        document.getElementById('pageSize').value = '10';
        currentPage = 1;
        loadCategories();
    }

    // View category details
    window.viewDetails = function (id) {
        showLoading();

        fetch(`/Category/GetDetails?id=${id}`)
            .then(response => response.json())
            .then(result => {
                hideLoading();
                if (result.success) {
                    showCategoryDetailsModal(result.data);
                } else {
                    showToast('Lỗi', result.message, 'error');
                }
            })
            .catch(error => {
                console.error('Error:', error);
                hideLoading();
                showToast('Lỗi', 'Không thể tải thông tin danh mục.', 'error');
            });
    };

    // Edit category
    window.editCategory = function (id) {
        showLoading();

        fetch(`/Category/GetDetails?id=${id}`)
            .then(response => response.json())
            .then(result => {
                hideLoading();
                if (result.success) {
                    populateCategoryForm(result.data);
                    const modal = new bootstrap.Modal(document.getElementById('addCategoryModal'));
                    document.getElementById('addCategoryModalLabel').innerHTML = '<i class="fas fa-edit"></i> Chỉnh sửa danh mục';
                    modal.show();
                } else {
                    showToast('Lỗi', result.message, 'error');
                }
            })
            .catch(error => {
                console.error('Error:', error);
                hideLoading();
                showToast('Lỗi', 'Không thể tải thông tin danh mục.', 'error');
            });
    };

    // Toggle category status
    window.toggleStatus = function (id, currentStatus) {
        const action = currentStatus ? 'tạm dừng' : 'kích hoạt';
        
        if (!confirm(`Bạn có chắc muốn ${action} danh mục này?`)) {
            return;
        }

        showLoading();

        fetch(`/Category/ToggleActive`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
            },
            body: JSON.stringify(id)
        })
            .then(response => response.json())
            .then(result => {
                hideLoading();
                if (result.success) {
                    showToast('Thành công', result.message, 'success');
                    loadCategories();
                } else {
                    showToast('Lỗi', result.message, 'error');
                }
            })
            .catch(error => {
                console.error('Error:', error);
                hideLoading();
                showToast('Lỗi', 'Không thể cập nhật trạng thái.', 'error');
            });
    };

    // Delete category
    window.deleteCategory = function (id, name) {
        if (!confirm(`Bạn có chắc muốn xóa danh mục "${name}"?\n\nLưu ý: Tất cả sản phẩm trong danh mục này sẽ bị ảnh hưởng!`)) {
            return;
        }

        showLoading();

        fetch(`/Category/DeleteAjax`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
            },
            body: JSON.stringify(id)
        })
            .then(response => response.json())
            .then(result => {
                hideLoading();
                if (result.success) {
                    showToast('Thành công', result.message, 'success');
                    loadCategories();
                } else {
                    showToast('Lỗi', result.message, 'error');
                }
            })
            .catch(error => {
                console.error('Error:', error);
                hideLoading();
                showToast('Lỗi', 'Không thể xóa danh mục.', 'error');
            });
    };

    // Save category (add or update)
    function saveCategory() {
        const form = document.getElementById('categoryForm');
        if (!form.checkValidity()) {
            form.reportValidity();
            return;
        }

        const formData = {
            id: document.getElementById('categoryId').value,
            name: document.getElementById('categoryName').value,
            description: document.getElementById('description').value,
            icon: document.getElementById('icon').value,
            displayOrder: parseInt(document.getElementById('displayOrder').value) || 1,
            parentCategoryId: document.getElementById('parentCategoryId').value || null,
            isActive: document.getElementById('isActiveCheckbox').checked
        };

        showLoading();

        const url = formData.id ? '/Category/Edit' : '/Category/Create';

        // TODO: Implement actual API call
        console.log('Saving category:', formData);

        // Simulated save for now
        setTimeout(() => {
            hideLoading();
            const modal = bootstrap.Modal.getInstance(document.getElementById('addCategoryModal'));
            modal.hide();
            showToast('Thành công', formData.id ? 'Đã cập nhật danh mục!' : 'Đã thêm danh mục mới!', 'success');
            loadCategories();
        }, 1000);
    }

    // Populate form for editing
    function populateCategoryForm(category) {
        document.getElementById('categoryId').value = category.id || '';
        document.getElementById('categoryName').value = category.name || '';
        document.getElementById('description').value = category.description || '';
        document.getElementById('icon').value = category.icon || '';
        document.getElementById('displayOrder').value = category.displayOrder || 1;
        document.getElementById('parentCategoryId').value = category.parentCategoryId || '';
        document.getElementById('isActiveCheckbox').checked = category.isActive;
    }

    // Reset category form
    function resetCategoryForm() {
        document.getElementById('categoryForm').reset();
        document.getElementById('categoryId').value = '';
        document.getElementById('addCategoryModalLabel').innerHTML = '<i class="fas fa-plus"></i> Thêm danh mục mới';
    }

    // Show category details modal
    function showCategoryDetailsModal(category) {
        const detailsHtml = `
            <div class="modal fade" id="detailsModal" tabindex="-1">
                <div class="modal-dialog modal-lg">
                    <div class="modal-content">
                        <div class="modal-header bg-info text-white">
                            <h5 class="modal-title"><i class="fas fa-info-circle"></i> Chi tiết danh mục</h5>
                            <button type="button" class="btn-close btn-close-white" data-bs-dismiss="modal"></button>
                        </div>
                        <div class="modal-body">
                            <div class="row">
                                <div class="col-md-6 mb-3">
                                    <strong>ID:</strong> #${category.id}
                                </div>
                                <div class="col-md-6 mb-3">
                                    <strong>Tên:</strong> ${category.name}
                                </div>
                                <div class="col-md-12 mb-3">
                                    <strong>Mô tả:</strong> ${category.description || 'Không có'}
                                </div>
                                <div class="col-md-6 mb-3">
                                    <strong>Thứ tự:</strong> ${category.displayOrder}
                                </div>
                                <div class="col-md-6 mb-3">
                                    <strong>Trạng thái:</strong> 
                                    <span class="badge bg-${category.isActive ? 'success' : 'warning'}">
                                        ${category.isActive ? 'Hoạt động' : 'Tạm dừng'}
                                    </span>
                                </div>
                                <div class="col-md-6 mb-3">
                                    <strong>Danh mục cha:</strong> ${category.parentCategoryName || 'Danh mục gốc'}
                                </div>
                                <div class="col-md-6 mb-3">
                                    <strong>Số danh mục con:</strong> ${category.subCategoriesCount || 0}
                                </div>
                                <div class="col-md-6 mb-3">
                                    <strong>Ngày tạo:</strong> ${new Date(category.createdAt).toLocaleString('vi-VN')}
                                </div>
                                <div class="col-md-6 mb-3">
                                    <strong>Cập nhật:</strong> ${category.updatedAt ? new Date(category.updatedAt).toLocaleString('vi-VN') : 'Chưa cập nhật'}
                                </div>
                            </div>
                        </div>
                        <div class="modal-footer">
                            <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">Đóng</button>
                            <button type="button" class="btn btn-primary" onclick="editCategory(${category.id})" data-bs-dismiss="modal">
                                <i class="fas fa-edit"></i> Chỉnh sửa
                            </button>
                        </div>
                    </div>
                </div>
            </div>
        `;

        // Remove existing modal if any
        const existingModal = document.getElementById('detailsModal');
        if (existingModal) {
            existingModal.remove();
        }

        // Add new modal to body
        document.body.insertAdjacentHTML('beforeend', detailsHtml);

        // Show modal
        const modal = new bootstrap.Modal(document.getElementById('detailsModal'));
        modal.show();

        // Remove modal from DOM when hidden
        document.getElementById('detailsModal').addEventListener('hidden.bs.modal', function () {
            this.remove();
        });
    }

    // Show loading overlay
    function showLoading() {
        const overlay = document.getElementById('loadingOverlay');
        if (overlay) {
            overlay.style.display = 'flex';
        }
    }

    // Hide loading overlay
    function hideLoading() {
        const overlay = document.getElementById('loadingOverlay');
        if (overlay) {
            overlay.style.display = 'none';
        }
    }

    // Show toast notification
    function showToast(title, message, type = 'info') {
        // Create toast container if not exists
        let toastContainer = document.querySelector('.toast-container');
        if (!toastContainer) {
            toastContainer = document.createElement('div');
            toastContainer.className = 'toast-container position-fixed top-0 end-0 p-3';
            document.body.appendChild(toastContainer);
        }

        const toastId = 'toast-' + Date.now();
        const bgClass = type === 'success' ? 'bg-success' : type === 'error' ? 'bg-danger' : 'bg-info';
        const icon = type === 'success' ? 'check-circle' : type === 'error' ? 'exclamation-circle' : 'info-circle';

        const toastHtml = `
            <div id="${toastId}" class="toast align-items-center text-white ${bgClass} border-0" role="alert">
                <div class="d-flex">
                    <div class="toast-body">
                        <i class="fas fa-${icon} me-2"></i>
                        <strong>${title}:</strong> ${message}
                    </div>
                    <button type="button" class="btn-close btn-close-white me-2 m-auto" data-bs-dismiss="toast"></button>
                </div>
            </div>
        `;

        toastContainer.insertAdjacentHTML('beforeend', toastHtml);

        const toastElement = document.getElementById(toastId);
        const toast = new bootstrap.Toast(toastElement, {
            autohide: true,
            delay: 3000
        });

        toast.show();

        // Remove toast element after hidden
        toastElement.addEventListener('hidden.bs.toast', function () {
            this.remove();
        });
    }

})();
