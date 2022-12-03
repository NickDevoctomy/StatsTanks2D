using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Map : MonoBehaviour
{
    public Texture2D MapTexture;
    public Material WallMaterial;
    public float WallHeight = 1;
    public float WallYOffset = 0;
    public bool ForceRegeneration = true;

    void Start()
    {
        Generate();
    }

    void Update()
    {
        
    }

    public void Generate()
    {
        var walls = transform.Find("Walls");
        if (walls == null || ForceRegeneration)
        {
            if(walls != null)
            {
                DestroyImmediate(walls.gameObject);
            }

            GenerateWalls();
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

    private void GenerateWalls()
    {
        var allWalls = new List<GameObject>();
        var offset = new Vector2(-(MapTexture.width / 2), -(MapTexture.height / 2));
        for (int x = 0; x < MapTexture.width; x++)
        {
            for (int y = 0; y < MapTexture.height; y++)
            {
                Color pixel = MapTexture.GetPixel(x, y);
                var isWall = (pixel == Color.black);
                if (isWall)
                {
                    var wall = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    wall.name = $"Wall({x},{y})";
                    wall.transform.localScale = new Vector3(1, WallHeight, 1);
                    wall.transform.position = new Vector3(offset.x + x, 0f + WallYOffset, offset.y + y);
                    wall.transform.parent = transform;
                    allWalls.Add(wall);
                }
            }
        }

        MergeWalls(allWalls);
    }

    private void GenerateFloor()
    {
        var floor = GameObject.CreatePrimitive(PrimitiveType.Plane);
        floor.name = "Floor";
        floor.transform.localScale = new Vector3(MapTexture.width / 9, 1, MapTexture.height / 9);
        floor.transform.position = new Vector3(0, -0.5f, 0);
        floor.transform.parent = transform;
    }

    private void MergeWalls(
        List<GameObject> walls)
    {
        var combineInstances = walls.Select(x => CreateCombineInstanceFromGameObject(x)).ToArray();
        walls.ForEach(x => DestroyImmediate(x));

        var wallsMesh = new GameObject("Walls");
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
