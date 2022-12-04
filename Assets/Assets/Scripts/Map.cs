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
        if(!cellGroupInfo.Enabled)
        {
            return;
        }

        var generated = new List<GameObject>();
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
                        cellGroupInfo.YOffset,
                        cellGroupInfo.Material);

                    generated.Add(cell);
                }
            }
        }

        if(cellGroupInfo.Merge)
        {
            MergeCells(
                cellGroupInfo.Key,
                generated,
                cellGroupInfo.Material,
                generated[0].tag);
        }
        else
        {
            var group = new GameObject(cellGroupInfo.Key);
            group.transform.parent = transform;
            group.SetActive(true);
            generated.ForEach(x =>
            {
                x.transform.parent = group.transform;
            });
        }
    }

    private void MergeCells(
        string Name,
        List<GameObject> cells,
        Material material,
        string tag)
    {
        var combineInstances = cells.Select(x => CreateCombineInstanceFromGameObject(x)).ToArray();
        cells.ForEach(x => DestroyImmediate(x));

        var combined = new GameObject(Name);
        combined.transform.parent = transform;
        var meshRenderer = combined.AddComponent<MeshRenderer>();
        meshRenderer.material = material;
        var meshFilter = combined.AddComponent<MeshFilter>();
        meshFilter.sharedMesh = new Mesh
        {
            indexFormat = UnityEngine.Rendering.IndexFormat.UInt32
        };
        meshFilter.sharedMesh.CombineMeshes(combineInstances);
        var meshCollider = combined.AddComponent<MeshCollider>();
        meshCollider.sharedMesh = meshFilter.sharedMesh;
        combined.tag = tag;
        combined.SetActive(true);
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
