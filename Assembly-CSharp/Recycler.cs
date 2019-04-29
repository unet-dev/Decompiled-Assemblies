using ConVar;
using Network;
using Oxide.Core;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class Recycler : StorageContainer
{
	public float recycleEfficiency = 0.5f;

	public SoundDefinition grindingLoopDef;

	public GameObjectRef startSound;

	public GameObjectRef stopSound;

	public Recycler()
	{
	}

	public bool HasRecyclable()
	{
		for (int i = 0; i < 6; i++)
		{
			Item slot = this.inventory.GetSlot(i);
			if (slot != null)
			{
				object obj = Interface.CallHook("CanRecycle", this, slot);
				if (obj as bool)
				{
					return (bool)obj;
				}
				if (slot.info.Blueprint != null)
				{
					return true;
				}
			}
		}
		return false;
	}

	public bool MoveItemToOutput(Item newItem)
	{
		int num = -1;
		int num1 = 6;
		while (num1 < 12)
		{
			Item slot = this.inventory.GetSlot(num1);
			if (slot != null)
			{
				if (slot.CanStack(newItem))
				{
					if (slot.amount + newItem.amount > slot.info.stackable)
					{
						int num2 = Mathf.Min(slot.info.stackable - slot.amount, newItem.amount);
						newItem.UseItem(num2);
						slot.amount += num2;
						slot.MarkDirty();
						newItem.MarkDirty();
					}
					else
					{
						num = num1;
						break;
					}
				}
				if (newItem.amount <= 0)
				{
					return true;
				}
				num1++;
			}
			else
			{
				num = num1;
				break;
			}
		}
		if (num != -1 && newItem.MoveToContainer(this.inventory, num, true))
		{
			return true;
		}
		newItem.Drop(base.transform.position + new Vector3(0f, 2f, 0f), base.GetInheritedDropVelocity() + (base.transform.forward * 2f), new Quaternion());
		return false;
	}

	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		bool flag;
		using (TimeWarning timeWarning = TimeWarning.New("Recycler.OnRpcMessage", 0.1f))
		{
			if (rpc != -127127424 || !(player != null))
			{
				return base.OnRpcMessage(player, rpc, msg);
			}
			else
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log(string.Concat("SV_RPCMessage: ", player, " - SVSwitch "));
				}
				using (TimeWarning timeWarning1 = TimeWarning.New("SVSwitch", 0.1f))
				{
					using (TimeWarning timeWarning2 = TimeWarning.New("Conditions", 0.1f))
					{
						if (!BaseEntity.RPC_Server.MaxDistance.Test("SVSwitch", this, player, 3f))
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
							this.SVSwitch(rPCMessage);
						}
					}
					catch (Exception exception)
					{
						player.Kick("RPC Error in SVSwitch");
						Debug.LogException(exception);
					}
				}
				flag = true;
			}
		}
		return flag;
	}

	public void RecycleThink()
	{
		bool flag = false;
		float single = this.recycleEfficiency;
		for (int i = 0; i < 6; i++)
		{
			Item slot = this.inventory.GetSlot(i);
			if (slot != null)
			{
				if (Interface.CallHook("OnRecycleItem", this, slot) != null)
				{
					return;
				}
				if (slot.info.Blueprint != null)
				{
					if (slot.hasCondition)
					{
						single = Mathf.Clamp01(single * Mathf.Clamp(slot.conditionNormalized * slot.maxConditionNormalized, 0.1f, 1f));
					}
					int num = 1;
					if (slot.amount > 1)
					{
						num = Mathf.CeilToInt(Mathf.Min((float)slot.amount, (float)slot.info.stackable * 0.1f));
					}
					if (slot.info.Blueprint.scrapFromRecycle > 0)
					{
						int blueprint = slot.info.Blueprint.scrapFromRecycle * num;
						if (slot.info.stackable == 1 && slot.hasCondition)
						{
							blueprint = Mathf.CeilToInt((float)blueprint * slot.conditionNormalized);
						}
						if (blueprint >= 1)
						{
							Item item = ItemManager.CreateByName("scrap", blueprint, (ulong)0);
							this.MoveItemToOutput(item);
						}
					}
					slot.UseItem(num);
					List<ItemAmount>.Enumerator enumerator = slot.info.Blueprint.ingredients.GetEnumerator();
					try
					{
					Label0:
						while (enumerator.MoveNext())
						{
							ItemAmount current = enumerator.Current;
							if (current.itemDef.shortname == "scrap")
							{
								continue;
							}
							float blueprint1 = (float)current.amount / (float)slot.info.Blueprint.amountToCreate;
							int num1 = 0;
							if (blueprint1 > 1f)
							{
								num1 = Mathf.CeilToInt(Mathf.Clamp(blueprint1 * single * UnityEngine.Random.Range(1f, 1f), 0f, current.amount) * (float)num);
							}
							else
							{
								for (int j = 0; j < num; j++)
								{
									if (UnityEngine.Random.Range(0f, 1f) <= blueprint1 * single)
									{
										num1++;
									}
								}
							}
							if (num1 <= 0)
							{
								continue;
							}
							int num2 = Mathf.CeilToInt((float)num1 / (float)current.itemDef.stackable);
							for (int k = 0; k < num2; k++)
							{
								int num3 = (num1 > current.itemDef.stackable ? current.itemDef.stackable : num1);
								if (!this.MoveItemToOutput(ItemManager.Create(current.itemDef, num3, (ulong)0)))
								{
									flag = true;
								}
								num1 -= num3;
								if (num1 <= 0)
								{
									goto Label0;
								}
							}
						}
						break;
					}
					finally
					{
						((IDisposable)enumerator).Dispose();
					}
				}
			}
		}
		if (flag || !this.HasRecyclable())
		{
			this.StopRecycling();
		}
	}

	public void StartRecycling()
	{
		if (base.IsOn())
		{
			return;
		}
		base.InvokeRepeating(new Action(this.RecycleThink), 5f, 5f);
		Effect.server.Run(this.startSound.resourcePath, this, 0, Vector3.zero, Vector3.zero, null, false);
		base.SetFlag(BaseEntity.Flags.On, true, false, true);
		base.SendNetworkUpdateImmediate(false);
	}

	public void StopRecycling()
	{
		base.CancelInvoke(new Action(this.RecycleThink));
		if (!base.IsOn())
		{
			return;
		}
		Effect.server.Run(this.stopSound.resourcePath, this, 0, Vector3.zero, Vector3.zero, null, false);
		base.SetFlag(BaseEntity.Flags.On, false, false, true);
		base.SendNetworkUpdateImmediate(false);
	}

	[MaxDistance(3f)]
	[RPC_Server]
	private void SVSwitch(BaseEntity.RPCMessage msg)
	{
		bool flag = msg.read.Bit();
		if (flag == base.IsOn())
		{
			return;
		}
		if (msg.player == null)
		{
			return;
		}
		if (Interface.CallHook("OnRecyclerToggle", this, msg.player) != null)
		{
			return;
		}
		if (flag && !this.HasRecyclable())
		{
			return;
		}
		if (!flag)
		{
			this.StopRecycling();
			return;
		}
		foreach (Item item in this.inventory.itemList)
		{
			item.CollectedForCrafting(msg.player);
		}
		this.StartRecycling();
	}
}