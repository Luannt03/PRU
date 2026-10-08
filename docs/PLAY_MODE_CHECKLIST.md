# Kiểm tra trong Unity 6000.6.0f1

## Bước đầu: import và compile

1. Unity Hub → Projects → Add → Add project from disk → chọn thư mục ChemLab9.
2. Đợi Unity hoàn thành import/package restore. Window → General → Console, bỏ Collapse nếu cần; không bỏ qua lỗi đỏ.
3. ChemLab9 → Validate Project. Kết quả dự kiến: thông báo dữ liệu/scene/pipeline hợp lệ.
4. ChemLab9 → Open Main Menu → Play → Bắt đầu.
5. Trong Play, Hierarchy phải có một Player/Camera và đúng năm child station dưới Stations. Dụng cụ và UI được tạo khi Play.

Nếu compile báo assembly không tìm thấy: Window → Package Management → Package Manager, chờ UGUI/TMP/URP và Test Framework resolve theo manifest đã có. Không xóa lockfile hay tự nâng package. Nếu báo thiếu Database/Font/Material: chọn Systems, đối chiếu mapping ở PREFABS.md; đây là field bắt buộc, không tiếp tục kiểm thử phản ứng khi mất reference.

## Đi lại và vào/ra bàn

- [ ] WASD đi trên sàn, không xuyên bàn/tường; đi chéo không nhanh hơn bình thường.
- [ ] Chuột xoay ngang và dọc; giới hạn pitch, không lật camera.
- [ ] Nhìn vào bàn ở gần, E hiển thị panel và mở con trỏ; camera/movement dừng.
- [ ] Click Bắt đầu, kéo dụng cụ đúng mặt bàn; vùng đúng xanh, vùng sai trả lại vị trí trước kéo.
- [ ] Click UI không tác động vật phía sau. Khi Pause, các nút bài học phía sau không thao tác được.
- [ ] Esc rời bàn; con trỏ khóa, mouse look/movement trở lại; không có player/camera thứ hai.

Nếu không nhận E: click vào Game view; kiểm tra project input là Input Manager (Old). Nếu raycast không đúng: chọn root collider, component StationController/Interactable ở parent; model hình ảnh ở Ignore Raycast, collider logic ở Default. Không đổi sang Rigidbody player hay Input System giữa chừng.

## Bài 1 — CO2

- [ ] Bắt đầu, kéo quỳ vào nước trước dẫn khí: quỳ tím, không cho hoàn thành.
- [ ] Click nguồn khi ống chưa nối: thông báo cần nối ống; không tăng liều.
- [ ] Kéo tip vào cốc nước, bật nguồn: có bọt trong cốc, liều tăng và dừng tối đa 100%.
- [ ] Quỳ sau đủ CO2 và dừng khí: giấy đỏ; **nước không đỏ**.
- [ ] Quỳ có hoạt ảnh nâng lên miệng cốc, nhúng xuống nước, đổi màu khi nhúng rồi nhấc ra giữa phía trước bàn. Không đổi màu ngay lúc thả.
- [ ] Pause giữa hoạt ảnh quỳ: vị trí giữ nguyên; Resume tiếp tục. Thử lại giữa hoạt ảnh không có callback cũ làm đỏ mẫu mới; kích thước quỳ về ban đầu.
- [ ] Nhãn dụng cụ Bài 1/2/8 nhỏ hơn, tên ngắn; hướng dẫn thao tác đầy đủ vẫn có ở gợi ý dưới màn hình và panel.
- [ ] Chuyển tip sang nước vôi, bật khí: đục tăng dần, kết tủa trắng; không dẫn vượt một liều.
- [ ] Đầu ống đã nằm trong cốc vẫn chọn/kéo được sau khi tắt khí; vùng nhận không chặn việc chọn dụng cụ.
- [ ] Trả lời sai: có phản hồi; chọn đúng cả hai câu: +100 lần đầu.
- [ ] Thử lại: nước trong, quỳ tím, lượng khí = 0, dụng cụ về home, VFX dừng; luyện lại không cộng điểm nữa.

## Bài 2 — CaO

- [ ] Chỉ thị vào nước trước CaO: không hồng.
- [ ] Kéo thìa CaO vào cốc: có nghiêng/rót, mẫu giảm, nhiệt kế tăng từ 25 tới 50 °C theo mô hình.
- [ ] Lặp CaO hoặc kéo chỉ thị lúc đang phản ứng: bị chặn; không tạo thêm chất/điểm.
- [ ] Đợi phản ứng mô phỏng xong, thêm chỉ thị: hỗn hợp hồng và có giải thích bazơ/ít tan/tỏa nhiệt.
- [ ] Thử lại ngay lúc đang rót: coroutine dừng, sample hiện lại, nhiệt độ/màu/contents reset; không có callback cũ làm hồng mẫu mới.
- [ ] Tắt/đổi tùy chọn ShowSteam trong Play (Lesson02Manager): logic phản ứng/điểm vẫn đúng, vì không dựa vào VFX.

Trường hợp thiếu nước/sai oxide được kiểm tra bằng EditMode suite; gameplay giới hạn dụng cụ để không tạo thí nghiệm tùy ý thiếu an toàn/ngoài phạm vi. Không cần sửa asset database để làm bài đúng.

