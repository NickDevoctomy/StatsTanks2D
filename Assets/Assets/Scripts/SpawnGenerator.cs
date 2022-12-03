using UnityEngine;

public class SpawnGenerator : IGenerator
{
    public string Key => "Spawn";

    public GameObject Generate(
        Transform parent,
        Vector2 position,
        Vector2 offset,
        float ceilingHeight,
        float yOffset)
    {
        var cell = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cell.name = $"{Key}({position.x},{position.y})";
        cell.transform.localScale = new Vector3(1f, 0.1f, 1f);
        cell.transform.position = new Vector3(offset.x + position.x, 0f + yOffset, offset.y + position.y);
        cell.transform.parent = parent;
        return cell;
    }

    public bool IsApplicable(Color color)
    {
        return color == Color.red;
    }
}
