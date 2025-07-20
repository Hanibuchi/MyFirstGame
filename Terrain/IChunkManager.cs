using Newtonsoft.Json.Linq;
using UnityEngine;

public interface IChunkManager
{
    Vector2Int ChunkPos { get; } // チャンクの位置

    void OnGenerated(IAreaManager areaManager, Vector2Int chunkPos);
    JObject OnDeactivated();
    JObject MakeChunkData();
    void ApplyChunkData(JObject chunkData);
}