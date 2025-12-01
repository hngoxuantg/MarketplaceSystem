/**
 * Category Edit - Simple & Clean Version
 * Handles category, attribute, and option updates with individual save buttons
 */

// Global variables
const API_BASE = '/Category';

// ==================== INITIALIZATION ====================
document.addEventListener('DOMContentLoaded', function () {
    console.log('Category Edit initialized. Category ID:', categoryId);
    
    initCategoryUpdate();
    initAttributeHandlers();
    initOptionHandlers();
    initAddAttributeModal();
});

// ==================== 1. UPDATE CATEGORY ====================
function initCategoryUpdate() {
    const btn = document.getElementById('btnUpdateCategory');
    if (!btn) return;
    
    btn.addEventListener('click', async function () {
        const data = {
            name: getValue('categoryName'),
            description: getValue('categoryDescription'),
            parentCategoryId: getIntValue('categoryParentId'),
            displayOrder: getIntValue('categoryDisplayOrder') || 1,
            isActive: getCheckboxValue('categoryIsActive')
        };

        if (!data.name) {
            return showError('Vui lòng nhập tên danh mục!');
        }

        await callApi('PUT', `/api/admin/v1/categories/${categoryId}`, data, 
            'Cập nhật danh mục thành công!', true);
    });
}

// ==================== 2. UPDATE ATTRIBUTE ====================
function initAttributeHandlers() {
    // Update buttons
    document.querySelectorAll('.btn-update-attribute').forEach(btn => {
        btn.addEventListener('click', async function () {
            const attrId = this.dataset.attributeId;
            await updateAttribute(attrId);
        });
    });

    // Delete buttons
    document.querySelectorAll('.btn-delete-attribute').forEach(btn => {
        btn.addEventListener('click', async function () {
            if (!confirm('Bạn có chắc chắn muốn xóa thuộc tính này?')) return;
            
            const attrId = this.dataset.attributeId;
            await callApi('DELETE', `/api/admin/v1/categories/${categoryId}/attributes/${attrId}`, 
                null, 'Đã xóa thuộc tính!', true);
        });
    });
}

async function updateAttribute(attrId) {
    const card = document.querySelector(`.attribute-item[data-attribute-id="${attrId}"]`);
    if (!card) return showError('Không tìm thấy thuộc tính!');

    const data = {
        name: getValueFromCard(card, '.attr-name'),
        displayName: getValueFromCard(card, '.attr-display-name'),
        isRequired: getCheckboxFromCard(card, '.attr-is-required'),
        displayOrder: getIntValueFromCard(card, '.attr-display-order') || 1,
        placeholder: getValueFromCard(card, '.attr-placeholder')
    };

    if (!data.name || !data.displayName) {
        return showError('Vui lòng nhập đầy đủ tên thuộc tính!');
    }

    await callApi('PATCH', `/api/admin/v1/categories/${categoryId}/attributes/${attrId}`, 
        data, 'Cập nhật thuộc tính thành công!');
}

// ==================== 3. UPDATE OPTION ====================
function initOptionHandlers() {
    // Update buttons
    document.querySelectorAll('.btn-update-option').forEach(btn => {
        btn.addEventListener('click', async function () {
            const attrId = this.dataset.attributeId;
            const optId = this.dataset.optionId;
            await updateOption(attrId, optId);
        });
    });

    // Delete buttons
    document.querySelectorAll('.btn-delete-option').forEach(btn => {
        btn.addEventListener('click', async function () {
            if (!confirm('Bạn có chắc chắn muốn xóa option này?')) return;
            
            const attrId = this.dataset.attributeId;
            const optId = this.dataset.optionId;
            await callApi('DELETE', 
                `/api/admin/v1/categories/${categoryId}/attributes/${attrId}/options/${optId}`, 
                null, 'Đã xóa option!', true);
        });
    });
}

async function updateOption(attrId, optId) {
    const card = document.querySelector(`.option-item[data-option-id="${optId}"]`);
    if (!card) return showError('Không tìm thấy option!');

    const data = {
        id: parseInt(optId),
        value: getValueFromCard(card, '.opt-value'),
        displayText: getValueFromCard(card, '.opt-display-text'),
        isActive: getCheckboxFromCard(card, '.opt-is-active'),
        categoryAttributeId: parseInt(attrId)
    };

    if (!data.value || !data.displayText) {
        return showError('Vui lòng nhập đầy đủ giá trị và tên hiển thị!');
    }

    await callApi('PATCH', 
        `/api/admin/v1/categories/${categoryId}/attributes/${attrId}/options/${optId}`, 
        data, 'Cập nhật option thành công!');
}

// ==================== 4. ADD NEW ATTRIBUTE ====================
function initAddAttributeModal() {
    const btnAdd = document.getElementById('btnAddAttribute');
    if (!btnAdd) return;

    const modal = new bootstrap.Modal(document.getElementById('addAttributeModal'));
    
    btnAdd.addEventListener('click', () => {
        resetAddAttributeForm();
        modal.show();
    });

    // Show/hide options based on type
    document.getElementById('newAttrType').addEventListener('change', function () {
        const container = document.getElementById('newAttrOptionsContainer');
        container.style.display = (this.value === 'Select' || this.value === 'MultiSelect') ? 'block' : 'none';
    });

    // Add option row
    document.getElementById('btnAddNewOption').addEventListener('click', addOptionRow);

    // Save attribute
    document.getElementById('btnSaveNewAttribute').addEventListener('click', async function () {
        await saveNewAttribute();
    });
}

