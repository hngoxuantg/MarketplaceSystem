# Category Management - H??ng D?n S? D?ng

## T?ng quan

H? th?ng qu?n lý danh m?c v?i ??y ?? CRUD operations và qu?n lý category attributes.

## C?u trúc Files

### Controllers
- `CategoryController.cs` - Controller v?i các actions:
  - `Index()` - Danh sách danh m?c v?i filter/pagination
  - `Details(id)` - Xem chi ti?t danh m?c
  - `Create()` - Form t?o danh m?c m?i
  - `Edit(id)` - Form ch?nh s?a danh m?c
  - `Delete(id)` - Xóa danh m?c (AJAX)
  - `ToggleStatus(id)` - B?t/t?t danh m?c (AJAX)
  - `GetCategoriesPartial()` - Load partial view (AJAX)

### Views

#### 1. Index.cshtml
- Danh sách danh m?c
- Statistics cards
- Filter form
- Partial view table

#### 2. Details.cshtml
- Thông tin chi ti?t danh m?c
- Danh sách danh m?c con
- Danh sách attributes
- Buttons: Edit, Back

#### 3. Edit.cshtml
- Form ch?nh s?a danh m?c
- Qu?n lý attributes (thêm/s?a/xóa)
- Icon preview
- Validation

#### 4. Create.cshtml
- Form t?o danh m?c m?i
- Tùy ch?n thêm attributes
- Icon preview
- Validation

### Partial Views

#### _CategoriesTablePartial.cshtml
- B?ng danh sách categories
- Pagination
- Action buttons

#### _CategoryAttributesPartial.cshtml
- Hi?n th? danh sách attributes (read-only)
- Hi?n th? options cho Select type

#### _AttributeEditorPartial.cshtml
- Form editor cho t?ng attribute
- Dynamic options management
- Validation

### Assets

#### CSS
- `category-index.css` - Styles cho trang Index
- `category-details.css` - Styles cho trang Details
- `category-edit.css` - Styles cho trang Edit/Create

#### JavaScript
- `category-index.js` - Logic cho Index page (filter, pagination)
- `category-edit.js` - Logic cho Edit/Create page (attributes management)

## Ch?c n?ng chính

### 1. Danh sách danh m?c (Index)

**URL**: `/Category/Index`

**Features**:
- Statistics cards (t?ng, active, inactive, subcategories)
- Filter by:
  - Search term (name/description)
  - Status (active/inactive/all)
  - Page size (5/10/20/50)
- Pagination không reload trang
- Actions: View, Edit, Toggle Status, Delete

**Quy trình**:
1. Load trang ? Hi?n th? toàn b? categories
2. Nh?p filter ? Submit form ? AJAX load partial view
3. Click page number ? AJAX load trang m?i

### 2. Xem chi ti?t (Details)

**URL**: `/Category/Details/{id}`

**Hi?n th?**:
- Thông tin c? b?n (ID, Name, Description, Icon, Parent, Order, Status)
- Statistics (s? subcategories, s? attributes)
- Th?i gian (Created, Updated)
- Danh sách subcategories (n?u có)
- Danh sách attributes (n?u có)

**Actions**:
- Edit ? Chuy?n sang trang Edit
- Back ? Quay l?i Index

### 3. Ch?nh s?a (Edit)

**URL**: `/Category/Edit/{id}`

**Form fields**:

**Thông tin c? b?n**:
- Name* (required)
- Description
- Icon (URL ho?c Font Awesome class)
- Parent Category (dropdown)
- Display Order
- Is Active (switch)

**Attributes Management**:
- Thêm/S?a/Xóa attributes
- M?i attribute có:
  - Name (key)* - Tên dùng trong code
  - Display Name* - Tên hi?n th?
  - Attribute Type* - Select t?: Text, Number, Select, MultiSelect, Boolean, Date
  - Placeholder
  - Display Order
  - Is Required (switch)
  - Options (n?u type = Select/MultiSelect)

**Options Management** (cho Select/MultiSelect):
- Value (key)* - Giá tr? dùng trong code
- Display Text* - Text hi?n th?
- Is Active - Có hi?n th? option này không
- Thêm/Xóa options ??ng

**Features**:
- Icon preview realtime
- Dynamic attributes list
- Auto re-indexing khi submit
- Validation client-side & server-side

**Quy trình**:
1. Load page ? Populate form v?i data hi?n t?i
2. Edit fields ? Preview icon (n?u thay ??i)
3. Click "Thêm thu?c tính" ? Thêm attribute m?i
4. Ch?n type = Select ? Hi?n options section
5. Click "Thêm option" ? Thêm option m?i
6. Submit ? Validate ? Save ? Redirect to Details

### 4. T?o m?i (Create)

**URL**: `/Category/Create`

T??ng t? Edit nh?ng:
- Không có ID
- Form tr?ng
- Có th? b? qua attributes (thêm sau)
- Submit ? Redirect to Index

### 5. Xóa (Delete)

**Method**: POST AJAX

**Quy trình**:
1. Click button Delete ? Confirm dialog
2. User confirm ? POST `/Category/Delete`
3. Success ? Toast notification ? Reload table
4. Error ? Toast error message

### 6. Toggle Status

**Method**: POST AJAX

**Quy trình**:
1. Click button Toggle ? Confirm dialog
2. User confirm ? POST `/Category/ToggleStatus`
3. Success ? Toast notification ? Reload table
4. Error ? Toast error message

## Attribute Types

### 1. Text (1)
- Input text thông th??ng
- VD: Tên s?n ph?m, Mô t? ng?n

### 2. Number (2)
- Input number
- VD: Dung tích, Công su?t

