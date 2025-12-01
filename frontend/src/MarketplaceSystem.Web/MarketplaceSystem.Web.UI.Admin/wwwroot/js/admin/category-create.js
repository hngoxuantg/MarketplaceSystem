(function () {
    'use strict';

    let attributeIndex = 0;

    document.addEventListener('DOMContentLoaded', function () {
        khoiTaoForm();
        khoiTaoSuKienThuocTinh();
    });

    function khoiTaoForm() {
        // Icon file preview
        const iconFileInput = document.getElementById('iconFileInput');
        if (iconFileInput) {
            iconFileInput.addEventListener('change', xemTruocIconFile);
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

    function xemTruocIconFile() {
        const iconFileInput = document.getElementById('iconFileInput');
        const preview = document.getElementById('iconPreview');

        if (!iconFileInput || !preview) return;

        const file = iconFileInput.files[0];
        preview.innerHTML = '';

        if (!file) {
            return;
        }

        if (!file.type.startsWith('image/')) {
            preview.innerHTML = '<small class="text-danger">Vui lòng chọn file hình ảnh!</small>';
            return;
        }

        const reader = new FileReader();
        reader.onload = function (e) {
            const img = document.createElement('img');
            img.src = e.target.result;
            img.alt = 'Xem trước icon';
            img.className = 'img-thumbnail';
            img.style.maxWidth = '150px';
            img.style.maxHeight = '150px';
            preview.appendChild(img);

            const fileInfo = document.createElement('div');
            fileInfo.className = 'mt-2 text-muted';
            fileInfo.innerHTML = `<small><i class="fas fa-file-image"></i> ${file.name} (${formatFileSize(file.size)})</small>`;
            preview.appendChild(fileInfo);
        };
        reader.readAsDataURL(file);
    }

    function formatFileSize(bytes) {
        if (bytes === 0) return '0 Bytes';
        const k = 1024;
        const sizes = ['Bytes', 'KB', 'MB', 'GB'];
        const i = Math.floor(Math.log(bytes) / Math.log(k));
        return Math.round(bytes / Math.pow(k, i) * 100) / 100 + ' ' + sizes[i];
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
                                   class="form-control" min="1" required />
                        </div>

                        <div class="col-md-2 mb-3">
                            <label class="form-label">Bắt buộc</label>
                            <div class="form-check form-switch mt-2">
                                <input type="hidden" name="Attributes[${index}].IsRequired" value="false" />
                                <input type="checkbox" name="Attributes[${index}].IsRequired" value="true" 
                                       class="form-check-input" />
                            </div>
                        </div>
                    </div>

                    <div class="row">
                        <div class="col-md-4 mb-3">
                            <label class="form-label">Loại dữ liệu <span class="text-danger">*</span></label>
                            <select name="Attributes[${index}].AttributeType" class="form-select attribute-type-select" required>
                                <option value="">-- Chọn loại --</option>
                                <option value="Text">Text - Văn bản</option>
                                <option value="Number">Number - Số</option>
                                <option value="Select">Select - Lựa chọn</option>
                                <option value="MultiSelect">MultiSelect - Nhiều lựa chọn</option>
                                <option value="Boolean">Boolean - Có/Không</option>
                                <option value="Date">Date - Ngày tháng</option>
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
            } else {
                // Re-index all attributes
                danhLaiChiSoThuocTinh();
            }
        }, 300);
    }

    function xuLyThayDoiLoaiThuocTinh(select) {
        const attributeItem = select.closest('.attribute-item');
        const optionsContainer = attributeItem.querySelector('.options-container');
        const value = select.value;

        if (value === 'Select' || value === 'MultiSelect') {
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
                <input type="text" name="Attributes[${attributeIndex}].AttributeOptions[${optionIndex}].Value" 
                       class="form-control" placeholder="Giá trị (key)" required />
                <input type="text" name="Attributes[${attributeIndex}].AttributeOptions[${optionIndex}].DisplayText" 
                       class="form-control" placeholder="Tên hiển thị" required />
                <button type="button" class="btn btn-outline-danger btn-remove-option">
                    <i class="fas fa-times"></i>
                </button>
            </div>
        `;
    }

    function xoaLuaChon(optionItem) {
        optionItem.style.opacity = '0';
        setTimeout(() => {
            const attributeItem = optionItem.closest('.attribute-item');
            optionItem.remove();
            
            // Re-index options
            const optionsList = attributeItem.querySelector('.options-list');
            const options = optionsList.querySelectorAll('.option-item');
            const attrIndex = attributeItem.dataset.index;
            
            options.forEach((opt, idx) => {
                const inputs = opt.querySelectorAll('input');
                inputs.forEach(input => {
                    const name = input.getAttribute('name');
                    if (name && name.includes('AttributeOptions')) {
                        const newName = name.replace(/AttributeOptions\[\d+\]/, `AttributeOptions[${idx}]`);
                        input.setAttribute('name', newName);
                    }
                });
            });
        }, 200);
    }

    const form = document.getElementById('createCategoryForm');
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
            
            const heading = attr.querySelector('h6');
            if (heading) {
                heading.innerHTML = `<span class="badge bg-primary">${index + 1}</span> Thuộc tính #${index + 1}`;
            }
        });
    }

})();
