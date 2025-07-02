using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using MyGame;
using Unity.VisualScripting;
using UnityEngine;
using Zenject;

public class PlayerParty : Party
{
    public static string PlayerPartyPath => Path.Combine(GameManager.PlayerDataPath, "PlayerParty");
    public override bool IsPlayerParty => true;
    PartyMember Player => Leader;
    IPartyInventoryUI m_inventoryUI;
    IEquipmentUI m_equipmentUI;

    public void OnGameStart()
    {
        Init();
        if (File.Exists(PlayerPartyPath))
        {
            string json = EditFile.ReadAndDecompressJson(PlayerPartyPath);
            if (m_serializeHandler == null)
                m_serializeHandler = GetComponent<SerializeManager>();
            m_serializeHandler.LoadState(EditFile.JsonToJObject(json));
        }
        else
        {
            PartyMember player = ResourceManager.Instance.GetMob(ResourceManager.MobID.NPC.ToString()).GetComponent<PartyMember>();
            AddMember(player);
        }
    }

    public override void Init()
    {
        m_equipmentUI = UIManager.Instance.GetEquipmentUI();
        m_inventoryUI = UIManager.Instance.GetInventoryUI();
        m_inventoryUI.SetParty(this);
        base.Init();
    }

    public void Save()
    {
        if (TryGetComponent(out SerializeManager serializeManager))
            EditFile.CompressAndSaveJson(PlayerPartyPath, serializeManager.SaveState().ToString());
        else
            Debug.LogError($"SerializeManager not found on {gameObject.name}");
    }

    public override void GameOver()
    {
        // Vector3 pos = Player.transform.position;
        // AreaManager area = TerrainManager.GetAreaFromPos(pos);
        // Vector2Int chunkPos = TerrainManager.GetChunkFromPos(pos);
        // float hiringCost = area.GetHireAmount();
        // var traitors = HireMembersForEnemy(hiringCost);

        // List<HiredMemberData> traitorDatas = traitors.Select(traitorAndCost =>
        // {
        //     (PartyMember traitor, float cost) = traitorAndCost;
        //     RemoveMember(traitor);
        //     return new HiredMemberData()
        //     {
        //         memberData = traitor.GetComponent<SerializeManager>().SaveState(),
        // hiredArea = area,
        // hiredChunkPos = chunkPos,
        //         hiringCost = cost,
        //     };
        // }).ToList();
        // string causeOfDeath = Player.GetComponent<DeathHandler>().CreateCauseOfDeath();
        // GameManager.Instance.GameOver(causeOfDeath, area, chunkPos, pos, hiringCost, traitorDatas);
    }

    List<(PartyMember, float)> HireMembersForEnemy(float hiringCost)
    {
        var sortedList = MemberList
            .Skip(1)
            .Select(item => new { member = item, cost = item.GetHiringCost() })
            .OrderByDescending(item => item.cost)
            .ToList();

        List<(PartyMember, float)> result = new();
        float remaining = hiringCost;

        foreach (var item in sortedList)
        {
            if (remaining - item.cost >= 0)
            {
                remaining -= item.cost;
                result.Add((item.member, item.cost));
            }
            else
            {
                break;
            }
        }
        return result;
    }

    public override void AddMember(PartyMember member, int index)
    {
        base.AddMember(member, index);
        RefreshEquipmentUI();
    }

    public override void SwitchMembers(PartyMember memberA, PartyMember memberB)
    {
        base.SwitchMembers(memberA, memberB);
        RefreshEquipmentUI();
    }
    public void RefreshEquipmentUI()
    {
        m_equipmentUI.DetachMemberUIs();
        foreach (var member in MemberList)
        {
            var equipmentUI = member.GetMemberUI();
            m_equipmentUI.SetMemberUI(equipmentUI.gameObject);
        }
    }

    public override void RefreshUI()
    {
        base.RefreshUI();
        m_inventoryUI.UpdateInventoryUI(ItemHolder.Items);
    }
}
