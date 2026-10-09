# ChemLab9 — Hóa học lớp 9 trong một phòng lab 3D

**Demo Y Bot bơi mới:** mở project riêng tại [SwimDemo/README.md](SwimDemo/README.md). Project này dùng file `YBot_Swim.fbx` đã cung cấp, có điều khiển tiến/lùi, quay, lặn và bơi lên trên Unity `6000.6.0f1`. Bản source ZIP riêng nằm trong `downloads/SwimDemo_Unity6000.6.zip`.

Project Unity đã được bổ sung từ bản ZIP đang làm dở: một phòng, năm bàn độc lập, thao tác 3D và câu hỏi tiếng Việt. Toàn bộ mã nguồn nằm trong `ChemLab9/`; bản upload gốc vẫn được giữ ở các scene/thư mục ban đầu để đối chiếu.

## Mở và chạy

1. Giải nén bản bàn giao hoặc lấy thư mục repository về máy.
2. Unity Hub → **Projects → Add → Add project from disk**; chọn thư mục **ChemLab9** chứa `Assets`, `Packages`, `ProjectSettings`.
3. Mở bằng **Unity 6.6 / 6000.6.0f1**, đúng phiên bản trong file bạn gửi. Đợi Package Manager hoàn tất; lần mở đầu cần mạng tải các package Unity, game sau build chạy offline.
4. Trong Editor: **ChemLab9 → Open Main Menu**. Hoặc mở `Assets/ChemLab9/Scenes/MainMenu.unity`.
5. Nhấn **Play**, click **Bắt đầu**, WASD đi tới một bàn, nhìn vào bàn và nhấn **E**.
6. Click **Bắt đầu** trên panel bàn; kéo dụng cụ bằng chuột hoặc click thiết bị theo hướng dẫn. Sau thao tác đúng, mở **Kết quả / Câu hỏi**.

**Scene mới có các root cấu hình sẵn. Dụng cụ, model, UI, player và reference được `ChemLabBootstrap` lắp ráp khi Play. Vì vậy scene nhìn ít object trước Play là có chủ ý. Không thêm camera/player bằng tay.** Bạn có thể quan sát toàn bộ hierarchy và reference trong Play Mode; các thay đổi Inspector trong Play không được lưu sau Stop.

## Cấu hình thực tế

- Editor: `6000.6.0f1` — theo `ProjectVersion.txt` của project và ảnh Unity Hub.
- Pipeline: **URP 17.7.0**, giữ hệ thống đã có trong ZIP. `Graphics` và cả hai quality level dùng `Assets/Settings/PC_RPAsset.asset`; không bắt đổi về Built-in.
- Điều khiển: **CharacterController + Input Manager cũ**. `Active Input Handling = Input Manager (Old)`. Package Input System vẫn được giữ vì đã có trong project; gameplay mới không dùng API của package đó.
- UI: UGUI + TextMeshPro, font dynamic tạo từ `Assets/Fonts/BeVietnamPro-Regular.ttf` hỗ trợ tiếng Việt. Các công thức trong UI dùng dạng ASCII để dễ đọc và tránh thiếu glyph; ký hiệu mũi tên vẫn là chữ Unicode.
- Không cần backend, tài khoản, thêm plugin trả phí hoặc dịch vụ khi chơi.

## Điều khiển

| Chế độ | Thao tác |
|---|---|
| Đi lại | WASD di chuyển, chuột xoay camera, E vào bàn, Esc mở Pause |
| Tại bàn | Chuột chọn/kéo dụng cụ; click nguồn CO2 hoặc thiết bị nhiệt; Esc rời bàn |
| Panel bàn | Bắt đầu, Thử lại, Kết quả/Câu hỏi, Rời bàn, Tạm dừng |
| Pause | Tiếp tục, Tùy chọn, Về menu, Thoát |

Khi vào bàn, camera dùng phần bên trái màn hình; panel nhiệm vụ nằm bên phải. Con trỏ được mở, di chuyển/mouse look dừng. Kéo bị giới hạn trên mặt bàn; vùng nhận hợp lệ đổi màu xanh. Thả sai trả về vị trí trước khi kéo. Click UI không kích hoạt dụng cụ phía sau. Trong Game view, click vào cửa sổ để Unity nhận input nếu bạn vừa chuyển cửa sổ.

## Năm bài và demo

