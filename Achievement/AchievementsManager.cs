using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Unity.VisualStudio.Editor;
using MyGame;
using UnityEngine;

public class AchievementsManager : MonoBehaviour
{
    public static AchievementsManager Instance { get; private set; }
    string AchievementsPath => GetAchievementsPath(GameManager.PlayerDataPath);
    string GetAchievementsPath(string playerDataPath)
    {
        return Path.Combine(playerDataPath, "Achievements");
    }

    [SerializeField] List<Achievement> baseAchievements = new(); // インスペクタから設定する
    readonly Dictionary<string, AchievementData> Achievements = new();

    public void OnAppStart()
    {
        Instance = this;
        foreach (var achievement in baseAchievements)
        {
            Achievements[achievement.Name] = new()
            {
                Rarity = achievement.Rarity,
                Sprite = achievement.Sprite,
            };
        }
        LoadAllSaveAchievements();
    }
    public void OnGameStart()
    {
        ResetAchievements();
        LoadAchievements(AchievementsPath);
    }
    public void OnReturnToTitle()
    {
        LoadAllSaveAchievements();
    }
    public void Save()
    {
        AchievementsData achievementsData = new()
        {
            Achievements = Achievements,
        };
        EditFile.SaveCompressedJson(AchievementsPath, achievementsData);
    }

    public void Unlock(string name)
    {
        if (Achievements.ContainsKey(name))
        {
            Achievements[name].isAchieved = true;
        }
    }
/// <summary>
/// 報酬を設定する。達成済みなら自由に設定でき，そうでないなら無効にはできるが有効にはできない。
/// </summary>
/// <param name="name"></param>
/// <param name="isEnabled"></param>
    public void EnableReward(string name, bool isEnabled)
    {
        if (Achievements.ContainsKey(name))
        {
            if (isEnabled && Achievements[name].isAchieved)
                Achievements[name].isRewardEnabled = true;
            else
                Achievements[name].isRewardEnabled = false;
        }
    }
    public bool IsEnabled(string name)
    {
        if (Achievements.ContainsKey(name))
        {
            return Achievements[name].isRewardEnabled;
        }
        return false;
    }
    public bool IsUnlocked(string name)
    {
        if (Achievements.ContainsKey(name))
        {
            return Achievements[name].isAchieved;
        }
        return false;
    }
    public List<(string, AchievementData)> GetAchievementsFromRarity(params Rarity[] rarities)
    {
        HashSet<Rarity> raritySet = new(rarities);
        return Achievements.Where(a => raritySet.Contains(a.Value.Rarity)).Select(a => (a.Key, GetAchievementData(a.Key))).ToList();
    }
    public AchievementData GetAchievementData(string name)
    {
        return Achievements.ContainsKey(name) ? Achievements[name] : null;
    }

    /// <summary>
    /// すべてのセーブデータの実績を統合したのをAchievementsに格納する。
    /// </summary>
    void LoadAllSaveAchievements()
    {
        ResetAchievements();
        foreach (string saveSlotPath in Directory.GetDirectories(ApplicationManager.SaveDirectoryPath))
        {
            string achievementsPath = GetAchievementsPath(GameManager.GetPlayerDataPath(saveSlotPath));
            LoadAchievements(achievementsPath);
        }
    }

    void LoadAchievements(string achievementsPath)
    {
        if (File.Exists(achievementsPath))
        {
            AchievementsData achievementsData = EditFile.LoadCompressedJson<AchievementsData>(achievementsPath);
            var achievementDict = achievementsData.Achievements;
            foreach (var keyValue in achievementDict)
            {
                if (Achievements.ContainsKey(keyValue.Key))
                {
                    if (keyValue.Value.isAchieved)
                        Achievements[keyValue.Key].isAchieved = true;
                    if (keyValue.Value.isRewardEnabled)
                        Achievements[keyValue.Key].isRewardEnabled = true;
                }
            }
        }
    }
    void ResetAchievements()
    {
        foreach (var keyValue in Achievements)
        {
            keyValue.Value.isAchieved = false;
        }
    }
}

public enum Rarity
{
    Common,
    Uncommon,
    Rare,
    Epic,
    Legendary
}
