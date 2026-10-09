# ChemLab9Amounts — đủ năm bài, sửa Bài 1 và Bài 2

Project Unity mới, tách khỏi `ChemLab9` và `SwimDemo`. Giữ một phòng lab với đủ năm bàn **Bài 1, 2, 8, 30, 31**; thêm bảng kết quả Bài 1, chọn khối lượng CaO và sửa hoạt ảnh đổ ở Bài 2. Tiến độ lưu riêng để có thể demo từ 0/5 mà không ghi đè tiến độ project cũ.

## Mở đúng project

1. Giải nén `ChemLab9Amounts_Unity6000.6.zip`.
2. **Unity Hub → Projects → Add → Add project from disk** → chọn thư mục **ChemLab9Amounts** chứa `Assets`, `Packages`, `ProjectSettings`. Không chọn thư mục cha `PRU`, project ChemLab9 cũ hoặc project SwimDemo.
3. Mở bằng **Unity 6.6 / 6000.6.0f1**. Đợi package và shader import xong; lần đầu cần mạng để lấy package Unity. Bản Windows sau build chơi offline.
4. MainMenu được tự mở khi Editor đang ở Untitled chưa sửa. Nếu chưa mở, chọn **ChemLab9Amounts → Open Main Menu** trên thanh menu, hoặc double-click `Assets/ChemLab9/Scenes/MainMenu.unity`.
5. Chọn **ChemLab9Amounts → Validate Project**, kiểm tra Console, nhấn **Play → Bắt đầu**. Hoặc chọn **Bài học → Bài 1/Bài 2** để được đưa tới gần bàn trong cùng phòng.
6. Click tab Game, đi lại bằng WASD, nhìn vào bàn và nhấn E. Nhấn **Bắt đầu** ở panel rồi thao tác dụng cụ 3D.

Project giữ **URP 17.7.0** và những model/material đã có trong bản ChemLab9. Điều khiển dùng **CharacterController + Input Manager (Old)**; không tự đổi Active Input Handling sang New. Package Input System và asset actions không dùng được bỏ khỏi bản mới để tránh popup yêu cầu bật backend khác. Camera, dụng cụ và UI được bootstrap tạo lúc Play; trước Play, scene có các root và reference cấu hình sẵn.

## Bài 1 — kết tủa 10 giây và bảng kết quả

1. Nhấn Bắt đầu ở bàn Bài 1.
2. Kéo đầu ống vào **cốc nước**, click nguồn CO2, đợi nguồn tự dừng. Kéo quỳ vào nước để xem hoạt ảnh nhúng và đưa giấy ra phía trước; giấy hóa đỏ, nước vẫn không màu.
3. Kéo đầu ống sang **cốc nước vôi trong**, click nguồn CO2. Độ đục và kết tủa trắng tăng dần trong **10 giây dẫn khí liên tục**.
4. Lúc chưa đủ liều, panel ghi đang đợi; chưa hiện slider kết quả. Tắt nguồn hoặc Pause sẽ dừng tiến trình, bật lại tiếp tục phần còn lại.
5. Khi đủ liều, nguồn tự dừng. Bảng bên phải tự hiện **phương trình, hiện tượng, khối lượng CaCO3 và slider kết quả chỉ đọc**.
6. Chọn Kết quả / Câu hỏi, trả lời để hoàn thành bàn. Thử lại tạo mẫu mới và ẩn kết quả cũ.

Phép tính minh họa cố định: **0,044 g CO2 + 0,074 g Ca(OH)2 → 0,100 g CaCO3 + 0,018 g H2O**. Nước vôi là dung dịch ban đầu trong suốt, có thể xem là 0,074 g Ca(OH)2 trong khoảng 100 mL nước. MVP giới hạn lượng CO2, không dẫn dư; không áp dụng công thức tăng kết tủa mãi khi CO2 dư. Thời gian 10 giây phục vụ gameplay, không phải tốc độ thực nghiệm.

## Bài 2 — nhập khối lượng và đổ đúng miệng cốc

