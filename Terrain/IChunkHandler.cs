using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IChunkHandler
{
    public ChunkManager BossChunkManager { get; set; }
    public void OnChunkGenerate();
    public void OnChunkDeactivate();
    public void OnChunkActivate();
    public void OnChunkReset();
}
