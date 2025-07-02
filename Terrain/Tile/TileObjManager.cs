using System;
using System.Collections;
using System.Collections.Generic;
using MyGame;
using Unity.Mathematics;
using UnityEditor.iOS;
using UnityEngine;
using UnityEngine.Tilemaps;

// このオブジェクトをDestroyするとRuleTileはそのまま残る。DestroyしてからRuleTile.SetTile(pos,null)するとエラー出る。RuleTileを消したいときは必ずこのオブジェクトより先に消さないといけない。
[RequireComponent(typeof(PoolableResourceComponent))]
public class TileObjManager : MonoBehaviour
{
    ChunkHandler _chunkHandler;
    public Vector3Int Position { get; private set; } // tilemap上の位置を保持する変数

    void Awake()
    {
        _chunkHandler = GetComponent<ChunkHandler>();
        if (TryGetComponent(out DeathHandler deathHandler))
            deathHandler.OnDead += OnDead;
    }

    private void Start()
    {
        transform.position = TerrainManager.Instance.TerrainTilemap.GetCellCenterWorld(Position);
    }

    public void Init(Vector3Int pos)
    {
        Position = pos;
    }

    void OnDead()
    {
        _chunkHandler.ChunkManager?.DeleteTile(this);
    }
}