1. Vào bàn Bài 2. Trong ô **Khối lượng CaO (g)**, nhập lượng từ **0,10 đến 5,00 g**, hoặc kéo slider chọn lượng. Cả `1,25` và `1.25` đều được chấp nhận; hai control cập nhật cùng một giá trị.
2. Mẫu có **50,0 g nước** ban đầu. Nhấn Bắt đầu rồi kéo **thìa CaO vào vùng nhận ở cốc**.
3. Thìa được đưa lên trên miệng cốc, nghiêng và đổ từ mép lòng thìa. Mẫu CaO trên thìa nhỏ dần rồi biến mất. Liều chọn trước được khóa ngay khi bắt đầu đổ; không đổi khối lượng giữa hoạt ảnh.
4. Chất chỉ được thêm vào cốc khi lượt đổ hoàn tất. Sau đó phản ứng/nhiệt độ minh họa chạy khoảng 3 giây. Khi hoàn tất, panel tự hiện **CaO phản ứng, nước tiêu thụ, Ca(OH)2 tạo thành, phương trình và slider 100%**.
5. Kéo **lọ phenolphthalein vào cùng cốc**. Nắp được ẩn khi mở, lọ nghiêng để miệng lọ nằm trên tâm cốc; dòng chỉ thị đi từ miệng lọ xuống chất lỏng. Lọ dựng lại, về vị trí ban đầu và đóng nắp. Hỗn hợp hóa hồng vì có môi trường bazơ.
6. Mở Kết quả / Câu hỏi và trả lời để hoàn thành. Nút Thử lại tạo mẫu mới, cho phép đổi lượng; giữ lại số đang chọn để bạn dễ so sánh nhiều lượt.

Ví dụ kết quả lý thuyết:

| CaO chọn | CaO phản ứng | H2O tiêu thụ | Ca(OH)2 tạo thành |
|---|---:|---:|---:|
| 1,00 g | 1,000 g | 0,321 g | 1,321 g |
| 2,00 g | 2,000 g | 0,643 g | 2,643 g |
| 5,00 g | 5,000 g | 1,607 g | 6,607 g |

Ca(OH)2 ít tan: khối lượng trên gồm phần tan và không tan, không có nghĩa toàn bộ sản phẩm là dung dịch trong suốt. Phản ứng tỏa nhiệt; **hơi và nhiệt độ chỉ minh họa**, không phải khí sản phẩm và không dùng để tính thể tích hơi thực tế. Khối lượng tính theo chất tinh khiết, hiệu suất lý thuyết 100%, nguyên tử khối làm tròn lớp 9: Ca=40, C=12, O=16, H=1.

Hai slider có vai trò khác nhau: **slider chọn lượng** ở Bài 2 được kéo trước lượt đổ; **slider kết quả** ở Bài 1/2 chỉ hiện sau khi hoàn tất và không thể kéo để tự thay đổi kết quả.

## Ba bài còn lại và điều khiển chung

- **Bài 8:** kéo ống Cu(OH)2 vào kẹp trên đèn cồn, bật nhiệt, đợi chất rắn xanh chuyển đen, tắt nhiệt và trả lời.
- **Bài 30:** chọn Na, nguyên tố Z=17 (Cl), Ca; xem Z, proton, electron trung hòa và lớp electron.
- **Bài 31:** so sánh Li–Na–K, Na–Mg–Al; đổi chế độ tính chất và trả lời xu hướng.
- WASD đi lại, chuột xoay camera, E vào bàn; ở bàn dùng chuột kéo/click dụng cụ. Esc rời bàn hoặc mở Pause; menu Pause có Tiếp tục/Tùy chọn/Về menu/Thoát.
- UI được raycast riêng: click ô nhập/slider không đồng thời kéo dụng cụ sau UI. Thả sai vùng trả vật về vị trí cũ. Có thể làm các bàn theo thứ tự tùy chọn.
- Điểm tối đa 500, tiến độ 5/5; không cộng điểm lặp. Tùy chọn → Xóa tiến độ 5 bàn để demo lại.

## Kiểm tra trên Unity của bạn

