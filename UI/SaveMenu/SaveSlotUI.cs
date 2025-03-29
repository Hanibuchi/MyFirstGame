using System.Collections;
using System.Collections.Generic;
using System.IO;
using MyGame;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SaveSlotUI : MonoBehaviour, IResourceHandler
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

    private void Awake()
    {
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
        DeleteCautionUI deleteCautionUI = ResourceManager.Get(ResourceManager.UIID.DeleteCautionUI).GetComponent<DeleteCautionUI>();
        deleteCautionUI.Open(this, saveSlotName, saveHeaderData);
    }

    public void Delete()
    {
        ApplicationManager.Instance.DeleteSaveSlot(saveSlotName);
        ResourceManager.Release(ResourceManager.UIID.SaveSlotUI, gameObject);
    }

    public void OnRelease()
    {
        transform.SetParent(null);
    }
}
