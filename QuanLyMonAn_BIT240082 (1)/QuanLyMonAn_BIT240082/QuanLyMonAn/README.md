# Quản lý món ăn - BIT240082

ASP.NET Core MVC (.NET 8), Entity Framework Core và PostgreSQL.

## Mở và chạy ngay

1. Mở `QuanLyMonAn.sln` bằng Visual Studio có workload ASP.NET và .NET 8 SDK.
2. Bật PostgreSQL. Kiểm tra `QuanLyMonAn/appsettings.json`: Host/Port/Username/Password phải đúng với máy bạn, Database là `BIT240082_MID`.
3. Nhấn F5 hoặc Ctrl+F5. Khi chưa có database, ứng dụng tự tạo database, ba bảng và dữ liệu mẫu (ba loại, năm món, sáu ảnh). Tài khoản PostgreSQL dùng để kết nối cần có quyền tạo database.
4. Database đã có bảng được giữ lại. Khi khởi động, các bản ghi mẫu còn đúng Id và tên tiếng Anh gốc được đổi tên/mô tả sang tiếng Việt; giá, trạng thái, thời gian và ảnh được giữ nguyên. Món người dùng đã đổi tên hoặc tự tạo không bị cập nhật.

Bản này đã xử lý lỗi `database "BIT240082_MID" does not exist` bằng bước khởi tạo trước khi tải trang danh sách. Nếu database có sẵn nhưng dùng schema khác hoặc chỉ có một phần bảng, cần hoàn thiện schema trước khi chạy.

## Tạo database thủ công bằng pgAdmin (nếu muốn)

Chỉ chọn cách này nếu chưa dùng bước tự khởi tạo ở trên:

1. Mở Query Tool của database `postgres` và chạy riêng file `Database/create_database.sql`:

```sql
CREATE DATABASE "BIT240082_MID";
```

2. Refresh Databases; chọn `BIT240082_MID`, mở Query Tool của chính database này.
3. Chạy file `Database/schema.sql` để tạo bảng, thêm dữ liệu mẫu và đặt sequence sau các ID mẫu.
4. Mở `.sln` rồi chạy F5. Không cần chạy lại script nếu các bảng đã có.

Không chạy CREATE DATABASE cùng cả script tạo bảng trong một lần. Script schema không xóa dữ liệu cũ; nếu bảng đã tồn tại thì script sẽ báo lỗi và transaction không áp dụng thêm dữ liệu.

## Ngôn ngữ và giá tiền

- Mặc định English. Nút English/Tiếng Việt dịch menu, nút, bộ lọc, trạng thái, phân loại thời gian và thông báo/validation.
- Cookie lưu ngôn ngữ; liên kết đổi ngôn ngữ giữ đường dẫn và điều kiện lọc hiện tại.
- Giá luôn dùng VND; ví dụ `100.000 ₫` ở cả hai ngôn ngữ. Tên và mô tả món do người dùng nhập được giữ nguyên.

## MSSV

Database là `BIT240082_MID`. Ba bảng `BIT240082_DishCategories`, `BIT240082_Dishes`, `BIT240082_DishImages`; tất cả cột SQL có tiền tố `BIT240082_`. Các model là `DishCategory_BIT240082`, `Dish_BIT240082`, `DishImage_BIT240082`.

Việc đổi tên trong project không tự chuyển dữ liệu từ một database có tên/prefix khác.

## EF Core Migrations

Project hiện chưa chứa migration: bước chạy đầu dùng EnsureCreated để tạo schema và các bản ghi HasData. Không chạy InitialCreate/Update-Database lên database đã được tạo theo cách này. Nếu chọn dùng migrations từ đầu, scaffold migration trước khi chạy app và áp dụng vào database mới/chưa có bảng. Khi project có migration, bước khởi tạo chuyển sang MigrateAsync.

## Kiểm tra nhanh

1. Mở trang danh sách, đổi English ↔ Tiếng Việt; kiểm tra giá vẫn là VND và điều kiện lọc còn nguyên.
2. Tạo hoặc sửa món; thử tên trống, giá/thời gian bằng 0, category sai và tên trùng trong cùng loại.
3. Thêm món mới sau các món seed để kiểm tra Id tự tăng; thêm nhiều ảnh và đổi ảnh đại diện.
4. Thử xóa loại đang có món; truy cập Id không tồn tại; lọc Available và kiểm tra món ngừng bán được ẩn.
5. Trong pgAdmin, kiểm tra tên database, ba bảng và mọi cột theo MSSV.

## Kiểm thử trong môi trường tạo gói

Đã kiểm tra cấu trúc mã, resource dịch, script SQL và ZIP. Chưa chạy build hoặc thử PostgreSQL thực tế vì môi trường tạo gói không có .NET SDK/PostgreSQL.

## Menu mẫu bằng tiếng Việt

- Loại món: Khai vị, Món chính, Tráng miệng.
- Món ăn: Chả giò, Salad bò nướng, Cơm sườn nướng, Phở bò, Chè ba màu.
- Tên và mô tả mẫu trong HasData và schema.sql đã dùng tiếng Việt.
- Database mới nhận dữ liệu mẫu tiếng Việt ngay khi tạo. Database đang có các món mẫu tiếng Anh được cập nhật khi chạy project mới.
- Có thể chạy riêng `Database/update_sample_menu_vi.sql` trong pgAdmin trên `BIT240082_MID` để cập nhật.
- Nếu tên tiếng Việt đích đã tồn tại trong cùng loại, bản ghi mẫu tương ứng được bỏ qua để tránh vi phạm ràng buộc tên món duy nhất; hãy đổi tên trùng bằng giao diện Edit trước khi chạy lại.

## Thêm ảnh ngay khi tạo món

Trang Tạo món có ô **Đường dẫn ảnh (không bắt buộc)**. Dán URL HTTP/HTTPS của ảnh và bấm Lưu: món và ảnh được lưu cùng nhau; ảnh đầu tiên được đặt làm ảnh đại diện. Để trống vẫn tạo món bình thường. Có thể thêm ảnh khác và đổi ảnh đại diện ở trang Chi tiết.

URL không hợp lệ hoặc dài quá 500 ký tự sẽ được báo lỗi ngay trên form. Nhãn, hướng dẫn và thông báo hỗ trợ English/Tiếng Việt.
