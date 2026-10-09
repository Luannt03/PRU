using System.Collections.Generic;
using ChemLab9.Data;
using ChemLab9.PeriodicTable;
namespace ChemLab9.Lessons
{
    public class Lesson31Manager : LessonManager
    {
        public PeriodicTrendVisualizer Visualizer;
        public int Mode { get; private set; }
        public bool RadiusSolved { get; private set; }
        bool viewedMetallic;
        readonly List<string> order = new List<string>();
        public override bool Ready => RadiusSolved && viewedMetallic;
        public override string Status => (Mode == 0 ? "Click các mô hình theo bán kính tăng: Li, Na, K.\nĐã chọn: " + string.Join(" → ", order) : "Chế độ B: quan sát mũi tên và thanh minh họa.\nTrong chu kỳ, tính kim loại nhìn chung giảm từ trái sang phải; tính phi kim tăng.\nTrong nhóm chính, tính kim loại nhìn chung tăng từ trên xuống dưới.\nKhí hiếm có lớp ngoài bền; không xếp đơn giản vào chiều tăng tính phi kim.") + "\nMô hình minh họa, không theo tỉ lệ thực; không có đơn vị pm.";
        public void SetMode(int mode)
        {
            if (!Started) return;
            Mode = mode; order.Clear(); if (mode == 1) viewedMetallic = true;
            Visualizer.ShowMode(mode); Feedback = mode == 0 ? "Quan sát Na–Mg–Al và Li–Na–K; click Li, Na, K theo thứ tự tăng." : "Đọc mũi tên xu hướng và so sánh các thanh mức độ minh họa.";
        }
        public void Select(string symbol)
        {
            if (!Started || Mode != 0 || RadiusSolved) return;
            if (symbol != "Li" && symbol != "Na" && symbol != "K") { Feedback = "Nhiệm vụ sắp xếp sử dụng Li, Na, K."; return; }
            if (order.Contains(symbol)) { Feedback = "Không chọn lặp. Click nguyên tố tiếp theo."; return; }
            order.Add(symbol);
            if (order.Count == 3)
            {
                RadiusSolved = ElementCatalog.IsRadiusOrder(order.ToArray());
                Feedback = RadiusSolved ? "Đúng: Li < Na < K. Chuyển chế độ B rồi trả lời câu hỏi." : "Chưa đúng. Bán kính tăng khi đi xuống nhóm Li–Na–K. Chọn lại từ đầu.";
                if (!RadiusSolved) order.Clear();
            }
        }
        public override void ResetLesson() { base.ResetLesson(); RadiusSolved = viewedMetallic = false; order.Clear(); Mode = 0; Visualizer.ShowMode(0); }
    }
}