### 3. Select (3)
- Dropdown ??n l?a ch?n
- C?n ??nh ngh?a Options
- VD: Hãng xe, Màu s?c

### 4. MultiSelect (4)
- Dropdown nhi?u l?a ch?n
- C?n ??nh ngh?a Options
- VD: Tính n?ng, Lo?i k?t n?i

### 5. Boolean (5)
- Checkbox Có/Không
- VD: Còn b?o hành, Có hóa ??n

### 6. Date (6)
- Date picker
- VD: Ngày mua, Ngày h?t h?n

## JavaScript Logic

### category-index.js

**Functions**:
```javascript
loadCategories() // Load categories v?i filter qua AJAX
loadPage(pageNumber) // Load trang c? th?
resetFilters() // Reset form filter
toggleStatus(id, currentStatus) // B?t/t?t category
deleteCategory(id, name) // Xóa category
showLoading() / hideLoading() // Loading overlay
showToast(title, message, type) // Toast notification
```

### category-edit.js

**Functions**:
```javascript
previewIcon() // Preview icon (FA ho?c URL)
addAttribute() // Thêm attribute m?i
removeAttribute(element) // Xóa attribute
handleAttributeTypeChange(select) // Show/hide options
addOption(attributeItem) // Thêm option cho Select
removeOption(element) // Xóa option
reindexAttributes() // Re-index tr??c khi submit
createAttributeElement(index) // T?o HTML attribute
createOptionElement(attrIndex, optIndex) // T?o HTML option
```

## Validation

### Server-side (Model Validation)

**CategoryViewModel**:
- Name: Required, MaxLength(100)
- Description: MaxLength(500)
- DisplayOrder: Range(1, 9999)

**CategoryAttributeViewModel**:
- Name: Required, MaxLength(50), Regex(^[a-z0-9_]+$)
- DisplayName: Required, MaxLength(100)
- AttributeType: Required
- DisplayOrder: Range(1, 999)

**AttributeOptionViewModel**:
- Value: Required, MaxLength(50)
- DisplayText: Required, MaxLength(100)

### Client-side (HTML5 + JavaScript)

- Required fields
- Number min/max
- Pattern validation
- Dynamic validation theo attribute type

## API Integration (TODO)

### CategoryService Interface

```csharp
Task<ApiResponse<PaginatedResponse<CategoryViewModel>>> GetCategoriesAsync()
Task<ApiResponse<CategoryViewModel>> GetCategoryByIdAsync(int id)
Task<ApiResponse<CategoryViewModel>> CreateCategoryAsync(CategoryViewModel model)
Task<ApiResponse<CategoryViewModel>> UpdateCategoryAsync(int id, CategoryViewModel model)
Task<ApiResponse<bool>> DeleteCategoryAsync(int id)
Task<ApiResponse<bool>> ToggleStatusAsync(int id)
```

### Endpoints c?n implement

```
GET    /admin/v1/categories              - List categories
GET    /admin/v1/categories/{id}          - Get category by ID
POST   /admin/v1/categories               - Create category
PUT    /admin/v1/categories/{id}          - Update category
DELETE /admin/v1/categories/{id}          - Delete category
PATCH  /admin/v1/categories/{id}/status   - Toggle status
```

## Responsive Design

### Desktop (? 992px)
- Form 2 columns (main + sidebar)
- Full table with all columns
- Large modals

### Tablet (768px - 991px)
- Form 2 columns
- Table scrollable horizontal
- Medium modals

### Mobile (< 768px)
- Form 1 column stacked
- Table compact
- Options stacked vertically
- Full-width buttons

## Troubleshooting

### Attributes không hi?n th?
- Check Model.Attributes có null không
- Verify partial view path
- Check Console errors

### Options không hi?n th? khi ch?n Select
- Check JavaScript handleAttributeTypeChange()
- Verify attribute-type-select class
- Check event listener

### Submit form attributes b? tr?ng
- Check name attributes format: `Attributes[0].Name`
- Verify reindexAttributes() ???c g?i
- Check FormData trong Network tab

### Icon không preview
- Check input value format
- Verify Font Awesome loaded
- Check image URL CORS

## Best Practices

### 1. Naming Convention
- Attribute Name: lowercase, no spaces, underscores (VD: `brand_name`)
- Display Name: Readable text (VD: "Hãng xe")

### 2. Display Order
- Start from 1
- Increment by 1
- Leave gaps for future insertion

### 3. Attributes Organization
- Group related attributes
- Required first, optional later
- Common first (Name, Brand), specific later

### 4. Options
- Value: Short, unique (VD: `honda`, `yamaha`)
- Display Text: User-friendly (VD: "Honda", "Yamaha")
- Keep active options reasonable (< 50)

## Testing Checklist

- [ ] List categories hi?n th? ?úng
- [ ] Filter ho?t ??ng (search, status, page size)
- [ ] Pagination ho?t ??ng
- [ ] View details hi?n th? ??y ?? thông tin
- [ ] Edit form populate ?úng data
- [ ] Create form tr?ng
- [ ] Icon preview ho?t ??ng (FA + URL)
- [ ] Add attribute ??ng
- [ ] Remove attribute có confirm
- [ ] Add option cho Select type
- [ ] Remove option
- [ ] Submit form v?i validation
- [ ] Toggle status ho?t ??ng
- [ ] Delete có confirm
- [ ] Toast notifications hi?n th?
- [ ] Responsive trên mobile
- [ ] Back button ho?t ??ng

## Summary

? **Complete CRUD** cho Category
? **Attributes Management** v?i dynamic options
? **Partial Views** không reload trang
? **Icon Preview** realtime
? **Validation** client + server
? **Responsive** design
? **Toast Notifications**
? **Loading States**

**C?n hoàn thi?n**: API Integration trong CategoryService
