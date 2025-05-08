using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

[JsonObject(MemberSerialization.OptIn)]
public class LevelHandler : MonoBehaviour, ISerializeHandler
{
    LevelData m_levelData;
    [JsonProperty][SerializeField] ulong m_baseLevel;
    public ulong BaseLevel
    {
        get => m_baseLevel;
        protected set { m_baseLevel = value; OnBaseLevelChanged?.Invoke(m_baseLevel); }
    }
    public event Action<ulong> OnBaseLevelChanged;

    [JsonProperty][SerializeField] ulong m_level;
    public ulong Level
    {
        get => m_level;
        private set { m_level = value; OnLevelChanged?.Invoke(m_level); }
    }
    public event Action<ulong> OnLevelChanged;

    [JsonProperty][SerializeField] float m_experience;
    public float Experience
    {
        get => m_experience;
        private set { m_experience = value; OnExperienceChanged?.Invoke(m_experience); }
    }
    public event Action<float> OnExperienceChanged;


    [JsonProperty][SerializeField] float m_experienceToNextLevel;
    public float ExperienceToNextLevel
    {
        get => m_experienceToNextLevel;
        protected set { m_experienceToNextLevel = value; OnExperienceToNextLevelChanged?.Invoke(m_experienceToNextLevel); }
    }
    public event Action<float> OnExperienceToNextLevelChanged;

    // StatusEffect m_statusEffect;
    private void Awake()
    {
        // m_statusEffect = GetComponent<StatusEffect>();
    }


    // 経験値を追加する
    public void AddExperience(float additionalExperience)
    {
        Experience += additionalExperience;
        while (Experience >= ExperienceToNextLevel)
        {
            Experience -= ExperienceToNextLevel;
            LevelUp();
        }
    }

    /// <summary>
    /// レベルアップ。ただし，BaseLevelが上がるだけでLevelは上がらない。
    /// </summary>
    void LevelUp()
    {
        BaseLevel++;
        CalculateExperienceToNextLevel();

        ResetToBase();
        // m_statusEffect?.Recalculate();

        Debug.Log("レベルアップ！ Lv." + BaseLevel);
    }
    void CalculateExperienceToNextLevel()
    {
        if (m_levelData == null)
        {
            Debug.LogWarning("levelData is null");
            return;
        }
        ExperienceToNextLevel = TotalExperience(BaseLevel + 1) - TotalExperience(BaseLevel);
    }

    float TotalExperience(ulong level)
    {
        float result = 0;
        switch (m_levelData.growthType)
        {
            case GrowthType.Linear: result = Functions.Linear(level - 1, m_levelData.value1, m_levelData.baseValue); break;
            case GrowthType.Quadratic: result = Functions.Quadratic(level - 1, m_levelData.value1, m_levelData.value2, m_levelData.baseValue); break;
            case GrowthType.Exponential: result = Functions.Exponential2(level - 1, m_levelData.value1, m_levelData.baseValue); break;
            case GrowthType.Logistic: result = Functions.Logistic2(level - 1, m_levelData.value1, m_levelData.value2, m_levelData.baseValue); break;
            case GrowthType.Gompertz: result = Functions.Gompertz2(level - 1, m_levelData.value1, m_levelData.value2, m_levelData.baseValue); break;
        }
        return result;
    }

    public void Initialize(LevelData levelData)
    {
        m_levelData = levelData;
    }
    public void ResetToBase()
    {
        Level = BaseLevel;
    }
    public enum GrowthType
    {
        Linear,
        Quadratic,
        Exponential,
        Logistic,
        Gompertz,
    }
}
