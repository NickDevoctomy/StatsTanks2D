using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class WallGenerator : IGenerator
{
    public string Key => "Wall";
    public GameObject Generate(
        Transform parent,
        Vector2 position,
        Vector2 offset,
        float ceilingHeight,
        float yOffset)
    {
        var wall = GameObject.CreatePrimitive(PrimitiveType.Cube);
        wall.name = $"{Key}({position.x},{position.y})";
        wall.transform.localScale = new Vector3(1, ceilingHeight, 1);
        wall.transform.position = new Vector3(offset.x + position.x, 0f + yOffset, offset.y + position.y);
        wall.transform.parent = parent;
        return wall;
    }

    public bool IsApplicable(Color color)
    {
        return color == Color.black;
    }
}
