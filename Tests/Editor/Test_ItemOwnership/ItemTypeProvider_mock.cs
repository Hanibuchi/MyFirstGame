class ItemTypeProvider_mock : IItemTypeProvider
{
	ItemType _itemType;
	public ItemTypeProvider_mock(ItemType itemType)
	{
		_itemType = itemType;
	}
	public ItemType GetItemType()
	{
		return _itemType;
	}
}