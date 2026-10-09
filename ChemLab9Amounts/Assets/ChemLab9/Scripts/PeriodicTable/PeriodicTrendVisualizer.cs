using System.Collections.Generic;
using UnityEngine;
using ChemLab9.Core;
using ChemLab9.Data;
using ChemLab9.Interaction;
using ChemLab9.Lessons;
namespace ChemLab9.PeriodicTable
{
    public class PeriodicTrendVisualizer : MonoBehaviour
    {
        readonly List<Transform> spheres = new List<Transform>();
        readonly List<float> targets = new List<float>();
        GameObject radiusRoot, metallicRoot;
        static readonly Color BoardColor = new Color(.045f, .09f, .14f);
        static readonly Color NoteColor = new Color(.10f, .23f, .30f);
        public void Build(StationController station, Lesson31Manager lesson, ChemicalDatabase database)
        {
            LabFactory f = ChemLabBootstrap.Factory;
            f.Shape("TrendBoard", transform, new Vector3(0, 2.30f, .62f), new Vector3(3.4f, 2.42f, .10f), BoardColor);
            radiusRoot = f.Empty("RadiusMode", transform, Vector3.zero);
            metallicRoot = f.Empty("MetallicMode", transform, Vector3.zero);
            f.PanelLabel("DownGroup", radiusRoot.transform, new Vector3(0, 3.36f, .50f),
                "Xuống nhóm: Li -> Na -> K | Bán kính tăng", new Vector2(3.18f, .22f), .115f, NoteColor);
            f.PanelLabel("AcrossPeriod", radiusRoot.transform, new Vector3(0, 2.24f, .50f),
                "Chu kỳ 3: Na -> Mg -> Al | Bán kính giảm", new Vector2(3.18f, .17f), .095f, NoteColor);
            string[] symbols = { "Li", "Na", "K", "Na", "Mg", "Al" };
            for (int i = 0; i < symbols.Length; i++)
            {
                ElementRecord element = database.Element(symbols[i]);
                string symbol = element.symbol;
                float row = i < 3 ? 2.80f : 1.78f;
                float x = (i % 3 - 1) * 1.06f;
                Color category = PeriodicTableBuilder.CategoryColor(element.category);
                f.Shape("ElementCard_" + i, radiusRoot.transform, new Vector3(x, row, .51f),
                    new Vector3(.97f, .92f, .04f), Color.Lerp(category, BoardColor, .72f));
                GameObject sphere = f.Shape("AtomModel_" + i + "_" + symbol, radiusRoot.transform,
                    new Vector3(x, row + .12f, .16f), Vector3.one * .18f, category, PrimitiveType.Sphere, true);
                Interactable click = sphere.AddComponent<Interactable>();
                click.Station = station; click.Label = "Click nguyên tử " + symbol; click.Click = () => lesson.Select(symbol);
                spheres.Add(sphere.transform);
                // Keep illustrations inside their cards, even if a database value is edited.
                targets.Add(Mathf.Clamp(element.relativeRadius * .46f, .24f, .60f));
                f.PanelLabel("Name_" + i, radiusRoot.transform, new Vector3(x, row - .33f, .12f),
                    symbol + " - " + element.name, new Vector2(.88f, .22f), .135f, NoteColor);
            }
            f.PanelLabel("TrendExplanation", metallicRoot.transform, new Vector3(0, 2.90f, .45f),
                "Trong chu kỳ, từ trái sang phải:\nKim loại giảm | Phi kim tăng\nTrong nhóm chính, từ trên xuống dưới:\nKim loại tăng | Phi kim giảm",
                new Vector2(3.18f, .90f), .13f, NoteColor);
            float[] strengths = { 1f, .7f, .4f };
            string[] period = { "Na", "Mg", "Al" };
            for (int i = 0; i < period.Length; i++)
            {
                float x = (i - 1) * 1.06f;
                f.Shape("MetallicCard_" + i, metallicRoot.transform, new Vector3(x, 1.98f, .51f),
                    new Vector3(.97f, .80f, .04f), new Color(.08f, .16f, .22f));
                f.Shape("MetallicBar_" + period[i], metallicRoot.transform, new Vector3(x, 1.82f + strengths[i] * .23f, .20f),
                    new Vector3(.32f, strengths[i] * .46f, .12f), new Color(.25f, .7f, .8f));
                f.PanelLabel("BarLabel_" + i, metallicRoot.transform, new Vector3(x, 1.66f, .12f),
                    period[i], new Vector2(.88f, .20f), .135f, NoteColor);
            }
            f.PanelLabel("NobleGasNote", metallicRoot.transform, new Vector3(0, 1.45f, .45f),
                "Khí hiếm: lớp ngoài bền; cần giải thích riêng.", new Vector2(3.18f, .22f), .10f, NoteColor);
            f.PanelLabel("Illustration", transform, new Vector3(0, 1.22f, .45f),
                "Mô hình minh họa, không theo tỉ lệ thực.\nKhông có pm; thanh mức độ chỉ minh họa.",
                new Vector2(3.18f, .22f), .10f, NoteColor);
            ShowMode(0);
        }
        public void ShowMode(int mode)
        {
            if (radiusRoot == null) return;
            radiusRoot.SetActive(mode == 0); metallicRoot.SetActive(mode == 1);
            foreach (Transform sphere in spheres) sphere.localScale = Vector3.one * .18f;
        }
        void Update()
        {
            if (radiusRoot == null || !radiusRoot.activeSelf) return;
            for (int i = 0; i < spheres.Count; i++)
                spheres[i].localScale = Vector3.MoveTowards(spheres[i].localScale, Vector3.one * targets[i], Time.deltaTime * .4f);
        }
    }
}
