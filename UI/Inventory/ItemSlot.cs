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

    /// <summary>
    /// 他の枠に入ってプレビューを出しているかどうか
    /// </summary>
    public bool isFixed;

    [HideInInspector] public Transform parentAfterDrag; // 不要？
    RectTransform m_rt;
    public Transform PartnerSlotSpacing; // 右側にいるパートナーのスロットスペーシング

    public void Init(Item newItem, Sprite sprite)
    {
        ItemParent = newItem;
        gameObject.name = newItem.name + "Slot";
        SetRayCastTarget(true);
        if (m_itemImage == null)
        {
            Debug.LogWarning("itemImage is null");
            return;
        }
        m_itemImage.sprite = sprite;
        if (!m_itemImage.gameObject.TryGetComponent(out AspectRatioFitter fitter))
        {
            Debug.LogWarning("fitter is null");
            return;
        }
        fitter.aspectRatio = sprite.bounds.size.x / sprite.bounds.size.y;
    }










    public IItemParent ItemParent { get; private set; }
    public void SetItemParent(IItemParent itemParent)
    {
        ItemParent = itemParent;
    }


    public virtual void InitSlots(int slotCount)
    {

    }
    public void AddItem(int index, Item item)
    {
        if (ItemParent.CanAddItem(index, item))
            ItemParent.AddItem(index, item);
        else
        {
            Debug.Log("Cannot add item to this slot.");
            // ここでアイテムが入れられなかった時の処理。
        }
    }
    /// <summary>
    /// 子供のUIを指定されたindexにセットする。indexは0から始まる。
    /// </summary>
    /// <param name="itemSlot"></param>
    /// <param name="index"></param>
    public void SetItemSlot(ItemSlot itemSlot, int index)
    {
        if (0 <= index && index <= m_itemSlotFrame.childCount)
        {
            itemSlot.SetID(index);
            itemSlot.SetItemParentUI(this);
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
    /// 子供のUIをセットする。
    /// </summary>
    /// <param name="itemSlot"></param>
    public override void SetItemSlot(ItemSlot itemSlot)
    {
        SetItemSlot(itemSlot, m_itemSlotFrame.childCount);
    }

    public override void OnRelease()
    {
        base.OnRelease();
        transform.SetParent(null);
        if (ItemParent is Item item)
            item.OnReleaseItemSlotUI();
    }
















    [SerializeField] Image m_itemImage;

    /// <summary>
    /// UIの当たり判定を設定する
    /// </summary>
    void SetRayCastTarget(bool isActive)
    {
        if (TryGetComponent(out Image itemSlotImage))
            itemSlotImage.raycastTarget = isActive;
        if (transform.GetChild(0).TryGetComponent(out Image itemImageFrameImage))
            itemImageFrameImage.raycastTarget = isActive;
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
        DragSystem.Instance.ItemSlotBeginDrag(this, (Item)ItemParent);

        // else
        // {
        //     eventData.pointerDrag = Item.gameObject;
        //     Item.OnBeginDrag(eventData);
        // }

        eventData.Use();
    }

    public void BeginDrag()
    {
        SetRayCastTarget(false);
        // ドラッグ開始時にzを前に出して前に描画されるようにする。
        SetZPos(10);
        DragSetup();
        gameObject.SetActive(true);
    }

    /// <summary>
    /// ドラッグ再開時の諸設定。ドラッグ中の親をScreenView（枠外にドラッグされたことを検知するための背景のUI）にする。ドラッグ開始時とかぶるところがあるためOnBeginDragとは別で作成した。
    /// </summary>
    public void DragSetup()
    {

        // 今までいたスロットとの関係を断つ。
        if (transform.parent != null && transform.parent.gameObject.TryGetComponent(out ItemSlot itemSlot))
            itemSlot.RemoveItemSlot(this);

        transform.SetParent(GameManager.Instance.DragContainer.transform);

        isFixed = false;

        // itemSlot.SetAtMousePos(); // こうすることで一瞬ちらつくのを防ぐ
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

    // public void OnDrag(PointerEventData eventData)
    // {
    //     // Debug.Log($"IsItemDragging: {IsItemDragging}");
    //     if (IsItemDragging == false && isFixed == false)
    //     {
    //         SetAtMousePos();
    //         if (eventData.pointerDrag != gameObject)
    //             eventData.pointerDrag = gameObject;
    //     }
    //     else
    //     {
    //         Item.MoveItem();
    //         if (eventData.pointerDrag != Item.gameObject)
    //             eventData.pointerDrag = Item.gameObject;
    //     }

    //     eventData.Use();
    // }

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
        //DragSystem.EndDragを実行
        DragSystem.Instance.EndDrag();


        // Debug.Log("ItemSlot OnEndDrag");
        // GameManager.Instance.CurrentlyDraggedItem = null;

        // // if (IsItemDragging == false)
        // EndDrag();

        // Item.EndDrag();
    }

    public void EndDrag()
    {
        m_dragOffset = Vector2.zero;

        if (transform.parent == GameManager.Instance.DragContainer && parentAfterDrag != null)
            transform.SetParent(parentAfterDrag);

        if (transform.parent.TryGetComponent(out ItemSlot itemSlot) && itemSlot.ItemParent != null) // 他の2種のSlotでは内部的な処理をOnDropで行うのに対し，ItemSlotではここで行う
            itemSlot.ItemParent.AddItem((Item)ItemParent);

        SetRayCastTarget(true);

        // 前に出してたのを戻す。
        SetZPos(0);
    }

    // protected override void ProcessOnPointerEnter(ItemSlot itemSlot)
    // {
    //     // OnPointerExit(new PointerEventData(EventSystem.current));
    //     itemSlot.parentAfterDrag = transform;
    //     // if (transform.childCount == 1)
    //     // AddItemSlotAt(1, itemSlot);
    //     // else
    //     //     AddItemSlotAt(2, itemSlot.transform);
    //     itemSlot.isFixed = true;
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

    // // 継承して何も操作しないために消しちゃダメ。IventtorySlotとEquipmentSlotではOnDropを通じて内部的な処理を行うが，ItemSlotではOnEndDragを通じてそれを行う。
    // public override void ProcessOnDrop(ItemSlot itemSlot) { }

    // public override bool CanAddItem(ItemSlot item)
    // {
    //     if (item != null && Item != null)
    //         return Item.CanAddItem(item.Item);
    //     else return false;
    // }


    /// <summary>
    /// 左右のSlotSpacingを消す。
    /// </summary>
    /// <param name="itemSlotTransform"></param>
    public void RemoveItemSlot(ItemSlot itemSlot)
    {
        if (itemSlot != null && itemSlot.PartnerSlotSpacing != null)
        {
            Destroy(itemSlot.PartnerSlotSpacing.gameObject);
            itemSlot.PartnerSlotSpacing = null;
        }
    }


    // public Transform GetSlotSpacing()
    // {
    //     if (PartnerSlotSpacing == null)
    //     {
    //         PartnerSlotSpacing = ResourceManager.GetOther(ResourceManager.UIID.SlotSpacing.ToString()).transform;
    //         if (PartnerSlotSpacing.TryGetComponent(out SlotSpacing slotSpacing))
    //             slotSpacing.PartnerItemSlot = transform;
    //         else
    //             Debug.LogWarning("SlotSpacing is wrong");
    //     }
    //     return PartnerSlotSpacing;
    // }
}

