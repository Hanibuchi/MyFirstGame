using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using MyGame;

public class PlayerStatusUI : UI
{
    GameObject m_obj;
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


    Health m_health;
    Mana m_mana;
    LevelHandler m_level;
    Job m_job;


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
    public void RegisterStatus(GameObject obj)
    {
        m_obj = obj;

        UpdateNPCName(obj.name);

        if (m_obj.TryGetComponent(out m_health))
        {
            UpdateMaxtHP(m_health.MaxHP);
            UpdateHP(m_health.HP);
        }

        if (m_obj.TryGetComponent(out m_mana))
        {
            UpdateMaxtMP(m_mana.MaxMP);
            UpdateMP(m_mana.MP);
        }

        // if (m_obj.TryGetComponent(out m_level))
        // {
        //     UpdateNPCLevel(m_level.Level);
        // }

        // if (m_obj.TryGetComponent(out m_job))
        // {
        //     UpdateNPCJob(m_job.Job);
        // }

        SetCallbacks(true);
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
        SetCallbacks(false);
    }


    void SetCallbacks(bool isRegister)
    {
        if (isRegister)
        {
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

    public void UpdateHP(float currentHP)
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
}