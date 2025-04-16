using System.Collections;
using System.Collections.Generic;
using MyGame;
using UnityEngine;

public interface IItem
{
    public void RegisterNextItemsToShot(Shot shot);
    public void SetBaseExtras(Shot shot);
}
