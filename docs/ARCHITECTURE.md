# Kiến trúc và dữ liệu

## Phạm vi

Một phòng, năm station. UI hỗ trợ hoạt động; khí/nhiệt/đổ mẫu/phản hồi đều xuất phát từ thao tác lên vật thể 3D. Scene menu và phòng riêng; chọn bài không tạo thêm phòng. Không thêm Jump/Crouch, parser công thức, CO2 dư, hoạt động hóa học C hoặc mạng.

## Luồng

PlayerInteractor raycast → Interactable/DraggableObject → DropZone hợp lệ → LessonManager → SimulationState → ChemicalContainer/indicator/equipment visual → câu hỏi → ProgressLedger + PlayerPrefs.

`GameManager` quản lý chế độ, Pause, scene state và tiến độ; `StationController` vào/ra/reset bàn; các lesson manager quản lý nhiệm vụ riêng. `LabUI` dựng giao diện chung và hiển thị trạng thái. `ChemLabBootstrap` là composition root: nó chỉ lắp ráp/reference, không suy luận phản ứng hoặc chấm điểm.

## Dữ liệu

`ChemicalDatabase.asset` là dữ liệu runtime chung. `ElementRecord` là record serialize trong database, giúp 20 nguyên tố có cùng nguồn cho Bài 30 và 31. Các SubstanceData/ReactionData/LessonData là ScriptableObject riêng. Không có neutron; atomicMass là nguyên tử khối trung bình, không phải A của đồng vị.

`ElementCatalog` là seed các nguyên tố để tạo/kiểm tra database. Bán kính minh họa được lưu vào `ElementRecord.relativeRadius` và `radiusNote`; visualizer đọc database. Các ô trống ghi rõ chưa triển khai, không giả dữ liệu 118 nguyên tố.

`ReactionSystem` đọc phương trình/giải thích từ database theo kết quả logic. `SimulationState` thực hiện bốn rule cố định, cần đúng chất/điều kiện. Vì phạm vi MVP nhỏ nên chưa viết engine phản ứng tổng quát; thay nội dung ReactionData không tự tạo rule hóa học mới. `IndicatorController` đọc môi trường logic, không đọc màu material.

## State của thí nghiệm

- Lượng chất không âm, capacity không vượt 12 liều mô phỏng. NaN/Infinity/giá trị âm bị từ chối.
- CaOH2 ban đầu là nước vôi trong; sau hydration có thể hơi đục để minh họa tính ít tan.
- Nguồn khí cần nối đúng cốc, chạy tối đa một liều mỗi cốc; tắt trước khi chuyển ống hoặc kiểm tra quỳ. VFX bọt không quyết định có phản ứng.
- Thìa CaO và lọ chỉ thị dùng coroutine nghiêng/rót; cập nhật chất khi thao tác được thực hiện. Các thao tác đồng thời/lặp khi đang rót bị chặn.
- Heating cần CuOH2 trong zone. Tắt sớm giữ HeatSeconds; đủ thời gian tạo CuO một lần. Hình chất rắn đổi dần để minh họa, hóa học được xác nhận tại điểm hoàn tất.
- Rời bàn dừng khí/nhiệt; Pause dùng timeScale = 0, chặn tương tác và pause audio. Camera/cursor được trả về đúng chế độ khi thoát.
- Retry hủy coroutine dụng cụ trước khi reset dữ liệu/reference thiết bị. Không xóa completed ledger để điểm không tăng lặp.

## Model và pipeline

Package upload đã có material URP; giữ URP và tạo material của project. Wrapper vẫn tham chiếu mesh/texture/material của model được gửi. Model nguồn không có script gameplay. Factory fit bounds trong instance để chuẩn hóa kích thước và bỏ collider hình ảnh trước khi thêm collider logic.

Chọn Beaker/TestTube không có `with water`/`water` cho hai dụng cụ cần chất chứa động. Chất lỏng trong đèn cồn là phần model trang trí, không phải cốc phản ứng. Không sửa trực tiếp prefab/material gốc.

## Lưu dữ liệu

Không DontDestroyOnLoad cả phòng. Chuyển scene tạo lại room/state mẫu và tải progress/settings từ PlayerPrefs. Tài khoản online không cần thiết; không lưu lời giải vào server. Xóa tiến độ qua Options, không xóa cả PlayerPrefs của ứng dụng khác.
