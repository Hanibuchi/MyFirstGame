using System;
using System.Collections;
using System.Collections.Generic;
using MyGame;
using TMPro;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.UI;

public class NPCEquipmentMenu : UI, INPCStatusUI, IItemParentUI
{
    public IItemParent ItemParent { get; private set; }
    public void SetItemParent(IItemParent itemParent)
    {
        ItemParent = itemParent;
    }

    /// <summary>
    /// EquipmentSlotsをItemCapacityの数だけ生成する。
    /// </summary>
    public void InitSlots(int slotCount)
    {
        for (int i = 0; i < slotCount; i++)
        {
            GenAndSetSlot(i);
        }
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
    public void DetachChildrenUI()
    {
        foreach (Transform slotTrs in m_itemSlotFrame)
        {
            if (slotTrs.TryGetComponent(out InventorySlot slot))
            {
                slot.DetachChildrenUI();
            }
        }
    }

    public void SetItemSlot(ItemSlot itemSlot, int index)
    {
        var slot = m_itemSlotFrame.GetChild(index).GetComponent<InventorySlot>();

        if (itemSlot != null)
            slot.SetItemSlot(itemSlot);
    }
    /// <summary>
    /// 装備スロットを追加する。
    /// </summary>
    public void AddSlot()
    {
        GenAndSetSlot(m_npcManager.ItemCapacity);
    }

    /// <summary>
    /// inventorySlotを生成して，諸設定をする。一番後ろに追加されることを想定している。
    /// </summary>
    /// <param name="index"></param>
    void GenAndSetSlot(int index)
    {
        var slot = ResourceManager.GetOther(ResourceManager.ItemSlotID.InventorySlot.ToString()).GetComponent<InventorySlot>();
        slot.SetID(index);
        slot.SetItemParentUI(this);
        slot.transform.SetParent(m_itemSlotFrame);
    }





















    NPCManager m_npcManager; // 後々MobManagerにしたいが，アイテムの処理などがだるく時間がかかる
    [SerializeField] protected Image npcImage;
    [SerializeField] protected TextMeshProUGUI npcName;
    [SerializeField] protected TextMeshProUGUI npcLevel;
    [SerializeField] protected TextMeshProUGUI npcJob;
    [SerializeField] protected TextMeshProUGUI hpTMP;
    [SerializeField] protected Slider hpSlider;
    [SerializeField] protected TextMeshProUGUI mpTMP;
    [SerializeField] protected Slider mpSlider;
    [SerializeField] protected Transform m_itemSlotFrame;

    /// <summary>
    /// NPCEquipmentMenuを初期化し，ステータスが表示されるよう登録する。
    /// </summary>
    /// <param name="newNPCManager"></param>
    public void RegisterStatus(NPCManager newNPCManager)
    {
        m_npcManager = newNPCManager;

        GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1); // なぜかscaleが(0,0)になるため対症療法的にこうしてる。

        UpdateNPCImage(newNPCManager.NPCImage);
        UpdateNPCName(newNPCManager.gameObject.name);
        UpdateNPCLevel(newNPCManager.CurrentLevel);
        UpdateNPCJob(newNPCManager.Job);
        UpdateCurrentHP(newNPCManager.CurrentHP);
        UpdateMaxtHP(newNPCManager.CurrentMaxHP);
        UpdateCurrentMP(newNPCManager.CurrentMP);
        UpdateMaxtMP(newNPCManager.CurrentMaxMP);

        newNPCManager.OnNPCImageChanged += UpdateNPCImage;
        newNPCManager.OnCurrentLevelChanged += UpdateNPCLevel;
        newNPCManager.OnJobChanged += UpdateNPCJob;
        newNPCManager.OnCurrentHPChanged += UpdateCurrentHP;
        newNPCManager.OnCurrentMaxHPChanged += UpdateMaxtHP;
        newNPCManager.OnCurrentMPChanged += UpdateCurrentMP;
        newNPCManager.OnCurrentMaxMPChanged += UpdateMaxtMP;

        InitSlots(m_npcManager.ItemCapacity);

        name = m_npcManager.gameObject.name + "EquipMentMenu";
    }


    void OnDestroy()
    {
        // アイテムのUIも削除する。
        UnregisterStatus();
    }

    public override void OnRelease()
    {
        base.OnRelease();
        UnregisterStatus();
        UIManager.Instance.EquipmentMenuManager.RemoveMember(this);
    }

    /// <summary>
    /// 登録を解除する。
    /// </summary>
    /// <param name="equipmentMenu"></param>
    public void UnregisterStatus()
    {
        if (m_npcManager == null)
            return;
        m_npcManager.OnNPCImageChanged -= UpdateNPCImage;
        m_npcManager.OnCurrentLevelChanged -= UpdateNPCLevel;
        m_npcManager.OnJobChanged -= UpdateNPCJob;
        m_npcManager.OnCurrentHPChanged -= UpdateCurrentHP;
        m_npcManager.OnCurrentMaxHPChanged -= UpdateMaxtHP;
        m_npcManager.OnCurrentMPChanged -= UpdateCurrentMP;
        m_npcManager.OnCurrentMaxMPChanged -= UpdateMaxtMP;
    }










    public void UpdateNPCImage(Sprite newNPCImage)
    {
        if (npcImage != null)
        {
            npcImage.sprite = newNPCImage;
        }
        else
        {
            Debug.LogWarning("Init() might be not done");
        }
    }

    public void UpdateNPCName(string newNPCName)
    {
        if (npcName != null)
        {
            npcName.text = newNPCName;
        }
        else
        {
            Debug.LogWarning("Init() might be not done");
        }
    }

    public void UpdateNPCLevel(ulong newLevel)
    {
        if (npcLevel != null)
        {
            npcLevel.text = "Lv. " + newLevel.ToString("F0");
        }
        else
        {
            Debug.LogWarning("Init() might be not done");
        }
    }

    public void UpdateNPCJob(Jobs newNPCJob)
    {
        if (npcJob != null)
        {
            npcJob.text = newNPCJob.ToString();
        }
        else
        {
            Debug.LogWarning("Init() might be not done");
        }
    }

    public void UpdateCurrentHP(float currentHP)
    {
        if (hpSlider != null && mpSlider != null)
        {
            hpSlider.value = currentHP;
            hpTMP.text = $"HP {currentHP:F0}";
        }
        else
        {
            Debug.LogWarning("Init() might be not done");
        }
    }
    public void UpdateMaxtHP(float maxHP)
    {
        if (hpSlider != null && mpSlider != null)
        {
            hpSlider.maxValue = maxHP;
        }
        else
        {
            Debug.LogWarning("Init() might be not done");
        }
    }

    public void UpdateCurrentMP(float currentMP)
    {
        if (mpSlider != null && mpSlider != null)
        {
            mpSlider.value = currentMP;
            mpTMP.text = $"MP {currentMP:F0}";
        }
        else
        {
            Debug.LogWarning("Init() might be not done");
        }
    }
    public void UpdateMaxtMP(float maxMP)
    {
        if (mpSlider != null && mpSlider != null)
        {
            mpSlider.maxValue = maxMP;
        }
        else
        {
            Debug.LogWarning("Init() might be not done");
        }
    }
}