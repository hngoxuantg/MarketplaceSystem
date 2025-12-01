// Category Edit with Individual Update Buttons
document.addEventListener('DOMContentLoaded', function () {
    setupCategoryUpdate();
    setupAttributeHandlers();
    setupOptionHandlers();
    setupAddAttributeModal();
});

// ===== UPDATE CATEGORY =====
function setupCategoryUpdate() {
    const btnUpdate = document.getElementById('btnUpdateCategory');
    if (btnUpdate) {
        btnUpdate.addEventListener('click', updateCategory);
    }
}

async function updateCategory() {
    const name = document.getElementById('categoryName').value.trim();
    const description = document.getElementById('categoryDescription').value.trim();
    const displayOrder = parseInt(document.getElementById('categoryDisplayOrder').value);
    const parentId = document.getElementById('categoryParentId').value;
    const isActive = document.getElementById('categoryIsActive').checked;

    if (!name) {
        alert('Vui lòng nhập tên danh mục!');
        return;
    }

    const data = {
        name: name,
        description: description || null,
        parentCategoryId: parentId ? parseInt(parentId) : null,
        displayOrder: displayOrder,
        isActive: isActive
    };

    console.log('Updating category:', categoryId, data);

    try {
        const response = await fetch(`/Category/UpdateCategoryAjax?id=${categoryId}`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'RequestVerificationToken': antiForgeryToken
            },
            body: JSON.stringify(data)
        });

        console.log('Response status:', response.status);
        console.log('Response headers:', response.headers.get('content-type'));

        if (!response.ok) {
            throw new Error(`HTTP error! status: ${response.status}`);
        }

        const contentType = response.headers.get('content-type');
        if (!contentType || !contentType.includes('application/json')) {
            const text = await response.text();
            console.error('Non-JSON response:', text.substring(0, 200));
            throw new Error('Server returned non-JSON response');
        }

        const result = await response.json();
        console.log('Result:', result);
        
        if (result.success) {
            showAlert('success', result.message || 'Cập nhật danh mục thành công!');
            setTimeout(() => location.reload(), 1500);
        } else {
            showAlert('danger', result.message || 'Cập nhật thất bại!');
        }
    } catch (error) {
        console.error('Error:', error);
        showAlert('danger', 'Có lỗi xảy ra khi cập nhật: ' + error.message);
    }
}

// ===== UPDATE ATTRIBUTE =====
function setupAttributeHandlers() {
    // Update attribute buttons
    document.querySelectorAll('.btn-update-attribute').forEach(btn => {
        btn.addEventListener('click', function () {
            const categoryId = this.getAttribute('data-category-id');
            const attributeId = this.getAttribute('data-attribute-id');
            updateAttribute(categoryId, attributeId);
        });
    });

    // Delete attribute buttons
    document.querySelectorAll('.btn-delete-attribute').forEach(btn => {
        btn.addEventListener('click', function () {
            const categoryId = this.getAttribute('data-category-id');
            const attributeId = this.getAttribute('data-attribute-id');
            deleteAttribute(categoryId, attributeId);
        });
    });
}

async function updateAttribute(categoryId, attributeId) {
    const attrCard = document.querySelector(`.attribute-item[data-attribute-id="${attributeId}"]`);
    
    if (!attrCard) {
        console.error('Attribute card not found:', attributeId);
        alert('Không tìm thấy thuộc tính!');
        return;
    }

    const nameInput = attrCard.querySelector('.attr-name');
    const displayNameInput = attrCard.querySelector('.attr-display-name');
    const isRequiredInput = attrCard.querySelector('.attr-is-required');
    const displayOrderInput = attrCard.querySelector('.attr-display-order');
    const placeholderInput = attrCard.querySelector('.attr-placeholder');

    // Debug: Log inputs
    console.log('Name input:', nameInput, nameInput?.value);
    console.log('DisplayName input:', displayNameInput, displayNameInput?.value);
    
    const data = {
        name: nameInput?.value?.trim() || '',
        displayName: displayNameInput?.value?.trim() || '',
        isRequired: isRequiredInput?.checked || false,
        displayOrder: parseInt(displayOrderInput?.value) || 1,
        placeholder: placeholderInput?.value?.trim() || null
    };

    console.log('Sending data:', data);

    if (!data.name || !data.displayName) {
        alert('Vui lòng nhập đầy đủ tên thuộc tính và tên hiển thị!');
        return;
    }

    try {
        const response = await fetch(`/Category/UpdateAttribute?categoryId=${categoryId}&attributeId=${attributeId}`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'RequestVerificationToken': antiForgeryToken
            },
            body: JSON.stringify(data)
        });

        const result = await response.json();
        console.log('Response:', result);
        
        if (result.success) {
            showAlert('success', result.message || 'Cập nhật thuộc tính thành công!');
        } else {
            showAlert('danger', result.message || 'Cập nhật thất bại!');
        }
    } catch (error) {
        console.error('Error:', error);
        showAlert('danger', 'Có lỗi xảy ra khi cập nhật thuộc tính!');
    }
}

