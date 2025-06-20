using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class GameOverUI : UI
{
    string causeOfDeath;
    AreaManager areaManager;
    Vector2Int chunkPos;
    Vector3 pos;
    float hiringCost;
    List<HiredMemberData> traitorDatas;
    [SerializeField] Button OKButton;
    protected override void Awake()
    {
        base.Awake();
        OKButton.onClick.AddListener(OK);
    }
    public void Open(string causeOfDeath, AreaManager areaManager, Vector2Int chunkPos, Vector3 pos, float hiringCost, List<HiredMemberData> traitorDatas)
    {
        this.causeOfDeath = causeOfDeath;
        this.areaManager = areaManager;
        this.chunkPos = chunkPos;
        this.pos = pos;
        this.hiringCost = hiringCost;
        this.traitorDatas = traitorDatas;
    }

    void OK()
    {
        Close();
        GameOverInfoUI gameOverInfoUI = ResourceManager.Instance.GetOther(ResourceManager.UIID.GameOverInfoUI.ToString()).GetComponent<GameOverInfoUI>();
        gameOverInfoUI.Open(causeOfDeath, areaManager, chunkPos, pos, hiringCost, traitorDatas);
    }

    void Close()
    {
        GetComponent<PoolableResourceComponent>().Release();
    }
}
