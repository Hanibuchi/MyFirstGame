using System.Collections;
using System.Collections.Generic;
using MyGame;
using UnityEngine;

/// <summary>
/// 再読み込みの際に使用される，アイテムの状態を保存するためのクラス
/// </summary>
[SerializeField]
public class ObjectData
{
    public ResourceManager.ItemID ItemID;
    public float BaseMaxHP;
    public float CurrentMaxHP;
    public float CurrentHP;
    public Damage BaseDamageRate;
    public Damage CurrentDamageRate;
    public List<StatusData> StatusDataList = new();

    public Vector3 LocalPos;
    public Quaternion LocalRotate;
    public Vector3 LocalScale;
}
