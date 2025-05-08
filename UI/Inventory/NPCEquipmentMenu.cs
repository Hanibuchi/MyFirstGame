using System;
using System.Collections;
using System.Collections.Generic;
using MyGame;
using TMPro;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.UI;

public class NPCEquipmentMenu : UI, IItemParentUI
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
            item?.OnAddItemFailed();
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
        {
            slot.SetItemSlot(itemSlot);
        }
        else
        {
            Debug.Log("itemSlot is null");
            slot.DetachChildrenUI();
        }
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
















    // NPCImageManager m_imageManager;
    Health m_health;
    Mana m_mana;
    LevelHandler m_level;
    Job m_job;


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
    public void RegisterStatus(GameObject obj)
    {
        m_npcManager = obj.GetComponent<NPCManager>();

        GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1); // なぜかscaleが(0,0)になるため対症療法的にこうしてる。

        UpdateNPCName(obj.name);

        // if (obj.TryGetComponent(out m_imageManager))
        // {
        //     UpdateNPCImage(m_imageManager.GetImage());
        //     m_imageManager.OnNPCImageChanged += UpdateNPCImage;
        // }


        if (obj.TryGetComponent(out m_health))
        {
            UpdateMaxtHP(m_health.MaxHP);
            UpdateHP(m_health.HP);
        }

        if (obj.TryGetComponent(out m_mana))
        {
            UpdateMaxtMP(m_mana.MaxMP);
            UpdateMP(m_mana.MP);
        }

        // if (obj.TryGetComponent(out m_level))
        // {
        //     UpdateNPCLevel(m_level.Level);
        // }

        // if (obj.TryGetComponent(out m_job))
        // {
        //     UpdateNPCJob(m_job.Job);
        // }

        SetCallbacks(true);

        InitSlots(m_npcManager.ItemCapacity);

        name = obj.name + "EquipMentMenu";
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
        SetCallbacks(false);
    }
    void SetCallbacks(bool isRegister)
    {
        if (isRegister)
        {
            // if ( m_imageManager!=null)
            //     m_imageManager.OnNPCImageChanged += UpdateNPCImage;
            if (m_health != null)
            {
                m_health.OnMaxHPChanged += UpdateMaxtHP;
                m_health.OnHPChanged += UpdateHP;
            }
            if (m_mana != null)
            {
                m_mana.OnMaxMPChanged += UpdateMaxtMP;
                m_mana.OnMPChanged += UpdateMP;
            }
            // if (m_level != null)
            //     m_level.OnLevelChanged += UpdateNPCLevel;
            // if (m_job != null)
            //     m_job.OnJobChanged += UpdateNPCJob;
        }
        else
        {
            // if ( m_imageManager!=null)
            //     m_imageManager.OnNPCImageChanged -= UpdateNPCImage;
            if (m_health != null)
            {
                m_health.OnMaxHPChanged -= UpdateMaxtHP;
                m_health.OnHPChanged -= UpdateHP;
            }
            if (m_mana != null)
            {
                m_mana.OnMaxMPChanged -= UpdateMaxtMP;
                m_mana.OnMPChanged -= UpdateMP;
            }
            // if (m_level != null)
            //     m_level.OnLevelChanged -= UpdateNPCLevel;
            // if (m_job != null)
            //     m_job.OnJobChanged -= UpdateNPCJob;
        }
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

    public void UpdateHP(float currentHP)
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

    public void UpdateMP(float currentMP)
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