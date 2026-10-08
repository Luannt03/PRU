# Hierarchy, prefab và mapping

## Cách project lắp ráp scene

`ChemLabBootstrap.Build()` chạy trong `Awake`. Scene `ChemistryLab` đã có root LabRoom, Player, Stations, Systems, Canvas (RectTransform), EventSystem và đúng năm station. Bootstrap tạo phần nhìn thấy, component và nối reference trên các root đó. Scene `MainMenu` chỉ tạo menu/camera/UI.

Không kéo thêm Player, Main Camera hay EventSystem vào scene demo. Không gọi công cụ tạo Classroom cũ để dựng scene mới.

## Mapping Inspector trên Systems → ChemLabBootstrap

Các field đã được điền trong cả hai scene:

| Field | Asset/reference |
|---|---|
| MainMenu | Bật cho MainMenu; tắt cho ChemistryLab |
| Database | `Assets/ChemLab9/Data/ChemicalDatabase.asset` |
| VietnameseFont | `Assets/Fonts/BeVietnamPro-Regular.ttf` |
| FontShaderReference | Font asset có sẵn `Assets/Fonts/Roboto-Dynamic SDF.asset`, chỉ giữ shader trong build; chữ mới dùng BeVietnamPro |
| OpaqueMaterial | `Assets/ChemLab9/Materials/Opaque.mat` |
| GlassMaterial | `Assets/ChemLab9/Materials/Glass.mat` |
| TableModel | Project wrapper `Prefabs/ExperimentTable.prefab` |
| BeakerModel | Project wrapper `Prefabs/Beaker.prefab`, model không có lớp nước trang trí |
| TubeModel | Project wrapper `Prefabs/TestTube.prefab`, model không có lớp nước trang trí |
| HeaterModel | Project wrapper `Prefabs/Heater.prefab` |
| ShelfModel | Prefab `shelf` thực sự có trong asset upload |
| ClickSound / CorrectSound / IncorrectSound | WAV tương ứng ở `Assets/ChemLab9/Audio` |

Nếu model tùy chọn thiếu, builder dùng primitive. Database/font/material là bắt buộc và có thông báo lỗi rõ ràng. Thay model bằng cách tạo wrapper của project, rồi kéo wrapper mới vào field phù hợp. Factory tự fit bounds, bỏ collider model trong instance và tạo collider logic riêng; tài nguyên gốc không bị chỉnh.

## Vị trí station và player

Các transform sau là **local dưới Stations**, Stations ở world origin, scale (1,1,1):

| Station | Position | Rotation | Scale |
|---|---|---|---|
| Station_Lesson01_AcidicOxide | (-4.2, 0, 3.6) | (0,0,0) | (1,1,1) |
| Station_Lesson02_BasicOxide | (0,0,3.6) | (0,0,0) | (1,1,1) |
| Station_Lesson08_InsolubleBase | (4.2,0,3.6) | (0,0,0) | (1,1,1) |
| Station_Lesson30_PeriodicTable | (-3,0,-3.6) | (0,180,0) | (1,1,1) |
| Station_Lesson31_PeriodicTrends | (3,0,-3.6) | (0,180,0) | (1,1,1) |

Player spawn world (0,0.05,0). CharacterController: height 1.8, radius 0.3, center (0,0.9,0), stepOffset 0.25. Camera local (0,1.6,0), FOV 65, near clip 0.03. Không có Rigidbody trên player.

Khi vào bàn, player ở station-local (0,0.05,-2.4), yaw theo station. Camera pitch +13° ở bàn thí nghiệm, -7° ở bàn nguyên tố. Rời bàn khôi phục vị trí/rotation trước đó. WorkHeight = 1.18; kéo được clamp local x [-1.65,1.65], z [-0.7,0.65].

## Các wrapper/template prefab đã cung cấp

Các prefab trong thư mục project là wrapper/template hình ảnh; ngoài Player, chúng không tự chạy bài học khi kéo riêng vào scene. Scene demo gắn logic và reference bằng bootstrap. Điều này tránh reference giữa station được đóng cứng trong model của nhà phát hành.