| Bài | Thao tác và hiện tượng |
|---|---|
| 1 | Kéo đầu ống vào nước → bật CO2 → đủ liều tự dừng → kéo quỳ vào nước: quỳ đỏ. Chuyển ống sang nước vôi, bật khí: đục dần và kết tủa trắng. Trả lời hai câu hỏi. |
| 2 | Kéo thìa CaO vào nước: rót mẫu, CaO giảm dần, nhiệt kế mô phỏng tăng. Đợi phản ứng minh họa xong, kéo phenolphthalein vào cốc: hồng. Có thể thử chỉ thị với nước trước để thấy không hồng. |
| 8 | Kéo ống Cu(OH)2 xanh vào kẹp nung → click thiết bị → đủ thời gian chất rắn đen → click để tắt nhiệt → trả lời. Tắt sớm tạm dừng, bật lại tiếp tục. |
| 30 | Click Na, nguyên tố Z=17 (Cl), rồi Ca. Xem điện tích hạt nhân, p/e, nhóm, chu kỳ, electron theo lớp và mô hình 3D. Trả lời ba câu hỏi. |
| 31 | Chế độ A: click Li, Na, K theo bán kính tăng; xem Na–Mg–Al giảm. Chọn chế độ B xem xu hướng tính kim loại/phi kim rồi trả lời. |

Mỗi bàn 100 điểm khi hoàn thành tất cả nhiệm vụ/câu hỏi; tổng tối đa 500. Làm sai được phản hồi và có thể chọn lại. Spam đáp án hay luyện lại không cộng thêm điểm. Chọn bài từ menu chỉ đặt người chơi gần bàn trong **cùng ChemistryLab**, không chuyển sang scene bài riêng.

## Các sửa chữa quan trọng

- Đưa `ChemicalDatabaseCreator.cs` vào `Assets/Editor` để code UnityEditor không lọt vào Windows player.
- Bỏ suy luận phản ứng từ `Contains("cu")`, `Contains("oh")` và trộn màu tùy ý. Scene `Classroom` cũ còn chế độ tương thích đơn giản; demo đầy đủ dùng hai scene mới.
- Giữ class/player cũ `PlayerController`, bổ sung khóa điều khiển và di chuyển chéo được giới hạn tốc độ; chuyển file vào assembly runtime mới, giữ GUID.
- Chuẩn hóa pipeline/input đang có, thêm scene build, database, menu, năm lesson manager, station mode, kéo/thả, lượng chất, VFX, quiz, điểm và lưu tiến độ.
- Dữ liệu 20 nguyên tố có Z, nhóm/chu kỳ, electron theo lớp. Bán kính chỉ là kích thước minh họa ở các ví dụ đã chọn.

## Cấu trúc và tài liệu

- `Assets/ChemLab9/Scenes/`: MainMenu, ChemistryLab.
- `Scripts/Core/`: bootstrap, game/station, scene loader, tiến độ, dựng model và âm thanh.
- `Scripts/Chemistry/`: mô hình lượng chất, bốn phản ứng, chất chứa, chất chỉ thị.
- `Scripts/Interaction/`, `Player/`, `Equipment/`: raycast, kéo/thả, dòng rót, dẫn khí, gia nhiệt.
- `Scripts/Data/`, `Data/`: ScriptableObject database, bài học/câu hỏi, chất và phản ứng.
- `Scripts/Lessons/`, `PeriodicTable/`, `UI/`: năm bài, mô hình nguyên tử và giao diện.
- `Prefabs/`: wrapper/template riêng của project, không sửa model gốc.
- [Hướng dẫn kiểm tra trong Unity](docs/PLAY_MODE_CHECKLIST.md).
- [Hierarchy, prefab và mapping](docs/PREFABS.md).
- [Kiến trúc và mô hình hóa học](docs/ARCHITECTURE.md).
- [Kết quả kiểm thử và giới hạn](docs/VALIDATION.md).
- [Attribution](docs/ATTRIBUTION.md).

## Giả định mô phỏng

Một đơn vị chất là **liều gameplay**, không phải mol/mL. Dung tích tối đa 12 đơn vị, cốc nước khởi tạo 8 đơn vị, mẫu rắn/nước vôi 1 đơn vị. Các biến đổi chỉ bảo đảm không âm/vượt dung tích trong mô hình đơn giản, không mô phỏng cân bằng/nồng độ thực.

