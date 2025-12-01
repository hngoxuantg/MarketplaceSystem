# Hướng dẫn sử dụng chức năng Tạo Danh mục

## 📋 Tổng quan

Chức năng tạo danh mục cho phép admin thêm danh mục mới vào hệ thống Marketplace với đầy đủ thông tin và thuộc tính động.

## 🔧 Các thành phần đã cập nhật

### 1. **Model** (`CreateCategoryViewModel.cs`)
- ✅ Validation đầy đủ cho tất cả field
- ✅ Hỗ trợ upload file icon (IFormFile)
- ✅ AttributeType là string (Text, Number, Select, MultiSelect, Boolean, Date)
- ✅ DisplayOrder bắt buộc >= 1
- ✅ Attributes và AttributeOptions với validation

### 2. **Service** (`CategoryService.cs`)
- ✅ `CreateCategoryAsync()` - Tạo danh mục mới
- ✅ `UploadCategoryIconAsync()` - Upload icon cho danh mục

### 3. **Controller** (`CategoryController.cs`)
- ✅ GET Create - Load form với danh sách parent categories
- ✅ POST Create - Xử lý tạo danh mục và upload icon
- ✅ Error handling đầy đủ từ backend API
- ✅ MapApiErrorsToModelState từ BaseController

### 4. **View** (`Create.cshtml`)
- ✅ Form với enctype="multipart/form-data" để upload file
- ✅ Upload file icon thay vì nhập URL/Font Awesome
- ✅ Preview icon trước khi submit
- ✅ Quản lý thuộc tính động với JavaScript
- ✅ Validation client-side

### 5. **JavaScript** (`category-create.js`)
- ✅ Preview icon file upload
- ✅ Thêm/xóa thuộc tính động
- ✅ Thêm/xóa options cho Select/MultiSelect
- ✅ Auto re-index khi xóa items
- ✅ Format file size

## 📡 API Endpoints

### 1. Create Category
```http
POST https://localhost:7079/api/admin/v1/categories
Content-Type: application/json
```

**Request Body:**
```json
{
  "name": "Xe máy",
  "description": "Danh mục xe máy các loại",
  "parentCategoryId": null,
  "displayOrder": 1,
  "isActive": true,
  "attributes": [
    {
      "name": "brand",
      "displayName": "Hãng xe",
      "attributeType": "Select",
      "isRequired": true,
      "displayOrder": 1,
      "placeholder": "Chọn hãng xe",
      "attributeOptions": [
        {
          "value": "honda",
          "displayText": "Honda"
        },
        {
          "value": "yamaha",
          "displayText": "Yamaha"
        }
      ]
    },
    {
      "name": "year",
      "displayName": "Năm sản xuất",
      "attributeType": "Number",
      "isRequired": false,
      "displayOrder": 2,
      "placeholder": "Nhập năm sản xuất"
    }
  ]
}
```

**Success Response:**
```json
{
  "success": true,
  "message": "Tạo danh mục thành công",
  "data": {
    "id": 123,
    "name": "Xe máy",
    "description": "Danh mục xe máy các loại",
    "displayOrder": 1,
    "isActive": true,
    ...
  }
}
```

**Error Response:**
```json
{
  "success": false,
  "title": "Đã xảy ra một hoặc nhiều lỗi xác thực.",
  "errors": {
    "displayOrder": [
      "Thứ tự hiển thị phải lớn hơn 0!"
    ],
    "attributes[0].DisplayOrder": [
      "Thứ tự hiển thị là bắt buộc!",
      "Thứ tự hiển thị phải lớn hơn 0!"
    ]
  },
  "error": {
    "code": "VALIDATION_ERROR",
    "type": "ValidatorException"
  },
  "timestamp": "2025-11-13T15:24:52.7542091Z"
}
```

### 2. Upload Category Icon
```http
POST https://localhost:7079/api/admin/v1/files/upload-category-icon/{id}
Content-Type: multipart/form-data
```

**Form Data:**
- `icon`: File hình ảnh (PNG, JPG, SVG...) - Tên field phải là **"icon"** (lowercase)

**Success Response:**
```json
{
  "success": true,
  "message": "Upload icon cho danh mục thành công!",
  "data": "https://cdn.example.com/icons/category-123.png"
}
```

