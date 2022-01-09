using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;

[RequireComponent(typeof(MeshFilter))]
public class HeightMapGenerator : MonoBehaviour
{
    [Header("Noise Settings")]
    public int seed, mapSize = 256;
    public bool randomizeSeed;
    public int numOctaves = 7;
    public float persistence = 0.5f;
    public float lacunarity = 2f;
    public float scale = 2f, elevationScale = 10f;

    [Header("Erosion")]
    public int delay = 3;
    public int iters = 1000;
    public int iters_per_frame = 100;

    bool running = false;
    public Color Normal;
    public Color Running;
    public ErosionUI ErosionUI;

    internal float[] map;
    internal MeshFilter MeshFilter; 

    private void Start()
    {
        //Get MeshFilter
        MeshFilter = GetComponent<MeshFilter>();

        //Generate Terrain
        GenerateTerrain(mapSize);
        GenerateMesh(map);
    }

    public void Erode_OnPress()
    {
        if (running) return;
        //Get Erosion
        Erosion er = FindObjectOfType<Erosion>();
        StartCoroutine(Erode(er));
    }


    public void Randomize_OnPress()
    {
        if (running) return;
        seed = Random.Range(-10000, 10000);
        persistence = Random.Range(persistence / 1.2f, persistence * 1.2f);
        lacunarity = Random.Range(lacunarity / 1.2f, lacunarity * 1.2f);
        //elevationScale = Random.Range(0.8f, 1.2f) * elevationScale;
        GenerateTerrain(mapSize);
        GenerateMesh(map);
    }

    IEnumerator Erode(Erosion er)
    {
        Camera.main.backgroundColor = Running;
        ErosionUI.EnableLookAround();
        running = true;
        for (int i = 0; i < iters / iters_per_frame; i++)
        {
            yield return Erode_Single(er);
            GenerateMesh(map);
        }
        Camera.main.backgroundColor = Normal;
        ErosionUI.EndLook();
        running = false;
    }

    public void GenerateMesh(float[] map)
    {
        //Define Vertices and triangles
        Vector3[] verts = new Vector3[mapSize * mapSize];
        int[] triangles = new int[(mapSize - 1) * (mapSize - 1) * 6];
        int t = 0;

        //Loop through and create mesh
        for (int y = 0; y < mapSize; y++)
        {
            for (int x = 0; x < mapSize; x++)
            {
                int i = y * mapSize + x;

                Vector2 percent = new Vector2(x / (mapSize - 1f), y / (mapSize - 1f));
                Vector3 pos = new Vector3(percent.x * 2 - 1, 0, percent.y * 2 - 1) * scale;
                pos += Vector3.up * map[i] * elevationScale;
                verts[i] = pos;

                // Construct triangles
                if (x != mapSize - 1 && y != mapSize - 1)
                {

                    triangles[t + 0] = i + mapSize;
                    triangles[t + 1] = i + mapSize + 1;
                    triangles[t + 2] = i;

                    triangles[t + 3] = i + mapSize + 1;
                    triangles[t + 4] = i + 1;
                    triangles[t + 5] = i;
                    t += 6;
                }
            }
        }

        //Now, use those values in the mesh
        Mesh mesh = new Mesh();
        mesh.indexFormat = IndexFormat.UInt32;
        mesh.vertices = verts;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
        MeshFilter.sharedMesh = mesh;
    }

    public float[] GenerateTerrain(int mapsize)
    {
        //Generate map
        float[] map = new float[mapsize * mapsize];

        //Initialize Seed
        seed = (randomizeSeed) ? UnityEngine.Random.Range(-10000, 10000) : seed;

        //Create Random
        System.Random random = new System.Random(seed);

        //Create Offsets
        Vector2[] offsets = new Vector2[numOctaves];
        for (int i = 0; i < numOctaves; i++){
            offsets[i] = new Vector2(random.Next(-1000, 1000), random.Next(-1000, 1000));
        }
        
        //Define some values
        float minValue = float.MaxValue;
        float maxValue = float.MinValue;

        //PERLIN NOISE TIME BOIZ!
        for (int y = 0; y < mapsize; y++)
        {
            for (int x = 0; x < mapsize; x++)
            {
                float noise = 0f;
                float scale = this.scale;
                float weight = 1;
                for (int i = 0; i < numOctaves; i++){
                    Vector2 p = offsets[i] + new Vector2(x / (float)mapsize, y / (float)mapsize) * scale;
                    noise += Mathf.PerlinNoise(p.x, p.y) * weight;
                    weight *= persistence;
                    scale *= lacunarity;
                }

                //Assign Noise
                map[y * mapsize + x] = noise;

                //Define Min & max
                minValue = Mathf.Min(noise, minValue);
                maxValue = Mathf.Max(noise, maxValue);
            }
        }

        //Normalize
        if (maxValue != minValue){
            for (int i = 0; i < map.Length; i++)
            {
                map[i] = (map[i] - minValue) / (maxValue - minValue);
            }
        }

        this.map = map;
        return map;
    }

    public IEnumerator Erode_Single(Erosion er)
    {
        yield return new WaitForSeconds(delay);
        float[] map_ = er.Erode(map, mapSize, iters_per_frame);
        map = map_;
    }
}
