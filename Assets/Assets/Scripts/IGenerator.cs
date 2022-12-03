using UnityEngine;

public interface IGenerator 
{
    public string Key { get; }
    bool IsApplicable(Color color);
    GameObject Generate(
        Transform parent,
        Vector2 position,
        Vector2 offset,
        float ceilingHeight,
        float yOffset);
}
