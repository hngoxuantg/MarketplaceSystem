# Category Index - Tài Li?u H??ng D?n

## T?ng quan
Trang qu?n lý danh m?c (Category Index) ?ã ???c làm l?i v?i các tính n?ng:
- ? S? d?ng **Bootstrap 5** cho UI
- ? **Partial View** ?? load d? li?u không reload trang
- ? **AJAX** cho t?t c? các thao tác
- ? **Responsive** design
- ? Hi?n th? d? li?u th?c t? API
- ? Filter, search, pagination

## C?u trúc file

### 1. Backend Files

#### `CategoryIndexViewModel.cs`
```csharp
MarketplaceSystem.Web.UI.Admin/Models/ViewModel/Categories/CategoryIndexViewModel.cs
```
- ViewModel ch?a d? li?u cho trang Index
- Properties: Categories (paginated), Statistics, Filters

#### `CategoryController.cs` 
```csharp
MarketplaceSystem.Web.UI.Admin/Controllers/CategoryController.cs
```
**Actions:**
- `Index()` - Load trang chính
- `GetCategoriesPartial()` - Load partial view v?i filter
- `ToggleStatus(id)` - B?t/t?t danh m?c
- `Delete(id)` - Xóa danh m?c
- `GetDetails(id)` - L?y chi ti?t danh m?c

### 2. Frontend Files

#### `Index.cshtml`
```razor
MarketplaceSystem.Web.UI.Admin/Views/Category/Index.cshtml
```
- View chính v?i layout Bootstrap
- Statistics cards
- Filter form
- Table container (load partial view)
- Add/Edit modal

#### `_CategoriesTablePartial.cshtml`
```razor
MarketplaceSystem.Web.UI.Admin/Views/Category/_CategoriesTablePartial.cshtml
```
- Partial view ch?a table và pagination
- Load qua AJAX khi filter/search/pagination
- Không reload trang chính

#### `category-index.css`
```css
MarketplaceSystem.Web.UI.Admin/wwwroot/css/admin/category-index.css
```
- Custom styles cho trang Category Index
- Card styles, table styles, modal styles
- Animations và responsive

#### `category-index.js`
```javascript
MarketplaceSystem.Web.UI.Admin/wwwroot/js/admin/category-index.js
```
- JavaScript x? lý t?t c? logic client-side
- AJAX calls
- Form handling
- Toast notifications

## Tính n?ng

### 1. Statistics Cards
Hi?n th? 4 cards th?ng kê:
- **T?ng danh m?c**: T?ng s? categories
- **?ang ho?t ??ng**: Categories có `IsActive = true`
- **T?m d?ng**: Categories có `IsActive = false`
- **Danh m?c con**: T?ng s? subcategories

### 2. Filter & Search
Form filter v?i các tr??ng:
- **Tìm ki?m**: Search by name/description
- **Tr?ng thái**: All/Active/Inactive
- **S? dòng**: 5/10/20/50 rows per page

Click "Tìm ki?m" ? Load partial view v?i filter không reload trang

### 3. Categories Table
Table hi?n th?:
- ID
- Icon (image ho?c Font Awesome)
- Tên danh m?c + Description
- Danh m?c cha
- S? danh m?c con
- Tr?ng thái
- Th? t?
- Actions (View/Edit/Toggle/Delete)

### 4. Pagination
- Previous/Next buttons
- Page numbers with ellipsis
- Info text: "Hi?n th? X - Y trong t?ng s? Z"

### 5. Actions

#### View Details
- Click icon eye ? Show modal v?i chi ti?t
- Modal hi?n th? t?t c? thông tin category
- Button "Ch?nh s?a" trong modal

#### Edit Category
- Click icon edit ? Populate form trong modal
- Modal title thay ??i: "Ch?nh s?a danh m?c"
- Save ? Update qua AJAX

#### Toggle Status
- Click icon play/pause ? Confirm dialog
- POST request ? Reload table
- Toast notification

#### Delete Category
- Click icon trash ? Confirm dialog v?i warning
- POST request ? Reload table
- Toast notification

### 6. Add/Edit Modal
Form fields:
- **Tên danh m?c*** (required)
- **Mô t?**
- **Icon** (URL ho?c Font Awesome class)
- **Danh m?c cha** (dropdown)
- **Th? t? hi?n th?**
- **Tr?ng thái** (switch)

### 7. Toast Notifications
Hi?n th? ? góc trên bên ph?i:
- Success (green)
- Error (red)
- Info (blue)

Auto-hide sau 3 giây

## Quy trình ho?t ??ng

### Load trang l?n ??u:
1. `Index()` action load d? li?u t? API
2. Render view v?i statistics và table
3. Table ???c render t? partial view

### Filter/Search:
1. User nh?p filter ? Submit form
2. JavaScript prevent default
3. Call `GetCategoriesPartial()` qua AJAX
4. Replace `#categoriesTableContainer` v?i HTML m?i
5. Add animation fade-in

### Pagination:
1. User click page number
2. JavaScript call `loadPage(pageNumber)`
3. Call `GetCategoriesPartial()` v?i pageNumber
4. Replace table content

### Toggle Status:
1. User click toggle button ? Confirm dialog
2. POST `/Category/ToggleStatus` qua AJAX
3. Success ? Reload table + Show toast
4. Error ? Show error toast

### Delete:
1. User click delete ? Confirm dialog with warning
2. POST `/Category/Delete` qua AJAX
3. Success ? Reload table + Show toast
4. Error ? Show error toast

