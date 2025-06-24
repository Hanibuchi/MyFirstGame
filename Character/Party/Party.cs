using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using MyGame;
using Newtonsoft.Json;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

[JsonObject(MemberSerialization.OptIn)]
public class Party : MonoBehaviour, ISerializableComponent, IPartyItemUIRefresher
{
    protected SerializeManager m_serializeHandler;
    [JsonProperty][SerializeField] List<PartyMember> m_members = new();
    public List<PartyMember> MemberList => m_members;
    public PartyMember Leader => MemberList[0];
    /// <summary>
    /// PlayerのPartyかどうか。
    /// </summary>
    public virtual bool IsPlayerParty => false;
    [JsonProperty][field: SerializeField] public uint Capacity { get; private set; } = 20;

    private void Awake()
    {
        m_serializeHandler = GetComponent<SerializeManager>();
    }
    public virtual void Init()
    {
        _partyItemHolder.SetPartyItemTracker(_partyItemTracker);
        _partyItemHolder.SetPartyItemUIRefresher(this);
        ItemHolder.ResetItems();
    }

    /// <summary>
    /// 末尾に新規メンバーを追加。
    /// </summary>
    /// <param name="member"></param>
    public void AddMember(PartyMember member)
    {
        AddMember(member, MemberList.Count);
    }

    /// <summary>
    /// 新規メンバーをindex番目に追加する。既存メンバーなら何もしない
    /// </summary>
    /// <param name="index"></param>
    /// <param name="member"></param>
    /// <returns></returns>
    public virtual void AddMember(PartyMember member, int index)
    {
        if (MemberList.IndexOf(member) != -1)
        {
            Debug.LogWarning("member is already in this party");
            return;
        }
        if (MemberList.Count >= Capacity)
        {
            Debug.Log("Party is full");
            return;
        }

        if (0 <= index && index <= MemberList.Count)
        {
            MemberList.Insert(index, member);
            member.OnJoinParty(this);
            if (index == 0)
            {
                member.OnBecomeLeader();
            }
            if (member.TryGetComponent(out ItemUser itemUser))
                ItemHolder.AddMember(itemUser.ItemHolder);
        }
        else
            Debug.LogWarning("idex is out of range");
    }

    public void RemoveMember(PartyMember member)
    {
        if (!MemberList.Contains(member))
        {
            Debug.LogWarning("member is not this party member");
            return;
        }

        if (member.IsLeader) // もしリーダーの場合，２番目のメンバーをリーダーにする。
        {
            if (MemberList.Count >= 2)
                ChangeLeader(member, MemberList[1]);
            else // 一人しかいない場合
            {
                member.OnResignLeader();
            }
        }

        MemberList.Remove(member);
        member.OnLeaveParty();

        if (member.TryGetComponent(out ItemUser itemUser))
            ItemHolder.RemoveMember(itemUser.ItemHolder);
    }

    /// <summary>
    /// 既存のメンバーをindex番目にする
    /// </summary>
    /// <param name="member"></param>
    /// <param name="index"></param>
    public void InsertMember(PartyMember member, int index)
    {
        int currentIndex = MemberList.IndexOf(member);
        if (currentIndex == -1) // もしメンバーでないなら追加する。
        {
            Debug.LogWarning("member is not in this party");
            return;
        }

        if (0 <= index && index < MemberList.Count)
        {
            if (index == 0)
            {
                ChangeLeader(MemberList[0], member);
            }
            MemberList.Remove(member);
            MemberList.Insert(index, member);
        }
    }

    /// <summary>
    /// 2人のメンバーの場所を交代する。
    /// </summary>
    /// <param name="memberA"></param>
    /// <param name="memberB"></param>
    /// <returns></returns>
    public virtual void SwitchMembers(PartyMember memberA, PartyMember memberB)
    {
        if (!MemberList.Contains(memberA) || !MemberList.Contains(memberB))
        {
            Debug.LogWarning("SwitchMembers do not contain the member");
            return;
        }

        int indexA = MemberList.IndexOf(memberA);
        int indexB = MemberList.IndexOf(memberB);

        if (indexA == 0)
            ChangeLeader(memberA, memberB);
        if (indexB == 0)
            ChangeLeader(memberB, memberA);

        (MemberList[indexB], MemberList[indexA]) = (MemberList[indexA], MemberList[indexB]);
    }

    // 現リーダー(PartyMembers[0])と次のリーダーを交換する。
    void ChangeLeader(PartyMember prevLeader, PartyMember nextLeader)
    {
        prevLeader.OnResignLeader();
        nextLeader.OnBecomeLeader();
    }

    // ゲームオーバーの処理。
    public virtual void GameOver()
    {
        foreach (var member in MemberList)
        {
            member.Surrender();
        }
        Debug.Log("ゲームオーバー！！！Leader: " + MemberList[0].name);
        PartyManager.Instance.ReleaseParty(this);
    }


    [SerializeField] PartyItemTracker _partyItemTracker = new();
    public PartyItemTracker PartyItemTracker => _partyItemTracker;
    [SerializeField] PartyItemHolder _partyItemHolder = new();
    public PartyItemHolder ItemHolder => _partyItemHolder;

    public virtual void RefreshUI()
    {
    }

    // ここまでアイテム管理関連の改良版

//


}
