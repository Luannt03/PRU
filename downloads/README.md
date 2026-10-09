# Bản project Unity để tải riêng

`SwimDemo_Unity6000.6.zip` chứa **project Unity source**, với file `YBot_Swim.fbx` đã cung cấp. ZIP không chứa cache Library hoặc bản Windows đã build.

Trong GitHub, mở file ZIP và nhấn **Download raw file**, sau đó giải nén. Trong Unity Hub, chọn thư mục `SwimDemo` chứa `Assets`, `Packages`, `ProjectSettings`; dùng Unity `6000.6.0f1`, mở scene Swimming rồi Play. Hướng dẫn đầy đủ nằm trong `SwimDemo/README.md` bên trong ZIP.

Để tạo lại ZIP sau khi sửa source, chạy `python tools/package_swim_demo.py` từ thư mục repository. Các kiểm tra đã thực hiện và các kiểm tra Unity còn cần chạy được ghi trong README của project.
