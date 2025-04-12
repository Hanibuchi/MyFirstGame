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
    public string[] tileIDs;
    public List<PartyData> partys = new();
    public List<NPCData> npcs = new();
    public List<MobData> mobs = new();
    public List<ObjectData> items = new();
}