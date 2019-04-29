using ConVar;
using Network;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class BaseLiquidVessel : AttackEntity
{
	[Header("Liquid Vessel")]
	public GameObjectRef thrownWaterObject;

	public GameObjectRef ThrowEffect3P;

	public SoundDefinition throwSound3P;

	public GameObjectRef fillFromContainer;

	public GameObjectRef fillFromWorld;

	public bool hasLid;

	public float throwScale = 10f;

	public bool canDrinkFrom;

	public bool updateVMWater;

	public float minThrowFrac;

	public bool useThrowAnim;

	public float fillMlPerSec = 500f;

	private float lastFillTime;

	private float nextFreeTime;

	public BaseLiquidVessel()
	{
	}

	public void AddLiquid(ItemDefinition liquidType, int amount)
	{
		if (amount <= 0)
		{
			return;
		}
		Item item = this.GetItem();
		Item slot = item.contents.GetSlot(0);
		ItemModContainer component = item.info.GetComponent<ItemModContainer>();
		if (slot != null)
		{
			int num = Mathf.Clamp(slot.amount + amount, 0, component.maxStackSize);
			ItemDefinition itemDefinition = WaterResource.Merge(slot.info, liquidType);
			if (itemDefinition == slot.info)
			{
				slot.amount = num;
			}
			else
			{
				slot.Remove(0f);
				slot = ItemManager.Create(itemDefinition, num, (ulong)0);
				slot.MoveToContainer(item.contents, -1, true);
			}
			slot.MarkDirty();
			base.SendNetworkUpdateImmediate(false);
		}
		else
		{
			Item item1 = ItemManager.Create(liquidType, amount, (ulong)0);
			if (item1 != null)
			{
				item1.MoveToContainer(item.contents, -1, true);
				return;
			}
		}
	}

	public int AmountHeld()
	{
		Item slot = this.GetItem().contents.GetSlot(0);
		if (slot == null)
		{
			return 0;
		}
		return slot.amount;
	}

	public bool CanDrink()
	{
		BasePlayer ownerPlayer = base.GetOwnerPlayer();
		if (!ownerPlayer)
		{
			return false;
		}
		if (!ownerPlayer.metabolism.CanConsume())
		{
			return false;
		}
		if (!this.canDrinkFrom)
		{
			return false;
		}
		Item item = this.GetItem();
		if (item == null)
		{
			return false;
		}
		if (item.contents == null)
		{
			return false;
		}
		if (item.contents.itemList == null)
		{
			return false;
		}
		if (item.contents.itemList.Count == 0)
		{
			return false;
		}
		return true;
	}

	public bool CanFillFromWorld()
	{
		BasePlayer ownerPlayer = base.GetOwnerPlayer();
		if (!ownerPlayer)
		{
			return false;
		}
		return ownerPlayer.WaterFactor() >= 0.05f;
	}

	public bool CanThrow()
	{
		return this.HeldFraction() > this.minThrowFrac;
	}

	private void ClearBusy()
	{
		this.nextFreeTime = UnityEngine.Time.realtimeSinceStartup - 1f;
	}

	[IsActiveItem]
	[RPC_Server]
	private void DoDrink(BaseEntity.RPCMessage msg)
	{
		if (!msg.player.CanInteract())
		{
			return;
		}
		Item item = this.GetItem();
		if (item == null)
		{
			return;
		}
		if (item.contents == null)
		{
			return;
		}
		if (!msg.player.metabolism.CanConsume())
		{
			return;
		}
		foreach (Item content in item.contents.itemList)
		{
			ItemModConsume component = content.info.GetComponent<ItemModConsume>();
			if (component == null || !component.CanDoAction(content, msg.player))
			{
				continue;
			}
			component.DoAction(content, msg.player);
			return;
		}
	}

	public void DoThrow(Vector3 pos, Vector3 velocity)
	{
		BasePlayer ownerPlayer = base.GetOwnerPlayer();
		if (ownerPlayer == null)
		{
			return;
		}
		Item item = this.GetItem();
		if (item == null)
		{
			return;
		}
		if (item.contents == null)
		{
			return;
		}
		Item slot = item.contents.GetSlot(0);
		if (slot != null && slot.amount > 0)
		{
			Vector3 vector3 = ownerPlayer.eyes.position + (ownerPlayer.eyes.BodyForward() * 1f);
			WaterBall waterBall = GameManager.server.CreateEntity(this.thrownWaterObject.resourcePath, vector3, Quaternion.identity, true) as WaterBall;
			if (waterBall)
			{
				waterBall.liquidType = slot.info;
				waterBall.waterAmount = slot.amount;
				waterBall.transform.position = vector3;
				waterBall.SetVelocity(velocity);
				waterBall.Spawn();
			}
			slot.UseItem(slot.amount);
			slot.MarkDirty();
			base.SendNetworkUpdateImmediate(false);
		}
	}

	public void FillCheck()
	{
		if (base.isClient)
		{
			return;
		}
		BasePlayer ownerPlayer = base.GetOwnerPlayer();
		if (!ownerPlayer)
		{
			return;
		}
		float single = UnityEngine.Time.realtimeSinceStartup - this.lastFillTime;
		Vector3 vector3 = ownerPlayer.transform.position - new Vector3(0f, 1f, 0f);
		if (this.CanFillFromWorld())
		{
			this.AddLiquid(WaterResource.GetAtPoint(vector3), Mathf.FloorToInt(single * this.fillMlPerSec));
			return;
		}
		LiquidContainer facingLiquidContainer = this.GetFacingLiquidContainer();
		if (facingLiquidContainer != null && facingLiquidContainer.HasLiquidItem())
		{
			int num = Mathf.CeilToInt((1f - this.HeldFraction()) * (float)this.MaxHoldable());
			if (num > 0)
			{
				Item liquidItem = facingLiquidContainer.GetLiquidItem();
				int num1 = Mathf.Min(Mathf.CeilToInt(single * this.fillMlPerSec), Mathf.Min(liquidItem.amount, num));
				this.AddLiquid(liquidItem.info, num1);
				liquidItem.UseItem(num1);
				facingLiquidContainer.OpenTap(2f);
			}
		}
	}

	public LiquidContainer GetFacingLiquidContainer()
	{
		RaycastHit raycastHit;
		BasePlayer ownerPlayer = base.GetOwnerPlayer();
		if (!ownerPlayer)
		{
			return null;
		}
		if (UnityEngine.Physics.Raycast(ownerPlayer.eyes.HeadRay(), out raycastHit, 2f, 1236478737))
		{
			BaseEntity entity = raycastHit.GetEntity();
			if (entity && !raycastHit.collider.gameObject.CompareTag("Not Player Usable") && !raycastHit.collider.gameObject.CompareTag("Usable Primary"))
			{
				entity = entity.ToServer<BaseEntity>();
				return entity.GetComponent<LiquidContainer>();
			}
		}
		return null;
	}

	public float HeldFraction()
	{
		return (float)this.AmountHeld() / (float)this.MaxHoldable();
	}

	private bool IsWeaponBusy()
	{
		return UnityEngine.Time.realtimeSinceStartup < this.nextFreeTime;
	}

	public void LoseWater(int amount)
	{
		Item slot = this.GetItem().contents.GetSlot(0);
		if (slot != null)
		{
			slot.UseItem(amount);
			slot.MarkDirty();
			base.SendNetworkUpdateImmediate(false);
		}
	}

	public int MaxHoldable()
	{
		return this.GetItem().info.GetComponent<ItemModContainer>().maxStackSize;
	}

	public override void OnHeldChanged()
	{
		base.OnHeldChanged();
		if (base.IsDisabled())
		{
			this.StopFilling();
		}
		if (!this.hasLid)
		{
			this.DoThrow(base.transform.position, Vector3.zero);
			Item item = this.GetItem();
			if (item == null)
			{
				return;
			}
			item.contents.SetLocked(base.IsDisabled());
			base.SendNetworkUpdateImmediate(false);
		}
	}

	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		bool flag;
		BaseEntity.RPCMessage rPCMessage;
		using (TimeWarning timeWarning = TimeWarning.New("BaseLiquidVessel.OnRpcMessage", 0.1f))
		{
			if (rpc == -281530647 && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log(string.Concat("SV_RPCMessage: ", player, " - DoDrink "));
				}
				using (TimeWarning timeWarning1 = TimeWarning.New("DoDrink", 0.1f))
				{
					using (TimeWarning timeWarning2 = TimeWarning.New("Conditions", 0.1f))
					{
						if (!BaseEntity.RPC_Server.IsActiveItem.Test("DoDrink", this, player))
						{
							flag = true;
							return flag;
						}
					}
					try
					{
						using (timeWarning2 = TimeWarning.New("Call", 0.1f))
						{
							rPCMessage = new BaseEntity.RPCMessage()
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.DoDrink(rPCMessage);
						}
					}
					catch (Exception exception)
					{
						player.Kick("RPC Error in DoDrink");
						Debug.LogException(exception);
					}
				}
				flag = true;
			}
			else if (rpc == -1513621468 && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log(string.Concat("SV_RPCMessage: ", player, " - SendFilling "));
				}
				using (timeWarning1 = TimeWarning.New("SendFilling", 0.1f))
				{
					try
					{
						using (timeWarning2 = TimeWarning.New("Call", 0.1f))
						{
							rPCMessage = new BaseEntity.RPCMessage()
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.SendFilling(rPCMessage);
						}
					}
					catch (Exception exception1)
					{
						player.Kick("RPC Error in SendFilling");
						Debug.LogException(exception1);
					}
				}
				flag = true;
			}
			else if (rpc != -1256199475 || !(player != null))
			{
				return base.OnRpcMessage(player, rpc, msg);
			}
			else
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log(string.Concat("SV_RPCMessage: ", player, " - ThrowContents "));
				}
				using (timeWarning1 = TimeWarning.New("ThrowContents", 0.1f))
				{
					try
					{
						using (timeWarning2 = TimeWarning.New("Call", 0.1f))
						{
							rPCMessage = new BaseEntity.RPCMessage()
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.ThrowContents(rPCMessage);
						}
					}
					catch (Exception exception2)
					{
						player.Kick("RPC Error in ThrowContents");
						Debug.LogException(exception2);
					}
				}
				flag = true;
			}
		}
		return flag;
	}

	[RPC_Server]
	private void SendFilling(BaseEntity.RPCMessage msg)
	{
		this.SetFilling(msg.read.Bit());
	}

	public override void ServerInit()
	{
		base.ServerInit();
		base.InvokeRepeating(new Action(this.FillCheck), 1f, 1f);
	}

	private void SetBusyFor(float dur)
	{
		this.nextFreeTime = UnityEngine.Time.realtimeSinceStartup + dur;
	}

	public void SetFilling(bool isFilling)
	{
		base.SetFlag(BaseEntity.Flags.Open, isFilling, false, true);
		if (isFilling)
		{
			this.StartFilling();
			return;
		}
		this.StopFilling();
	}

	public void StartFilling()
	{
		float single = UnityEngine.Time.realtimeSinceStartup - this.lastFillTime;
		this.StopFilling();
		base.InvokeRepeating(new Action(this.FillCheck), 0f, 0.3f);
		if (single > 1f)
		{
			LiquidContainer facingLiquidContainer = this.GetFacingLiquidContainer();
			if (facingLiquidContainer != null && facingLiquidContainer.GetLiquidItem() != null)
			{
				Effect.server.Run(this.fillFromContainer.resourcePath, facingLiquidContainer.transform.position, Vector3.up, null, false);
			}
			else if (this.CanFillFromWorld())
			{
				Effect.server.Run(this.fillFromWorld.resourcePath, base.GetOwnerPlayer(), 0, Vector3.zero, Vector3.up, null, false);
			}
		}
		this.lastFillTime = UnityEngine.Time.realtimeSinceStartup;
	}

	public void StopFilling()
	{
		base.CancelInvoke(new Action(this.FillCheck));
	}

	[RPC_Server]
	private void ThrowContents(BaseEntity.RPCMessage msg)
	{
		BasePlayer ownerPlayer = base.GetOwnerPlayer();
		if (ownerPlayer == null)
		{
			return;
		}
		this.DoThrow(ownerPlayer.eyes.position + (ownerPlayer.eyes.BodyForward() * 1f), ownerPlayer.estimatedVelocity + (ownerPlayer.eyes.BodyForward() * this.throwScale));
		Effect.server.Run(this.ThrowEffect3P.resourcePath, ownerPlayer.transform.position, ownerPlayer.eyes.BodyForward(), ownerPlayer.net.connection, false);
	}
}