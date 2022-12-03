using System.Collections;
using System.Collections.Generic;
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
        var wall = GameObject.CreatePrimitive(PrimitiveType.Cube);
        wall.name = $"{Key}({position.x},{position.y})";
        wall.transform.localScale = new Vector3(1f, 0.5f, 1f);
        wall.transform.position = new Vector3(offset.x + position.x, 0f, offset.y + position.y);
        wall.transform.parent = parent;
        return wall;
    }

    public bool IsApplicable(Color color)
    {
        return color == Color.red;
    }
}
