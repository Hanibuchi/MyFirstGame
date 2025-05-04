using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using MyGame;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

// Partyの概要
// このPartyコンポネントがくっついた空オブジェクトをメンバーの親オブジェクトにする。こうすることでリーダーの変更の度にPartyを消さなくて済むし，実際のPartyの感じに似ていて把握しやすい。
public class Party : MonoBehaviour, IItemOwner
{
    [SerializeField] List<NPCManager> m_members = new();
    public List<NPCManager> Members => m_members;
    public NPCManager Leader => Members[0];
    /// <summary>
    /// PlayerのPartyかどうか。
    /// </summary>
    public virtual bool IsPlayerParty => false;
    public int MaxMembers { get; private set; } = 20;

    // public void Test()//デバッグ
    // {
    //     if (PartyMembers.Count == 0)
    //     {
    //         MemberPos = new List<Transform> { gameObject.transform };
    //         AddMember(gameObject.GetComponent<NPCManager>());
    //     }
    //     else
    //     {
    //         GameObject test = Instantiate(testNPC);
    //         AddMember(test.GetComponent<NPCManager>());
    //     }
    // }

    private void Awake()
    {
        OwnerParty = this;
        Owner = this;
        InitItems();
    }
    public virtual void Init()
    {
    }

    /// <summary>
    /// 末尾に新規メンバーを追加。
    /// </summary>
    /// <param name="member"></param>
    public void AddMember(NPCManager member)
    {
        AddMember(member, Members.Count);
    }

    /// <summary>
    /// 新規メンバーをindex番目に追加する。
    /// </summary>
    /// <param name="index"></param>
    /// <param name="member"></param>
    /// <returns></returns>
    public void AddMember(NPCManager member, int index)
    {
        if (Members.IndexOf(member) != -1)
        {
            InsertMember(member, index);
            return;
        }
        if (Members.Count >= MaxMembers)
        {
            Debug.Log("Party is full");
        }

        if (0 <= index && index <= Members.Count)
        {
            Members.Insert(index, member);
            member.OnJoinParty(this, index);
            // Debug.Log($"aaa index: {index}");
            if (index == 0)
            {
                // Debug.Log("bbb");
                member.OnBecomeLeader();
            }
        }
        else
            Debug.LogWarning("idex is out of range");
    }

    // 現リーダー(PartyMembers[0])と次のリーダーを交換する。
    public void ChangeLeader(NPCManager nextLeader)
    {
        if (nextLeader.IsLeader)
            return;

        if (Members.Contains(nextLeader))
        {
            nextLeader.OnBecomeLeader();
            Members[0].OnResignLeader();
            SwitchMembers(nextLeader, Members[0]);
        }
        else
        {
            Debug.Log("SwitchMembers do not contain the member");
        }
    }

    /// <summary>
    /// 既存のメンバーをindex番目にする
    /// </summary>
    /// <param name="member"></param>
    /// <param name="index"></param>
    public void InsertMember(NPCManager member, int index)
    {
        int currentIndex = Members.IndexOf(member);
        if (currentIndex == -1) // もしメンバーでないなら追加する。
        {
            AddMember(member, index);
            return;
        }

        if (0 <= index && index < Members.Count)
        {
            Members.Remove(member);
            // if (index > currentIndex) index--;
            Members.Insert(index, member);
        }
    }

    /// <summary>
    /// 2人のメンバーの場所を交代する。UIでスイッチする必要はない。代わりにInsertMemberを使う。
    /// </summary>
    /// <param name="memberA"></param>
    /// <param name="memberB"></param>
    /// <returns></returns>
    protected virtual bool SwitchMembers(NPCManager memberA, NPCManager memberB)
    {
        if (!Members.Contains(memberA) || !Members.Contains(memberB))
        {
            Debug.LogWarning("SwitchMembers do not contain the member");
            return false;
        }

        int indexA = Members.IndexOf(memberA);
        int indexB = Members.IndexOf(memberB);

        (Members[indexB], Members[indexA]) = (Members[indexA], Members[indexB]);
        return true;
    }

    public virtual bool RemoveMember(NPCManager member)
    {
        if (!Members.Contains(member))
        {
            Debug.LogWarning("member is not this party member");
            return false;
        }

        if (member.IsLeader) // もしリーダーの場合，２番目のメンバーをリーダーにする。
        {
            if (Members.Count >= 2)
                ChangeLeader(Members[1]);
            else // 一人しかいない場合
            {
                member.OnResignLeader();
                PartyManager.Instance.Delete(this);
            }
        }

        Members.Remove(member);
        member.OnLeaveParty();

        return true;
    }




