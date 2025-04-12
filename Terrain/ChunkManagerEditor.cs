using UnityEngine;
using UnityEditor;
using Unity.VisualScripting;

[CustomEditor(typeof(ChunkManager))]
public class ChunkManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        ChunkManager chunk = (ChunkManager)target;
        if (GUILayout.Button("実行"))
        {
            chunk.CreateChunkAsset();
        }
    }
}