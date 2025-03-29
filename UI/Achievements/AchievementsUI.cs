using System;
using System.Collections;
using System.Collections.Generic;
using MyGame;
using UnityEngine;
using UnityEngine.UI;

public class AchievementsUI : BackableMenuUI
{
    [SerializeField] Toggle common;
    [SerializeField] Toggle uncommon;
    [SerializeField] Toggle rare;
    [SerializeField] Toggle epic;
    [SerializeField] Toggle legendary;
    [SerializeField] RectTransform achievementsFrame;
    protected override void Awake()
    {
        base.Awake();
        common.onValueChanged.AddListener(UpdateAchievementsDisplay);
        uncommon.onValueChanged.AddListener(UpdateAchievementsDisplay);
        rare.onValueChanged.AddListener(UpdateAchievementsDisplay);
        epic.onValueChanged.AddListener(UpdateAchievementsDisplay);
        legendary.onValueChanged.AddListener(UpdateAchievementsDisplay);
    }
    public override void Open()
    {
        base.Open();
        UpdateAchievementsDisplay(true);
    }


    public override void Close()
    {
        base.Close();
        ResetToggles();
    }
    protected override void OnDestroy()
    {
        base.OnDestroy();
        common.onValueChanged.RemoveAllListeners();
        uncommon.onValueChanged.RemoveAllListeners();
        rare.onValueChanged.RemoveAllListeners();
        epic.onValueChanged.RemoveAllListeners();
        legendary.onValueChanged.RemoveAllListeners();
    }

    /// <summary>
    /// トグルの状態を読み取り，適切なUIを表示。改良の余地がある。遅かったら改良。変化したものだけ表示を変更するようにする場合，不要なものは再読み込みしないため表示中の変更はUIに反映されない。
    /// </summary>
    void UpdateAchievementsDisplay(bool a)
    {
        ReleaseAllEntrys();
        List<(string, AchievementData)> nameAndAchievementDatas = AchievementsManager.Instance.GetAchievementsFromRarity(GetRarities());
        foreach (var na in nameAndAchievementDatas)
        {
            (string name, AchievementData data) = na;
            AchievementEntryUI entryUI = ResourceManager.Get(ResourceManager.UIID.AchievementEntryUI).GetComponent<AchievementEntryUI>();
            entryUI.Set(name, data);
            entryUI.transform.SetParent(achievementsFrame);
        }
    }
    Rarity[] GetRarities()
    {
        List<Rarity> rarities = new();
        if (common.isOn) rarities.Add(Rarity.Common);
        if (uncommon.isOn) rarities.Add(Rarity.Uncommon);
        if (rare.isOn) rarities.Add(Rarity.Rare);
        if (epic.isOn) rarities.Add(Rarity.Epic);
        if (legendary.isOn) rarities.Add(Rarity.Legendary);
        return rarities.ToArray();
    }

    void ResetToggles()
    {
        common.isOn = true;
        uncommon.isOn = true;
        rare.isOn = true;
        epic.isOn = true;
        legendary.isOn = true;
    }
    void ReleaseAllEntrys()
    {
        var entryUIs = achievementsFrame.GetComponentsInChildren<AchievementEntryUI>();
        foreach (var entryUI in entryUIs)
        {
            entryUI.Release();
        }
    }
}
