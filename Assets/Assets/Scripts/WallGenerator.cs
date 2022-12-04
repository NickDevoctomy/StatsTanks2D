using UnityEngine;

public class WallGenerator : IGenerator
{
    public string Key => "Wall";
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
        cell.transform.localScale = new Vector3(1, ceilingHeight, 1);
        cell.transform.position = new Vector3(offset.x + position.x, 0f + yOffset, offset.y + position.y);
        cell.transform.parent = parent;
        return cell;
    }

    public bool IsApplicable(Color color)
    {
        return color == Color.black;
    }
}
