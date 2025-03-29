using System;
using System.Collections;
using System.Collections.Generic;
using MyGame;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.PlayerLoop;

namespace MyGame
{
    public class EquipmentSlot : InventorySlot, IPointerEnterHandler, IPointerExitHandler
    {
        public NPCManager npcManager; // 装備アイテムを更新するメソッド呼び出し用
        public int id;

        public override void ProcessOnDrop(ItemSlot itemSlot)
        {
            base.ProcessOnDrop(itemSlot);
            npcManager.SetItem(id, itemSlot.Item);
        }

        private void OnTransformChildrenChanged()
        {
            if (npcManager.IsPlayer)
            {
                UIManager.Instance.PlayerStatusUI.PlayerStatusSlotsFrame.UpdateItemSlots(transform.parent.gameObject);
            }
        }
    }
}