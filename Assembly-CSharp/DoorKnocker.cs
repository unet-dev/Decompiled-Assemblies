using Network;
using System;
using UnityEngine;

public class DoorKnocker : BaseCombatEntity
{
	public Animator knocker1;

	public Animator knocker2;

	public DoorKnocker()
	{
	}

	public void Knock(BasePlayer player)
	{
		base.ClientRPC<Vector3>(null, "ClientKnock", player.transform.position);
	}

	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning timeWarning = TimeWarning.New("DoorKnocker.OnRpcMessage", 0.1f))
		{
		}
		return base.OnRpcMessage(player, rpc, msg);
	}
}