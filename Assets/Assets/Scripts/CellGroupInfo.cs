using System;
using UnityEngine;

[Serializable]
public struct CellGroupInfo
{
    public bool Enabled;
    public string Key;
    public Material Material;
    public float YOffset;
    public bool Merge;
    public bool IncludeNavMeshSurface;
}