using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DeleteCautionUI : MonoBehaviour
{
    SaveSlotUI saveSlotUI;
    [SerializeField] TMP_Text discription;
    [SerializeField] Button yesButton;
    [SerializeField] Button noButton;
    public void Awake()
    {
        yesButton.onClick.AddListener(Yes);
        noButton.onClick.AddListener(No);
    }
    public void Open(SaveSlotUI saveSlotUI, string slotName, SaveHeaderData saveHeaderData)
    {
        this.saveSlotUI = saveSlotUI;
        discription.text = $@"{slotName} を本当に削除しますか？
        深さ: {saveHeaderData.Depth}
        プレイ時間: {saveHeaderData.playTime}
        モード: {saveHeaderData.GameMode}";
    }
    public void Yes()
    {
        saveSlotUI.Delete();
        Close();
    }
    public void No()
    {
        UIManager.Instance.Open(UIManager.UIType.SaveMenu);
        Close();
    }

    void Close()
    {
        ResourceManager.Release(ResourceManager.UIID.DeleteCautionUI, gameObject);
    }
}
