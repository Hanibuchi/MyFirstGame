using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace MyGame
{
    /// <summary>
    /// ドロップのアイテムと確率を表す。
    /// </summary>
    [Serializable]
    public class DropItem
    {
        public float DropRate;
        public GameObject Item;
    }
}
