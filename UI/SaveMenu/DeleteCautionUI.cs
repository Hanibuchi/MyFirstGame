using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DeleteCautionUI : UIPageBase, IDeleteCautionUI
{
    SaveSlotUI saveSlotUI;
    [SerializeField] TMP_Text discription;
    [SerializeField] Button yesButton;
    [SerializeField] Button noButton;
    protected override void Awake()
    {
        yesButton.onClick.AddListener(Yes);
        noButton.onClick.AddListener(No);
    }
    public void SetStats(SaveSlotUI saveSlotUI, string slotName, SaveHeaderData saveHeaderData)
    {
        this.saveSlotUI = saveSlotUI;
        discription.text = $@"{slotName} を本当に削除しますか？
        深さ: {saveHeaderData.Depth}
        プレイ時間: {saveHeaderData.playTime}
        モード: {saveHeaderData.GameMode}";
    }
    public void Yes()
    {
        UIManager.Instance.Back();
        saveSlotUI.Delete();
    }
    public void No()
    {
        UIManager.Instance.Back();
    }
}
