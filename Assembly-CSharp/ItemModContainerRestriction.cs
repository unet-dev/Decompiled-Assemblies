using System;

public class ItemModContainerRestriction : ItemMod
{
	[InspectorFlags]
	public ItemModContainerRestriction.SlotFlags slotFlags;

	public ItemModContainerRestriction()
	{
	}

	public bool CanExistWith(ItemModContainerRestriction other)
	{
		if (other == null)
		{
			return true;
		}
		if ((int)(this.slotFlags & other.slotFlags) != 0)
		{
			return false;
		}
		return true;
	}

	[Flags]
	public enum SlotFlags
	{
		Map = 1
	}
}