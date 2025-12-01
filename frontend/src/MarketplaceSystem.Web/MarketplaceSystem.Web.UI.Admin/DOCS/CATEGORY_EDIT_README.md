# Category Edit - Simple Version

## 🎯 Mục đích

Làm lại form chỉnh sửa danh mục một cách **đơn giản, rõ ràng, dễ maintain**.

## 📁 Files được tạo/sửa

### Mới tạo:
- `wwwroot/js/admin/category-edit-simple.js` - JavaScript chính (code mới, sạch, đơn giản)
- `DOCS/CATEGORY_EDIT_SIMPLE_GUIDE.md` - Hướng dẫn chi tiết

### Đã sửa:
- `Views/Category/Edit.cshtml` - Đổi script reference từ `category-edit-new.js` sang `category-edit-simple.js`

### Không thay đổi (vẫn dùng được):
- `Controllers/CategoryController.cs`
- `Services/CategoryService.cs`
- `Interfaces/ICategoryService.cs`
- `Models/ViewModel/Categories/*.cs`
- `wwwroot/css/admin/category-edit.css`

## 🚀 Cách sử dụng

### 1. Truy cập trang Edit Category
```
https://localhost:7079/Category/Edit/{id}
```

### 2. Các chức năng chính

#### A. Cập nhật thông tin Category
1. Sửa các trường: Name, Description, Parent Category, Display Order, Is Active
2. Click **"Cập nhật thông tin cơ bản"**
3. API sẽ được gọi: `PUT /api/admin/v1/categories/{id}`

#### B. Cập nhật Attribute
1. Tìm attribute cần sửa
2. Sửa các trường: Display Name, Is Required, Display Order, Placeholder
3. Click **"Cập nhật thuộc tính này"** (trong card của attribute đó)
4. API: `PATCH /api/admin/v1/categories/{categoryId}/attributes/{attributeId}`

**Lưu ý**: Không thể đổi loại attribute (Text -> Select)

#### C. Thêm Attribute mới
1. Click **"Thêm thuộc tính mới"**
2. Điền thông tin:
   - Name (viết thường, không dấu, VD: `brand`)
   - Display Name (VD: `Hãng xe`)
   - Attribute Type: Text, Number, Select, MultiSelect, Boolean, Date
   - Display Order
   - Is Required
   - Placeholder
3. Nếu chọn Select/MultiSelect, click "Thêm option" và điền value + display text
4. Click **"Lưu thuộc tính"**
5. API: `POST /api/admin/v1/categories/{categoryId}/attributes`

#### D. Cập nhật Option
1. Tìm option trong attribute Select/MultiSelect
2. Sửa: Value, Display Text, Is Active
3. Click **"Cập nhật option này"**
4. API: `PATCH /api/admin/v1/categories/{categoryId}/attributes/{attributeId}/options/{optionId}`

#### E. Xóa Attribute
1. Click nút **"Xóa"** ở header của attribute card
2. Xác nhận
3. API: `DELETE /api/admin/v1/categories/{categoryId}/attributes/{attributeId}`

#### F. Xóa Option
1. Click nút xóa (icon trash) bên cạnh option
2. Xác nhận
3. API: `DELETE /api/admin/v1/categories/{categoryId}/attributes/{attributeId}/options/{optionId}`

## 📋 API Endpoints

| Method | Endpoint | Mô tả |
|--------|----------|-------|
| PUT | `/api/admin/v1/categories/{id}` | Update category |
| POST | `/api/admin/v1/categories/{categoryId}/attributes` | Create attribute |
| PATCH | `/api/admin/v1/categories/{categoryId}/attributes/{id}` | Update attribute |
| DELETE | `/api/admin/v1/categories/{categoryId}/attributes/{id}` | Delete attribute |
| PATCH | `/api/admin/v1/categories/{categoryId}/attributes/{attributeId}/options/{id}` | Update option |
| DELETE | `/api/admin/v1/categories/{categoryId}/attributes/{attributeId}/options/{id}` | Delete option |

## 🔑 Authentication

API sử dụng Bearer Token từ cookie:
```javascript
headers: {
  'Authorization': `Bearer ${getCookie('authToken')}`
}
```

Nếu token hết hạn, hệ thống sẽ trả về 401 Unauthorized.

## ⚠️ Lưu ý quan trọng

1. **Category cha**: Nếu category có sub-categories, không thể thêm attributes
2. **Attribute Type**: Không thể thay đổi sau khi tạo
3. **Validation**: 
   - Name, DisplayName: bắt buộc
   - DisplayOrder: phải >= 1
4. **Reload**: Sau khi create/delete sẽ tự động reload trang

## 🎨 UI/UX

- Mỗi phần có nút **cập nhật riêng**
- Alert hiển thị ở top của trang
- Tự động đóng alert sau 5 giây
- Confirmation cho các thao tác xóa
- Loading state khi call API

## 🐛 Debug

Mở Browser Console (F12) để xem:
- API calls: method, endpoint, request body
- Response từ server
- Error messages chi tiết

## 📖 Tài liệu chi tiết

Xem file: `DOCS/CATEGORY_EDIT_SIMPLE_GUIDE.md`

## 💡 Ưu điểm so với version cũ

✅ **Đơn giản hơn**: Gọi API trực tiếp, không qua Controller proxy  
✅ **Rõ ràng hơn**: Mỗi function một nhiệm vụ  
✅ **Dễ debug**: Console.log đầy đủ  
✅ **Code ngắn gọn**: Helper functions tái sử dụng  
✅ **Không Anti-Forgery Token**: Dùng Bearer Token  
✅ **Dễ maintain**: Comments rõ ràng, structure logic  

## 🔧 Troubleshooting

### Lỗi 401 Unauthorized
→ Token hết hạn, login lại

### Lỗi 400 Bad Request
→ Kiểm tra request body trong Console, verify validation

### Không tìm thấy attribute/option
→ Kiểm tra `data-attribute-id` và `data-option-id` trong HTML

### Không reload sau update
→ Kiểm tra parameter `reload=true` trong `callApi()`

## 🎓 Code Structure

```javascript
// Main API caller
callApi(method, endpoint, data, successMsg, reload)

// Initialization
initCategoryUpdate()
initAttributeHandlers()
initOptionHandlers()
initAddAttributeModal()

// Actions
updateAttribute(attrId)
updateOption(attrId, optId)
saveNewAttribute()

// Helpers
getValue(id)
getIntValue(id)
getCheckboxValue(id)
getValueFromCard(card, selector)
getCookie(name)
showSuccess(msg)
showError(msg)
```

## 📞 Support

Nếu có lỗi hoặc cần hỗ trợ, kiểm tra:
1. Browser Console (F12)
2. Network tab để xem API calls
3. Response từ server
4. File `CATEGORY_EDIT_SIMPLE_GUIDE.md` để biết thêm chi tiết
