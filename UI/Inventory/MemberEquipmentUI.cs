using System;
using System.Collections;
using System.Collections.Generic;
using MyGame;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.UI;

public class MemberEquipmentUI : UI, IItemParentUI, IMemberEquipmentUI
{
    public ItemUser ItemUser { get; private set; }
    public void SetItemUser(ItemUser itemUser)
    {
        ItemUser = itemUser;
    }

    public void AddItem(int index, Item item)
    {
        if (ItemUser.ItemHolder.CanAddItemAt(index, item.ItemHolder))
            ItemUser.ItemHolder.AddItemAt(index, item.ItemHolder);
        else
        {
            Debug.Log("Cannot add item to this slot.");
            // ここでアイテムが入れられなかった時の処理。
            item?.OnAddItemFailed();
        }
    }


    public void UpdateEquipmentUI(List<IChildItemHolder> itemHolders)
    {
        foreach (Transform slotTrs in m_itemSlotFrame)
        {
            if (slotTrs.TryGetComponent(out InventorySlot slot))
            {
                slot.DetachChildrenUI();
            }
            if (slotTrs.TryGetComponent(out PoolableResourceComponent poolable))
            {
                poolable.Release();
            }
        }

        Debug.Log($"count: {itemHolders.Count}");
        for (int i = 0; i < itemHolders.Count; i++)
        {
            InventorySlot slot = GenAndSetSlot(i);
            Debug.Log($"slot: {slot}");

            var itemHolder = itemHolders[i];
            if (itemHolder != null)
            {
                var item = itemHolder.GetItem();
                ItemSlot itemSlot = item.GetItemSlotUI();
                if (itemSlot == null)
                {
                    item.RefreshUI();
                    itemSlot = item.GetItemSlotUI();
                }
                slot.SetItemSlot(itemSlot);
            }
        }
        foreach (Transform slotTrs in m_itemSlotFrame)
            Debug.Log($"slot: {slotTrs}");
    }

    /// <summary>
    /// inventorySlotを生成して，諸設定をする。一番後ろに追加されることを想定している。
    /// </summary>
    /// <param name="index"></param>
    InventorySlot GenAndSetSlot(int index)
    {
        var slot = ResourceManager.Instance.GetOther(ResourceManager.ItemSlotID.InventorySlot.ToString()).GetComponent<InventorySlot>();
        slot.SetID(index);
        slot.SetItemParentUI(this);
        slot.transform.SetParent(m_itemSlotFrame);
        slot.transform.SetAsLastSibling();
        return slot;
    }















    // NPCImageManager m_imageManager;
    Health m_health;
    Mana m_mana;
    LevelHandler m_level;
    JobHandler m_job;
    PartyMember m_partyMember;


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
    public void RegisterStatus(GameObject obj)
    {
        UnregisterStatus();
        m_npcManager = obj.GetComponent<NPCManager>();

        GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1); // なぜかscaleが(0,0)になるため対症療法的にこうしてる。

        UpdateNPCName(obj.name);

        if (obj.TryGetComponent(out m_partyMember))
        {
            UpdateNPCImage(m_partyMember.GetImage());
            m_partyMember.OnNPCImageChanged += UpdateNPCImage;
        }


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

        if (obj.TryGetComponent(out m_level))
        {
            UpdateNPCLevel(m_level.Level);
        }

        if (obj.TryGetComponent(out m_job))
        {
            UpdateNPCJob(m_job.Job);
        }

        SetCallbacks(true);

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
    }

    /// <summary>
    /// 登録を解除する。
    /// </summary>
    /// <param name="equipmentMenu"></param>
    void UnregisterStatus()
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
            if (m_level != null)
                m_level.OnLevelChanged += UpdateNPCLevel;
            if (m_job != null)
                m_job.OnJobChanged += UpdateNPCJob;
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
            if (m_level != null)
                m_level.OnLevelChanged -= UpdateNPCLevel;
            if (m_job != null)
                m_job.OnJobChanged -= UpdateNPCJob;
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

    public void UpdateNPCJob(JobType newNPCJob)
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