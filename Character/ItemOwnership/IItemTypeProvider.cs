using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IItemTypeProvider
{
    ItemType GetItemType();
}

[System.Flags]
public enum ItemType
{
    None = 0,
    Attack = 1 << 0,
    ParameterModifier = 1 << 1,
    ProjectileModifier = 1 << 2,
}
