using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using UnityEngine;

public class Test_PartyItemHolder
{
    PartyItemHolder _partyItemHolder;
    ItemTypeProvider_mock _itemTypeProvider_mock;

    [SetUp]
    public void SetUp()
    {
        _partyItemHolder = new PartyItemHolder();
        _itemTypeProvider_mock = new ItemTypeProvider_mock(ItemType.None);
        _partyItemHolder.SetPartyItemTracker(new PartyItemTracker());
        _partyItemHolder.ResetItems();
        for (int i = 0; i < 4; i++)
            _partyItemHolder.AddItemCapacity();
    }

    [Test]
    public void Can_Add_Items_Correctly()
    {
        for (int i = 0; i < _partyItemHolder.ItemCapacity; i++)
        {
            var child = new ChildItemHolder();
            child.SetItemTypeProvider(_itemTypeProvider_mock);
            Assert.IsTrue(_partyItemHolder.CanAddItem(child));
            _partyItemHolder.AddItem(child);
            Assert.AreSame(_partyItemHolder.Items[i], child);
            Assert.AreSame(_partyItemHolder.PartyItemTracker, child.PartyItemTracker);
        }

        var overflowItem = new ChildItemHolder();
        overflowItem.SetItemTypeProvider(_itemTypeProvider_mock);
        Assert.IsFalse(_partyItemHolder.CanAddItem(overflowItem));
        int countBefore = _partyItemHolder.Items.Count(a => a != null);
        _partyItemHolder.AddItem(overflowItem);
        Assert.AreEqual(countBefore, _partyItemHolder.Items.Count(a => a != null));
    }

    [Test]
    public void Can_Insert_Item_At_Index()
    {
        var item1 = new ChildItemHolder();
        item1.SetItemTypeProvider(_itemTypeProvider_mock);
        _partyItemHolder.AddItem(item1);

        var item2 = new ChildItemHolder();
        item2.SetItemTypeProvider(_itemTypeProvider_mock);
        _partyItemHolder.AddItemAt(0, item2);

        Assert.AreSame(item2, _partyItemHolder.Items[0]);
        Assert.AreSame(item1, _partyItemHolder.Items[1]);

        List<ChildItemHolder> childItems = Enumerable.Repeat<ChildItemHolder>(null, 8).ToList();
        _partyItemHolder.ResetItems();
        childItems[1] = AddItemAt(1);
        childItems[2] = AddItemAt(2);
        childItems[3] = AddItemAt(3);
        item1 = new();
        item1.SetItemTypeProvider(_itemTypeProvider_mock);
        _partyItemHolder.AddItemAt(0, item1);
        for (int i = 0; i < 4; i++)
        {
            if (i == 0)
                Assert.AreSame(item1, _partyItemHolder.Items[i]);
            else
                Assert.AreSame(childItems[i], _partyItemHolder.Items[i]);
        }

        childItems = Enumerable.Repeat<ChildItemHolder>(null, 8).ToList();
        _partyItemHolder.ResetItems();
        childItems[0] = AddItemAt(1);
        item1 = new();
        item1.SetItemTypeProvider(_itemTypeProvider_mock);
        _partyItemHolder.AddItemAt(1, item1);
        for (int i = 0; i < 2; i++)
        {
            if (i == 1)
                Assert.AreSame(item1, _partyItemHolder.Items[i]);
            else
                Assert.AreSame(childItems[i], _partyItemHolder.Items[i]);
        }

        childItems = Enumerable.Repeat<ChildItemHolder>(null, 8).ToList();
        _partyItemHolder.ResetItems();
        childItems[1] = AddItemAt(1);
        childItems[3] = AddItemAt(2);
        item1 = new();
        item1.SetItemTypeProvider(_itemTypeProvider_mock);
        _partyItemHolder.AddItemAt(2, item1);
        for (int i = 0; i < 3; i++)
        {
            if (i == 2)
                Assert.AreSame(item1, _partyItemHolder.Items[i]);
            else
                Assert.AreSame(childItems[i], _partyItemHolder.Items[i]);
        }

        childItems = Enumerable.Repeat<ChildItemHolder>(null, 4).ToList();
        _partyItemHolder.ResetItems();
        childItems[0] = AddItemAt(1);
        childItems[1] = AddItemAt(2);
        childItems[3] = AddItemAt(3);
        item1 = new();
        item1.SetItemTypeProvider(_itemTypeProvider_mock);
        _partyItemHolder.AddItemAt(2, item1);
        for (int i = 0; i < 4; i++)
        {
            if (i == 2)
                Assert.AreSame(item1, _partyItemHolder.Items[i]);
            else
                Assert.AreSame(childItems[i], _partyItemHolder.Items[i]);
        }
    }

    ChildItemHolder AddItemAt(int index)
    {
        var childItem = new ChildItemHolder();
        childItem.SetItemTypeProvider(_itemTypeProvider_mock);
        _partyItemHolder.AddItemAt(index, childItem);
        return childItem;
    }

    [Test]
    public void Should_Respect_IsFixedSize()
    {
        _partyItemHolder.SetPartyItemTracker(new PartyItemTracker());
        _partyItemHolder.ResetItems();

        Assert.AreEqual(_partyItemHolder.ItemCapacity, _partyItemHolder.Items.Count);

        for (int i = 0; i < _partyItemHolder.ItemCapacity; i++)
        {
            var child = new ChildItemHolder();
            child.SetItemTypeProvider(_itemTypeProvider_mock);
            Assert.IsTrue(_partyItemHolder.CanAddItem(child));
            _partyItemHolder.AddItem(child);
            Assert.AreSame(_partyItemHolder.Items[i], child);
        }

        var overflow = new ChildItemHolder();
        overflow.SetItemTypeProvider(_itemTypeProvider_mock);
        Assert.IsFalse(_partyItemHolder.CanAddItem(overflow));
        _partyItemHolder.AddItem(overflow);
        foreach (var item in _partyItemHolder.Items)
        {
            Assert.AreNotSame(item, overflow);
        }
    }

    [Test]
    public void CanAddItemAt_Index_OutOfBounds_ShouldBeFalse()
    {
        var item = new ChildItemHolder();
        item.SetItemTypeProvider(_itemTypeProvider_mock);
        Assert.IsFalse(_partyItemHolder.CanAddItemAt(-1, item));
        Assert.IsFalse(_partyItemHolder.CanAddItemAt(1000, item));
    }

    [Test]
    public void CanAddAndRemoveMemberCorrectly()
    {
        List<MemberItemHolder> members = Enumerable.Repeat<MemberItemHolder>(null, 8).ToList();
        for (int i = 0; i < 8; i++)
        {
            var member = new MemberItemHolder();
            members[i] = member;
            _partyItemHolder.AddMember(member);
            Assert.AreSame(_partyItemHolder.PartyItemTracker, member.PartyItemTracker);
        }

        for (int i = 8 - 1; i >= 0; i--)
        {
            var member = members[i];
            _partyItemHolder.RemoveMember(member);
            Assert.IsNull(member.PartyItemTracker);
        }
    }
}
