# MarketplaceSystem Admin Panel

Trang quản trị cho hệ thống Marketplace được xây dựng với ASP.NET Core MVC.

## 🚀 Tính năng chính

### Dashboard

- **Thống kê tổng quan**: Doanh thu, đơn hàng, người dùng mới
- **Biểu đồ**: Doanh thu theo tháng, phân bố danh mục sản phẩm
- **Đơn hàng gần đây**: Hiển thị danh sách đơn hàng mới nhất

### Quản lý Sản phẩm (`/Product`)

- Xem danh sách sản phẩm với phân trang
- Tìm kiếm và lọc theo danh mục
- Thêm, sửa, xóa sản phẩm
- Hiển thị hình ảnh, giá, số lượng tồn kho

### Quản lý Người dùng (`/User`)

- Danh sách người dùng
- Phân loại theo vai trò (Admin, Người bán, Người mua)
- Khóa/Mở khóa tài khoản
- Xem chi tiết người dùng

### Quản lý Đơn hàng (`/Order`)

- Danh sách đơn hàng với nhiều trạng thái
- Lọc theo trạng thái và ngày
- Thống kê tổng quan đơn hàng
- Xem chi tiết đơn hàng

## 🎨 Giao diện

### Layout

- **Sidebar Navigation**: Menu điều hướng với các module chính
- **Top Navbar**: Thông báo, menu người dùng
- **Responsive**: Tương thích mobile với sidebar toggle
- **Modern Design**: Sử dụng Bootstrap 5 và Font Awesome 6

### Màu sắc

- Primary: `#4e73df`
- Success: `#1cc88a`
- Info: `#36b9cc`
- Warning: `#f6c23e`
- Danger: `#e74a3b`

## 🛠️ Cấu trúc thư mục

```
MarketplaceSystem.Web.UI.Admin/
├── Controllers/
│   ├── HomeController.cs       # Dashboard
│   ├── ProductController.cs    # Quản lý sản phẩm
│   ├── UserController.cs       # Quản lý người dùng
│   └── OrderController.cs      # Quản lý đơn hàng
├── Views/
│   ├── Shared/
│   │   └── _Layout.cshtml      # Layout chính
│   ├── Home/
│   │   └── Index.cshtml        # Dashboard
│   ├── Product/
│   │   └── Index.cshtml        # Danh sách sản phẩm
│   ├── User/
│   │   └── Index.cshtml        # Danh sách người dùng
│   └── Order/
│       └── Index.cshtml        # Danh sách đơn hàng
├── wwwroot/
│   ├── css/
│   │   └── admin.css           # CSS tùy chỉnh cho admin
│   └── js/
│       └── admin.js            # JavaScript cho admin
└── Program.cs                  # Cấu hình ứng dụng
```

## 📦 Cài đặt và Chạy

### Yêu cầu

- .NET 8.0 SDK hoặc cao hơn
- Visual Studio 2022 hoặc VS Code với C# extension

### Các bước chạy

1. **Mở terminal và chuyển đến thư mục project:**

   ```cmd
   cd d:\CNPMNC\MarketplaceSystem\frontend\src\MarketplaceSystem.Web\MarketplaceSystem.Web.UI.Admin
   ```

2. **Build project:**

   ```cmd
   dotnet build
   ```

3. **Chạy ứng dụng:**

   ```cmd
   dotnet run
   ```

4. **Mở trình duyệt và truy cập:**
   - HTTPS: `https://localhost:50818`
   - HTTP: `http://localhost:50819`

### Hoặc chạy trực tiếp với đường dẫn đầy đủ:

```cmd
dotnet run --project "d:\CNPMNC\MarketplaceSystem\frontend\src\MarketplaceSystem.Web\MarketplaceSystem.Web.UI.Admin\MarketplaceSystem.Web.UI.Admin.csproj"
```

## 🎯 Routes chính

| Route                  | Mô tả                       |
| ---------------------- | --------------------------- |
| `/` hoặc `/Home/Index` | Dashboard - Trang chủ admin |
| `/Product/Index`       | Danh sách sản phẩm          |
| `/Product/Create`      | Thêm sản phẩm mới           |
| `/Product/Edit/{id}`   | Sửa sản phẩm                |
| `/User/Index`          | Danh sách người dùng        |
| `/User/Details/{id}`   | Chi tiết người dùng         |
| `/Order/Index`         | Danh sách đơn hàng          |
| `/Order/Details/{id}`  | Chi tiết đơn hàng           |

## 📊 Thư viện sử dụng

- **Bootstrap 5**: Framework CSS
- **Font Awesome 6**: Icons
- **Chart.js**: Biểu đồ và thống kê
- **jQuery**: DOM manipulation
- **ASP.NET Core MVC**: Framework backend

## ✨ Tính năng JavaScript

File `admin.js` cung cấp các tính năng:

- Sidebar toggle cho mobile
- Active menu highlighting
- Confirm delete dialogs
- Table search functionality
- Notification system
- AJAX form submission helper
- Export table to CSV
- Format currency và date

## 🎨 Custom CSS

File `admin.css` bao gồm:

- Responsive sidebar
- Card styling với shadows
- Statistics cards với border colors
- Table hover effects
- Badge và button styling
- Smooth animations
- Custom scrollbar cho sidebar

## 📝 Ghi chú

- Project hiện đang dùng dữ liệu tĩnh (mock data) cho demo
- Cần kết nối với API backend để lấy dữ liệu thực
- Các warning về nullable properties có thể được fix bằng cách thêm `required` modifier hoặc nullable reference types

## 🔧 Tùy chỉnh

### Thay đổi màu sắc

Chỉnh sửa CSS variables trong `admin.css`:

```css
:root {
  --primary-color: #4e73df;
  --success-color: #1cc88a;
  /* ... */
}
```

### Thêm menu mới

Chỉnh sửa sidebar trong `_Layout.cshtml`:

```html
<li class="nav-item">
  <a class="nav-link" asp-controller="YourController" asp-action="Index">
    <i class="fas fa-icon"></i> Menu Item
  </a>
</li>
```

## 📞 Hỗ trợ

Nếu gặp vấn đề, vui lòng:

1. Kiểm tra `.NET SDK` đã cài đặt: `dotnet --info`
2. Kiểm tra port 50818/50819 có bị chiếm dụng không
3. Xem logs trong terminal để biết lỗi cụ thể

## 🚀 Tiếp theo

- [ ] Kết nối với API backend
- [ ] Thêm authentication và authorization
- [ ] Implement CRUD operations thực sự
- [ ] Thêm file upload cho ảnh sản phẩm
- [ ] Thêm export Excel/PDF cho báo cáo
- [ ] Implement real-time notifications với SignalR
- [ ] Thêm dark mode toggle
