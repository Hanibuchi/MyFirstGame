using System.Collections;
using System.Collections.Generic;
using System.IO;
using MyGame;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SaveSlotUI : UI
{
    SaveMenuUI saveMenuUI;
    string saveSlotName;
    SaveHeaderData saveHeaderData;
    [SerializeField] TMP_Text slotName;
    [SerializeField] TMP_Text gameMode;
    [SerializeField] TMP_Text depth;
    [SerializeField] TMP_Text time;
    [SerializeField] Button deleteButton;
    // [SerializeField]  image;

    protected override void Awake()
    {
        base.Awake();
        deleteButton.onClick.AddListener(ConfirmDelete);
    }

    public void Init(SaveMenuUI saveMenuUI, SaveHeaderData saveHeaderData, string saveSlotName)
    {
        this.saveMenuUI = saveMenuUI;
        this.saveSlotName = saveSlotName;
        slotName.text = saveSlotName;
        this.saveHeaderData = saveHeaderData;
        gameMode.text = saveHeaderData.GameMode.ToString();
        depth.text = saveHeaderData.Depth.ToString();
        time.text = Functions.FormatTime(saveHeaderData.playTime);
    }
    public void Restart()
    {
        saveMenuUI.Restart(saveSlotName);
    }

    public void ConfirmDelete()
    {
        saveMenuUI.Close();
        DeleteCautionUI deleteCautionUI = ResourceManager.GetOther(ResourceManager.UIID.DeleteCautionUI.ToString()).GetComponent<DeleteCautionUI>();
        deleteCautionUI.Open(this, saveSlotName, saveHeaderData);
    }

    public void Delete()
    {
        ApplicationManager.Instance.DeleteSaveSlot(saveSlotName);
        ResourceManager.ReleaseOther(ResourceManager.UIID.SaveSlotUI.ToString(), gameObject);
    }

    public override void OnRelease()
    {
        base.OnRelease();
        transform.SetParent(null);
    }
}
