/**
 * Test Script for Category Edit APIs
 * Copy-paste vào Browser Console để test
 */

// ==================== CONFIGURATION ====================
const BASE_URL = 'https://localhost:7079/api/admin/v1';
const TEST_CATEGORY_ID = 234234; // Thay bằng ID thực tế
const TEST_ATTRIBUTE_ID = 234; // Thay bằng ID thực tế
const TEST_OPTION_ID = 234324; // Thay bằng ID thực tế

// Get token from cookie
function getToken() {
    const value = `; ${document.cookie}`;
    const parts = value.split(`; authToken=`);
    if (parts.length === 2) return parts.pop().split(';').shift();
    return null;
}

const TOKEN = getToken();

// Helper function
async function testApi(name, method, url, body = null) {
    console.log(`\n========== TEST: ${name} ==========`);
    console.log(`${method} ${url}`);
    if (body) console.log('Body:', body);
    
    try {
        const options = {
            method: method,
            headers: {
                'Content-Type': 'application/json',
                'Authorization': `Bearer ${TOKEN}`
            }
        };
        
        if (body && method !== 'GET') {
            options.body = JSON.stringify(body);
        }
        
        const response = await fetch(url, options);
        const data = await response.json();
        
        console.log('Status:', response.status, response.ok ? '✅' : '❌');
        console.log('Response:', data);
        
        return { success: response.ok, data };
    } catch (error) {
        console.error('Error:', error);
        return { success: false, error };
    }
}

// ==================== TEST FUNCTIONS ====================

async function test1_UpdateCategory() {
    const url = `${BASE_URL}/categories/${TEST_CATEGORY_ID}`;
    const body = {
        name: "Test Category Updated",
        description: "Updated description",
        parentCategoryId: null,
        displayOrder: 1,
        isActive: true
    };
    
    return await testApi('1. Update Category', 'PUT', url, body);
}

async function test2_CreateAttribute() {
    const url = `${BASE_URL}/categories/${TEST_CATEGORY_ID}/attributes`;
    const body = {
        name: "test_attr",
        displayName: "Test Attribute",
        attributeType: "Text",
        isRequired: false,
        displayOrder: 1,
        placeholder: "Enter test value"
    };
    
    return await testApi('2. Create Attribute', 'POST', url, body);
}

async function test3_CreateAttributeWithOptions() {
    const url = `${BASE_URL}/categories/${TEST_CATEGORY_ID}/attributes`;
    const body = {
        name: "test_select",
        displayName: "Test Select",
        attributeType: "Select",
        isRequired: true,
        displayOrder: 2,
        placeholder: "Choose option",
        attributeOptions: [
            { value: "opt1", displayText: "Option 1" },
            { value: "opt2", displayText: "Option 2" }
        ]
    };
    
    return await testApi('3. Create Attribute with Options', 'POST', url, body);
}

async function test4_UpdateAttribute() {
    const url = `${BASE_URL}/categories/${TEST_CATEGORY_ID}/attributes/${TEST_ATTRIBUTE_ID}`;
    const body = {
        name: "test_attr",
        displayName: "Test Attribute Updated",
        isRequired: true,
        displayOrder: 5,
        placeholder: "Updated placeholder"
    };
    
    return await testApi('4. Update Attribute', 'PATCH', url, body);
}

async function test5_UpdateOption() {
    const url = `${BASE_URL}/categories/${TEST_CATEGORY_ID}/attributes/${TEST_ATTRIBUTE_ID}/options/${TEST_OPTION_ID}`;
    const body = {
        id: TEST_OPTION_ID,
        value: "updated_value",
        displayText: "Updated Option",
        isActive: true,
        categoryAttributeId: TEST_ATTRIBUTE_ID
    };
    
    return await testApi('5. Update Option', 'PATCH', url, body);
}

async function test6_DeleteOption() {
    const url = `${BASE_URL}/categories/${TEST_CATEGORY_ID}/attributes/${TEST_ATTRIBUTE_ID}/options/${TEST_OPTION_ID}`;
    
    return await testApi('6. Delete Option', 'DELETE', url);
}

async function test7_DeleteAttribute() {
    const url = `${BASE_URL}/categories/${TEST_CATEGORY_ID}/attributes/${TEST_ATTRIBUTE_ID}`;
    
    return await testApi('7. Delete Attribute', 'DELETE', url);
}

// ==================== RUN ALL TESTS ====================

async function runAllTests() {
    console.log('🚀 Starting API Tests...');
    console.log('Token:', TOKEN ? '✅ Found' : '❌ Not Found');
    
    if (!TOKEN) {
        console.error('❌ No auth token found! Please login first.');
        return;
    }
    
    const results = [];
    
    // Run tests sequentially
    results.push(await test1_UpdateCategory());
    await new Promise(r => setTimeout(r, 500)); // Wait 500ms
    
    results.push(await test2_CreateAttribute());
    await new Promise(r => setTimeout(r, 500));
    
    results.push(await test3_CreateAttributeWithOptions());
    await new Promise(r => setTimeout(r, 500));
    
    results.push(await test4_UpdateAttribute());
    await new Promise(r => setTimeout(r, 500));
    
    results.push(await test5_UpdateOption());
    await new Promise(r => setTimeout(r, 500));
    
    // Uncomment to test delete operations
    // results.push(await test6_DeleteOption());
    // await new Promise(r => setTimeout(r, 500));
    
    // results.push(await test7_DeleteAttribute());
    
    // Summary
    console.log('\n========== TEST SUMMARY ==========');
    const passed = results.filter(r => r.success).length;
    const total = results.length;
    console.log(`Passed: ${passed}/${total}`);
    console.log(passed === total ? '✅ All tests passed!' : '❌ Some tests failed');
}

// ==================== QUICK TESTS ====================

// Test individual endpoint
async function quickTest(testName) {
    switch(testName) {
        case 'category': return await test1_UpdateCategory();
        case 'create-attr': return await test2_CreateAttribute();
        case 'create-select': return await test3_CreateAttributeWithOptions();
        case 'update-attr': return await test4_UpdateAttribute();
        case 'update-option': return await test5_UpdateOption();
        case 'delete-option': return await test6_DeleteOption();
        case 'delete-attr': return await test7_DeleteAttribute();
        default: 
            console.log('Available tests: category, create-attr, create-select, update-attr, update-option, delete-option, delete-attr');
    }
}

// ==================== INSTRUCTIONS ====================
console.log(`
📝 HƯỚNG DẪN SỬ DỤNG:

1. Thay đổi TEST_CATEGORY_ID, TEST_ATTRIBUTE_ID, TEST_OPTION_ID ở đầu file
2. Chạy tất cả tests:
   runAllTests()

3. Chạy từng test riêng lẻ:
   quickTest('category')
   quickTest('create-attr')
   quickTest('create-select')
   quickTest('update-attr')
   quickTest('update-option')
   quickTest('delete-option')
   quickTest('delete-attr')

4. Chạy từng function:
   test1_UpdateCategory()
   test2_CreateAttribute()
   ...
`);
