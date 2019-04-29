using System;

public class ContainerSourceLocalPlayer : ItemContainerSource
{
	public PlayerInventory.Type type;

	public ContainerSourceLocalPlayer()
	{
	}

	public override ItemContainer GetItemContainer()
	{
		return null;
	}
}