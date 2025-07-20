using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using UnityEngine;

public class ChunkDataEditor
{
    public static JObject MergeSubChunkDatas(List<JObject> subChunkDatas, float tileSizeX, float tileSizeY)
    {
        int count = subChunkDatas.Count;
        int gridSize = (int)Math.Sqrt(count);

        if (gridSize * gridSize != count)
        {
            Debug.LogWarning("subChunkDatas.Count is not square number");
            return null;
        }

        // 1つ目の SubChunk のサイズを基準とする
        JObject first = subChunkDatas[0];
        var handler = first["SubChunkHandler"];
        int subWidth = handler["sizex"].Value<int>();
        int subHeight = handler["sizey"].Value<int>();

        int totalWidth = subWidth * gridSize;
        int totalHeight = subHeight * gridSize;
        JArray mergedTileIDs = new JArray(new object[totalWidth * totalHeight]);
        JArray mergedObjects = new JArray();

        for (int index = 0; index < subChunkDatas.Count; index++)
        {
            JObject sub = subChunkDatas[index];
            var subHandler = sub["SubChunkHandler"];
            JArray subTileIDs = (JArray)subHandler["tileIDs"];
            JArray subObjects = (JArray)subHandler["objects"];

            // 左下→右下→左上→右上 の順に並んでいると仮定
            int chunkX = index % gridSize;
            int chunkY = index / gridSize;

            for (int y = 0; y < subHeight; y++)
            {
                for (int x = 0; x < subWidth; x++)
                {
                    int subIndex = y * subWidth + x;

                    int globalX = chunkX * subWidth + x;
                    int globalY = chunkY * subHeight + y;
                    int globalIndex = globalY * totalWidth + globalX;

                    mergedTileIDs[globalIndex] = subTileIDs[subIndex];
                }
            }

            foreach (var obj in subObjects)
            {
                JObject objClone = (JObject)obj.DeepClone();

                var pos = objClone["Item3"]?["TransformSerializer"]?["m_position"];
                if (pos != null)
                {
                    float localX = pos["x"].Value<float>();
                    float localY = pos["y"].Value<float>();

                    float offsetX = (chunkX - ((gridSize - 1) / 2f)) * subWidth * tileSizeX;
                    float offsetY = (chunkY - ((gridSize - 1) / 2f)) * subHeight * tileSizeY;

                    pos["x"] = localX + offsetX;
                    pos["y"] = localY + offsetY;
                }

                mergedObjects.Add(objClone);
            }
        }

        var chunkManager = new JObject
        {
            ["sizex"] = totalWidth,
            ["sizey"] = totalHeight,
            ["tileIDs"] = mergedTileIDs,
            ["objects"] = mergedObjects
        };

        var poolable = new JObject
        {
            ["m_type"] = 3,
            ["m_id"] = "ChunkManager"
        };

        var result = new JObject
        {
            ["ChunkManager"] = chunkManager,
            ["PoolableResourceComponent"] = poolable
        };

        return result;
    }
}
