using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Codice.Client.BaseCommands.TubeClient;
using Codice.CM.Common;
using NUnit.Framework;
using UnityEngine;

public class Test_ChildItemHolder
{
    ChildItemHolder _childItemHolder;
    ItemTypeProvider_mock _itemTypeProvider_mock;
    [SetUp]
    public void SetUp()
    {
        _childItemHolder = new();
        _itemTypeProvider_mock = new(ItemType.None);
        _childItemHolder.SetItemTypeProvider(_itemTypeProvider_mock);
        _childItemHolder.SetPartyItemTracker(new PartyItemTracker());
        _childItemHolder.SetMemberItemTracker(new MemberItemTracker());
        _childItemHolder.ResetItems();
    }

    [Test]
    public void ChildITemHolder_ShouldHaveSame_ItemTypeProvider_AsItemTypeProvider()
    {
        Assert.AreEqual(_itemTypeProvider_mock.GetItemType(), _childItemHolder.GetItemType());
    }
    [Test]
    public void CanAddItemsCorrectly()
    {
        ChildItemHolder childItem1;
        ChildItemHolder childItem2;

        for (int i = 0; i < _childItemHolder.ItemCapacity; i++)
        {
            ChildItemHolder childItem = new ChildItemHolder();
            childItem.SetItemTypeProvider(_itemTypeProvider_mock);
            Assert.IsTrue(_childItemHolder.CanAddItem(childItem));
            _childItemHolder.AddItem(childItem);
            Assert.AreSame(_childItemHolder.Items[i], childItem);
            Assert.AreSame(_childItemHolder.PartyItemTracker, childItem.PartyItemTracker);
            Assert.AreSame(_childItemHolder.MemberItemTracker, childItem.MemberItemTracker);
        }
        childItem1 = new ChildItemHolder();
        childItem1.SetItemTypeProvider(_itemTypeProvider_mock);
        Assert.IsTrue(!_childItemHolder.CanAddItem(childItem1));
        int prevLength = _childItemHolder.Items.Count;
        _childItemHolder.AddItem(childItem1);
        Assert.AreEqual(prevLength, _childItemHolder.Items.Count);
        foreach (var item in _childItemHolder.Items)
        {
            Assert.AreNotSame(childItem1, item);
        }

        _childItemHolder.ResetItems();
        childItem1 = new ChildItemHolder();
        childItem1.SetItemTypeProvider(_itemTypeProvider_mock);
        _childItemHolder.AddItem(childItem1);
        childItem2 = new ChildItemHolder();
        childItem2.SetItemTypeProvider(_itemTypeProvider_mock);
        _childItemHolder.AddItemAt(0, childItem2);
        Assert.AreSame(childItem2, _childItemHolder.Items[0]);
        Assert.AreSame(childItem1, _childItemHolder.Items[1]);
    }
    [Test]
    public void CanAddItemsCorrectly_Fixed()
    {
        ChildItemHolder childItem1;
        ChildItemHolder childItem2;

        _childItemHolder.IsFixedSize = true;
        _childItemHolder.ResetItems();
        Assert.AreEqual(_childItemHolder.Items.Count, _childItemHolder.ItemCapacity);

        for (int i = 0; i < _childItemHolder.ItemCapacity; i++)
        {
            ChildItemHolder childItem = new ChildItemHolder();
            childItem.SetItemTypeProvider(_itemTypeProvider_mock);
            Assert.IsTrue(_childItemHolder.CanAddItem(childItem));
            _childItemHolder.AddItem(childItem);
            Assert.AreSame(_childItemHolder.Items[i], childItem);
            Assert.AreSame(_childItemHolder.PartyItemTracker, childItem.PartyItemTracker);
            Assert.AreSame(_childItemHolder.MemberItemTracker, childItem.MemberItemTracker);
        }
        childItem1 = new ChildItemHolder();
        childItem1.SetItemTypeProvider(_itemTypeProvider_mock);
        Assert.IsFalse(_childItemHolder.CanAddItem(childItem1));
        _childItemHolder.AddItem(childItem1);
        foreach (var item in _childItemHolder.Items)
        {
            Assert.AreNotSame(childItem1, item);
        }

        _childItemHolder.ResetItems();
        childItem1 = new ChildItemHolder();
        childItem1.SetItemTypeProvider(_itemTypeProvider_mock);
        _childItemHolder.AddItem(childItem1);
        childItem2 = new ChildItemHolder();
        childItem2.SetItemTypeProvider(_itemTypeProvider_mock);
        Assert.IsTrue(_childItemHolder.CanAddItemAt(0, childItem2));
        _childItemHolder.AddItemAt(0, childItem2);
        Assert.AreSame(childItem2, _childItemHolder.Items[0]);
        Assert.AreSame(childItem1, _childItemHolder.Items[1]);


        List<ChildItemHolder> childItems = Enumerable.Repeat<ChildItemHolder>(null, 8).ToList();
        _childItemHolder.ResetItems();
        childItems[1] = AddItemAt(1);
        childItems[2] = AddItemAt(2);
        childItems[3] = AddItemAt(3);
        childItem1 = new();
        childItem1.SetItemTypeProvider(_itemTypeProvider_mock);
        _childItemHolder.AddItemAt(0, childItem1);
        for (int i = 0; i < 4; i++)
        {
            if (i == 0)
                Assert.AreSame(childItem1, _childItemHolder.Items[i]);
            else
                Assert.AreSame(childItems[i], _childItemHolder.Items[i]);
        }

        childItems = Enumerable.Repeat<ChildItemHolder>(null, 8).ToList();
        _childItemHolder.ResetItems();
        childItems[0] = AddItemAt(1);
        childItem1 = new();
        childItem1.SetItemTypeProvider(_itemTypeProvider_mock);
        _childItemHolder.AddItemAt(1, childItem1);
        for (int i = 0; i < 2; i++)
        {
            if (i == 1)
                Assert.AreSame(childItem1, _childItemHolder.Items[i]);
            else
                Assert.AreSame(childItems[i], _childItemHolder.Items[i]);
        }

        childItems = Enumerable.Repeat<ChildItemHolder>(null, 8).ToList();
        _childItemHolder.ResetItems();
        childItems[1] = AddItemAt(1);
        childItems[3] = AddItemAt(2);
        childItem1 = new();
        childItem1.SetItemTypeProvider(_itemTypeProvider_mock);
        _childItemHolder.AddItemAt(2, childItem1);
        for (int i = 0; i < 2; i++)
        {
            if (i == 2)
                Assert.AreSame(childItem1, _childItemHolder.Items[i]);
            else
                Assert.AreSame(childItems[i], _childItemHolder.Items[i]);
        }

        childItems = Enumerable.Repeat<ChildItemHolder>(null, 8).ToList();
        _childItemHolder.ResetItems();
        childItems[0] = AddItemAt(1);
        childItems[1] = AddItemAt(2);
        childItems[3] = AddItemAt(3);
        childItems[4] = AddItemAt(4);
        childItem1 = new();
        childItem1.SetItemTypeProvider(_itemTypeProvider_mock);
        _childItemHolder.AddItemAt(2, childItem1);
        for (int i = 0; i < 5; i++)
        {
            if (i == 2)
                Assert.AreSame(childItem1, _childItemHolder.Items[i]);
            else
                Assert.AreSame(childItems[i], _childItemHolder.Items[i]);
        }
    }
    ChildItemHolder AddItemAt(int index)
    {
        var childItem = new ChildItemHolder();
        childItem.SetItemTypeProvider(_itemTypeProvider_mock);
        _childItemHolder.AddItemAt(index, childItem);
        return childItem;
    }

