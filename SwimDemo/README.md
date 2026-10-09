# SwimDemo — Y Bot bơi và điều khiển trong nước

Project riêng cho **Unity Editor 6.6 / 6000.6.0f1**, Windows PC. Đã đưa vào đúng file `YBot_Swim.fbx` bạn gửi, bao gồm mesh, bộ xương và animation. Nhân vật cử động khớp bằng animation; bạn điều khiển vị trí và hướng bơi bằng bàn phím.

## Mở lần đầu — làm theo thứ tự này

1. Tải bản `SwimDemo_Unity6000.6.zip`, click phải → **Extract All / Giải nén tất cả**. Không mở project ngay bên trong ZIP.
2. Mở **Unity Hub → Projects → Add → Add project from disk**. Chọn thư mục **SwimDemo** vừa giải nén. Bên trong thư mục bạn chọn phải có đủ **Assets, Packages, ProjectSettings**. Nếu lấy cả repository PRU, chọn `PRU/SwimDemo`, không chọn `PRU` hoặc `ChemLab9`.
3. Mở bằng Editor **6000.6.0f1**. Lần đầu cần mạng để Unity lấy Test Framework và dependencies; bản game Windows sau build chạy offline. Không cần đăng nhập Mixamo để chạy model đã được đưa vào project.
4. Đợi Unity import xong. Script Editor tự chuyển FBX sang **Humanoid**, tìm Avatar/clip thật trong FBX, gắn reference vào `SwimSettings.asset` và mở scene **Swimming** khi Editor đang ở scene Untitled chưa sửa.
5. Nếu vẫn hiện **Untitled**, chọn menu trên cùng **SwimDemo → Open Swimming Scene**. Cũng có thể double-click `Assets/SwimDemo/Scenes/Swimming.unity` trong cửa sổ Project.
6. Chọn **SwimDemo → Validate Project**. Khi Console báo cấu hình hợp lệ, nhấn **Play** ở thanh công cụ trên cùng.
7. Chuyển sang tab **Game**, click một lần vào vùng hình ảnh để nhận bàn phím. Bạn sẽ thấy bể nước, Y Bot đang cử động tay/chân và bảng điều khiển ở góc trái.
8. Giữ **W** để bơi tới, **C** để lặn xuống, **Space** để bơi lên. Nhấn **V** để chuyển camera xuống dưới nước và nhìn rõ khớp tay/chân. Giữ chuột phải và kéo để xoay góc nhìn; cuộn chuột để zoom.

Trước Play, scene chỉ có `SwimDemoSystems` với cấu hình sẵn. Bể nước, nhân vật, camera và UI được tạo khi Play bởi `SwimDemoBootstrap`; đây là cách project được thiết kế. Không cần thêm camera, Animator hoặc nhân vật thứ hai bằng tay. Sau Stop, những object được tạo khi chạy sẽ biến mất; Play lại sẽ tạo mới.

## Điều khiển

| Phím / thao tác | Kết quả |
|---|---|
| W / S | Tiến / lùi theo hướng nhân vật đang quay |
| A / D | Quay trái / phải; giữ cùng W để bơi vòng |
| Space | Bơi lên; dừng tại giới hạn gần mặt nước |
| C | Lặn xuống; dừng trước đáy bể |
| Chuột phải + kéo | Xoay camera quan sát quanh nhân vật |
| Con lăn | Phóng gần / xa |
| V hoặc nút Đổi góc nhìn | Chuyển camera trên / dưới mặt nước |
| M hoặc nút Tự bơi | Tự bơi quanh bể; nhấn W/S/A/D/Space/C sẽ trở về điều khiển tay |
| R hoặc nút Làm lại | Về giữa bể, độ sâu 0,7 m, đặt lại animation và hướng bơi |
| Esc / Tạm dừng / Tiếp tục | Dừng / tiếp tục chuyển động, animation, sóng và bọt khí |
| Thanh Tốc độ bơi | Điều chỉnh tốc độ di chuyển từ 0,5 đến 4 m/s |
| Thoát | Dừng Play trong Editor; đóng game khi chạy bản Windows |

Khi không giữ phím, nhân vật vẫn đạp nước bằng animation nhưng không tự tiến. Đây là demo với một clip bơi, chưa có các clip riêng cho đứng nước, bơi lùi hay xoay. Vị trí vẫn phản hồi đúng theo phím; chuyển động bơi lùi dùng lại clip đã cung cấp.

## Cấu hình đã chọn

