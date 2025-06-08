using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using UnityEngine;

public class Test_MemberItemHolder
{
    MemberItemHolder _memberItemHolder;
    ItemTypeProvider_mock _itemTypeProvider_mock;

    [SetUp]
    public void SetUp()
    {
        _memberItemHolder = new MemberItemHolder();
        _itemTypeProvider_mock = new ItemTypeProvider_mock(ItemType.None);
        _memberItemHolder.ResetItems();
        for (int i = 0; i < 4; i++)
            _memberItemHolder.AddItemCapacity();
    }

    [Test]
    public void Can_Add_Items_Correctly()
    {
        _memberItemHolder.SetPartyItemTracker(new PartyItemTracker());
        _memberItemHolder.SetMemberItemTracker(new MemberItemTracker());

        for (int i = 0; i < _memberItemHolder.ItemCapacity; i++)
        {
            var child = new ChildItemHolder();
            child.SetItemTypeProvider(_itemTypeProvider_mock);
            Assert.IsTrue(_memberItemHolder.CanAddItem(child));
            _memberItemHolder.AddItem(child);
            Assert.AreSame(_memberItemHolder.Items[i], child);
            Assert.AreSame(_memberItemHolder.PartyItemTracker, child.PartyItemTracker);
            Assert.AreSame(_memberItemHolder.MemberItemTracker, child.MemberItemTracker);
        }

        var overflowItem = new ChildItemHolder();
        overflowItem.SetItemTypeProvider(_itemTypeProvider_mock);
        Assert.IsFalse(_memberItemHolder.CanAddItem(overflowItem));
        int countBefore = _memberItemHolder.Items.Count(a => a != null);
        _memberItemHolder.AddItem(overflowItem);
        Assert.AreEqual(countBefore, _memberItemHolder.Items.Count(a => a != null));
    }

    [Test]
    public void Can_Insert_Item_At_Index()
    {
        _memberItemHolder.SetPartyItemTracker(new PartyItemTracker());
        _memberItemHolder.SetMemberItemTracker(new MemberItemTracker());

        var item1 = new ChildItemHolder();
        item1.SetItemTypeProvider(_itemTypeProvider_mock);
        _memberItemHolder.AddItem(item1);

        var item2 = new ChildItemHolder();
        item2.SetItemTypeProvider(_itemTypeProvider_mock);
        _memberItemHolder.AddItemAt(0, item2);

        Assert.AreSame(item2, _memberItemHolder.Items[0]);
        Assert.AreSame(item1, _memberItemHolder.Items[1]);


        List<ChildItemHolder> childItems = Enumerable.Repeat<ChildItemHolder>(null, 8).ToList();
        _memberItemHolder.ResetItems();
        childItems[1] = AddItemAt(1);
        childItems[2] = AddItemAt(2);
        childItems[3] = AddItemAt(3);
        item1 = new();
        item1.SetItemTypeProvider(_itemTypeProvider_mock);
        _memberItemHolder.AddItemAt(0, item1);
        for (int i = 0; i < 4; i++)
        {
            if (i == 0)
                Assert.AreSame(item1, _memberItemHolder.Items[i]);
            else
                Assert.AreSame(childItems[i], _memberItemHolder.Items[i]);
        }

        childItems = Enumerable.Repeat<ChildItemHolder>(null, 8).ToList();
        _memberItemHolder.ResetItems();
        childItems[0] = AddItemAt(1);
        item1 = new();
        item1.SetItemTypeProvider(_itemTypeProvider_mock);
        _memberItemHolder.AddItemAt(1, item1);
        for (int i = 0; i < 2; i++)
        {
            if (i == 1)
                Assert.AreSame(item1, _memberItemHolder.Items[i]);
            else
                Assert.AreSame(childItems[i], _memberItemHolder.Items[i]);
        }

        childItems = Enumerable.Repeat<ChildItemHolder>(null, 8).ToList();
        _memberItemHolder.ResetItems();
        childItems[1] = AddItemAt(1);
        childItems[3] = AddItemAt(2);
        item1 = new();
        item1.SetItemTypeProvider(_itemTypeProvider_mock);
        _memberItemHolder.AddItemAt(2, item1);
        for (int i = 0; i < 2; i++)
        {
            if (i == 2)
                Assert.AreSame(item1, _memberItemHolder.Items[i]);
            else
                Assert.AreSame(childItems[i], _memberItemHolder.Items[i]);
        }

        childItems = Enumerable.Repeat<ChildItemHolder>(null, 8).ToList();
        _memberItemHolder.ResetItems();
        childItems[0] = AddItemAt(1);
        childItems[1] = AddItemAt(2);
        childItems[3] = AddItemAt(3);
        childItems[4] = AddItemAt(4);
        item1 = new();
        item1.SetItemTypeProvider(_itemTypeProvider_mock);
        _memberItemHolder.AddItemAt(2, item1);
        for (int i = 0; i < 5; i++)
        {
            if (i == 2)
                Assert.AreSame(item1, _memberItemHolder.Items[i]);
            else
                Assert.AreSame(childItems[i], _memberItemHolder.Items[i]);
        }
    }
    ChildItemHolder AddItemAt(int index)
    {
        var childItem = new ChildItemHolder();
        childItem.SetItemTypeProvider(_itemTypeProvider_mock);
        _memberItemHolder.AddItemAt(index, childItem);
        return childItem;
    }


    [Test]
    public void Should_Respect_IsFixedSize()
    {
        _memberItemHolder.ResetItems();

        Assert.AreEqual(_memberItemHolder.ItemCapacity, _memberItemHolder.Items.Count);

        for (int i = 0; i < _memberItemHolder.ItemCapacity; i++)
        {
            var child = new ChildItemHolder();
            child.SetItemTypeProvider(_itemTypeProvider_mock);
            Assert.IsTrue(_memberItemHolder.CanAddItem(child));
            _memberItemHolder.AddItem(child);
            Assert.AreSame(_memberItemHolder.Items[i], child);
        }

        var overflow = new ChildItemHolder();
        overflow.SetItemTypeProvider(_itemTypeProvider_mock);
        Assert.IsFalse(_memberItemHolder.CanAddItem(overflow));
        _memberItemHolder.AddItem(overflow);
        foreach (var item in _memberItemHolder.Items)
        {
            Assert.AreNotSame(item, overflow);
        }
    }

    [Test]
    public void CanAddItemAt_Index_OutOfBounds_ShouldBeFalse()
    {
        var item = new ChildItemHolder();
        item.SetItemTypeProvider(_itemTypeProvider_mock);
        Assert.IsFalse(_memberItemHolder.CanAddItemAt(-1, item));
        Assert.IsFalse(_memberItemHolder.CanAddItemAt(1000, item));
    }
}