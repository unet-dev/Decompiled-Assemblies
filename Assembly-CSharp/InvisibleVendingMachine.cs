using Facepunch;
using ProtoBuf;
using System;
using System.Collections.Generic;
using UnityEngine;

public class InvisibleVendingMachine : NPCVendingMachine
{
	public GameObjectRef buyEffect;

	public NPCVendingOrderManifest vmoManifest;

	public InvisibleVendingMachine()
	{
	}

	public override void CompletePendingOrder()
	{
		Effect.server.Run(this.buyEffect.resourcePath, base.transform.position, Vector3.up, null, false);
		NPCShopKeeper nPCShopKeeper = this.GetNPCShopKeeper();
		if (nPCShopKeeper)
		{
			nPCShopKeeper.SignalBroadcast(BaseEntity.Signal.Gesture, "victory", null);
			if (this.vend_Player != null)
			{
				nPCShopKeeper.SetAimDirection(Vector3Ex.Direction2D(this.vend_Player.transform.position, nPCShopKeeper.transform.position));
			}
		}
		base.CompletePendingOrder();
	}

	public override float GetBuyDuration()
	{
		return 0.5f;
	}

	public NPCShopKeeper GetNPCShopKeeper()
	{
		List<NPCShopKeeper> list = Pool.GetList<NPCShopKeeper>();
		Vis.Entities<NPCShopKeeper>(base.transform.position, 2f, list, 131072, QueryTriggerInteraction.Collide);
		NPCShopKeeper item = null;
		if (list.Count > 0)
		{
			item = list[0];
		}
		Pool.FreeList<NPCShopKeeper>(ref list);
		return item;
	}

	public override bool HasVendingSounds()
	{
		return false;
	}

	public void KeeperLookAt(Vector3 pos)
	{
		NPCShopKeeper nPCShopKeeper = this.GetNPCShopKeeper();
		if (nPCShopKeeper == null)
		{
			return;
		}
		nPCShopKeeper.SetAimDirection(Vector3Ex.Direction2D(pos, nPCShopKeeper.transform.position));
	}

	public override void Load(BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (info.fromDisk && this.vmoManifest != null && info.msg.vendingMachine != null)
		{
			this.vendingOrders = this.vmoManifest.GetFromIndex(info.msg.vendingMachine.vmoIndex);
		}
	}

	public override bool PlayerOpenLoot(BasePlayer player, string panelToOpen)
	{
		this.KeeperLookAt(player.transform.position);
		return base.PlayerOpenLoot(player, panelToOpen);
	}

	public override void Save(BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		if (this.vmoManifest != null && info.msg.vendingMachine != null)
		{
			info.msg.vendingMachine.vmoIndex = this.vmoManifest.GetIndex(this.vendingOrders);
		}
	}
}