**⚠️ Lưu ý:**
- Backend nhận `IFormFile icon` (một file duy nhất)
- Field name phải là **"icon"** (không phải "IconFile" hay "Images")
- Frontend sử dụng `PostFileAsync` (single file), không phải `PostFilesAsync` (multiple files)
```

## 🎯 Luồng xử lý

### Khi submit form:

1. **Validate client-side** (JavaScript + HTML5)
   - Required fields
   - Number >= 1
   - File type cho icon

2. **Submit form** → Controller POST Create

3. **Validate server-side** (ModelState)
   - Data Annotations
   - Required fields
   - DisplayOrder >= 1

4. **Call API Create Category**
   - Gửi JSON đến backend
   - Nhận response với category ID

5. **Upload Icon (nếu có)**
   - Nếu có IconFile, gọi API upload
   - Gửi file với category ID
   - Không fail toàn bộ nếu upload lỗi, chỉ warning

6. **Redirect**
   - Success → Details page với TempData["Success"]
   - Error → Ở lại form với errors hiển thị

## 🔍 Validation Rules

### CreateCategoryViewModel
- `Name`: Required
- `DisplayOrder`: Required, >= 1
- `IconFile`: Optional, phải là image file

### CreateCategoryAttributeDto
- `Name`: Required
- `DisplayName`: Required
- `AttributeType`: Required (Text/Number/Select/MultiSelect/Boolean/Date)
- `DisplayOrder`: Required, >= 1

### CreateAttributeOptionDto
- `Value`: Required
- `DisplayText`: Required

## 💡 Ví dụ sử dụng

### Tạo danh mục đơn giản (không có attributes)
```
1. Nhập tên: "Điện thoại"
2. Nhập mô tả: "Điện thoại di động các loại"
3. Chọn file icon
4. Thứ tự: 1
5. Bật "Kích hoạt"
6. Submit
```

### Tạo danh mục với attributes
```
1. Nhập thông tin cơ bản
2. Click "Thêm thuộc tính"
3. Thuộc tính 1:
   - Name: brand
   - Display Name: Hãng
   - Type: Select
   - Required: Yes
   - Click "Thêm tùy chọn":
     * Value: apple, Display: Apple
     * Value: samsung, Display: Samsung
4. Thuộc tính 2:
   - Name: storage
   - Display Name: Dung lượng
   - Type: Number
   - Required: No
5. Submit
```

## 🐛 Xử lý lỗi

### Lỗi từ Backend API
- Hiển thị trong ModelState
- Mapping từ errors dictionary
- Hiển thị dưới từng field tương ứng

### Lỗi Upload Icon
- Không fail toàn bộ
- Hiển thị warning: "Danh mục đã được tạo nhưng không thể upload icon"
- Log error để debug

### Lỗi Network
- Try-catch trong controller
- Hiển thị thông báo chung
- Ở lại form với dữ liệu đã nhập

## 📝 Ghi chú

- Icon upload là **optional**, có thể thêm sau
- Attributes có thể để trống khi tạo, thêm sau qua Edit
- Backend sẽ validate AttributeType enum
- DisplayOrder phải >= 1 cả category và attributes
- ParentCategoryId = null nghĩa là danh mục gốc

## 🔄 Testing

### Test cases cần kiểm tra:

1. ✅ Tạo danh mục cơ bản (chỉ name)
2. ✅ Tạo với đầy đủ thông tin
3. ✅ Tạo với icon upload
4. ✅ Tạo với parent category
5. ✅ Tạo với attributes (Text, Number, Select)
6. ✅ Validation errors hiển thị đúng
7. ✅ Upload icon fail nhưng vẫn tạo được category
8. ✅ Thêm/xóa attributes động
9. ✅ Thêm/xóa options cho Select/MultiSelect
10. ✅ Re-index sau khi xóa

## 🚀 Cải tiến trong tương lai

- [ ] Drag & drop cho icon upload
- [ ] Crop/resize icon trước khi upload
- [ ] Duplicate attribute template
- [ ] Import/export attributes từ JSON
- [ ] Preview category trước khi tạo
- [ ] Auto-generate attribute name từ display name
