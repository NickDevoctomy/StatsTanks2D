using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Bot))]
public class BotEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        Bot bot = (Bot)target;

        if (GUILayout.Button("Position On Nav Mesh"))
        {
            bot.PositionOnNavMesh();
        }
    }
}