async function deleteAttribute(categoryId, attributeId) {
    if (!confirm('Bạn có chắc chắn muốn xóa thuộc tính này?')) {
        return;
    }

    try {
        const response = await fetch(`/Category/DeleteAttribute?categoryId=${categoryId}&attributeId=${attributeId}`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'RequestVerificationToken': antiForgeryToken
            }
        });

        const result = await response.json();
        if (result.success) {
            showAlert('success', result.message || 'Đã xóa thuộc tính!');
            setTimeout(() => location.reload(), 1000);
        } else {
            showAlert('danger', result.message || 'Xóa thất bại!');
        }
    } catch (error) {
        console.error('Error:', error);
        showAlert('danger', 'Có lỗi xảy ra khi xóa thuộc tính!');
    }
}

// ===== UPDATE OPTION =====
function setupOptionHandlers() {
    // Update option buttons
    document.querySelectorAll('.btn-update-option').forEach(btn => {
        btn.addEventListener('click', function () {
            const categoryId = this.getAttribute('data-category-id');
            const attributeId = this.getAttribute('data-attribute-id');
            const optionId = this.getAttribute('data-option-id');
            updateOption(categoryId, attributeId, optionId);
        });
    });

    // Delete option buttons
    document.querySelectorAll('.btn-delete-option').forEach(btn => {
        btn.addEventListener('click', function () {
            const categoryId = this.getAttribute('data-category-id');
            const attributeId = this.getAttribute('data-attribute-id');
            const optionId = this.getAttribute('data-option-id');
            deleteOption(categoryId, attributeId, optionId);
        });
    });
}

async function updateOption(categoryId, attributeId, optionId) {
    const optionCard = document.querySelector(`.option-item[data-option-id="${optionId}"]`);
    
    const data = {
        id: parseInt(optionId),
        value: optionCard.querySelector('.opt-value').value.trim(),
        displayText: optionCard.querySelector('.opt-display-text').value.trim(),
        isActive: optionCard.querySelector('.opt-is-active').checked,
        categoryAttributeId: parseInt(attributeId)
    };

    if (!data.value || !data.displayText) {
        alert('Vui lòng nhập đầy đủ giá trị và tên hiển thị!');
        return;
    }

    try {
        const response = await fetch(`/Category/UpdateAttributeOption?categoryId=${categoryId}&attributeId=${attributeId}&optionId=${optionId}`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'RequestVerificationToken': antiForgeryToken
            },
            body: JSON.stringify(data)
        });

        const result = await response.json();
        if (result.success) {
            showAlert('success', result.message || 'Cập nhật option thành công!');
        } else {
            showAlert('danger', result.message || 'Cập nhật thất bại!');
        }
    } catch (error) {
        console.error('Error:', error);
        showAlert('danger', 'Có lỗi xảy ra khi cập nhật option!');
    }
}

async function deleteOption(categoryId, attributeId, optionId) {
    if (!confirm('Bạn có chắc chắn muốn xóa option này?')) {
        return;
    }

    try {
        const response = await fetch(`/Category/DeleteAttributeOption?categoryId=${categoryId}&attributeId=${attributeId}&optionId=${optionId}`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'RequestVerificationToken': antiForgeryToken
            }
        });

        const result = await response.json();
        if (result.success) {
            showAlert('success', result.message || 'Đã xóa option!');
            setTimeout(() => location.reload(), 1000);
        } else {
            showAlert('danger', result.message || 'Xóa thất bại!');
        }
    } catch (error) {
        console.error('Error:', error);
        showAlert('danger', 'Có lỗi xảy ra khi xóa option!');
    }
}

