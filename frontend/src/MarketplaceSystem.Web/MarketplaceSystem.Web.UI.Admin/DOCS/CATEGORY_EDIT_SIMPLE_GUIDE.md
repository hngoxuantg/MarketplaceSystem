# Hướng dẫn chỉnh sửa danh mục - Phiên bản đơn giản

## Tổng quan

Form chỉnh sửa danh mục được thiết kế đơn giản với các nút cập nhật riêng cho từng phần:
- Category (thông tin cơ bản)
- Attributes (thuộc tính)
- Options (tùy chọn cho Select/MultiSelect)

## API Endpoints

### 1. Update Category
```
PUT /api/admin/v1/categories/{id}
Body: {
  "name": "string",
  "description": "string",
  "parentCategoryId": 0,
  "displayOrder": 0,
  "isActive": true
}
```

### 2. Create Attribute
```
POST /api/admin/v1/categories/{categoryId}/attributes
Body: {
  "name": "string",
  "displayName": "string",
  "attributeType": "Text|Number|Select|MultiSelect|Boolean|Date",
  "isRequired": true,
  "displayOrder": 0,
  "placeholder": "string",
  "attributeOptions": [
    {
      "value": "string",
      "displayText": "string"
    }
  ]
}
```

### 3. Update Attribute
```
PATCH /api/admin/v1/categories/{categoryId}/attributes/{attributeId}
Body: {
  "name": "string",
  "displayName": "string",
  "isRequired": true,
  "displayOrder": 0,
  "placeholder": "string"
}
```

### 4. Delete Attribute
```
DELETE /api/admin/v1/categories/{categoryId}/attributes/{attributeId}
```

### 5. Update Option
```
PATCH /api/admin/v1/categories/{categoryId}/attributes/{attributeId}/options/{optionId}
Body: {
  "id": 0,
  "value": "string",
  "displayText": "string",
  "isActive": true,
  "categoryAttributeId": 0
}
```

### 6. Delete Option
```
DELETE /api/admin/v1/categories/{categoryId}/attributes/{attributeId}/options/{optionId}
```

## Cấu trúc File

```
Views/Category/Edit.cshtml          - View chính
wwwroot/js/admin/category-edit-simple.js  - JavaScript xử lý
Controllers/CategoryController.cs   - Controller (không cần thiết cho API trực tiếp)
Services/CategoryService.cs         - Service layer
Interfaces/ICategoryService.cs      - Interface
Models/ViewModel/Categories/        - ViewModels
```

## Luồng hoạt động

### 1. Update Category
1. User nhập thông tin vào form
2. Click nút "Cập nhật thông tin cơ bản"
3. JS gọi `PUT /api/admin/v1/categories/{id}`
4. Hiển thị thông báo và reload trang

### 2. Update Attribute
1. User sửa thông tin attribute trong card
2. Click nút "Cập nhật thuộc tính này"
3. JS gọi `PATCH /api/admin/v1/categories/{categoryId}/attributes/{attributeId}`
4. Hiển thị thông báo

**Lưu ý**: Không thể đổi type của attribute (Text -> Select)

### 3. Update Option
1. User sửa thông tin option
2. Click nút "Cập nhật option này"
3. JS gọi `PATCH /api/admin/v1/categories/{categoryId}/attributes/{attributeId}/options/{optionId}`
4. Hiển thị thông báo

### 4. Create New Attribute
1. Click "Thêm thuộc tính mới"
2. Điền thông tin trong modal
3. Nếu chọn Select/MultiSelect, thêm options
4. Click "Lưu thuộc tính"
5. JS gọi `POST /api/admin/v1/categories/{categoryId}/attributes`
6. Reload trang để hiển thị attribute mới

### 5. Delete
- Delete Attribute: Xác nhận -> gọi DELETE API -> reload
- Delete Option: Xác nhận -> gọi DELETE API -> reload

## Quy tắc Business Logic

1. **Category cha**: Không thể thêm attribute nếu category có sub-categories
2. **Attribute Type**: Không thể thay đổi type sau khi tạo
3. **Options**: Chỉ hiển thị với Select và MultiSelect
4. **Validation**:
   - Name, DisplayName: bắt buộc
   - DisplayOrder: >= 1
   - AttributeType: bắt buộc khi tạo mới

## Code JavaScript chính

```javascript
// Main API caller
async function callApi(method, endpoint, data, successMsg, reload = false)

// Category
function initCategoryUpdate()

// Attributes
function initAttributeHandlers()
async function updateAttribute(attrId)

// Options
function initOptionHandlers()
async function updateOption(attrId, optId)

// Add new attribute
function initAddAttributeModal()
async function saveNewAttribute()
```

## Xử lý lỗi

Tất cả các API call đều có try-catch:
- Hiển thị alert màu đỏ nếu lỗi
- Console.log chi tiết lỗi
- Không reload trang nếu có lỗi

## Authentication

API sử dụng Bearer Token từ cookie `authToken`:
```javascript
headers: {
  'Authorization': `Bearer ${getCookie('authToken') || ''}`
}
```

## Cải tiến so với phiên bản cũ

1. **Đơn giản hơn**: Gọi API trực tiếp, không qua Controller proxy
2. **Rõ ràng hơn**: Mỗi function có một nhiệm vụ duy nhất
3. **Dễ debug**: Console.log đầy đủ, error message rõ ràng
4. **Code ngắn gọn**: Các helper functions tái sử dụng
5. **Không dùng Anti-Forgery Token**: Vì gọi API trực tiếp với Bearer Token

## Testing

1. Test update category: Đổi tên, description, parent, order, active
2. Test update attribute: Đổi displayName, isRequired, order, placeholder
3. Test update option: Đổi value, displayText, isActive
4. Test create attribute: Tạo Text, Number, Select với options
5. Test delete: Attribute và Option
6. Test validation: Bỏ trống required fields
7. Test parent category: Verify không hiển thị form thêm attribute

## Troubleshooting

### Lỗi "Không tìm thấy thuộc tính"
- Kiểm tra `data-attribute-id` trong HTML
- Kiểm tra selector `.attribute-item[data-attribute-id="${attrId}"]`

### API trả về 401 Unauthorized
- Kiểm tra cookie `authToken` còn hiệu lực
- Login lại nếu cần

### API trả về 400 Bad Request
- Kiểm tra request body trong Console
- Verify validation ở backend

### Không reload sau khi update
- Kiểm tra parameter `reload = true` trong callApi
- Kiểm tra Console có lỗi không
