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
    [SerializeField] LevelData m_levelData;
    [SerializeField] StatusInitializer m_statusInitializer;
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

    /// <summary>
    /// BaseLevelをセットする。簡略化のため，ついでにResetToBaseもする。
    /// </summary>
    /// <param name="level"></param>
    public void SetBaseLevel(ulong level)
    {
        BaseLevel = level;
        CalculateExperienceToNextLevel();
        ResetToBase();
    }

    /// <summary>
    /// Levelを変化させる。ステータス効果用。
    /// </summary>
    /// <param name="additionalLevel"></param>
    public void ChangeLevel(ulong additionalLevel)
    {
        Level += additionalLevel;
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
    /// レベルアップ。
    /// </summary>
    void LevelUp()
    {
        BaseLevel++;
        CalculateExperienceToNextLevel();
        ResetToBase();
        m_statusInitializer.ResetToBase();

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
        ExperienceToNextLevel = m_levelData.baseLevelGrowthCurve.Function(BaseLevel);
    }

    public void Initialize(LevelData levelData)
    {
        // m_statusEffect = GetComponent<StatusEffect>();
        m_levelData = levelData;
        m_statusInitializer = GetComponent<StatusInitializer>();
    }
    public void ResetToBase()
    {
        Level = BaseLevel;
    }
}
