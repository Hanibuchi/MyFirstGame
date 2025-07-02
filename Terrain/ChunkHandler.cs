using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PoolableResourceComponent))]
public class ChunkHandler : MonoBehaviour
{
    public ChunkManager ChunkManager { get; private set; }

    public void OnRegistered(ChunkManager chunkManager)
    {
        ChunkManager = chunkManager;
    }
    public void OnUnregistered()
    {
        ChunkManager = null;
    }
}