| Prefab / object runtime | Visual/child và component runtime | Collider / Rigidbody |
|---|---|---|
| Player | Camera, AudioListener; PlayerController, PlayerInteractor | CharacterController; không Rigidbody |
| ExperimentTable | Model bàn; StationSign; station root có StationController + Lesson manager | BoxCollider center (0,.55,0), size (3.5,1.1,1.4); không Rigidbody |
| Beaker | Model thủy tinh; Liquid; Precipitate; ChemicalContainer; DropZone + SnapPoint + Highlight | BoxCollider vùng nhận; không Rigidbody |
| TestTube | Model thủy tinh; chất rắn riêng; ChemicalContainer; DraggableObject | BoxCollider; không Rigidbody |
| ChemicalBottle | Model, nắp, FlowEffect; DraggableObject loại IndicatorBottle | BoxCollider; không Rigidbody |
| GasSource | Model; Interactable gọi Gas.Toggle; gợi ý click | BoxCollider; không Rigidbody |
| GasTube | TubeTip; DraggableObject loại GasTube; GasTubeVisual nối nguồn với tip | BoxCollider tip; không Rigidbody |
| Heater | Model; Interactable; HeatZone; HeatingController; hơi nước | BoxCollider; không Rigidbody |
| ElementTile | Cube nổi; label Z/ký hiệu/tên; Interactable gọi Select(ElementRecord) | BoxCollider; không Rigidbody |
| AtomModel | Nucleus, Shell_i (LineRenderer), Electron_i (Sphere); AtomVisualizer | Không collider ở mô hình Bài 30; Sphere collider ở Bài 31 |
| Liquid | Cylinder/renderer riêng, material thủy tinh riêng | Không collider/Rigidbody |
| Precipitate | Cylinder/renderer riêng, màu chất rắn | Không collider/Rigidbody |
| ResultPanel | Image, TMP text, Button; LabUI quản lý câu hỏi | UI raycast; không collider/Rigidbody |

Renderer hình ảnh không cần tương tác được đặt layer 2 (Ignore Raycast). Collider điều khiển ở wrapper/vùng nhận; raycast lấy component ở parent. Không bật Rigidbody nếu không có nhu cầu vật lý thật.

## Reference được nối khi Play

| Component.field | Object/component đích |
|---|---|
| PlayerController.cameraHolder | Transform của FirstPersonCamera |
| PlayerInteractor.View | Camera của FirstPersonCamera |
| Interactable.Station / DraggableObject.Station | StationController của bàn sở hữu |
| DraggableObject.Kind | GasTube / Litmus / CaOSpoon / IndicatorBottle / CopperSample |
| DropZone.Station / ZoneId / Accepted | Station sở hữu, ID `water`/`lime`/`heating`, loại dụng cụ hợp lệ |
| DropZone.SnapPoint / HighlightRenderer | Child SnapPoint / renderer Highlight |
| ChemicalContainer.Liquid / Solid / Initial | Renderer Liquid / Precipitate / chất khởi tạo riêng của cốc |
| Lesson01.Water / Limewater / Gas / Litmus | Hai ChemicalContainer, GasDeliveryController, Paper renderer |
| Lesson02.Beaker / CaOSample / Steam | Cốc nước, mẫu rắn trên thìa, ParticleSystem hơi minh họa |
| Lesson02.ThermometerFill / ThermometerText | Thanh nhiệt kế và TMP label |
| Lesson08.Sample / Heater | ChemicalContainer của ống / HeatingController |
| HeatingController.Sample / HeatVisual / Steam | Mẫu, hình vùng nhiệt, ParticleSystem nước |
| Lesson30.Atom | AtomVisualizer ở child AtomModel |
| Lesson31.Visualizer | PeriodicTrendVisualizer của bàn |

## Button và callback

Button được tạo và gắn listener bằng `LabUI.Button`; Inspector OnClick có thể không có dòng persistent vì đây là listener runtime. Không gán thêm cùng callback bằng tay, tránh gọi hai lần.

| Button | Object/hệ thống đích | Callback và tham số |
|---|---|---|
| MainMenu/Start | SceneLoader | StartLab(0) |
| MainMenu/Lesson_1/2/8/30/31 | SceneLoader | StartLab(mã bài tương ứng) |
| Station/Begin | LessonManager của bàn đang mở | Begin() |
| Station/Retry | StationController của bàn đang mở | ResetLesson() |
| Station/Questions | LabUI | OpenResult() |
| Station/ExitStation | GameManager | ExitStation() |
| Station/RadiusMode | Lesson31Manager | SetMode(0) |
| Station/MetalMode | Lesson31Manager | SetMode(1) |
| Result/Answer_i | LessonManager | Answer(i) rồi refresh kết quả |
| Pause/Resume | GameManager | SetPause(false) |
| Options/ClearProgress | GameManager | ClearProgress() |
| Quit | SceneLoader | Quit() |

Các field trên không phải yêu cầu kéo tay khi mở project bàn giao. Mapping phục vụ kiểm tra/debug và thay model có kiểm soát.