- CO2 chỉ dẫn tối đa một liều mỗi cốc; không có chế độ dẫn dư. CO2 không có màu. Nước sau CO2 vẫn không màu; giấy quỳ đổi đỏ.
- Nước vôi tạo CaCO3 trắng. CaO + nước tỏa nhiệt và tạo Ca(OH)2 ít tan. Nhiệt kế 25→50 °C trong 3 giây là **cấu hình demo**, không phải kết quả tính nhiệt thực nghiệm. Hơi chỉ minh họa nước, không phải khí sản phẩm mới.
- Cu(OH)2 cần đúng vùng nung và tổng 5 giây gameplay. Tắt nhiệt không đảo ngược CuO; thử lại tạo mẫu mới.
- Mô hình electron theo lớp không thể hiện quỹ đạo thực. Không hiển thị neutron hoặc suy ra neutron từ nguyên tử khối trung bình.
- Bán kính Li/Na/K và Na/Mg/Al là kích thước tương đối, không có đơn vị pm. Thanh tính kim loại chỉ minh họa; khí hiếm có giải thích riêng. Chưa có chế độ hoạt động hóa học C.

## Thêm/chỉnh dữ liệu

Chọn `Assets/ChemLab9/Data/ChemicalDatabase.asset` trong Inspector. `elements` là dữ liệu chung cho hai bàn cuối; giữ tổng electron = Z cho nguyên tử trung hòa, số lớp = chu kỳ trong phạm vi 20 nguyên tố này. Chỉnh `relativeRadius`/`radiusNote` chỉ cho ví dụ được giải thích rõ.

Các file `Data/Substances`, `Reactions`, `Lessons` là ScriptableObject. Bốn quy tắc phản ứng của MVP được kiểm tra trong `SimulationState.cs`; chỉnh công thức hiển thị không tự tạo một phản ứng mới. Muốn thêm phản ứng/nguyên tố ngoài phạm vi hiện có phải bổ sung rule, nhiệm vụ và kiểm thử phù hợp. Không áp dụng máy móc 2–8–8 cho nguyên tố ngoài phạm vi.

`tools/seed_project.py` là công cụ **chủ động tạo lại dữ liệu/scene mẫu**, không cần chạy để mở Unity; nó ghi đè seed, nên chỉ chạy khi muốn tái tạo cấu hình gốc. `tools/create_prefabs.py` tạo lại wrapper từ model trong ZIP. Gameplay không phụ thuộc Python.

## Lưu, thử lại và xóa tiến độ

PlayerPrefs dùng khóa `ChemLab9.Completed.<mã bài>`, `ChemLab9.MouseSensitivity`, `ChemLab9.Volume`. Đóng/mở game giữ tiến độ/setting. Thử lại reset mẫu, timer, màu, chất chứa, dụng cụ, VFX và câu hỏi của bàn, giữ điểm hoàn thành đã lưu. **Tùy chọn → Xóa tiến độ 5 bàn** xóa điểm/tiến độ; nếu muốn làm lại một bàn đang mở, bấm Thử lại.

Pause dừng thời gian gameplay, coroutine rót, VFX/animation và âm thanh. Rời bàn dừng nguồn CO2/gia nhiệt; các thao tác rót đã bắt đầu có thể hoàn tất trong cùng scene. Mẫu và nhiệm vụ của từng bàn độc lập.

## Kiểm thử và build Windows

- **ChemLab9 → Validate Project**: kiểm tra dữ liệu, electron, quiz, scene build và pipeline trong Editor.
- **Window → General → Test Runner → EditMode → Run All**: chạy 30 tình huống logic cùng bộ production code. Đợi Unity compile trước khi mở Test Runner.
- Test Runner → **PlayMode → Run All**: năm kiểm tra tích hợp scene/tool, Pause/heating, hoạt ảnh quỳ/reset, vị trí nung và ô thông tin/nhãn xu hướng; chúng chưa được chạy trong cloud.
- Thực hiện checklist Play Mode, sau đó **ChemLab9 → Build Windows x64**. Hoặc File → Build Profiles → Windows, bảo đảm MainMenu đứng trước ChemistryLab.
- Nếu thiếu target, Unity Hub → Installs → Manage → Add modules → Windows Build Support phù hợp với backend đang dùng.
- Output: `ChemLab9/Builds/Windows/ChemLab9.exe`. Copy **toàn bộ thư mục Windows**, gồm `_Data` và DLL, sang máy demo. Test file EXE ngoài Editor.

Có thể chạy logic ngoài Unity bằng .NET 8: `dotnet run --project tools/LogicChecks -- ChemLab9/Assets` từ root repository. `python tools/validate_assets.py` kiểm tra YAML/GUID tĩnh, cần PyYAML. Không thay thế Play Mode hoặc Windows build.

**Môi trường sửa code hiện không có Unity Editor/license. Logic và kiểm tra tĩnh đã chạy; chưa thể khẳng định Unity import, Play Mode hoặc Windows EXE đã được kiểm thử. Xem `docs/VALIDATION.md` để biết kết quả cụ thể.**
