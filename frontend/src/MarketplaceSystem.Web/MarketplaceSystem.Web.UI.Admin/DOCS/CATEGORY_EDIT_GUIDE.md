# Hướng dẫn Chỉnh sửa Danh mục (Category Edit)

## Tổng quan

Chức năng chỉnh sửa danh mục cho phép:
1. Cập nhật thông tin cơ bản của danh mục
2. Quản lý thuộc tính (attributes) của danh mục
3. Quản lý tùy chọn (options) của thuộc tính

## API Endpoints đã implement

### 1. Cập nhật Danh mục
```
PUT /api/admin/v1/categories/{id}
```

**Request Body:**
```json
{
  "name": "string",
  "description": "string",
  "parentCategoryId": 0,
  "displayOrder": 0,
  "isActive": true
}
```

**Controller Action:** `CategoryController.Edit()` (POST)

---

### 2. Cập nhật Thuộc tính
```
PATCH /api/admin/v1/categories/{categoryId}/attributes/{attributeId}
```

**Request Body:**
```json
{
  "name": "string",
  "displayName": "string",
  "isRequired": true,
  "displayOrder": 0,
  "placeholder": "string"
}
```

**Controller Action:** `CategoryController.UpdateAttribute()` (POST, AJAX)

---

### 3. Xóa Thuộc tính
```
DELETE /api/admin/v1/categories/{categoryId}/attributes/{attributeId}
```

**Controller Action:** `CategoryController.DeleteAttribute()` (POST, AJAX)

---

### 4. Cập nhật Option
```
PATCH /api/admin/v1/categories/{categoryId}/attributes/{attributeId}/options/{optionId}
```

**Request Body:**
```json
{
  "id": 0,
  "value": "string",
  "displayText": "string",
  "isActive": true,
  "categoryAttributeId": 0
}
```

**Controller Action:** `CategoryController.UpdateAttributeOption()` (POST, AJAX)

---

### 5. Xóa Option
```
DELETE /api/admin/v1/categories/{categoryId}/attributes/{attributeId}/options/{optionId}
```

**Controller Action:** `CategoryController.DeleteAttributeOption()` (POST, AJAX)

---

## Files đã tạo/cập nhật

### Models

#### 1. `UpdateCategoryViewModel.cs`
```csharp
public class UpdateCategoryViewModel
{
    [Required(ErrorMessage = "Tên danh mục là bắt buộc!")]
    public required string Name { get; set; }

    public string? Description { get; set; }

    public int? ParentCategoryId { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Thứ tự hiển thị phải lớn hơn 0!")]
    public int DisplayOrder { get; set; } = 1;

    public bool IsActive { get; set; } = true;
}
```

#### 2. `UpdateCategoryAttributeViewModel.cs`
```csharp
public class UpdateCategoryAttributeViewModel
{
    [Required(ErrorMessage = "Tên thuộc tính là bắt buộc!")]
    public required string Name { get; set; }

    [Required(ErrorMessage = "Tên hiển thị là bắt buộc!")]
    public required string DisplayName { get; set; }

    public bool IsRequired { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Thứ tự hiển thị phải lớn hơn 0!")]
    public int DisplayOrder { get; set; } = 1;

    public string? Placeholder { get; set; }
}
```

#### 3. `UpdateAttributeOptionViewModel.cs`
```csharp
public class UpdateAttributeOptionViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Giá trị là bắt buộc!")]
    public required string Value { get; set; }

    [Required(ErrorMessage = "Tên hiển thị là bắt buộc!")]
    public required string DisplayText { get; set; }

    public bool IsActive { get; set; } = true;

    public int CategoryAttributeId { get; set; }
}
```

---

### Services

#### `ICategoryService.cs` - Đã thêm methods:
```csharp
Task<ApiResponse<CategoryViewModel>?> UpdateCategoryAsync(int categoryId, UpdateCategoryViewModel model, CancellationToken cancellation = default);

Task<ApiResponse?> UpdateAttributeAsync(int categoryId, int attributeId, UpdateCategoryAttributeViewModel model, CancellationToken cancellation = default);

Task<ApiResponse?> DeleteAttributeAsync(int categoryId, int attributeId, CancellationToken cancellation = default);

Task<ApiResponse?> UpdateAttributeOptionAsync(int categoryId, int attributeId, int optionId, UpdateAttributeOptionViewModel model, CancellationToken cancellation = default);

Task<ApiResponse?> DeleteAttributeOptionAsync(int categoryId, int attributeId, int optionId, CancellationToken cancellation = default);
```

#### `CategoryService.cs` - Đã implement:
- ✅ `UpdateCategoryAsync()` - Gọi PUT /categories/{id}
- ✅ `UpdateAttributeAsync()` - Gọi PATCH /categories/{id}/attributes/{id}
- ✅ `DeleteAttributeAsync()` - Gọi DELETE /categories/{id}/attributes/{id}
- ✅ `UpdateAttributeOptionAsync()` - Gọi PATCH /categories/{id}/attributes/{id}/options/{id}
- ✅ `DeleteAttributeOptionAsync()` - Gọi DELETE /categories/{id}/attributes/{id}/options/{id}

---

### Base Services

#### `IBaseApiService.cs` - Đã thêm:
```csharp
Task<HttpResponseMessage?> PatchAsync<TRequest>(
    string endpoint,
    TRequest? request,
    CancellationToken cancellation = default) where TRequest : class;
```

