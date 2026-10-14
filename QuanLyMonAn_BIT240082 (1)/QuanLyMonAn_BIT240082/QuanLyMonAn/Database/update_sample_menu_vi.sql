-- Chạy trong database BIT240082_MID nếu muốn cập nhật riêng bằng pgAdmin.
-- Đổi tên/mô tả của các món và loại món mẫu còn mang tên tiếng Anh gốc.
-- Các trường giá, trạng thái, thời gian, Id, liên kết và ảnh không bị cập nhật.
BEGIN;

WITH translations (id, old_name, new_name, new_description) AS (
    VALUES
        (1, 'Appetizer', 'Khai vị', 'Các món khai vị nhẹ'),
        (2, 'Main Course', 'Món chính', 'Các món chính trong thực đơn'),
        (3, 'Dessert', 'Tráng miệng', 'Các món ngọt dùng sau bữa ăn')
)
UPDATE "BIT240082_DishCategories" AS c
SET "BIT240082_Name" = t.new_name,
    "BIT240082_Description" = t.new_description
FROM translations AS t
WHERE c."BIT240082_Id" = t.id
  AND c."BIT240082_Name" = t.old_name;

WITH translations (id, old_name, new_name, new_description) AS (
    VALUES
        (1, 'Spring Rolls', 'Chả giò', 'Chả giò chiên giòn'),
        (2, 'Grilled Beef Salad', 'Salad bò nướng', 'Salad rau tươi kết hợp bò nướng'),
        (3, 'Grilled Pork Chop Rice', 'Cơm sườn nướng', 'Cơm ăn kèm sườn heo nướng'),
        (4, 'Beef Noodle Soup', 'Phở bò', 'Phở bò truyền thống'),
        (5, 'Che Ba Mau', 'Chè ba màu', 'Món chè ngọt gồm ba màu')
)
UPDATE "BIT240082_Dishes" AS d
SET "BIT240082_Name" = t.new_name,
    "BIT240082_Description" = t.new_description
FROM translations AS t
WHERE d."BIT240082_Id" = t.id
  AND d."BIT240082_Name" = t.old_name
  AND NOT EXISTS (
      SELECT 1
      FROM "BIT240082_Dishes" AS other
      WHERE other."BIT240082_DishCategoryId" = d."BIT240082_DishCategoryId"
        AND other."BIT240082_Name" = t.new_name
        AND other."BIT240082_Id" <> d."BIT240082_Id"
  );

COMMIT;
