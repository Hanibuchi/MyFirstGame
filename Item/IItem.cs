using System.Collections;
using System.Collections.Generic;
using MyGame;
using UnityEngine;

public interface IItem
{
    IChildItemHolder ItemHolder { get; }
    public void RegisterNextItemsToShot(Shot shot);
    public void SetBaseExtras(Shot shot);
    public void RefreshUI();
    void FirstFire(Shot shot);
    void ProcessReloadAndMP(Shot shot);
    float CoolDownTime { get; }
    ItemSlot GetItemSlotUI();
    void OnAddItemFailed();
    void OnReleaseItemSlotUI();
    void EnableComponentsOnCollected(bool isEnable);
    void Drop();
}
