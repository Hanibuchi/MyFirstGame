using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using MyGame;
using Unity.VisualScripting;

public class ItemSlot : Slot, IItemParentUI, IPointerDownHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    Vector2 m_dragOffset;

    RectTransform m_rt;
    [SerializeField] Transform m_partnerSlotSpacing; // 右側にいるパートナーのスロットスペーシング

    public void Init(IItem newItem, Sprite sprite)
    {
        Item = newItem;

        SetRayCastTarget(true);
        if (m_itemImage != null)
        {
            m_itemImage.sprite = sprite;
        }
        else
            Debug.LogWarning("itemImage is null");
    }









    public IItem Item { get; private set; }
    public void SetItem(Item item)
    {
        Item = item;
    }
    public virtual void InitSlots(int slotCount)
    {

    }



    /// <summary>
    /// 子供のUIを指定されたindexにセットする。indexは0から始まる。
    /// </summary>
    /// <param name="itemSlot"></param>
    /// <param name="index"></param>
    public virtual void SetItemSlot(ItemSlot itemSlot, int index)
    {
        if (0 <= index && index <= m_itemSlotFrame.childCount)
        {
            itemSlot.SetID(index);
            itemSlot.SetItemParentUI(this);
            // Debug.Log($"itemSLot.ItemSlotParent: {}")
            itemSlot.transform.SetParent(m_itemSlotFrame);
            itemSlot.transform.SetSiblingIndex(index);

            // var slotSpacing = itemSlot.GetSlotSpacing();
            // slotSpacing.SetParent(transform);
            // slotSpacing.SetSiblingIndex(index + 1);
        }
        else
            Debug.LogWarning("index is weired");
    }
    /// <summary>
    /// これは呼ばれるべきでない。InvSlotなら入る場所が1つしかないためindexは必要ないが，ItemSlotは複数あるためどこに入れるか明確に指定されないといけない。
    /// </summary>
    /// <param name="itemSlot"></param>
    public override void SetItemSlot(ItemSlot itemSlot)
    {
        Debug.LogWarning("this should not be called");
    }

    public void AddItem(int index, Item item)
    {
        var itemHolder = Item.ItemHolder;
        if (itemHolder.CanAddItemAt(index, item.ItemHolder))
        {
            itemHolder.AddItemAt(index, item.ItemHolder);
        }
        else
        {
            Debug.Log("Cannot add item to this slot.");
            // ここでアイテムが入れられなかった時の処理。
            item?.OnAddItemFailed();
        }
    }

    public override void OnDrop(PointerEventData eventData)
    {
        Debug.Log("OnDrop");
        GameObject dropped = eventData.pointerDrag;
        if (dropped != null)
        {
            Debug.Log("dropped != null");
            IItem item = null;
            if (dropped.TryGetComponent(out ItemSlot itemSlot))
                item = itemSlot.Item;
            if (item == null)
                item = dropped.GetComponent<Item>();

            AddItem(m_id, (Item)item);
        }
        eventData.Use();
    }

    public override void OnRelease()
    {
        base.OnRelease();
        transform.SetParent(null);
        Item.OnReleaseItemSlotUI();
    }
















    [SerializeField] Image m_itemImage;

    /// <summary>
    /// UIの当たり判定を設定する
    /// </summary>
    protected virtual void SetRayCastTarget(bool isActive)
    {
        if (TryGetComponent(out Image itemSlotImage))
            itemSlotImage.raycastTarget = isActive;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        m_dragOffset = (Vector2)transform.position - eventData.position;
    }

    /* アイテムスロットのドラッグ操作の概要。
    1．OnBeginDragでにする。同時にzを前にすることでUIが最前線に表示されるようにする。
    2. OnDragでマウスの位置にこのUIがくるようにする。SlotにマウスがあるときはそのSlotの中に固定されるようにするため，isFixedがtrueのときは実行しない。
    3. OnEndDragでparentAfterDragを親にする。parentAfterDragはマウスがSlotの中に入ったときや出たときに更新される。同時にzを戻す。
    */
    public void OnBeginDrag(PointerEventData eventData)
    {
        // Debug.Log("Begin drag");
        DragSystem.Instance.ItemSlotBeginDrag(this, (Item)Item);

        eventData.Use();
    }

    public void BeginDrag()
    {
        SetRayCastTarget(false);
        // ドラッグ開始時にzを前に出して前に描画されるようにする。
        SetZPos(10);
        if (UIManager.Instance.GetEquipmentUI() is MonoBehaviour mono)
            transform.SetParent(mono.transform);
    }

    /// <summary>
    /// RectTransformのZ座標を設定する。Zが大きいほど前に表示されるため，前に表示したいときに使う。
    /// </summary>
    /// <param name="z"></param>
    void SetZPos(int z)
    {
        if (m_rt == null)
            m_rt = GetComponent<RectTransform>();

        Vector3 rtPos;
        rtPos = m_rt.position;
        rtPos.z = z;
        m_rt.position = rtPos;
    }

    /// <summary>
    /// ItemSlotの位置をマウスの位置にする
    /// </summary>
    public void SetAtMousePos()
    {
        Vector3 mousePos = Pointer.current.position.ReadValue() + m_dragOffset;
        mousePos.z = transform.position.z;
        transform.position = mousePos;
    }

    /// <summary>
    /// 必要なくても，Dragを使うにはBegin, Drag, Endの3つを必ず継承しないといけないみたい。
    /// </summary>
    /// <param name="eventData"></param>
    public void OnDrag(PointerEventData eventData)
    {
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        Debug.Log("ItemSlot OnEndDrag");
        DragSystem.Instance.EndDrag();
    }

    public void EndDrag()
    {
        m_dragOffset = Vector2.zero;

        SetRayCastTarget(true);
        // 前に出してたのを戻す。
        SetZPos(0);
    }


    /// <summary>
    /// 左右のSlotSpacingを消す。
    /// </summary>
    /// <param name="itemSlotTransform"></param>
    public void RemoveItemSlot(ItemSlot itemSlot)
    {
        if (itemSlot != null && itemSlot.m_partnerSlotSpacing != null)
        {
            Destroy(itemSlot.m_partnerSlotSpacing.gameObject);
            itemSlot.m_partnerSlotSpacing = null;
        }
    }
}

