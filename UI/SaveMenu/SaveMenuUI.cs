using System.Collections;
using System.Collections.Generic;
using System.IO;
using MyGame;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class SaveMenuUI : UIPageBase
{
    string SaveHeaderName => EditFile.GetCompressedJsonName("SaveHeader");
    [SerializeField] Transform saveSlotFrame;
    [SerializeField] Button newGameButton;
    int slotNumber;
    protected override void Awake()
    {
        base.Awake();
        newGameButton.onClick.AddListener(OpenNewGameUI);
    }
    protected override void OnOpenCompleted()
    {
        base.OnOpenCompleted();
        m_closeActionType = CloseActionType.Back;
    }
    public override void Show()
    {
        int childCount = saveSlotFrame.transform.childCount;
        for (int i = 0; i < childCount - 1; i++)
        {
            m_resourceManager.ReleaseOther(ResourceManager.UIID.SaveSlotUI.ToString(), saveSlotFrame.transform.GetChild(i).gameObject);
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
                SaveHeaderData saveHeaderData = EditFile.LoadCompressedJson<SaveHeaderData>(saveHeaderPath);
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
        base.Show();
    }

    void MakeSaveSlotUI(SaveHeaderData saveHeaderData, string saveSlotName)
    {
        SaveSlotUI saveSlotUI = m_resourceManager.GetOther(ResourceManager.UIID.SaveSlotUI.ToString()).GetComponent<SaveSlotUI>();
        saveSlotUI.transform.SetParent(saveSlotFrame);
        saveSlotUI.transform.SetSiblingIndex(transform.childCount - 1);
        saveSlotUI.Init(this, saveHeaderData, saveSlotName);
    }

    public void LoadGame(string saveSlotName)
    {
        m_saveSlotName = saveSlotName;
        m_closeActionType = CloseActionType.LoadGame;
        UIManager.Instance.CloseAll();
    }
    string m_saveSlotName;

    public void OpenNewGameUI()
    {
        UIManager.Instance.Show(UIPageType.NewGameUI);
        INewGameUI newGameUI = UIManager.Instance.GetNewGameUI();
        newGameUI.SetSaveSlotName(GetSaveSlotName(slotNumber));
    }
    CloseActionType m_closeActionType;
    enum CloseActionType
    {
        Back,
        NewGame,
        LoadGame,
    }
    protected override void OnCloseCompleted()
    {
        base.OnCloseCompleted();
        switch (m_closeActionType)
        {
            case CloseActionType.LoadGame:
                Debug.Log($"GameRestart!!! directoryPath:{m_saveSlotName}");
                ApplicationManager.Instance.LoadGame(m_saveSlotName);
                break;
            default:
                break;
        }
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
