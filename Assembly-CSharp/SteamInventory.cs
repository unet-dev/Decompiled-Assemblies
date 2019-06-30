using ConVar;
using Network;
using Steamworks;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Assertions;

public class SteamInventory : EntityComponent<BasePlayer>
{
	private InventoryItem[] Items;

	public SteamInventory()
	{
	}

	public bool HasItem(int itemid)
	{
		if (this.Items == null)
		{
			return false;
		}
		InventoryItem[] items = this.Items;
		for (int i = 0; i < (int)items.Length; i++)
		{
			if (items[i].DefId == itemid)
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
				if (Global.developer > 2)
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

	[FromOwner]
	[RPC_Server]
	private async Task UpdateSteamInventory(BaseEntity.RPCMessage msg)
	{
		byte[] numArray = msg.read.BytesWithSize();
		if (numArray != null)
		{
			InventoryResult? nullable = await Steamworks.SteamInventory.DeserializeAsync(numArray, -1);
			if (!nullable.HasValue)
			{
				UnityEngine.Debug.LogWarning("UpdateSteamInventory: result is null");
			}
			else if (base.gameObject)
			{
				this.Items = nullable.Value.GetItems(false);
				nullable.Value.Dispose();
			}
		}
		else
		{
			UnityEngine.Debug.LogWarning("UpdateSteamInventory: Data is null");
		}
	}
}