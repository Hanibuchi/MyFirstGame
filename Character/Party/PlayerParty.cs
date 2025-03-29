using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using MyGame;
using UnityEngine;

public class PlayerParty : Party
{
    public static string PlayerPartyPath => Path.Combine(GameManager.PlayerDataPath, "PlayerParty");
    public override bool IsPlayerParty => true;
    NPCManager PlayerManager => Leader;
    public void NewGame()
    {
        Init();
        NPCManager player = ResourceManager.Get(ResourceManager.MobID.NPC).GetComponent<NPCManager>();
        AddMember(player);
    }

    public void LoadWorld(PlayerPartyData playerPartyData)
    {
        LoadPartyData(playerPartyData);
    }

    public override void GameOver(string causeOfdeath)
    {
        float hiringCost = PlayerManager.BossChunkManager.BossAreaManager.GetHireAmount();
        var traitors = HireMembersForEnemy(hiringCost);
        AreaManager area = Leader.BossChunkManager.BossAreaManager;
        Vector2Int chunkPos = Leader.BossChunkManager.ChunkPos;
        Vector3 pos = Leader.transform.position;

        List<HiredMemberData> traitorDatas = traitors.Select(traitorAndCost =>
        {
            (NPCManager traitor, float cost) = traitorAndCost;
            RemoveMember(traitor);
            return new HiredMemberData()
            {
                NPCData = traitor.MakeNPCDataAndRelease(),
                HiredArea = area,
                HiredChunkPos = chunkPos,
                HiringCost = cost,
            };
        }).ToList();
        GameManager.Instance.GameOver(causeOfdeath, area, chunkPos, pos, hiringCost, traitorDatas);
    }

    List<(NPCManager, float)> HireMembersForEnemy(float hiringCost)
    {
        var sortedList = Members
            .Skip(1)
            .Select(item => new { member = item, cost = item.GetHiringCost() })
            .OrderByDescending(item => item.cost)
            .ToList();

        List<(NPCManager, float)> result = new();
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

    protected override bool SwitchMembers(NPCManager memberA, NPCManager memberB)
    {
        if (!base.SwitchMembers(memberA, memberB))
            return false;
        UIManager.Instance.EquipmentMenuManager.SwitchMembers(memberA.EquipmentMenu, memberB.EquipmentMenu);
        return true;
    }

    public override bool RemoveMember(NPCManager member)
    {
        if (!base.RemoveMember(member))
        {
            return false;
        }
        member.DestroyEquipmentMenu();
        return true;
    }
    public void Save()
    {
        ApplicationManager.SaveCompressedJson(PlayerPartyPath, MakePlayerPartyData());
    }

    public PlayerPartyData MakePlayerPartyData()
    {
        return FillPlayerPartyData(new());
    }

    protected PlayerPartyData FillPlayerPartyData(PlayerPartyData playerPartyData)
    {
        FillPartyData(playerPartyData);
        return playerPartyData;
    }
}
