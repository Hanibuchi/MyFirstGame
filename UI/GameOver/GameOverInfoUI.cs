using System.Collections;
using System.Collections.Generic;
using MyGame;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameOverInfoUI : UI
{
    [SerializeField] TMP_Text caseOfDeath;
    [SerializeField] TMP_Text area;
    // [SerializeField] TMP_Text chunk; // なんかメタい
    [SerializeField] TMP_Text depth;
    [SerializeField] TMP_Text traitors;

    [SerializeField] Button continuButton;
    [SerializeField] Button quitButton;

    protected override void Awake()
    {
        base.Awake();
        continuButton.onClick.AddListener(Continu);
        quitButton.onClick.AddListener(Quit);
    }

    public void Open(string causeOfDeath, AreaManager areaManager, Vector2Int chunkPos, Vector3 pos, float hiringCost, List<HiredMemberData> traitorDatas)
    {
        this.caseOfDeath.text = "死因: 「" + causeOfDeath + "」";
        this.area.text = "エリア: " + areaManager.gameObject.name;
        this.depth.text = "深さ: " + pos.y.ToString() + "m";
        string traitorsTxt = "";
        foreach (var traitorData in traitorDatas)
        {
            traitorsTxt += " $" + traitorData.hiringCost + "\n";
        }
        this.traitors.text = traitorsTxt != "" ? "別のパーティーに雇われた仲間: " + traitorsTxt : "";
    }

    void Close()
    {
        GetComponent<PoolableResourceComponent>().Release();
    }

    void Continu()
    {
        Close();
        GameManager.Instance.RespawnPlayer();
    }
    void Quit()
    {
        Close();
        ApplicationManager.Instance.Quit();
    }

    private void OnDestroy()
    {
        continuButton.onClick.RemoveAllListeners();
        quitButton.onClick.RemoveAllListeners();
    }
}
