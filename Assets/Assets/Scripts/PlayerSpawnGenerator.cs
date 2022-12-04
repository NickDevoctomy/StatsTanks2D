using UnityEngine;

public class PlayerSpawnGenerator : IGenerator
{
    public string Key => "PlayerSpawn";

    public GameObject Generate(
        Transform parent,
        Vector2 position,
        Vector2 offset,
        float ceilingHeight,
        float yOffset,
        Material material)
    {
        var cell = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cell.name = $"{Key}({position.x},{position.y})";
        cell.transform.localScale = new Vector3(1f, 0.1f, 1f);
        cell.transform.position = new Vector3(offset.x + position.x, 0f + yOffset, offset.y + position.y);
        cell.transform.parent = parent;

        if(material != null)
        {
            var meshRenderer = cell.GetComponent<MeshRenderer>();
            meshRenderer.material = material;
        }

        var spawnPoint = new GameObject();
        spawnPoint.name = $"{Key}({position.x},{position.y})";
        spawnPoint.transform.parent = cell.transform;
        spawnPoint.transform.localPosition = new Vector3(0f, 10f, 0f);
        spawnPoint.tag = "PlayerSpawnPoint";
        return cell;
    }

    public bool IsApplicable(Color color)
    {
        return color == Color.red;
    }
}
