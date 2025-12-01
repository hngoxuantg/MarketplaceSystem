(function () {
    'use strict';

    let attributeIndex = 0;

    document.addEventListener('DOMContentLoaded', function () {
        khoiTaoForm();
        khoiTaoSuKienThuocTinh();
    });

    function khoiTaoForm() {
        const btnXemTruocIcon = document.getElementById('btnPreviewIcon');
        if (btnXemTruocIcon) {
            btnXemTruocIcon.addEventListener('click', xemTruocIcon);
        }

        const iconInput = document.querySelector('input[name="Icon"]');
        if (iconInput) {
            iconInput.addEventListener('input', debounce(xemTruocIcon, 500));
        }

        const attributes = document.querySelectorAll('.attribute-item');
        attributeIndex = attributes.length;
    }

    function khoiTaoSuKienThuocTinh() {
        const btnThemThuocTinh = document.getElementById('btnAddAttribute');
        if (btnThemThuocTinh) {
            btnThemThuocTinh.addEventListener('click', themThuocTinh);
        }

        document.addEventListener('change', function (e) {
            if (e.target.classList.contains('attribute-type-select')) {
                xuLyThayDoiLoaiThuocTinh(e.target);
            }
        });

        document.addEventListener('click', function (e) {
            if (e.target.closest('.btn-remove-attribute')) {
                e.preventDefault();
                xoaThuocTinh(e.target.closest('.attribute-item'));
            }
        });

        document.addEventListener('click', function (e) {
            if (e.target.closest('.btn-add-option')) {
                e.preventDefault();
                themLuaChon(e.target.closest('.attribute-item'));
            }
        });

        document.addEventListener('click', function (e) {
            if (e.target.closest('.btn-remove-option')) {
                e.preventDefault();
                xoaLuaChon(e.target.closest('.option-item'));
            }
        });
    }

    function xemTruocIcon() {
        const iconInput = document.querySelector('input[name="Icon"]');
        const preview = document.getElementById('iconPreview');

        if (!iconInput || !preview) return;

        const iconValue = iconInput.value.trim();
        preview.innerHTML = '';

        if (!iconValue) {
            preview.innerHTML = '<small class="text-muted">Nhập icon để xem trước</small>';
            return;
        }

        if (iconValue.startsWith('http')) {
            const img = document.createElement('img');
            img.src = iconValue;
            img.alt = 'Xem trước icon';
            img.className = 'img-thumbnail';
            img.style.maxWidth = '100px';
            img.style.maxHeight = '100px';
            img.onerror = function () {
                preview.innerHTML = '<small class="text-danger">URL ảnh không hợp lệ</small>';
            };
            preview.appendChild(img);
        } else if (iconValue.startsWith('fa-')) {
            const icon = document.createElement('i');
            icon.className = `fas ${iconValue} fa-3x text-primary`;
            preview.appendChild(icon);
        } else {
            preview.innerHTML = '<small class="text-muted">Icon phải là class Font Awesome (fa-...) hoặc URL ảnh</small>';
        }
    }

    function themThuocTinh() {
        const container = document.getElementById('attributesContainer');
        const emptyMessage = document.getElementById('emptyMessage');

        if (emptyMessage) {
            emptyMessage.remove();
        }

        const newAttribute = taoPhanTuThuocTinh(attributeIndex);
        container.insertAdjacentHTML('beforeend', newAttribute);
        attributeIndex++;

        const newElement = container.lastElementChild;
        newElement.scrollIntoView({ behavior: 'smooth', block: 'nearest' });
    }

    function taoPhanTuThuocTinh(index) {
        return `
            <div class="attribute-item card mb-3" data-index="${index}">
                <div class="card-body">
                    <div class="d-flex justify-content-between align-items-start mb-3">
                        <h6 class="mb-0">
                            <span class="badge bg-primary">${index + 1}</span>
                            Thuộc tính #${index + 1}
                        </h6>
                        <button type="button" class="btn btn-sm btn-outline-danger btn-remove-attribute">
                            <i class="fas fa-trash"></i>
                        </button>
                    </div>

                    <input type="hidden" name="Attributes[${index}].Id" value="0" />

                    <div class="row">
                        <div class="col-md-4 mb-3">
                            <label class="form-label">Tên (key) <span class="text-danger">*</span></label>
                            <input type="text" name="Attributes[${index}].Name" 
                                   class="form-control" placeholder="VD: brand" required />
                            <small class="text-muted">Dùng trong code, không dấu, viết thường</small>
                        </div>

                        <div class="col-md-4 mb-3">
                            <label class="form-label">Tên hiển thị <span class="text-danger">*</span></label>
                            <input type="text" name="Attributes[${index}].DisplayName" 
                                   class="form-control" placeholder="VD: Hãng xe" required />
                        </div>

                        <div class="col-md-2 mb-3">
                            <label class="form-label">Thứ tự</label>
                            <input type="number" name="Attributes[${index}].DisplayOrder" value="1" 
                                   class="form-control" min="1" />
                        </div>

                        <div class="col-md-2 mb-3">
                            <label class="form-label">Bắt buộc</label>
                            <div class="form-check form-switch mt-2">
                                <input type="checkbox" name="Attributes[${index}].IsRequired" value="true" 
                                       class="form-check-input" />
                            </div>
                        </div>
                    </div>

                    <div class="row">
                        <div class="col-md-4 mb-3">
                            <label class="form-label">Loại dữ liệu <span class="text-danger">*</span></label>
                            <select name="Attributes[${index}].AttributeType.Id" class="form-select attribute-type-select" required>
                                <option value="">-- Chọn loại --</option>
                                <option value="1">Text - Văn bản</option>
                                <option value="2">Number - Số</option>
                                <option value="3">Select - Lựa chọn</option>
                                <option value="4">MultiSelect - Nhiều lựa chọn</option>
                                <option value="5">Boolean - Có/Không</option>
                                <option value="6">Date - Ngày tháng</option>
                            </select>
                        </div>

                        <div class="col-md-8 mb-3">
                            <label class="form-label">Placeholder</label>
                            <input type="text" name="Attributes[${index}].Placeholder" 
                                   class="form-control" placeholder="VD: Nhập hãng xe..." />
                        </div>
                    </div>

                    <div class="options-container" style="display: none">
                        <div class="border-top pt-3 mt-2">
                            <div class="d-flex justify-content-between align-items-center mb-2">
                                <label class="form-label mb-0">
                                    <i class="fas fa-list-ul"></i> Tùy chọn
                                </label>
                                <button type="button" class="btn btn-sm btn-outline-success btn-add-option">
                                    <i class="fas fa-plus"></i> Thêm tùy chọn
                                </button>
                            </div>
                            <div class="options-list"></div>
                        </div>
                    </div>
                </div>
            </div>
        `;
    }

    function xoaThuocTinh(attributeItem) {
        if (!confirm('Bạn có chắc muốn xóa thuộc tính này?')) {
            return;
        }

        attributeItem.style.opacity = '0';
        setTimeout(() => {
            attributeItem.remove();

            const container = document.getElementById('attributesContainer');
            if (container.children.length === 0) {
                container.innerHTML = `
                    <div class="text-center text-muted py-4" id="emptyMessage">
                        <i class="fas fa-inbox fa-3x mb-3"></i>
                        <p>Chưa có thuộc tính nào. Nhấn "Thêm thuộc tính" để bắt đầu.</p>
                    </div>
                `;
            }
        }, 300);
    }

    function xuLyThayDoiLoaiThuocTinh(select) {
        const attributeItem = select.closest('.attribute-item');
        const optionsContainer = attributeItem.querySelector('.options-container');
        const value = select.value;

        if (value === '3' || value === '4') {
            optionsContainer.style.display = 'block';
        } else {
            optionsContainer.style.display = 'none';
        }
    }

    function themLuaChon(attributeItem) {
        const optionsList = attributeItem.querySelector('.options-list');
        const index = attributeItem.dataset.index;
        const optionIndex = optionsList.children.length;
        const newOption = taoPhanTuLuaChon(index, optionIndex);
        optionsList.insertAdjacentHTML('beforeend', newOption);
    }

    function taoPhanTuLuaChon(attributeIndex, optionIndex) {
        return `
            <div class="option-item input-group mb-2">
                <input type="hidden" name="Attributes[${attributeIndex}].Options[${optionIndex}].Id" value="0" />
                <input type="text" name="Attributes[${attributeIndex}].Options[${optionIndex}].Value" 
                       class="form-control" placeholder="Giá trị (key)" required />
                <input type="text" name="Attributes[${attributeIndex}].Options[${optionIndex}].DisplayText" 
                       class="form-control" placeholder="Tên hiển thị" required />
                <div class="input-group-text">
                    <input type="checkbox" name="Attributes[${attributeIndex}].Options[${optionIndex}].IsActive" 
                           value="true" checked class="form-check-input" />
                    <label class="ms-2 mb-0">Kích hoạt</label>
                </div>
                <button type="button" class="btn btn-outline-danger btn-remove-option">
                    <i class="fas fa-times"></i>
                </button>
            </div>
        `;
    }

    function xoaLuaChon(optionItem) {
        optionItem.style.opacity = '0';
        setTimeout(() => optionItem.remove(), 200);
    }

    function debounce(func, wait) {
        let timeout;
        return function executedFunction(...args) {
            const later = () => {
                clearTimeout(timeout);
                func(...args);
            };
            clearTimeout(timeout);
            timeout = setTimeout(later, wait);
        };
    }

    const form = document.getElementById('editCategoryForm');
    if (form) {
        form.addEventListener('submit', function () {
            danhLaiChiSoThuocTinh();
        });
    }

    function danhLaiChiSoThuocTinh() {
        const attributes = document.querySelectorAll('.attribute-item');
        attributes.forEach((attr, index) => {
            attr.dataset.index = index;
            const inputs = attr.querySelectorAll('input, select, textarea');
            inputs.forEach(input => {
                const name = input.getAttribute('name');
                if (name && name.startsWith('Attributes[')) {
                    const newName = name.replace(/Attributes\[\d+\]/, `Attributes[${index}]`);
                    input.setAttribute('name', newName);
                }
            });

            const badge = attr.querySelector('.badge');
            if (badge) {
                badge.textContent = index + 1;
            }
        });
    }

})();