function addOptionRow() {
    const html = `
        <div class="input-group mb-2 new-option-row">
            <input type="text" class="form-control new-opt-value" placeholder="Giá trị (value)" />
            <input type="text" class="form-control new-opt-display" placeholder="Tên hiển thị" />
            <button type="button" class="btn btn-outline-danger btn-remove-option">
                <i class="fas fa-times"></i>
            </button>
        </div>
    `;
    
    const container = document.getElementById('newOptionsList');
    container.insertAdjacentHTML('beforeend', html);
    
    // Setup remove button
    const rows = container.querySelectorAll('.new-option-row');
    const lastRow = rows[rows.length - 1];
    lastRow.querySelector('.btn-remove-option').addEventListener('click', function () {
        this.closest('.new-option-row').remove();
    });
}

async function saveNewAttribute() {
    const data = {
        name: getValue('newAttrName'),
        displayName: getValue('newAttrDisplayName'),
        attributeType: getValue('newAttrType'),
        isRequired: getCheckboxValue('newAttrIsRequired'),
        displayOrder: getIntValue('newAttrDisplayOrder') || 1,
        placeholder: getValue('newAttrPlaceholder'),
        attributeOptions: null
    };

    // Validation
    if (!data.name || !data.displayName || !data.attributeType) {
        return showError('Vui lòng nhập đầy đủ thông tin bắt buộc!');
    }

    // Collect options if needed
    if (data.attributeType === 'Select' || data.attributeType === 'MultiSelect') {
        data.attributeOptions = [];
        document.querySelectorAll('.new-option-row').forEach(row => {
            const value = row.querySelector('.new-opt-value').value.trim();
            const displayText = row.querySelector('.new-opt-display').value.trim();
            if (value && displayText) {
                data.attributeOptions.push({ value, displayText });
            }
        });
    }

    await callApi('POST', `/api/admin/v1/categories/${categoryId}/attributes`, 
        data, 'Tạo thuộc tính thành công!', true);
}

function resetAddAttributeForm() {
    document.getElementById('newAttrName').value = '';
    document.getElementById('newAttrDisplayName').value = '';
    document.getElementById('newAttrType').value = '';
    document.getElementById('newAttrDisplayOrder').value = '1';
    document.getElementById('newAttrIsRequired').checked = false;
    document.getElementById('newAttrPlaceholder').value = '';
    document.getElementById('newOptionsList').innerHTML = '';
    document.getElementById('newAttrOptionsContainer').style.display = 'none';
}

// ==================== HELPER FUNCTIONS ====================

/**
 * Main API caller
 */
async function callApi(method, endpoint, data, successMsg, reload = false) {
    try {
        const options = {
            method: method,
            headers: {
                'Content-Type': 'application/json',
                'Authorization': `Bearer ${getCookie('authToken') || ''}`
            }
        };

        if (data && method !== 'GET' && method !== 'DELETE') {
            options.body = JSON.stringify(data);
        }

        console.log(`${method} ${endpoint}`, data);

        const response = await fetch(endpoint, options);
        
        if (!response.ok) {
            const error = await response.text();
            console.error('API Error:', error);
            throw new Error(`HTTP ${response.status}: ${error}`);
        }

        const result = await response.json();
        console.log('API Result:', result);

        showSuccess(successMsg);
        
        if (reload) {
            setTimeout(() => location.reload(), 1000);
        }

        return result;
    } catch (error) {
        console.error('API Call Failed:', error);
        showError('Có lỗi xảy ra: ' + error.message);
        return null;
    }
}

/**
 * Get value from input
 */
function getValue(id) {
    const el = document.getElementById(id);
    return el ? el.value.trim() : '';
}

function getIntValue(id) {
    const val = getValue(id);
    return val ? parseInt(val) : null;
}

function getCheckboxValue(id) {
    const el = document.getElementById(id);
    return el ? el.checked : false;
}

/**
 * Get value from card/container
 */
function getValueFromCard(card, selector) {
    const el = card.querySelector(selector);
    return el ? el.value.trim() : '';
}

function getIntValueFromCard(card, selector) {
    const val = getValueFromCard(card, selector);
    return val ? parseInt(val) : null;
}

function getCheckboxFromCard(card, selector) {
    const el = card.querySelector(selector);
    return el ? el.checked : false;
}

/**
 * Get cookie value
 */
function getCookie(name) {
    const value = `; ${document.cookie}`;
    const parts = value.split(`; ${name}=`);
    if (parts.length === 2) return parts.pop().split(';').shift();
    return null;
}

/**
 * Show alerts
 */
function showSuccess(message) {
    showAlert('success', message, 'check-circle');
}

function showError(message) {
    showAlert('danger', message, 'exclamation-circle');
}

function showAlert(type, message, icon) {
    const html = `
        <div class="alert alert-${type} alert-dismissible fade show" role="alert">
            <i class="fas fa-${icon}"></i> ${message}
            <button type="button" class="btn-close" data-bs-dismiss="alert"></button>
        </div>
    `;
    
    const container = document.querySelector('.container-fluid') || document.body;
    const existing = container.querySelector('.alert');
    if (existing) existing.remove();
    
    container.insertAdjacentHTML('afterbegin', html);
    
    // Auto dismiss
    setTimeout(() => {
        const alert = container.querySelector('.alert');
        if (alert) new bootstrap.Alert(alert).close();
    }, 5000);
}
