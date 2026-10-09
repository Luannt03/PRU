using UnityEngine;
using ChemLab9.Core;
using ChemLab9.Data;
using ChemLab9.Interaction;
using ChemLab9.Lessons;
namespace ChemLab9.PeriodicTable
{
    public class PeriodicTableBuilder : MonoBehaviour
    {
        public static Color CategoryColor(ElementCategory category)
        {
            switch (category)
            {
                case ElementCategory.AlkaliMetal: return new Color(.78f,.35f,.22f);
                case ElementCategory.AlkalineEarth: return new Color(.75f,.6f,.2f);
                case ElementCategory.Metal: return new Color(.3f,.55f,.8f);
                case ElementCategory.Metalloid: return new Color(.4f,.6f,.4f);
                case ElementCategory.Nonmetal: return new Color(.2f,.6f,.6f);
                case ElementCategory.Halogen: return new Color(.45f,.35f,.7f);
                default: return new Color(.5f,.55f,.65f);
            }
        }
        public void Build(StationController station, Lesson30Manager lesson, ChemicalDatabase database)
        {
            LabFactory f = ChemLabBootstrap.Factory;
            f.Shape("Board", transform, new Vector3(0,2.12f,.55f), new Vector3(3.45f,1.05f,.1f), new Color(.06f,.12f,.18f));
            foreach (ElementRecord element in database.elements)
            {
                Vector3 p = new Vector3((element.group - 9.5f) * .18f, 2.5f - (element.period - 1) * .23f, .46f);
                GameObject tile = f.Shape("ElementTile_" + element.symbol, transform, p, new Vector3(.16f,.20f,.055f), CategoryColor(element.category), PrimitiveType.Cube, true);
                Interactable action = tile.AddComponent<Interactable>(); action.Station = station; action.Label = "Click " + element.symbol + " • " + element.name; action.Click = () => lesson.Select(element);
                // Text parent is the station, so its size is independent of the scaled tile mesh.
                f.Label("Label_" + element.symbol, transform, p + Vector3.back * .04f, element.atomicNumber + " " + element.symbol + "\n" + element.name, .065f).rectTransform.sizeDelta = new Vector2(.165f,.20f);
            }
            f.PanelLabel("Scope", transform, new Vector3(0,1.45f,.44f), "20 nguyên tố đầu • Các vị trí còn lại chưa triển khai", new Vector2(3.4f,.16f), .07f, new Color(.06f,.12f,.18f));
            f.PanelLabel("Legend", transform, new Vector3(0,1.24f,.44f), "Cam: kim loại kiềm • Vàng: kiềm thổ • Xanh lam: kim loại\nLục: á kim • Xanh ngọc: phi kim • Tím: halogen • Xám: khí hiếm", new Vector2(3.4f,.23f), .055f, new Color(.06f,.12f,.18f));
        }
    }
}
