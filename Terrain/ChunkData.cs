using System;
using System.Collections;
using System.Collections.Generic;
using MyGame;
using UnityEngine;
using UnityEngine.Android;
using UnityEngine.ResourceManagement.Util;
using UnityEngine.Tilemaps;

[Serializable]
public class ChunkData
{
    public ResourceManager.TileID[] TileIDs;
    public List<PartyData> Partys = new();
    public List<NPCData> NPCs = new();
    public List<MobData> Mobs = new();
    public List<ObjectData> Items = new();
}