    [Test]
    public void RemoveItems_ShouldRemoveSpecifiedItem()
    {
        var item1 = new ChildItemHolder();
        var item2 = new ChildItemHolder();
        item1.SetItemTypeProvider(_itemTypeProvider_mock);
        item2.SetItemTypeProvider(_itemTypeProvider_mock);

        _childItemHolder.AddItem(item1);
        _childItemHolder.AddItem(item2);

        Assert.AreEqual(2, _childItemHolder.Items.Count);

        _childItemHolder.RemoveItem(item1);

        Assert.AreEqual(1, _childItemHolder.Items.Count);
        Assert.AreSame(item2, _childItemHolder.Items[0]);
        Assert.IsFalse(_childItemHolder.Items.Contains(item1));
    }

    [Test]
    public void RemoveItems_ShouldRemoveSpecifiedItem_Fixed()
    {
        _childItemHolder.IsFixedSize = true;
        _childItemHolder.ResetItems();

        var item1 = new ChildItemHolder();
        var item2 = new ChildItemHolder();
        item1.SetItemTypeProvider(_itemTypeProvider_mock);
        item2.SetItemTypeProvider(_itemTypeProvider_mock);

        _childItemHolder.AddItem(item1);
        _childItemHolder.AddItem(item2);

        Assert.AreEqual(2, _childItemHolder.Items.Count(a => a != null));

        _childItemHolder.RemoveItem(item1);

        Assert.AreEqual(1, _childItemHolder.Items.Count(a => a != null));
        Assert.AreSame(item2, _childItemHolder.Items[1]);
        Assert.IsNull(_childItemHolder.Items[0]);
        Assert.IsFalse(_childItemHolder.Items.Contains(item1));
    }

