using System.Collections;
using System.Collections.Generic;
using MyGame;
using Unity.VisualScripting;
using UnityEditor.iOS;
using UnityEditor.VersionControl;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;

public class TestClass : MonoBehaviour
{
    public static TestClass Instance;
    public GameObject item;
    public void Test()
    {
    }
    public void Test2()
    {
    }
    public void Test_TerrainAllReset()
    {
        GameManager.Instance.TerrainManager.Reset();
    }
    public void Test_Message()
    {
        string message = "aaaaaaaaaaaaa";
        var messageUI = ResourceManager.GetOther(ResourceManager.UIID.MessageUI.ToString()).GetComponent<MessageUI>();
        messageUI.Open(message, () =>
UIManager.Instance.Open(UIManager.UIType.SaveMenu));
    }
    public ObjectManager objectManager;
    public void Test_AddStatus()
    {
        if (objectManager is IStatusAffectable statusAffectable)
        {
            statusAffectable.AddStatus(Status.StatusID.PowerBoost);
        }
    }
    public void Test_AddStatus2()
    {
        if (objectManager is IStatusAffectable statusAffectable)
        {
            var status = statusAffectable.AddStatus(Status.StatusID.PowerBoost, Status.StatusType.Wet, 1);
        }
    }
    void AAA()
    {
        Debug.Log("AAA");
    }
    public void Test_AddStatus3()
    {
        if (objectManager is IStatusAffectable statusAffectable)
        {
            statusAffectable.AddStatus(Status.StatusID.PowerBoost, Status.StatusType.Duration, 5);
        }
    }

    private void Awake()
    {
        DontDestroyOnLoad(transform.parent);
        Instance = this;
    }

    [SerializeField] string itemID = ResourceManager.ItemID.PurpleFurball.ToString();
    public void SpawnItem()
    {
        ResourceManager.GetItem(itemID);
    }
    [SerializeField] string mobID = ResourceManager.MobID.Enemy.ToString();
    [SerializeField] string otherID;
    public void SpawnMob()
    {
        var enemy = ResourceManager.GetMob(mobID);
        if (enemy != null)
            enemy.transform.position = Vector2.zero;
    }
    public void SpawnOther()
    {
        ResourceManager.GetOther(otherID);
    }
    public Party party;
    public NPCManager nextLeader;
    public void ChangeLeader()
    {
        party.ChangeLeader(nextLeader);
    }

    public void Test_GameStart()
    {
        ApplicationManager.Instance.CreateNewWorld(new InitGameData()
        {
            SaveSlotName = "",
            Seed = 0,
            GameMode = GameMode.Hard,
        });
    }

    public void Test_OpenSaveNemu()
    {
        UIManager.Instance.Open(UIManager.UIType.SaveMenu);
    }
    public string achievementName;
    public bool isEnableAchievement;
    public void UnlockAchievement()
    {
        AchievementsManager.Instance.Unlock(achievementName);
    }
    public void EnableReward()
    {
        AchievementsManager.Instance.EnableReward(achievementName, isEnableAchievement);
    }
}
