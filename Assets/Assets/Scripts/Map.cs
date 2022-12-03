using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Map : MonoBehaviour
{
    public CellGroupInfo[] CellGroups;
    public Texture2D MapTexture;
    public Material WallMaterial;
    public float CellHeight = 1;
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
        foreach(var curGenerator in _generators)
        {
            var cellGroup = transform.Find(curGenerator.Key);
            if (cellGroup == null || ForceRegeneration)
            {
                if (cellGroup != null)
                {
                    DestroyImmediate(cellGroup.gameObject);
                }

                DoGenerateCells(curGenerator);
            }
        }
    }

    private void DoGenerateCells(IGenerator generator)
    {
        var cellGroupInfo = CellGroups.SingleOrDefault(x => x.Key == generator.Key);
        var generatedByKey = new Dictionary<string, List<GameObject>>();
        var offset = new Vector2(-(MapTexture.width / 2), -(MapTexture.height / 2));
        for (int x = 0; x < MapTexture.width; x++)
        {
            for (int y = 0; y < MapTexture.height; y++)
            {
                Color pixel = MapTexture.GetPixel(x, y);     
                if(generator.IsApplicable(pixel))
                {
                    var cell = generator.Generate(
                        transform,
                        new Vector2(x, y),
                        offset,
                        CellHeight,
                        cellGroupInfo.YOffset);

                    if (!generatedByKey.ContainsKey(generator.Key))
                    {
                        generatedByKey.Add(generator.Key, new List<GameObject>());
                    }

                    generatedByKey[generator.Key].Add(cell);
                }
            }
        }

        foreach(var curCellGroup in generatedByKey.Keys)
        {
            MergeCells(
                curCellGroup,
                generatedByKey[curCellGroup].ToList(),
                cellGroupInfo.Material);
        }
    }

    private void MergeCells(
        string Name,
        List<GameObject> cells,
        Material material)
    {
        var combineInstances = cells.Select(x => CreateCombineInstanceFromGameObject(x)).ToArray();
        cells.ForEach(x => DestroyImmediate(x));

        var wallsMesh = new GameObject(Name);
        wallsMesh.transform.parent = transform;
        var meshRenderer = wallsMesh.AddComponent<MeshRenderer>();
        meshRenderer.material = material;
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