## Bài 8 — Cu(OH)2

- [ ] Bật nhiệt trước đặt ống: bị chặn.
- [ ] Đặt đúng kẹp rồi bật: timer tăng, chỉ **chất rắn** xanh dần đen, không đổi thủy tinh thành đen.
- [ ] Sau khi thả đúng kẹp, đáy ống nằm ngay trên vùng lửa của đèn cồn ở giữa bàn, ống nghiêng theo kẹp; không đứng xa bên cạnh đèn.
- [ ] Tắt khoảng 40%: thời gian giữ nguyên; bật lại tiếp tục.
- [ ] Đủ thời gian: CuO đen; phải tắt thiết bị trước khi mở câu hỏi.
- [ ] Tắt thiết bị không làm CuO xanh trở lại. Thử lại tạo mẫu Cu(OH)2 mới, timer = 0.
- [ ] Không thể kéo mẫu ra khi thiết bị đang bật.

## Bài 30 — nguyên tố

- [ ] Bảng có đúng vị trí nhóm/chu kỳ cho H…Ca; chưa có dữ liệu các ô khác.
- [ ] Bắt đầu, click nguyên tố khác: được xem thông tin, không tự tính là tìm Na.
- [ ] Na: Z=11, p=e=11, lớp 2/8/1; Cl: Z=17, lớp 2/8/7; Ca: Z=20, lớp 2/8/8/2, chu kỳ 4.
- [ ] Ô thông tin riêng phía trên panel luôn hiện Z, số proton/electron và phân bố electron khi chọn ô; cuộn phần nhiệm vụ bên dưới không làm thông tin này biến mất. Thử lại xóa thông tin lựa chọn cũ.
- [ ] Mô hình nguyên tử không đè lên chú giải/phạm vi dưới bảng; chữ trên ô nguyên tố giữ kích thước cũ.
- [ ] Mô hình có đúng số electron mỗi lớp; click nhanh giữa các ô không giữ lại mô hình cũ.
- [ ] Có nhãn đây là mô hình minh họa, không có dữ liệu neutron giả.
- [ ] Click panel không chọn ô phía sau; Esc trả lại điều khiển.

## Bài 31 — xu hướng

- [ ] Chế độ A: kích thước Li < Na < K, Na > Mg > Al; co/giãn khi đổi chế độ.
- [ ] Sphere lớn hơn bản cũ; mỗi tên/note ở ô nền riêng phía dưới sphere. Chữ và sphere nằm trong thẻ, không đè nhau. Mọi nhãn xu hướng/giải thích ở trong ô nền màu, kiểm tra cả A và B.
- [ ] Chọn K, Na, Li: phản hồi sai, cho chọn lại; click lặp cùng nguyên tố không được tính là đủ ba.
- [ ] Chọn Li, Na, K: nhiệm vụ đạt. Chuyển B quan sát mũi tên/thanh mức độ.
- [ ] Nhãn luôn là minh họa, không có pm; giải thích khí hiếm riêng.
- [ ] Trả lời đúng chiều giảm bán kính trong chu kỳ và tăng tính kim loại xuống nhóm.
- [ ] Đổi chế độ/Thử lại không làm kết quả nhiệm vụ trước đó rơi vào bàn khác.

## Pause, tiến độ và build

- [ ] Pause khi đang nung: timer không tăng; Resume tiếp tục.
- [ ] Pause lúc đang rót: pose/luồng/VFX không tiến triển; Resume tiếp tục.
- [ ] Hoàn thành 5 bàn theo thứ tự tùy ý: 5/5, 500 điểm; làm lại không vượt 500.
- [ ] Stop Play rồi Play: tiến độ/settings còn, mẫu thí nghiệm tạo mới.
- [ ] Tùy chọn → Xóa tiến độ: trở về 0/5, 0 điểm; có thể Thử lại và hoàn thành lại.
- [ ] Menu chọn bài đặt người chơi gần đúng bàn trong ChemistryLab.
- [ ] ChemLab9 → Build Windows x64: Console không có compile/missing script/null reference.
- [ ] Chạy EXE ngoài Editor, offline: kiểm tra vào phòng, một bài hóa học, hai bàn nguyên tố, thoát và mở lại.

## Material/UI lỗi

Material hồng: kiểm tra Project Settings → Graphics → Default Render Pipeline và Quality → Render Pipeline Asset đều là PC_RPAsset. Bản upload đã dùng shader URP; không chuyển Built-in hay chạy converter toàn bộ tài nguyên chỉ để thử. Model mới chưa tương thích cần material bản sao của project; xem shader thực tế trước.

Thiếu dấu: bootstrap cần đúng BeVietnamPro-Regular.ttf. Font atlas dynamic 1024 và multi-atlas; UI dùng công thức ASCII. Không thay font bằng LiberationSans mặc định nếu thiếu glyph tiếng Việt.

Không nhìn thấy liquid/precipitate: kiểm tra child renderer riêng trong Play, Initial và State. CaCO3 tăng dần nên lúc bắt đầu rất ít; CuOH2 ở phần solid, không nằm trên material thủy tinh. Không tạo thêm lớp nước vào model đã chứa nước trang trí.

Checklist này là bước nghiệm thu còn cần chạy; các mục chưa được đánh dấu thay bạn.