// ===== ADD NEW ATTRIBUTE MODAL =====
function setupAddAttributeModal() {
    const btnAdd = document.getElementById('btnAddAttribute');
    const modal = new bootstrap.Modal(document.getElementById('addAttributeModal'));
    
    if (btnAdd) {
        btnAdd.addEventListener('click', () => modal.show());
    }

    // Type change handler
    document.getElementById('newAttrType').addEventListener('change', function () {
        const optionsContainer = document.getElementById('newAttrOptionsContainer');
        if (this.value === 'Select' || this.value === 'MultiSelect') {
            optionsContainer.style.display = 'block';
        } else {
            optionsContainer.style.display = 'none';
        }
    });

    // Add option button
    document.getElementById('btnAddNewOption').addEventListener('click', addNewOptionRow);

    // Save button
    document.getElementById('btnSaveNewAttribute').addEventListener('click', saveNewAttribute);
}

let newOptionIndex = 0;

function addNewOptionRow() {
    const optionsList = document.getElementById('newOptionsList');
    const html = `
        <div class="input-group mb-2 new-option-row">
            <input type="text" class="form-control new-opt-value" placeholder="Giá trị (value)" required />
            <input type="text" class="form-control new-opt-display" placeholder="Tên hiển thị" required />
            <button type="button" class="btn btn-outline-danger btn-remove-new-option">
                <i class="fas fa-times"></i>
            </button>
        </div>
    `;
    optionsList.insertAdjacentHTML('beforeend', html);

    // Setup remove button
    const rows = optionsList.querySelectorAll('.new-option-row');
    const lastRow = rows[rows.length - 1];
    lastRow.querySelector('.btn-remove-new-option').addEventListener('click', function () {
        this.closest('.new-option-row').remove();
    });

    newOptionIndex++;
}

async function saveNewAttribute() {
    const name = document.getElementById('newAttrName').value.trim();
    const displayName = document.getElementById('newAttrDisplayName').value.trim();
    const attributeType = document.getElementById('newAttrType').value;
    const displayOrder = parseInt(document.getElementById('newAttrDisplayOrder').value);
    const isRequired = document.getElementById('newAttrIsRequired').checked;
    const placeholder = document.getElementById('newAttrPlaceholder').value.trim();

    // Validation
    if (!name || !displayName || !attributeType) {
        alert('Vui lòng nhập đầy đủ thông tin bắt buộc!');
        return;
    }

    // Collect options if Select/MultiSelect
    let options = null;
    if (attributeType === 'Select' || attributeType === 'MultiSelect') {
        options = [];
        document.querySelectorAll('.new-option-row').forEach(row => {
            const value = row.querySelector('.new-opt-value').value.trim();
            const displayText = row.querySelector('.new-opt-display').value.trim();
            if (value && displayText) {
                options.push({ value, displayText });
            }
        });
    }

    const data = {
        name,
        displayName,
        attributeType,
        isRequired,
        displayOrder,
        placeholder: placeholder || null,
        attributeOptions: options
    };

    try {
        const response = await fetch(`/Category/CreateAttribute?categoryId=${categoryId}`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'RequestVerificationToken': antiForgeryToken
            },
            body: JSON.stringify(data)
        });

        const result = await response.json();
        if (result.success) {
            showAlert('success', result.message || 'Tạo thuộc tính thành công!');
            setTimeout(() => location.reload(), 1500);
        } else {
            showAlert('danger', result.message || 'Tạo thuộc tính thất bại!');
        }
    } catch (error) {
        console.error('Error:', error);
        showAlert('danger', 'Có lỗi xảy ra khi tạo thuộc tính!');
    }
}

// ===== UTILITIES =====
function showAlert(type, message) {
    const alertHtml = `
        <div class="alert alert-${type} alert-dismissible fade show" role="alert">
            <i class="fas fa-${type === 'success' ? 'check-circle' : 'exclamation-circle'}"></i> ${message}
            <button type="button" class="btn-close" data-bs-dismiss="alert"></button>
        </div>
    `;
    
    const container = document.querySelector('.container-fluid') || document.body;
    container.insertAdjacentHTML('afterbegin', alertHtml);
    
    // Auto dismiss after 5 seconds
    setTimeout(() => {
        const alert = container.querySelector('.alert');
        if (alert) {
            const bsAlert = new bootstrap.Alert(alert);
            bsAlert.close();
        }
    }, 5000);
}