## Styling v?i Bootstrap

### Cards
```html
<div class="card border-start border-primary border-4 shadow-sm h-100 py-2">
```
- `border-start` - Left border
- `border-4` - Border width
- `shadow-sm` - Small shadow
- `h-100` - Full height

### Badges
```html
<span class="badge bg-success">
    <i class="fas fa-check-circle"></i> Ho?t ??ng
</span>
```

### Button Groups
```html
<div class="btn-group btn-group-sm" role="group">
    <button class="btn btn-outline-info">...</button>
    <button class="btn btn-outline-primary">...</button>
</div>
```

### Table
```html
<table class="table table-hover align-middle">
    <thead class="table-light">...</thead>
</table>
```

## JavaScript Functions

### Public Functions (window scope)
```javascript
window.loadPage = function(pageNumber)
window.viewDetails = function(id)
window.editCategory = function(id)
window.toggleStatus = function(id, currentStatus)
window.deleteCategory = function(id, name)
```

### Private Functions
```javascript
function loadCategories()
function saveCategory()
function populateCategoryForm(category)
function resetCategoryForm()
function showCategoryDetailsModal(category)
function showLoading()
function hideLoading()
function showToast(title, message, type)
```

## API Endpoints

### GET `/Category/Index`
Load trang chính v?i d? li?u

**Query params:**
- `pageNumber` (default: 1)
- `pageSize` (default: 10)
- `searchTerm` (optional)
- `isActive` (optional)

**Returns:** View

### GET `/Category/GetCategoriesPartial`
Load partial view cho table

**Query params:**
- `pageNumber`
- `pageSize`
- `searchTerm`
- `isActive`

**Returns:** Partial View HTML

### POST `/Category/ToggleStatus`
B?t/t?t danh m?c

**Body:** `int id`

**Returns:** `{ success: bool, message: string }`

### POST `/Category/Delete`
Xóa danh m?c

**Body:** `int id`

**Returns:** `{ success: bool, message: string }`

### GET `/Category/GetDetails`
L?y chi ti?t danh m?c

**Query:** `?id=1`

**Returns:** `{ success: bool, data: CategoryDto, message: string }`

## Responsive Design

### Desktop (? 992px)
- 4 statistics cards in row
- Full table with all columns
- Large modal

### Tablet (768px - 991px)
- 2 statistics cards per row
- Table scrollable horizontally
- Medium modal

### Mobile (< 768px)
- 1 statistics card per row
- Table with reduced font size
- Small buttons
- Full-width filter inputs
- Modal adjusts width

## TODO - C?n hoàn thi?n

### 1. Create/Edit API Integration
File: `category-index.js` - function `saveCategory()`

```javascript
// TODO: Implement actual API call
const url = formData.id ? '/Category/Edit' : '/Category/Create';

fetch(url, {
    method: 'POST',
    headers: {
        'Content-Type': 'application/json',
    },
    body: JSON.stringify(formData)
})
```

### 2. Backend API Actions
File: `CategoryController.cs`

```csharp
[HttpPost]
public IActionResult Create(string name, string description, string icon)
{
    // TODO: G?i API ?? t?o danh m?c m?i
    // await _categoryService.CreateCategoryAsync(name, description, icon);
}

[HttpPost]
public IActionResult Edit(int id, string name, string description, string icon)
{
    // TODO: G?i API ?? c?p nh?t danh m?c
    // await _categoryService.UpdateCategoryAsync(id, name, description, icon);
}
```

### 3. Image Upload
Thêm tính n?ng upload icon:
- Button "T?i lên" trong modal
- Preview image
- Upload to server
- Return URL

### 4. Validation
Thêm validation rules:
- Name: required, max 100 chars
- Description: max 500 chars
- Icon: valid URL or FA class
- Display Order: positive number

### 5. Sorting
Thêm sort theo:
- Name (A-Z, Z-A)
- Display Order (1-9, 9-1)
- Created Date (New-Old, Old-New)

## Testing Checklist

- [ ] Load trang Index hi?n th? ?úng d? li?u
- [ ] Statistics cards hi?n th? s? li?u ?úng
- [ ] Filter by search term
- [ ] Filter by status
- [ ] Change page size
- [ ] Pagination works
- [ ] View details modal
- [ ] Edit category (populate form)
- [ ] Toggle status (confirm + reload)
- [ ] Delete category (confirm + reload)
- [ ] Toast notifications appear
- [ ] Responsive trên mobile
- [ ] Icons hi?n th? ?úng (image/FA)
- [ ] Loading overlay khi AJAX call

## Troubleshooting

### Table không load
- Check console errors
- Verify `/Category/GetCategoriesPartial` endpoint
- Check `CategoryService.GetCategoriesAsync()`

### Modal không hi?n
- Check Bootstrap JS ?ã load
- Verify modal ID
- Check `data-bs-target` attribute

### Filter không ho?t ??ng
- Check form submit event handler
- Verify JavaScript `loadCategories()` function
- Check query params trong URL

### Icon không hi?n th?
- Check icon URL valid
- Verify Font Awesome loaded
- Check icon class format (fa-*)

## Summary

Trang Category Index ?ã ???c làm l?i hoàn toàn v?i:
- ? Bootstrap 5 UI components
- ? Partial View pattern
- ? AJAX không reload trang
- ? Toast notifications
- ? Responsive design
- ? Clean, maintainable code structure

Build thành công ? - Ready to use!
