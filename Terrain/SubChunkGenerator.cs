using System.Collections;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;

[JsonObject(MemberSerialization.OptIn)]
[RequireComponent(typeof(SerializeManager))]
public class SubChunkGenerator : MonoBehaviour, ISerializableComponent
{
    [SerializeField] Vector2Int chunkPos;
    [SerializeField] Vector2Int subChunkPos;

    public void SetChunkPos(Vector2Int chunkPos)
    {
        this.chunkPos = chunkPos;
    }
    public void SetSubChunkPos(Vector2Int subChunkPos)
    {
        this.subChunkPos = subChunkPos;
    }


    [SerializeField] List<ChunkHandler> _handlers;

    [JsonProperty] int sizex;
    [JsonProperty] int sizey;
    [JsonProperty] string[] tileIDs;
    [JsonProperty] List<(ResourceType type, string id, JObject objData)> objects = new();

    public void OnBeforeCreateChunkAsset()
    {
        var handlers = GetComponentsInChildren<ChunkHandler>();
        this._handlers.AddRange(handlers);
    }

    public JObject MakeChunkData()
    {
        return GetComponent<SerializeManager>().SaveState();
    }

    public void OnBeforeSerializeData()
    {
        (sizex, sizey) = TerrainManager.Instance.GetSubChunkSize();
        var tiles = TerrainManager.Instance.GetTiles(chunkPos, subChunkPos);
        tileIDs = new string[tiles.Length];
        Debug.Log($"tiles.Length: {tiles.Length}");
        for (int i = 0; i < tiles.Length; i++)
        {
            var tile = tiles[i];
            if (tile is MyTile baseTile)
            {
                tileIDs[i] = baseTile.ID;
            }
        }

        objects.Clear();
        foreach (var handler in _handlers)
        {
            if (handler != null && handler.TryGetComponent(out SerializeManager serializable) && handler.TryGetComponent(out PoolableResourceComponent poolable))
            {
                objects.Add((poolable.Type, poolable.ID, serializable.SaveState()));
            }
        }
    }

    /// <summary>
    /// ChunkDataからチャンクを生成
    /// </summary>
    /// <param name="chunkData"></param>
    public void ApplySubChunkData(JObject chunkData)
    {
        GetComponent<SerializeManager>().LoadState(chunkData);
    }

    public void OnAfterDeserializeData()
    {
        MyTile[] tiles = new MyTile[tileIDs.Length];
        for (int i = 0; i < tileIDs.Length; i++)
        {
            var tileID = tileIDs[i];
            if (tileID == null)
                tiles[i] = null;
            else
                tiles[i] = ResourceManager.Instance.GetTile(tileID);
        }
        TerrainManager.Instance.TerrainTilemap.SetTilesBlock(TerrainManager.Instance.GetBoundsInt(chunkPos, subChunkPos), tiles);

        foreach (var obj in objects)
        {
            var spawndObj = ResourceManager.Instance.Get(obj.type, obj.id);
            if (spawndObj != null)
            {
                spawndObj.transform.SetParent(transform);
                spawndObj.GetComponent<SerializeManager>().LoadState(obj.objData);
            }
            else
            {
                Debug.LogWarning($"Handler with ID {obj.id} not found.");
            }
        }
    }
}