    // ここからアイテム管理関連の改良版
    public Party OwnerParty { get; set; }
    public IItemOwner Owner { get; set; }
    [SerializeField] List<Item> m_items = new();
    /// <summary>
    /// アイテム
    /// </summary>
    public List<Item> Items { get => m_items; private set => m_items = value; }
    public int ItemCapacity { get; set; } = 1;
    public void InitItems()
    {
        ((IItemOwner)this).ResetItems();
    }
    // Party, MobM, BagItemに同じような処理あり（完全に同じとは限らない）。
    public bool CanAddItem(Item item)
    {
        if (Items.Count(x => x == null) >= 1)
            return true;
        else
        {
            foreach (var nextItem in Items)
            {
                if (nextItem is BagItem bag)
                {
                    Debug.Log($"bag.CanAddItem(item): {bag.CanAddItem(item)}");
                    if (bag.CanAddItem(item))
                        return true;
                }
            }
        }
        return false;
    }
    // Party, MobM, BagItemに同じような処理あり（完全に同じとは限らない）。
    public bool CanAddItem(int index, Item item)
    {
        if (0 <= index && index < Items.Count && item != null && Items[index] == null)
            return true;
        else
            return false;
    }
    // Party, MobM, BagItemに同じような処理あり（完全に同じとは限らない）。
    public void AddItem(Item item)
    {
        Debug.Log($"CanAddItem: {CanAddItem(item)}");
        if (CanAddItem(item))
        {
            for (int i = 0; i < Items.Count; i++)
            {
                if (Items[i] == null && CanAddItem(i, item))
                {
                    AddItem(i, item);
                    return;
                }
            }
            foreach (var nextItem in Items)
            {
                if (nextItem is BagItem bag && bag.CanAddItem(item))
                {
                    bag.AddItem(item);
                    return;
                }
            }
        }
    }
    // Party, MobM, BagItemに同じような処理あり（完全に同じとは限らない）。
    public void AddItem(int index, Item item)
    {
        if (CanAddItem(index, item))
        {
            Debug.Log($"Party, AddItem: {item} to {index}");
            item.RemovePrevRelation();
            Items[index] = item;
            item.OnAdded(this);
        }
        else
            Debug.LogWarning("item cannot be added");
    }
    public void RemoveItem(Item item)
    {
        if (Items.Contains(item))
        {
            int index = Items.IndexOf(item);
            Items[index] = null;
            item.OnRemoved();
        }
        else
            Debug.LogWarning("Item is not in this party.");
    }
    public virtual void RefreshItemSlotUIs()
    {
    }

    public void RegisterItem(Item item)
    {
        Debug.Log("Party, RegisterItem was called");
    }
    public void UnregisterItem(Item item)
    {
        Debug.Log("Party, UnregisterItem was called");
    }
    public virtual void RegisterItemAsParty(IItemOwner owner, Item item)
    {
        Debug.Log("Party, RegisterItemAsParty was called");
    }
    public virtual void UnregisterItemAsParty(Item item)
    {
        Debug.Log("Party, UnregisterItemAsParty was called");
    }
    public virtual void AddItemSlot()
    {
        ItemCapacity++;
        Items.Add(null);
    }

    // ここまでアイテム管理関連の改良版






    // ゲームオーバーの処理。publicメソッドAreAllAlliesDeadを実行してPartyMembersが全員死んでいたら実行される。
    public virtual void GameOver(string causeOfdeath)
    {
        foreach (var member in Members)
        {
            member.Surrender();
        }
        PartyManager.Instance.Delete(this);
        Debug.Log("ゲームオーバー！！！Leader: " + Members[0].name);
    }

    public PartyData MakePartyData()
    {
        return FillPartyData(new());
    }

    protected PartyData FillPartyData(PartyData partyData)
    {
        for (int i = 0; i < Members.Count; i++)
        {
            partyData.Members.Add(Members[i].MakeNPCData());
        }
        return partyData;
    }

    public static void SpawnParty(PartyData partyData)
    {
        Party party = PartyManager.Instance.Add();
        party.LoadPartyData(partyData);
    }

    protected void LoadPartyData(PartyData partyData)
    {
        var members = partyData.Members;
        for (int i = 0; i < members.Count; i++)
        {
            AddMember(NPCManager.SpawnNPC(members[i]));
        }
    }
}
