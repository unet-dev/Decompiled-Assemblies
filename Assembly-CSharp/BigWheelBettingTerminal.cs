using Network;
using System;
using UnityEngine;

public class BigWheelBettingTerminal : StorageContainer
{
	public BigWheelGame bigWheel;

	public Vector3 seatedPlayerOffset = Vector3.forward;

	public float offsetCheckRadius = 0.4f;

	public SoundDefinition winSound;

	public SoundDefinition loseSound;

	public BigWheelBettingTerminal()
	{
	}

	public bool IsPlayerValid(BasePlayer player)
	{
		if (!player.isMounted)
		{
			return false;
		}
		Vector3 vector3 = base.transform.TransformPoint(this.seatedPlayerOffset);
		if (Vector3Ex.Distance2D(player.transform.position, vector3) > this.offsetCheckRadius)
		{
			return false;
		}
		return true;
	}

	public new void OnDrawGizmos()
	{
		Gizmos.color = Color.yellow;
		Gizmos.DrawSphere(base.transform.TransformPoint(this.seatedPlayerOffset), this.offsetCheckRadius);
		base.OnDrawGizmos();
	}

	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning timeWarning = TimeWarning.New("BigWheelBettingTerminal.OnRpcMessage", 0.1f))
		{
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	public override bool PlayerOpenLoot(BasePlayer player, string panelToOpen)
	{
		if (!this.IsPlayerValid(player))
		{
			return false;
		}
		return base.PlayerOpenLoot(player, panelToOpen);
	}
}