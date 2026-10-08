# Kết quả kiểm tra bản bàn giao

Kiểm tra thực hiện trong cloud bằng .NET SDK 8.0.425, tải từ nguồn Microsoft và kiểm tra SHA-512 theo release metadata trước khi dùng.

| Kiểm tra | Kết quả |
|---|---|
| 30 tình huống logic dùng code production | Đạt 30/30 |
| Phân tích cú pháp C# bằng Roslyn, chế độ C# 9 + UNITY_EDITOR | Đạt 51 file; đây không phải compile với Unity assemblies |
| 37 file YAML mới, GUID, local fileID và reference bootstrap | Đạt |
| Dữ liệu lưu của 20 nguyên tố, tổng electron và chu kỳ | Đạt |
| 5 bài, 4 phản ứng, 10 chất/vật dụng chỉ thị, đáp án quiz | Đạt |
| Hai scene build, đúng 5 root station, 13 wrapper/template prefab | Đạt |
| Input cũ và reference pipeline nhất quán | Đạt kiểm tra tĩnh |
| Asset gốc của nhà phát hành so với ZIP upload | Giữ nguyên |
| Import/compile thật bằng Unity 6000.6.0f1 | Chưa chạy: cloud không có Editor/license |
| NUnit EditMode trong Unity | Đã cung cấp 30 tình huống; chưa chạy bằng Unity |
| Unity PlayMode integration tests | Đã cung cấp 2 bài test bootstrap/collider và Pause/heating; chưa chạy bằng Unity |
| Play Mode, hình ảnh, click/drag/UI, Pause và lưu PlayerPrefs | Chưa chạy; có checklist để kiểm tra |
| Build/EXE Windows ngoài Editor | Chưa chạy; đã thêm lệnh build và cấu hình scene |

Các tình huống logic gồm: CO2 cần nối ống và đúng nguồn/chất; giới hạn liều; kết tủa nước vôi; CaO thiếu nước/sai mẫu; chỉ thị trong nước và bazơ; heating ngoài zone/tắt sớm/pause/không đảo ngược; reset; lượng âm/NaN/vượt dung tích; dữ liệu electron; xu hướng; điểm không cộng lặp.

Lỗi phát hiện và đã sửa bằng regression test: sai số số thực khi dẫn CO2 từng phần có thể dừng trước liều 1. Bộ test kiểm tra liều 0.03 lặp nhiều lần và việc giới hạn dẫn dư.

Chưa có bằng chứng để gọi bản bàn giao là đã nghiệm thu demo Windows. Mã nguồn, scene seed, dữ liệu và hướng dẫn đã được đóng gói; các bước Unity còn lại cần chạy trên máy đã cài Editor của bạn.
