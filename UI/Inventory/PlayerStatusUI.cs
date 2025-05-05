using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using MyGame;

public class PlayerStatusUI : UI, INPCStatusUI
{
    NPCManager npcManager; // 後々MobManagerにしたいが，アイテムの処理などがだるく時間がかかる
    [SerializeField] protected Image npcImage;
    [SerializeField] protected TextMeshProUGUI npcName;
    [SerializeField] protected TextMeshProUGUI npcLevel;
    [SerializeField] protected TextMeshProUGUI npcJob;
    [SerializeField] protected TextMeshProUGUI hpTMP;
    [SerializeField] protected Slider hpSlider;
    [SerializeField] protected TextMeshProUGUI mpTMP;
    [SerializeField] protected Slider mpSlider;
    [SerializeField] PlayerStatusSlotsFrame m_playerStatusSlotsFrame;
    public PlayerStatusSlotsFrame PlayerStatusSlotsFrame { get => m_playerStatusSlotsFrame; private set => m_playerStatusSlotsFrame = value; }


    protected override void Awake()
    {
        base.Awake();
        UIManager.Instance.SetPlayerStatusUI(this);
    }

    public void Open()
    {
        gameObject.SetActive(true);
    }
    public void Close()
    {
        gameObject.SetActive(false);
    }

    /// <summary>
    /// NPCEquipmentMenuを初期化し，ステータスが表示されるよう登録する。
    /// </summary>
    /// <param name="equipmentMenu"></param>
    public void RegisterStatus(NPCManager newNPCManager)
    {
        if (npcManager != null)
            UnregisterStatus();

        npcManager = newNPCManager;

        GetStatusDisplayComponents();

        UpdateNPCName(newNPCManager.gameObject.name);
        UpdateNPCLevel(newNPCManager.CurrentLevel);
        UpdateNPCJob(newNPCManager.Job);
        UpdateMaxtHP(newNPCManager.CurrentMaxHP);
        UpdateCurrentHP(newNPCManager.CurrentHP);
        UpdateMaxtMP(newNPCManager.CurrentMaxMP);
        UpdateCurrentMP(newNPCManager.CurrentMP);

        SetCallbacks(true);
    }

    void GetStatusDisplayComponents()
    {
        // if (PlayerStatusSlotsFrame != null)
        // {
        //     playerStatusUISlots = new List<EquipmentSlot>(PlayerStatusSlotsFrame.GetComponentsInChildren<EquipmentSlot>());
        //     if (playerStatusUISlots != null)
        //         for (int i = 0; i < playerStatusUISlots.Count; i++)
        //         {
        //             playerStatusUISlots[i].npcManager = npcManager;
        //             playerStatusUISlots[i].id = i;
        //         }
        // }
        // else
        //     Debug.LogWarning("equipmentSlotsFrame moved");
    }

    void OnDestroy()
    {
        UnregisterStatus();
    }

    /// <summary>
    /// 登録を解除する。
    /// </summary>
    /// <param name="equipmentMenu"></param>
    public void UnregisterStatus()
    {
        if (npcManager == null)
        {
            return;
        }

        SetCallbacks(false);
    }


    void SetCallbacks(bool isRegister)
    {
        if (isRegister)
        {
            npcManager.OnCurrentLevelChanged += UpdateNPCLevel;
            npcManager.OnJobChanged += UpdateNPCJob;
            npcManager.OnCurrentHPChanged += UpdateCurrentHP;
            npcManager.OnCurrentMaxHPChanged += UpdateMaxtHP;
            npcManager.OnCurrentMPChanged += UpdateCurrentMP;
            npcManager.OnCurrentMaxMPChanged += UpdateMaxtMP;
        }
        else
        {
            npcManager.OnCurrentLevelChanged -= UpdateNPCLevel;
            npcManager.OnJobChanged -= UpdateNPCJob;
            npcManager.OnCurrentHPChanged -= UpdateCurrentHP;
            npcManager.OnCurrentMaxHPChanged -= UpdateMaxtHP;
            npcManager.OnCurrentMPChanged -= UpdateCurrentMP;
            npcManager.OnCurrentMaxMPChanged -= UpdateMaxtMP;
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

    public void UpdateCurrentHP(float currentHP)
    {
        if (hpSlider != null && mpSlider != null)
        {
            // Debug.Log($"currentHP: {currentHP}");
            hpSlider.value = currentHP;
            hpTMP.text = $"HP {currentHP:F0}";
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
}