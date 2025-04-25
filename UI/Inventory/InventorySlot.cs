using System.Collections;
using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using MyGame;

public class InventorySlot : Slot//, IPointerEnterHandler, IPointerExitHandler, IDropHandler
{
    /*
    アイテムスロットのドラッグ中のホバーの概要
    1. OnPointerEnterで，ドラッグ中のとき（eventData.pointerDragがnullでないかで判断），ドラッグしているのがItemSlotでなおかつこのスロットに追加できるとき（CanAddItemを使用)，ドラッグ中のアイテムスロットのparentAfterDragをこのスロットに設定し，親をこのスロットにし，isFixedをtrueにしてマウス位置に動かなくする。
    2. OnPointerExitでは，OnPointerEnterでぐちゃぐちゃにした設定をもとに戻す。
    */












    // // こっちでは見た目だけ子オブジェクトにする。OnDropでは内部的な処理もする。
    // public void OnPointerEnter(PointerEventData eventData)
    // {
    //     PointerEnter(eventData);
    // }

    // public void OnPointerExit(PointerEventData eventData)
    // {
    //     PointerExit(eventData);
    // }

    // // OnPointerEnterで既にCanAddItemで入れられるか判定しているため，ここでは判定しない。 
    // public void OnDrop(PointerEventData eventData)
    // {
    //     Drop(eventData);
    // }
}

