using Oxide.Core;
using System;
using UnityEngine;

public class PlayerBelt
{
	public static int SelectedSlot;

	protected BasePlayer player;

	public static int MaxBeltSlots
	{
		get
		{
			return 6;
		}
	}

	static PlayerBelt()
	{
		PlayerBelt.SelectedSlot = -1;
	}

	public PlayerBelt(BasePlayer player)
	{
		this.player = player;
	}

	public void DropActive(Vector3 position, Vector3 velocity)
	{
		Item activeItem = this.player.GetActiveItem();
		if (activeItem == null)
		{
			return;
		}
		if (Interface.CallHook("OnPlayerDropActiveItem", this.player, activeItem) != null)
		{
			return;
		}
		using (TimeWarning timeWarning = TimeWarning.New("PlayerBelt.DropActive", 0.1f))
		{
			activeItem.Drop(position, velocity, new Quaternion());
			this.player.svActiveItemID = 0;
			this.player.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
		}
	}
}