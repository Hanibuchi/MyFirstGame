using System.Collections;
using System.Collections.Generic;
using System.IO;
using MyGame;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class SaveMenuUI : BackableMenuUI
{
    string SaveHeaderName => ApplicationManager.GetCompressedJsonName("SaveHeader");
    [SerializeField] Transform saveSlotFrame;
    [SerializeField] Button newGameButton;
    int slotNumber;
    protected override void Awake()
    {
        base.Awake();
        newGameButton.onClick.AddListener(OpenNewGameUI);
    }
    public override void Open()
    {
        base.Open();
        int childCount = saveSlotFrame.transform.childCount;
        for (int i = 0; i < childCount - 1; i++)
        {
            ResourceManager.Release(ResourceManager.UIID.SaveSlotUI, saveSlotFrame.transform.GetChild(i).gameObject);
        }
        slotNumber = 1;
        // Debug.Log($"SaveMenu Open");
        foreach (string saveSlotPath in Directory.GetDirectories(ApplicationManager.SaveDirectoryPath))
        {
            // Debug.Log($"saveSlotPath: {saveSlotPath}");
            string saveHeaderPath = Path.Combine(saveSlotPath, SaveHeaderName);

            // Debug.Log($"saveHeaderPath: {saveHeaderPath}");
            if (File.Exists(saveHeaderPath))
            {
                SaveHeaderData saveHeaderData = ApplicationManager.LoadCompressedJson<SaveHeaderData>(saveHeaderPath);
                // Debug.Log($"saveHeaderData: {saveHeaderData}");
                if (saveHeaderData == null)
                {
                    Debug.LogWarning($"{saveHeaderPath} is not valid saveHeaderData");
                    continue;
                }
                string saveSlotName = Path.GetFileName(saveSlotPath);
                // Debug.Log($"saveSlotName: {saveSlotName}");
                if (saveSlotName == GetSaveSlotName(slotNumber))
                    slotNumber++;
                MakeSaveSlotUI(saveHeaderData, saveSlotName);
            }
        }
    }

    void MakeSaveSlotUI(SaveHeaderData saveHeaderData, string saveSlotName)
    {
        SaveSlotUI saveSlotUI = ResourceManager.Get(ResourceManager.UIID.SaveSlotUI).GetComponent<SaveSlotUI>();
        saveSlotUI.transform.SetParent(saveSlotFrame);
        saveSlotUI.transform.SetSiblingIndex(transform.childCount - 1);
        saveSlotUI.Init(this, saveHeaderData, saveSlotName);
    }

    public void Restart(string saveSlotName)
    {
        Close();
        Debug.Log($"GameRestart!!! directoryPath:{saveSlotName}");
        ApplicationManager.Instance.LoadWorld(saveSlotName);
    }

    public void OpenNewGameUI()
    {
        Close(() =>
        {
            NewGameUI newGameUI = ResourceManager.Get(ResourceManager.UIID.NewGameUI).GetComponent<NewGameUI>();
            newGameUI.Open();
            newGameUI.SetSaveSlotName(GetSaveSlotName(slotNumber));
        }
        );
    }

    string GetSaveSlotName(int i)
    {
        return $"SaveSlot_{i}";
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        newGameButton.onClick.RemoveAllListeners();
    }
}
