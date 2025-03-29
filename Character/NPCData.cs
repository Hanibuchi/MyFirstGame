using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 再読み込みの際に使用される，mobの状態を保存するためのクラス
/// </summary>
[Serializable]
public class NPCData : MobData
{
    public Jobs Job;
}