- **Built-in Render Pipeline**, không cài URP/HDRP cho project này. Project ChemLab9 là một project khác, với cấu hình riêng.
- **Input Manager (Old)**, `Active Input Handling = Input Manager (Old)`. Không cần bật Input System mới.
- Di chuyển kinematic bằng `SwimKinematics` + `SwimmerController`, không dùng trọng lực/Rigidbody để kéo nhân vật xuống đáy.
- FBX **Humanoid**, Avatar **Create From This Model**, animation lặp. Không cần Animator Controller hoặc plugin animation; clip chạy qua Unity Playables.
- Clip gốc có dịch chuyển tiến. `ImportedSwimAnimator` lấy pose từ clip rồi neo xương Hips vào vị trí nhân vật, để việc lặp clip không kéo nhân vật về đầu đường bơi.
- Mesh model được giữ nguyên trong FBX; vật liệu hiển thị được tạo riêng khi chạy. Không cần texture bên ngoài. Font UI tiếng Việt **Be Vietnam Pro** đã đi kèm.
- Bể rộng 18 × 12 m, đáy khoảng y = −3,5 m, mặt nước quanh y = 0. Tâm Hips giới hạn trong x = ±7,3 m, z = ±4,3 m; độ sâu từ 0,35 đến 2,5 m để chừa khoảng cho tay/chân.
- Mặt nước có sóng sin nhẹ, vật liệu trong suốt, sương mù dưới nước và bọt khí minh họa. Đây là mô hình phục vụ demo, không phải mô phỏng thủy động lực học hay hệ thống phản xạ/khúc xạ nước thực.

## Inspector — khi muốn chỉnh tốc độ

**Dừng Play trước khi sửa cấu hình.** Trong Project, chọn `Assets/SwimDemo/Resources/SwimSettings.asset`.

| Field | Mặc định / reference |
|---|---|
| Character Model | Model chính trong `YBot_Swim.fbx`, Editor tự gắn |
| Character Avatar | Avatar Humanoid được Unity import từ cùng FBX, Editor tự gắn |
| Swimming Clip | Clip thật trong cùng FBX, Editor tự gắn |
| Model Scale | 1 — Unity sử dụng đơn vị của file FBX |
| Model Euler Correction | (0, 0, 0); clip đã có tư thế bơi, không tự xoay thêm 90° |
| Move Speed | 1,8 m/s |
| Vertical Speed | 0,8 m/s |
| Animation Speed | 1 — hệ số tốc độ clip |

Trong scene, `SwimDemoSystems → Swim Demo Bootstrap` đã gắn `Settings`, `UIFont` và ba shader riêng. Các reference phụ thuộc vào fileID nội bộ của model/Avatar/clip được tìm bởi Editor sau import, không được bịa trước trong YAML. Lần đầu Unity sẽ lưu những reference thật vào asset settings; nếu dùng Git, có thể commit thay đổi này sau khi kiểm tra.

## Kiểm tra bằng mắt trong Play Mode

- [ ] Y Bot có mesh đầy đủ, đang bơi ngang; vai, khuỷu tay, hông và đầu gối thay đổi theo animation.
- [ ] W/S làm nhân vật tiến/lùi; A/D đổi hướng; thả phím thì dừng di chuyển nhưng animation vẫn chạy.
- [ ] Space đưa lên gần mặt nước, C đưa xuống sâu; giữ phím lâu không vượt mặt nước/đáy.
- [ ] Bơi tới mép bể không xuyên tường; quay lại được.
- [ ] V chuyển góc nhìn dưới nước; chuột phải và con lăn vẫn quan sát được nhân vật.
- [ ] M bật tự bơi, bấm W/S/A/D/Space/C lấy lại điều khiển; không nhảy vị trí khi bật tự bơi.
- [ ] Để chạy ít nhất 15 giây: animation lặp, không giật cả nhân vật về vị trí bắt đầu clip.
- [ ] Esc dừng cả tay/chân, sóng, bọt khí và di chuyển; Tiếp tục khôi phục hoạt động.
- [ ] R/Làm lại đặt vị trí, hướng và pose ban đầu; Stop → Play lại không tạo nhân vật/camera trùng.
- [ ] Console không có lỗi đỏ từ source project; bản Windows chạy được ngoài Editor.

Có sẵn test trong **Window → General → Test Runner**: tab **EditMode → Run All**, rồi tab **PlayMode → Run All**. Play Mode test kiểm tra khớp thật của FBX, neo Hips qua một vòng clip và pause/resume. Đợi setup hoàn tất trước khi chạy test.

## Nếu gặp vấn đề