    [Test]
    public void ResetItems_ShouldClearAllItems()
    {
        for (int i = 0; i < _childItemHolder.ItemCapacity; i++)
        {
            var item = new ChildItemHolder();
            item.SetItemTypeProvider(_itemTypeProvider_mock);
            _childItemHolder.AddItem(item);
        }

        Assert.AreEqual(_childItemHolder.ItemCapacity, _childItemHolder.Items.Count);

        _childItemHolder.ResetItems();

        Assert.AreEqual(0, _childItemHolder.Items.Count);

    }


    [Test]
    public void ResetItems_ShouldClearAllItems_Fixed()
    {
        _childItemHolder.IsFixedSize = true;
        _childItemHolder.ResetItems();

        for (int i = 0; i < _childItemHolder.ItemCapacity; i++)
        {
            var item = new ChildItemHolder();
            item.SetItemTypeProvider(_itemTypeProvider_mock);
            _childItemHolder.AddItem(item);
        }

        Assert.AreEqual(_childItemHolder.ItemCapacity, _childItemHolder.Items.Count);

        _childItemHolder.ResetItems();

        Assert.AreEqual(0, _childItemHolder.Items.Count(a => a != null));
    }

    [Test]
    public void RemoveItems_ShouldCallOnRemovedFromPartyAndMember()
    {
        var mockItem = new ChildItemHolder();
        mockItem.SetItemTypeProvider(_itemTypeProvider_mock);

        PartyItemTracker party = new();
        MemberItemTracker member = new();
        _childItemHolder.SetPartyItemTracker(party);
        _childItemHolder.SetMemberItemTracker(member);

        _childItemHolder.AddItem(mockItem);

        Assert.AreSame(mockItem.PartyItemTracker, party);
        Assert.AreSame(mockItem.MemberItemTracker, member);
        _childItemHolder.RemoveItem(mockItem);

        Assert.IsNull(mockItem.PartyItemTracker);
        Assert.IsNull(mockItem.MemberItemTracker);
    }
    [Test]
    public void RemoveItems_ShouldCallOnRemovedFromPartyAndMember_Fixed()
    {
        _childItemHolder.IsFixedSize = true;
        _childItemHolder.ResetItems();

        var mockItem = new ChildItemHolder();
        mockItem.SetItemTypeProvider(_itemTypeProvider_mock);

        PartyItemTracker party = new();
        MemberItemTracker member = new();
        _childItemHolder.SetPartyItemTracker(party);
        _childItemHolder.SetMemberItemTracker(member);

        _childItemHolder.AddItem(mockItem);

        Assert.AreSame(mockItem.PartyItemTracker, party);
        Assert.AreSame(mockItem.MemberItemTracker, member);
        _childItemHolder.RemoveItem(mockItem);

        Assert.IsNull(mockItem.PartyItemTracker);
        Assert.IsNull(mockItem.MemberItemTracker);
    }
}
