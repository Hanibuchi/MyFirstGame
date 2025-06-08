using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class Test_ItemOwnership
{
    private PartyItemTracker _partyTracker;
    private MemberItemTracker _memberTracker;
    private PartyItemHolder _partyHolder;
    private MemberItemHolder _memberHolder;
    private ChildItemHolder _child1;
    private ChildItemHolder _child2;

    [OneTimeSetUp]
    public void Init()
    {
        // トラッカーの初期化
        _partyTracker = new PartyItemTracker();
        _memberTracker = new MemberItemTracker();

        // パーティホルダー作成し，メンバー追加
        _partyHolder = new PartyItemHolder();
        // メンバーホルダー作成し，アイテム追加
        _memberHolder = new MemberItemHolder();
        // メンバーとアイテムの初期化
        _child1 = new ChildItemHolder();
        _child2 = new ChildItemHolder();

        _partyHolder.SetPartyItemTracker(_partyTracker);
        _partyHolder.AddMember(_memberHolder);

        _memberHolder.SetMemberItemTracker(_memberTracker);
        _memberHolder.AddItem(_child1);
        _memberHolder.AddItem(_child2);
    }

    [Test]
    public void ChildItems_ShouldHaveSame_PartyItemTracker_AsPartyHolder()
    {
        foreach (var child in _memberHolder.Items)
        {
            Assert.AreSame(_partyHolder.PartyItemTracker, child.PartyItemTracker,
                "Child item does not share the same PartyItemTracker.");
        }
    }

    [Test]
    public void ChildItems_ShouldHaveSame_MemberItemTracker_AsMemberHolder()
    {
        foreach (var child in _memberHolder.Items)
        {
            Assert.AreSame(_memberHolder.MemberItemTracker, child.MemberItemTracker,
                "Child item does not share the same MemberItemTracker.");
        }
    }

    [Test]
    public void MemberHolder_ShouldHaveSame_PartyItemTracker_AsPartyHolder()
    {
        Assert.AreSame(_partyHolder.PartyItemTracker, _memberHolder.PartyItemTracker,
            "MemberItemHolder does not share the same PartyItemTracker as PartyItemHolder.");
    }
}
