using ConVar;
using Network;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class SurveyCrater : BaseCombatEntity
{
	private ResourceDispenser resourceDispenser;

	public SurveyCrater()
	{
	}

	[RPC_Server]
	public void AnalysisComplete(BaseEntity.RPCMessage msg)
	{
		ResourceDepositManager.ResourceDeposit orCreate = ResourceDepositManager.GetOrCreate(base.transform.position);
		if (orCreate == null)
		{
			return;
		}
		Item item = ItemManager.CreateByName("note", 1, (ulong)0);
		item.text = "-Mineral Analysis-\n\n";
		float single = 10f;
		float single1 = 7.5f;
		foreach (ResourceDepositManager.ResourceDeposit.ResourceDepositEntry _resource in orCreate._resources)
		{
			float single2 = 60f / single * (single1 / _resource.workNeeded);
			Item item1 = item;
			item1.text = string.Concat(new string[] { item1.text, _resource.type.displayName.english, " : ", single2.ToString("0.0"), " pM\n" });
		}
		item.MarkDirty();
		msg.player.GiveItem(item, BaseEntity.GiveItemReason.PickedUp);
	}

	public override float BoundsPadding()
	{
		return 2f;
	}

	public override void OnAttacked(HitInfo info)
	{
		bool flag = base.isServer;
		base.OnAttacked(info);
	}

	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		bool flag;
		using (TimeWarning timeWarning = TimeWarning.New("SurveyCrater.OnRpcMessage", 0.1f))
		{
			if (rpc != -803720962 || !(player != null))
			{
				return base.OnRpcMessage(player, rpc, msg);
			}
			else
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log(string.Concat("SV_RPCMessage: ", player, " - AnalysisComplete "));
				}
				using (TimeWarning timeWarning1 = TimeWarning.New("AnalysisComplete", 0.1f))
				{
					try
					{
						using (TimeWarning timeWarning2 = TimeWarning.New("Call", 0.1f))
						{
							BaseEntity.RPCMessage rPCMessage = new BaseEntity.RPCMessage()
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.AnalysisComplete(rPCMessage);
						}
					}
					catch (Exception exception)
					{
						player.Kick("RPC Error in AnalysisComplete");
						Debug.LogException(exception);
					}
				}
				flag = true;
			}
		}
		return flag;
	}

	public void RemoveMe()
	{
		base.Kill(BaseNetworkable.DestroyMode.None);
	}

	public override void ServerInit()
	{
		base.ServerInit();
		base.Invoke(new Action(this.RemoveMe), 1800f);
	}
}