#### `BaseApiService.cs` - Đã implement:
```csharp
public virtual async Task<HttpResponseMessage?> PatchAsync<TRequest>(
    string endpoint,
    TRequest? request = null,
    CancellationToken cancellation = default) where TRequest : class
{
    HttpResponseMessage response;
    if (request == null)
    {
        response = await _httpClient.PatchAsync(endpoint, null, cancellation);
    }
    else
    {
        string json = JsonSerializer.Serialize(request);
        StringContent content = new StringContent(json, Encoding.UTF8, "application/json");

        response = await _httpClient.PatchAsync(endpoint, content, cancellation);
    }
    return response;
}
```

---

### Controllers

#### `CategoryController.cs` - Đã thêm actions:

1. **Edit (POST)** - Cập nhật danh mục
```csharp
[HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> Edit(int id, CategoryViewModel model, CancellationToken cancellation = default)
```

2. **UpdateAttribute (POST, AJAX)** - Cập nhật thuộc tính
```csharp
[HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> UpdateAttribute(int categoryId, int attributeId, UpdateCategoryAttributeViewModel model, CancellationToken cancellation = default)
```

3. **DeleteAttribute (POST, AJAX)** - Xóa thuộc tính
```csharp
[HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> DeleteAttribute(int categoryId, int attributeId, CancellationToken cancellation = default)
```

4. **UpdateAttributeOption (POST, AJAX)** - Cập nhật option
```csharp
[HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> UpdateAttributeOption(int categoryId, int attributeId, int optionId, UpdateAttributeOptionViewModel model, CancellationToken cancellation = default)
```

5. **DeleteAttributeOption (POST, AJAX)** - Xóa option
```csharp
[HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> DeleteAttributeOption(int categoryId, int attributeId, int optionId, CancellationToken cancellation = default)
```

---

## Frontend (Views & JavaScript)

### View: `Edit.cshtml`
- Form chỉnh sửa thông tin cơ bản danh mục
- Quản lý thuộc tính động
- Quản lý options cho thuộc tính Select/MultiSelect

### JavaScript: `category-edit.js`
File JavaScript đã có sẵn xử lý:
- ✅ Thêm/xóa thuộc tính
- ✅ Thêm/xóa options
- ✅ Xem trước icon
- ✅ Gọi API xóa thuộc tính khi đã tồn tại trên server
- ✅ Gọi API xóa option khi đã tồn tại trên server

**Functions chính:**
- `deleteAttributeFromServer()` - Gọi API DELETE attribute
- `deleteOptionFromServer()` - Gọi API DELETE option

---

## Luồng hoạt động

### 1. Chỉnh sửa thông tin cơ bản
```
User clicks "Lưu thay đổi" 
→ Form POST to CategoryController.Edit() 
→ Maps to UpdateCategoryViewModel 
→ Calls CategoryService.UpdateCategoryAsync() 
→ PUT /api/admin/v1/categories/{id} 
→ Returns success/error 
→ Redirects to Details page
```

### 2. Xóa thuộc tính đã lưu
```
User clicks "Xóa" on attribute (data-attribute-id exists) 
→ Confirms deletion 
→ AJAX POST to /Category/DeleteAttribute 
→ CategoryController.DeleteAttribute() 
→ Calls CategoryService.DeleteAttributeAsync() 
→ DELETE /api/admin/v1/categories/{categoryId}/attributes/{attributeId} 
→ Returns JSON {success, message} 
→ Removes from UI if success
```

### 3. Xóa option đã lưu
```
User clicks "Xóa" on option (data-option-id exists) 
→ Confirms deletion 
→ AJAX POST to /Category/DeleteAttributeOption 
→ CategoryController.DeleteAttributeOption() 
→ Calls CategoryService.DeleteAttributeOptionAsync() 
→ DELETE /api/admin/v1/categories/{categoryId}/attributes/{attributeId}/options/{optionId} 
→ Returns JSON {success, message} 
→ Removes from UI if success
```

---

## Lưu ý quan trọng

### Error Handling
Tất cả các service methods đều có try-catch cho `TaskCanceledException`:
```csharp
catch (TaskCanceledException)
{
    return null;
}
```

### Validation
- Models sử dụng Data Annotations cho validation
- ModelState được kiểm tra trong Controller
- API errors được map vào ModelState qua `MapApiErrorsToModelState()`

### AJAX Calls
- Sử dụng fetch API
- Gửi Anti-Forgery Token trong header
- Response format: `{success: boolean, message: string, data?: any}`

---

## Testing

### 1. Test cập nhật danh mục:
- Mở `/Category/Edit/{id}`
- Thay đổi Name, Description, DisplayOrder, IsActive
- Click "Lưu thay đổi"
- Verify redirect to Details với thông tin đã update

### 2. Test xóa thuộc tính:
- Tại Edit page, click "Xóa" trên thuộc tính đã lưu
- Confirm dialog
- Verify thuộc tính biến mất khỏi UI
- Reload page để verify thuộc tính đã bị xóa từ database

### 3. Test xóa option:
- Tại Edit page, click "Xóa" trên option đã lưu
- Confirm dialog
- Verify option biến mất khỏi UI
- Reload page để verify option đã bị xóa từ database

---

## Summary

✅ **Hoàn thành:**
- UpdateCategoryViewModel, UpdateCategoryAttributeViewModel, UpdateAttributeOptionViewModel
- PatchAsync method trong BaseApiService
- 5 service methods trong CategoryService
- 5 controller actions trong CategoryController
- Integration với JavaScript có sẵn (category-edit.js)

✅ **Chỉ chỉnh sửa Category-related files** (theo yêu cầu của user)

✅ **Encoding tiếng Việt đã được sửa** trong tất cả views

🎯 **Ready for testing!**
