using UnityEngine;
namespace SwimDemo
{
    public class WaterSurface : MonoBehaviour
    {
        public double SimulationTime { get; private set; }
        Material material;
        Mesh mesh;
        public void Build(Material waterMaterial)
        {
            material = waterMaterial;
            const int nx = 90, nz = 60;
            var vertices = new Vector3[(nx + 1) * (nz + 1)];
            var uv = new Vector2[vertices.Length]; var triangles = new int[nx * nz * 6];
            for (int z = 0; z <= nz; z++) for (int x = 0; x <= nx; x++)
            {
                int i = z * (nx + 1) + x; vertices[i] = new Vector3(-9 + 18f * x / nx, 0, -6 + 12f * z / nz);
                uv[i] = new Vector2((float)x / nx, (float)z / nz);
            }
            int t = 0;
            for (int z = 0; z < nz; z++) for (int x = 0; x < nx; x++)
            {
                int i = z * (nx + 1) + x;
                triangles[t++] = i; triangles[t++] = i + nx + 1; triangles[t++] = i + 1;
                triangles[t++] = i + 1; triangles[t++] = i + nx + 1; triangles[t++] = i + nx + 2;
            }
            mesh = new Mesh { name = "PoolWaveGrid", vertices = vertices, uv = uv, triangles = triangles };
            mesh.RecalculateNormals(); mesh.bounds = new Bounds(Vector3.zero, new Vector3(18, .4f, 12));
            gameObject.AddComponent<MeshFilter>().sharedMesh = mesh;
            gameObject.AddComponent<MeshRenderer>().sharedMaterial = material;
        }
        public float Height(float x, float z) => SwimKinematics.WaterHeight(x, z, SimulationTime);
        void Update()
        {
            if (Time.timeScale <= 0) return;
            SimulationTime += Time.deltaTime;
            if (material != null) material.SetFloat("_WaveTime", (float)SimulationTime);
        }
        void OnDestroy() { if (mesh != null) Destroy(mesh); }
    }
}
