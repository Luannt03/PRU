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
        public void Build(StationController station, Lesson31Manager lesson, ChemicalDatabase database)
        {
            LabFactory f = ChemLabBootstrap.Factory;
            radiusRoot = f.Empty("RadiusMode", transform, Vector3.zero); metallicRoot = f.Empty("MetallicMode", transform, Vector3.zero);
            string[] symbols = { "Li", "Na", "K", "Na", "Mg", "Al" };
            for (int i=0;i<symbols.Length;i++)
            {
                string symbol = symbols[i]; Vector3 p = new Vector3((i%3-1)*.85f, i<3?1.85f:1.23f, .25f);
                GameObject sphere = f.Shape("AtomModel_" + symbol, radiusRoot.transform, p, Vector3.one * .15f, PeriodicTableBuilder.CategoryColor(database.Element(symbol).category), PrimitiveType.Sphere, true);
                Interactable click = sphere.AddComponent<Interactable>(); click.Station = station; click.Label = "Click nguyên tử " + symbol; click.Click = () => lesson.Select(symbol);
                spheres.Add(sphere.transform); targets.Add(database.Element(symbol).relativeRadius * .37f);
                f.Label("Name_"+i, radiusRoot.transform, p + Vector3.down*.27f + Vector3.back * .15f, symbol, .1f);
            }
            f.Label("DownGroup", radiusRoot.transform, new Vector3(0,2.3f,.3f), "Xuống nhóm: Li → Na → K • bán kính tăng", .08f);
            f.Label("AcrossPeriod", radiusRoot.transform, new Vector3(0,.86f,.1f), "Trong chu kỳ 3: Na → Mg → Al • bán kính giảm", .08f);
            f.Label("TrendExplanation", metallicRoot.transform, new Vector3(0,2.18f,.3f), "Trong chu kỳ →\nTính kim loại giảm • tính phi kim tăng\n\nTrong nhóm chính ↓\nTính kim loại tăng • tính phi kim giảm", .11f);
            float[] strengths = { 1f, .7f, .4f };
            string[] period = { "Na", "Mg", "Al" };
            for (int i=0;i<3;i++)
            {
                float x=(i-1)*.85f;
                f.Shape("MetallicBar_"+period[i], metallicRoot.transform, new Vector3(x,1.15f + strengths[i]*.2f,.25f), new Vector3(.22f,strengths[i]*.4f,.12f), new Color(.25f,.7f,.8f));
                f.Label("BarLabel_"+i, metallicRoot.transform, new Vector3(x,1f,.1f), period[i], .1f);
            }
            f.Label("Illustration", transform, new Vector3(0,.62f,.1f), "Mô hình minh họa, không theo tỉ lệ thực; không dùng pm.\nThanh mức độ không phải một đại lượng hóa học đo được.", .065f);
            ShowMode(0);
        }
        public void ShowMode(int mode)
        {
            if (radiusRoot == null) return;
            radiusRoot.SetActive(mode == 0); metallicRoot.SetActive(mode == 1);
            foreach (Transform sphere in spheres) sphere.localScale = Vector3.one*.15f;
        }
        void Update()
        {
            if (radiusRoot == null || !radiusRoot.activeSelf) return;
            for (int i=0;i<spheres.Count;i++) spheres[i].localScale = Vector3.MoveTowards(spheres[i].localScale, Vector3.one*targets[i], Time.deltaTime * .4f);
        }
    }
}
