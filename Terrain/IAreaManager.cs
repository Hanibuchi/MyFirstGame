using Newtonsoft.Json.Linq;
using UnityEngine;

public interface IAreaManager
{
    void Init(TerrainManager terrainManager);
    JObject GetChunkData(Vector2Int chunkPos);
    void OnChunkGenerated(IChunkManager chunkManager, Vector2Int chunkPos);
    void OnChunkDeactivated(IChunkManager chunkManager, Vector2Int chunkPos);
    void Save();
}