- [ ] Phòng có đúng năm bàn, vào/ra từng bàn được; không tạo camera/player trùng sau Stop → Play.
- [ ] Bài 1 dẫn nước vôi khoảng 5 giây đạt 50%; khoảng 10 giây đạt 100%; bảng cuối chỉ xuất hiện sau khi hoàn tất.
- [ ] Bài 1 Thử lại làm dung dịch trong, xóa kết tủa và ẩn slider kết quả.
- [ ] Bài 2 nhập `1,25`/`1.25` hoặc kéo slider: cùng khối lượng; `abc`, số âm, 0 hoặc >5 g bị báo sai và không cho đổ.
- [ ] Thìa và miệng lọ nằm trên miệng cốc khi nghiêng; dòng đổ chạm chất lỏng bên trong, không đổ xuống cạnh bàn.
- [ ] Lượng CaO bị khóa khi bắt đầu đổ; thao tác lặp không thêm chất hai lần. Đổ 1 g và 2 g cho kết quả gấp đôi.
- [ ] Rời bàn trước khi dòng đổ hoàn tất hủy lượt đổ; quay lại có thể làm lại. Sau khi chất đã vào cốc thì trạng thái phản ứng được giữ.
- [ ] Phenolphthalein với nước trước phản ứng không hồng; sau phản ứng mới hồng. Nắp lọ đóng lại sau hoạt ảnh hoặc khi hủy thao tác.
- [ ] Pause dừng hoạt ảnh và timer; Thử lại reset màu, lượng chất, vật thể và kết quả; ba bài 8/30/31 vẫn chạy.
- [ ] Console không có lỗi đỏ trong source, build Windows chạy được ngoài Editor.

Có sẵn test tại **Window → General → Test Runner → EditMode / PlayMode → Run All**. Test Play Mode mới kiểm tra bảng cuối, khóa lượng khi đổ, vị trí outlet trên tâm cốc, reset và hủy thao tác khi rời bàn. Test này cần chạy trong Unity; cloud chưa chạy được.

## Build và các lỗi dễ gặp

Chọn **ChemLab9Amounts → Build Windows x64** sau khi Validate. Kết quả ở `Builds/Windows/ChemLab9Amounts.exe`; gửi toàn bộ thư mục Windows, gồm `_Data` và DLL. Có thể dùng File → Build Profiles → Windows với MainMenu và ChemistryLab trong Scene List. Cài Windows Build Support cho đúng Editor 6000.6.0f1 nếu thiếu module.

Nếu chỉ thấy Untitled, dùng menu Open Main Menu. Nếu không nhận phím, click tab Game và kiểm tra Input Manager (Old). Nếu material hồng, kiểm tra URP/Graphics/Quality và lỗi shader đầu tiên; project này giữ URP, không chuyển pipeline. Nếu package Editor báo lỗi `TraceVirtualOffset.urtshader` giống bản cũ, ghi lại lỗi đầu tiên và xem nó còn xuất hiện sau Stop, Clear và mở lại Editor; không tự sửa logic hóa học theo lỗi đó.

## Source và kết quả kiểm tra đã chạy

Source vẫn dưới `Assets/ChemLab9/` để giữ reference scene/prefab ổn định; tên project mới là ChemLab9Amounts. Các file thay đổi chính: `Scripts/Chemistry/QuantityCalculations.cs`, `Lessons/Lesson01Manager.cs`, `Lessons/Lesson02Manager.cs`, `Equipment/GasDeliveryController.cs`, `Equipment/FlowEffect.cs`, `Interaction/DraggableObject.cs`, `Core/ChemLabBootstrap.cs`, `Core/LabFactory.cs`, `UI/LabUI.cs`. Core/Player chỉ đổi khóa settings/progress; Editor thêm tự mở scene và menu cho project mới.

**45 test logic production đã chạy đạt trong .NET**, gồm 30 trường hợp cũ và 15 trường hợp khối lượng/input mới; C# đã được kiểm tra cú pháp. Kiểm tra tĩnh đã xác minh 37 asset YAML, GUID/reference, hai scene, năm bàn, dữ liệu 20 nguyên tố và giữ nguyên 452 file asset/font/import của bản gốc. Mã manager/visualizer của Bài 8/30/31 giữ nguyên.

**Chưa chạy Unity API compilation, shader compilation, Play Mode hay build Windows trong cloud vì không có Unity Editor/license.** ZIP là source project, không phải bản Windows đã được kiểm nghiệm. Các kiểm tra logic/tĩnh không thay thế việc mở Unity và kiểm tra tương tác/hình ảnh trên máy bạn.

Kiểm tra từ repository nếu có .NET 8 và Python/PyYAML:

```bash
dotnet run --project tools/QuantityChecks/QuantityChecks.csproj -- ChemLab9Amounts/Assets
python tools/validate_amount_assets.py
python tools/package_amount_demo.py
```

Asset Chemistry Lab Items Pack từ [Unity Asset Store](https://assetstore.unity.com/packages/3d/environments/chemistry-lab-items-pack-220212), giữ ở thư mục nhà phát hành và dùng wrapper của project. Font Be Vietnam Pro đi kèm giấy phép OFL trong Assets/Fonts. Không cần mua thêm asset hoặc plugin.
