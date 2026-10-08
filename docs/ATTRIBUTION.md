# Asset và attribution

## Model thực sự có trong ZIP

`Assets/3D Laboratory Environment with Appratus/` là nội dung bạn cung cấp. Dùng các prefab có thật: `Beaker`, `Glass_Lab_test_tube`, `table with drawers`, `Spirit_Lamp with water`, `shelf` và model flask cho wrapper hình ảnh. Chưa có metadata đáng tin cậy về tác giả/link/license của gói này trong ZIP, nên không tự gán tên tác giả hoặc giấy phép. Giữ nguyên model/material/texture gốc; kiểm tra quyền phân phối của asset khi chia sẻ repository/source ra ngoài.

Đây **không phải** bằng chứng Chemistry Lab Items Pack đã được import. Không có package đó trong file được gửi, nên bản sửa dùng asset hiện có và primitive cho phần còn thiếu.

Gói mục tiêu trong yêu cầu: [Chemistry Lab Items Pack](https://assetstore.unity.com/packages/3d/environments/chemistry-lab-items-pack-220212). Thông tin 1.0 / Unity 2019.4 / Built-in là thông tin bạn cung cấp, chưa được kiểm tra trực tiếp trong lượt sửa này. Không giả định prefab của gói đó hoặc khả năng tương thích URP sẵn.

Nếu bổ sung gói mục tiêu: đăng nhập Unity Hub/Asset Store cùng tài khoản, thêm asset vào tài khoản theo điều kiện trang store; trong Editor mở Window → Package Management → Package Manager → My Assets (hoặc bộ lọc My Assets theo giao diện thực tế), tìm asset → Download → Import. Kiểm tra package content/demo/material/scale trước. Tạo wrapper của project, rồi thay field model trên Systems → ChemLabBootstrap; không sửa material nguồn để ép pipeline. Nếu cần chuyển shader, làm trên bản material của project sau khi xác định shader/pipeline. Không cần mua thêm gói để chạy bản demo primitive hiện tại.

## Font

Be Vietnam Pro đã có trong ZIP. Giấy phép SIL Open Font License được kèm ở `Assets/Fonts/BeVietnamPro-OFL.txt`, lấy từ [Google Fonts / Be Vietnam Pro](https://github.com/google/fonts/tree/main/ofl/bevietnampro). Liberation Sans/TextMeshPro giữ file license có sẵn.

## Âm thanh và code

Ba WAV Click/Correct/Incorrect được tổng hợp bằng sóng sin cho project, không lấy từ thư viện bên ngoài. Logic/scene seed/UI mới là mã nguồn riêng của ChemLab9.

## Dữ liệu hóa học

Nguyên tử khối trung bình được làm tròn từ bảng giá trị quy ước CIAAW/IUPAC: https://ciaaw.org/atomic-weights.htm và https://iupac.org/what-we-do/periodic-table-of-elements/ . Với nguyên tố có khoảng biến thiên, database dùng một giá trị quy ước để hiển thị, không suy ra đồng vị hoặc neutron.

Kích thước bán kính không có số liệu pm: đây là các thang minh họa do project đặt cho Li–Na–K và Na–Mg–Al. Không trộn các định nghĩa bán kính khác nhau, không gọi mức độ minh họa là số đo thực nghiệm.
