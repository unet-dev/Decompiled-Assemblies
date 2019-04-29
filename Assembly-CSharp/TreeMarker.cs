using Network;
using System;
using UnityEngine;

public class TreeMarker : BaseEntity
{
	public GameObjectRef hitEffect;

	public SoundDefinition hitEffectSound;

	public GameObjectRef spawnEffect;

	public DeferredDecal myDecal;

	private Vector3 initialPosition;

	public TreeMarker()
	{
	}

	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning timeWarning = TimeWarning.New("TreeMarker.OnRpcMessage", 0.1f))
		{
		}
		return base.OnRpcMessage(player, rpc, msg);
	}
}