# Category Edit - Testing Checklist

## ✅ Pre-deployment Checklist

### 1. File Structure
- [x] `wwwroot/js/admin/category-edit-simple.js` - Created
- [x] `DOCS/CATEGORY_EDIT_SIMPLE_GUIDE.md` - Created
- [x] `DOCS/CATEGORY_EDIT_README.md` - Created
- [x] `wwwroot/js/admin/test-category-api.js` - Created (for testing)
- [x] `Views/Category/Edit.cshtml` - Updated to use new JS

### 2. Backend Requirements
- [ ] API `/api/admin/v1/categories/{id}` (PUT) - Works
- [ ] API `/api/admin/v1/categories/{id}/attributes` (POST) - Works
- [ ] API `/api/admin/v1/categories/{id}/attributes/{id}` (PATCH) - Works
- [ ] API `/api/admin/v1/categories/{id}/attributes/{id}` (DELETE) - Works
- [ ] API `/api/admin/v1/categories/{id}/attributes/{id}/options/{id}` (PATCH) - Works
- [ ] API `/api/admin/v1/categories/{id}/attributes/{id}/options/{id}` (DELETE) - Works
- [ ] Bearer Token authentication works
- [ ] CORS configured correctly

## 🧪 Manual Testing

### Test 1: Update Category
- [ ] Navigate to Edit page
- [ ] Change category name
- [ ] Change description
- [ ] Change parent category
- [ ] Change display order
- [ ] Toggle is active
- [ ] Click "Cập nhật thông tin cơ bản"
- [ ] Verify success message
- [ ] Verify page reloads with updated data

### Test 2: Create Text Attribute
- [ ] Click "Thêm thuộc tính mới"
- [ ] Enter Name (e.g., "color")
- [ ] Enter Display Name (e.g., "Màu sắc")
- [ ] Select Type: "Text"
- [ ] Enter Placeholder
- [ ] Check "Bắt buộc" if needed
- [ ] Click "Lưu thuộc tính"
- [ ] Verify success message
- [ ] Verify page reloads and shows new attribute

### Test 3: Create Select Attribute with Options
- [ ] Click "Thêm thuộc tính mới"
- [ ] Enter Name (e.g., "brand")
- [ ] Enter Display Name (e.g., "Thương hiệu")
- [ ] Select Type: "Select"
- [ ] Click "Thêm option" multiple times
- [ ] Fill in Value and Display Text for each option
- [ ] Click "Lưu thuộc tính"
- [ ] Verify success message
- [ ] Verify page reloads
- [ ] Verify options are displayed

### Test 4: Update Attribute
- [ ] Find an existing attribute
- [ ] Change Display Name
- [ ] Change Display Order
- [ ] Toggle "Bắt buộc"
- [ ] Change Placeholder
- [ ] Click "Cập nhật thuộc tính này"
- [ ] Verify success message
- [ ] Verify NO page reload
- [ ] Refresh and verify changes persisted

### Test 5: Update Option
- [ ] Find an option in a Select attribute
- [ ] Change Value
- [ ] Change Display Text
- [ ] Toggle Is Active
- [ ] Click "Cập nhật option này"
- [ ] Verify success message
- [ ] Refresh and verify changes persisted

### Test 6: Delete Option
- [ ] Find an option to delete
- [ ] Click delete button (trash icon)
- [ ] Confirm deletion
- [ ] Verify success message
- [ ] Verify page reloads
- [ ] Verify option is removed

### Test 7: Delete Attribute
- [ ] Find an attribute to delete
- [ ] Click "Xóa" button in card header
- [ ] Confirm deletion
- [ ] Verify success message
- [ ] Verify page reloads
- [ ] Verify attribute is removed

### Test 8: Parent Category Restriction
- [ ] Navigate to a parent category (has sub-categories)
- [ ] Verify "Thêm thuộc tính mới" button is NOT shown
- [ ] Verify warning message is displayed

### Test 9: Validation
- [ ] Try to update category with empty name → Should show error
- [ ] Try to create attribute with empty name → Should show error
- [ ] Try to create attribute without selecting type → Should show error
- [ ] Try to update option with empty value → Should show error

### Test 10: Error Handling
- [ ] Disconnect internet → Try to update → Should show error message
- [ ] Use invalid category ID in URL → Should redirect or show error
- [ ] Clear authToken cookie → Try to update → Should get 401 error

## 🔍 Browser Testing

### Chrome
- [ ] All features work
- [ ] Console has no errors
- [ ] Network tab shows correct API calls

### Firefox
- [ ] All features work
- [ ] Console has no errors

### Edge
- [ ] All features work
- [ ] Console has no errors

## 📱 Responsive Testing

### Desktop (1920x1080)
- [ ] Layout looks good
- [ ] All buttons accessible

### Tablet (768x1024)
- [ ] Layout adapts correctly
- [ ] Modal fits screen

### Mobile (375x667)
- [ ] Cards stack vertically
- [ ] Buttons are tappable
- [ ] Modal is usable

## 🚨 Edge Cases

- [ ] Category with no attributes → Empty state shows
- [ ] Attribute with no options → No options message shows
- [ ] Very long category name → Text wraps correctly
- [ ] Very long option list → Scrollable
- [ ] Multiple rapid clicks on save button → Should not duplicate requests
- [ ] Browser back button after save → Should show updated data

## 🔧 Developer Tools Check

### Console
- [ ] No JavaScript errors
- [ ] `categoryId` is logged correctly
- [ ] API calls are logged with request/response

### Network Tab
- [ ] PUT /api/admin/v1/categories/{id} returns 200
- [ ] POST /api/admin/v1/categories/{id}/attributes returns 200/201
- [ ] PATCH requests return 200/204
- [ ] DELETE requests return 200/204
- [ ] Request headers include Authorization: Bearer {token}
- [ ] Request bodies are JSON formatted correctly

### Application Tab
- [ ] authToken cookie exists
- [ ] authToken is valid (not expired)

## 📊 Performance

- [ ] Page loads in < 2 seconds
- [ ] API calls complete in < 1 second
- [ ] No memory leaks (check after multiple updates)
- [ ] Smooth scrolling with many attributes

## 🐛 Known Issues (if any)

Document any issues found:

| Issue | Severity | Status | Notes |
|-------|----------|--------|-------|
| | | | |

## ✍️ Notes

Any additional observations or recommendations:

---

## 🎯 Final Sign-off

- [ ] All critical tests passed
- [ ] No blocker issues
- [ ] Documentation is complete
- [ ] Ready for production

**Tested by**: _______________  
**Date**: _______________  
**Version**: 1.0.0 (Simple Edition)
