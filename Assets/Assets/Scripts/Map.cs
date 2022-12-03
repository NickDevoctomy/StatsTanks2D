using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Map : MonoBehaviour
{
    public Texture2D MapTexture;
    public Material WallMaterial;
    public float CellHeight = 1;
    public float CellYOffset = 0;
    public bool ForceRegeneration = true;

    private readonly List<IGenerator> _generators = GeneratorUtility.CreateGenerators();

    void Start()
    {
        Generate();
    }

    void Update()
    {
        
    }

    public void Generate()
    {
        var cells = transform.Find("Cells");
        if (cells == null || ForceRegeneration)
        {
            if(cells != null)
            {
                DestroyImmediate(cells.gameObject);
            }

            GenerateCells();
        }

        var floor = transform.Find("Floor");
        if (floor == null || ForceRegeneration)
        {
            if(floor != null)
            {
                DestroyImmediate(floor.gameObject);
            }

            GenerateFloor();
        }
    }

    private void GenerateCells()
    {
        var generatedByKey = new Dictionary<string, List<GameObject>>();
        var offset = new Vector2(-(MapTexture.width / 2), -(MapTexture.height / 2));
        for (int x = 0; x < MapTexture.width; x++)
        {
            for (int y = 0; y < MapTexture.height; y++)
            {
                Color pixel = MapTexture.GetPixel(x, y);     
                var generator = _generators.SingleOrDefault(x => x.IsApplicable(pixel));
                if(generator != null)
                {
                    var wall = generator.Generate(
                        transform,
                        new Vector2(x, y),
                        offset,
                        CellHeight,
                        CellYOffset);

                    if (!generatedByKey.ContainsKey(generator.Key))
                    {
                        generatedByKey.Add(generator.Key, new List<GameObject>());
                    }

                    generatedByKey[generator.Key].Add(wall);
                }
            }
        }

        MergeCells(
            "Walls",
            generatedByKey["Wall"].ToList());    // Merge all groups?
    }

    private void GenerateFloor()
    {
        var floor = GameObject.CreatePrimitive(PrimitiveType.Plane);
        floor.name = "Floor";
        floor.transform.localScale = new Vector3(MapTexture.width / 9, 1, MapTexture.height / 9);
        floor.transform.position = new Vector3(0, -0.5f, 0);
        floor.transform.parent = transform;
    }

    private void MergeCells(
        string Name,
        List<GameObject> cells)
    {
        var combineInstances = cells.Select(x => CreateCombineInstanceFromGameObject(x)).ToArray();
        cells.ForEach(x => DestroyImmediate(x));

        var wallsMesh = new GameObject(Name);
        wallsMesh.transform.parent = transform;
        var meshRenderer = wallsMesh.AddComponent<MeshRenderer>();
        meshRenderer.material = WallMaterial;
        var meshFilter = wallsMesh.AddComponent<MeshFilter>();
        meshFilter.sharedMesh = new Mesh
        {
            indexFormat = UnityEngine.Rendering.IndexFormat.UInt32
        };
        meshFilter.sharedMesh.CombineMeshes(combineInstances);
        var meshCollider = wallsMesh.AddComponent<MeshCollider>();
        meshCollider.sharedMesh = meshFilter.sharedMesh;
        wallsMesh.SetActive(true);
    }

    private CombineInstance CreateCombineInstanceFromGameObject(GameObject gameObject)
    {
        var meshFilter = gameObject.GetComponent<MeshFilter>();
        return new CombineInstance
        {
            mesh = meshFilter.sharedMesh,
            transform = gameObject.transform.localToWorldMatrix
        };
    }
}
