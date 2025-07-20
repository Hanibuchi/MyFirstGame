using UnityEngine;
using UnityEditor;
using Unity.VisualScripting;

[CustomEditor(typeof(TerrainManager))]
public class TerrainManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        TerrainManager terrainManager = (TerrainManager)target;

        GUILayout.Space(10);

        if (GUILayout.Button("Generate ChunkData"))
        {
            terrainManager.CreateChunkAsset();
        }
        if (GUILayout.Button("Generate SubChunkData"))
        {
            terrainManager.CreateSubChunkAsset();
        }
        if (GUILayout.Button("Generate Chunk"))
        {
            terrainManager.GenerateChunk();
        }
        if (GUILayout.Button("Generate SubChunk"))
        {
            terrainManager.GenerateSubChunk();
        }
    }//
}