| Hiện tượng | Cách xử lý ở project này |
|---|---|
| Không có menu SwimDemo, Assets trống, tên project khác | Kiểm tra Hub đang mở thư mục `SwimDemo` chứa ba thư mục bắt buộc, không phải thư mục cha hoặc một project trống khác. |
| Scene Untitled / không thấy bể trước Play | Mở **SwimDemo → Open Swimming Scene**, sau đó nhấn Play. Bể được tạo lúc chạy. |
| UI báo chưa cấu hình FBX | Dừng Play → **SwimDemo → Setup / Repair Mixamo Import** → đợi import → **Validate Project** → Play lại. |
| Avatar không hợp lệ | Chọn FBX → Inspector **Rig** → Animation Type **Humanoid**, Avatar Definition **Create From This Model**, **Apply → Configure**; kiểm tra xương được mapping. File đang có 65 xương Mixamo. Gửi lỗi đầu tiên nếu Configure báo mapping đỏ. |
| Nhân vật đứng T-pose / không cử động | Dừng Play; chạy Repair, xem `Swimming Clip` trong settings có reference thật. Không xóa Animator hoặc gắn một Animator Controller khác trong Play. |
| Phím không nhận | Click vào tab Game, đóng Pause. Project Settings → Player → Other Settings → Configuration → **Active Input Handling = Input Manager (Old)**. Nếu Unity yêu cầu restart thì restart. |
| Unity cảnh báo Input Manager cũ bị deprecated | Đây là cảnh báo về hướng phát triển API; project này được thiết kế dùng API cũ nhất quán. Không tự đổi Input Handling sang New. |
| Model / mặt nước hồng | Chạy Validate. Project này cần Built-in: Project Settings → Graphics → Default Render Pipeline và Quality → Render Pipeline Asset đều để None. Xem lỗi shader đầu tiên trong Console nếu vẫn hồng. |
| Góc trên nước khó thấy khớp | Nhấn V xuống dưới nước, cuộn để zoom gần và giữ chuột phải xoay camera. |
| Build thiếu scene hoặc Windows module | Dùng menu Build bên dưới; cài Windows Build Support cho chính Editor 6000.6.0f1 nếu Unity báo thiếu. |

Không sửa package/cache của ChemLab9 cho demo bơi này; hãy kiểm tra tên project và đường dẫn lỗi để biết project nào đang mở.

## Build Windows

1. Dừng Play, chạy **SwimDemo → Validate Project**.
2. Chọn **SwimDemo → Build Windows x64**. Menu build chạy setup, kiểm tra reference và đưa `Swimming.unity` vào build.
3. Kết quả nằm ở `SwimDemo/Builds/Windows/SwimDemo.exe` và các file đi kèm.
4. Chạy `.exe` bên ngoài Editor, thử W/S/A/D/Space/C, V, Pause và reset.
5. Gửi **toàn bộ thư mục Windows**, bao gồm `_Data`, DLL và các thư mục được Unity tạo; không chỉ gửi `.exe`.

Cũng có thể dùng **File → Build Profiles → Windows → Scene List**, bảo đảm chỉ bật `Assets/SwimDemo/Scenes/Swimming.unity`, rồi Build. Thiết lập backend Windows hiện là Mono.

## Cấu trúc source và giới hạn kiểm chứng

`Assets/SwimDemo/Characters` chứa FBX; `Scripts` chứa chuyển động, animation, camera, UI và dựng bể; `Shaders` chứa ba shader Built-in; `Editor` chứa import/setup/validation/build; `Resources/SwimSettings.asset` là dữ liệu; `Scenes/Swimming.unity` là scene bắt đầu; `Tests` chứa test logic và Play Mode; `Fonts` chứa font và giấy phép OFL.

Đã kiểm tra trong cloud: FBX giữ nguyên checksum bản upload; Blender 4.3.2 đọc được 65 xương, hai mesh skinned và một clip, có chuyển động khớp qua các frame. **21 kịch bản logic chuyển động đã chạy đạt**, mã C# đã được kiểm tra cú pháp; GUID, reference scene/font/shader, cấu hình build/input/pipeline đã được kiểm tra tĩnh. Báo cáo FBX: `docs/fbx-inspection.json`.

**Cloud không có Unity Editor/license:** chưa chạy Unity API compilation, shader compilation, import Humanoid của Unity, Play Mode hoặc build Windows. Những kiểm tra đó cần thực hiện với Editor của bạn; test có sẵn không đồng nghĩa chúng đã chạy trong Unity. Bản ZIP là mã nguồn project, không phải bản `.exe` đã build.

Các lệnh kiểm tra từ thư mục repository nếu máy có .NET SDK 8, Python/PyYAML và Blender:

```bash
dotnet run --project tools/SwimChecks/SwimChecks.csproj -- SwimDemo/Assets
python tools/validate_swim_assets.py
blender --background --python tools/inspect_swim_fbx.py -- --output SwimDemo/docs/fbx-inspection.json
```

## Nguồn tài nguyên

- Model Y Bot và motion bơi: FBX do bạn cung cấp; [Y Bot trên Mixamo](https://www.mixamo.com/#/?page=1&query=Y+Bot&type=Character), [tìm animation swimming](https://www.mixamo.com/#/?page=1&query=swimming&type=Motion%2CMotionPack). Quyền sử dụng tài nguyên theo điều khoản Adobe/Mixamo; source gameplay của project không thay thế giấy phép model/animation.
- Font Be Vietnam Pro: font đã có trong project của bạn, [nguồn Google Fonts](https://fonts.google.com/specimen/Be+Vietnam+Pro). Giấy phép SIL Open Font License được giữ trong `Assets/SwimDemo/Fonts/BeVietnamPro-OFL.txt`.
- Bể, sóng, bọt, UI và điều khiển được viết/tạo trong project, không cần mua asset khác.
