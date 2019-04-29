using ConVar;
using Facepunch.Steamworks;
using Network;
using Rust;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Assertions;

public class SteamInventory : EntityComponent<BasePlayer>
{
	private Facepunch.Steamworks.Inventory.Item[] Items;

	public SteamInventory()
	{
	}

	public bool HasItem(int itemid)
	{
		if (this.Items == null)
		{
			return false;
		}
		Facepunch.Steamworks.Inventory.Item[] items = this.Items;
		for (int i = 0; i < (int)items.Length; i++)
		{
			if (items[i].DefinitionId == itemid)
			{
				return true;
			}
		}
		return false;
	}

	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		bool flag;
		using (TimeWarning timeWarning = TimeWarning.New("SteamInventory.OnRpcMessage", 0.1f))
		{
			if (rpc != 643458331 || !(player != null))
			{
				return base.OnRpcMessage(player, rpc, msg);
			}
			else
			{
				UnityEngine.Assertions.Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (ConVar.Global.developer > 2)
				{
					UnityEngine.Debug.Log(string.Concat("SV_RPCMessage: ", player, " - UpdateSteamInventory "));
				}
				using (TimeWarning timeWarning1 = TimeWarning.New("UpdateSteamInventory", 0.1f))
				{
					using (TimeWarning timeWarning2 = TimeWarning.New("Conditions", 0.1f))
					{
						if (!BaseEntity.RPC_Server.FromOwner.Test("UpdateSteamInventory", this.GetBaseEntity(), player))
						{
							flag = true;
							return flag;
						}
					}
					try
					{
						using (timeWarning2 = TimeWarning.New("Call", 0.1f))
						{
							BaseEntity.RPCMessage rPCMessage = new BaseEntity.RPCMessage()
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.UpdateSteamInventory(rPCMessage);
						}
					}
					catch (Exception exception)
					{
						player.Kick("RPC Error in UpdateSteamInventory");
						UnityEngine.Debug.LogException(exception);
					}
				}
				flag = true;
			}
		}
		return flag;
	}

	private IEnumerator ProcessInventoryResult(Facepunch.Steamworks.Inventory.Result result)
	{
		SteamInventory items = null;
		float single = 0f;
		while (result.IsPending)
		{
			single += 1f;
			yield return CoroutineEx.waitForSeconds(1f);
			if (single <= 30f)
			{
				continue;
			}
			object[] objArray = new object[] { items.baseEntity.displayName };
			UnityEngine.Debug.LogFormat("Steam Inventory result timed out for {0}", objArray);
		}
		if (result.Items != null)
		{
			items.Items = result.Items;
		}
		result.Dispose();
	}

	[FromOwner]
	[RPC_Server]
	private void UpdateSteamInventory(BaseEntity.RPCMessage msg)
	{
		MemoryStream memoryStream = msg.read.MemoryStreamWithSize();
		if (memoryStream == null)
		{
			UnityEngine.Debug.LogWarning("UpdateSteamInventory: Data is null");
			return;
		}
		Facepunch.Steamworks.Inventory.Result result = Rust.Global.SteamServer.Inventory.Deserialize(memoryStream.GetBuffer(), (int)memoryStream.Length);
		if (result == null)
		{
			UnityEngine.Debug.LogWarning("UpdateSteamInventory: result is null");
			return;
		}
		base.StopAllCoroutines();
		base.StartCoroutine(this.ProcessInventoryResult(result));
	}
}