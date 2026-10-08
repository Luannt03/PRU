using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

public static class ClassroomSetupTool
{
    [MenuItem("ChemLab/3. Setup Classroom Hierarchy & Volume")]
    public static void SetupClassroomHierarchy()
    {
        string[] containers = new string[]
        {
            "--- ENVIRONMENT ---",
            "--- FURNITURE ---",
            "--- APPARATUS_STATIC ---",
            "--- LIGHTING & VOLUME ---",
            "--- MANAGERS ---"
        };

        foreach (string name in containers)
        {
            if (GameObject.Find(name) == null)
            {
                GameObject go = new GameObject(name);
                go.transform.position = Vector3.zero;
            }
        }

        GameObject lightingContainer = GameObject.Find("--- LIGHTING & VOLUME ---");
        GameObject globalVolumeObj = GameObject.Find("Global Volume");

        if (globalVolumeObj != null && lightingContainer != null)
        {
            globalVolumeObj.transform.SetParent(lightingContainer.transform);
        }

        AssetDatabase.Refresh();
        Debug.Log("[ChemLab] Đã tạo xong Hierarchy!");
    }
}