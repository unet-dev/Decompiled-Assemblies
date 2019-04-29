using ConVar;
using Facepunch;
using Network;
using ProtoBuf;
using System;
using UnityEngine;
using UnityEngine.Assertions;

public class HeldEntity : BaseEntity
{
	public Animator worldModelAnimator;

	public SoundDefinition thirdPersonDeploySound;

	public SoundDefinition thirdPersonAimSound;

	public SoundDefinition thirdPersonAimEndSound;

	[Header("Held Entity")]
	public string handBone = "r_prop";

	public AnimatorOverrideController HoldAnimationOverride;

	public NPCPlayerApex.ToolTypeEnum toolType;

	[Header("Hostility")]
	public float hostileScore;

	public HeldEntity.HolsterInfo holsterInfo;

	private bool holsterVisible;

	private HeldEntity.heldEntityVisState currentVisState;

	internal uint ownerItemUID;

	public bool hostile
	{
		get
		{
			return this.hostileScore > 0f;
		}
	}

	public HeldEntity()
	{
	}

	public virtual bool CanBeUsedInWater()
	{
		return false;
	}

	public virtual void ClearOwnerPlayer()
	{
		Assert.IsTrue(base.isServer, "Should be server!");
		base.SetParent(null, false, false);
		this.SetHeld(false);
	}

	public virtual void CollectedForCrafting(Item item, BasePlayer crafter)
	{
	}

	public void DestroyThis()
	{
		Item ownerItem = this.GetOwnerItem();
		if (ownerItem != null)
		{
			ownerItem.Remove(0f);
		}
	}

	public uint GetBone(string bone)
	{
		return StringPool.Get(bone);
	}

	public override Item GetItem()
	{
		return this.GetOwnerItem();
	}

	public Connection GetOwnerConnection()
	{
		BasePlayer ownerPlayer = this.GetOwnerPlayer();
		if (ownerPlayer == null)
		{
			return null;
		}
		if (ownerPlayer.net == null)
		{
			return null;
		}
		return ownerPlayer.net.connection;
	}

	protected Item GetOwnerItem()
	{
		BasePlayer ownerPlayer = this.GetOwnerPlayer();
		if (ownerPlayer == null || ownerPlayer.inventory == null)
		{
			return null;
		}
		return ownerPlayer.inventory.FindItemUID(this.ownerItemUID);
	}

	public ItemDefinition GetOwnerItemDefinition()
	{
		Item ownerItem = this.GetOwnerItem();
		if (ownerItem != null)
		{
			return ownerItem.info;
		}
		Debug.LogWarning("GetOwnerItem - null!", this);
		return null;
	}

	public BasePlayer GetOwnerPlayer()
	{
		BaseEntity parentEntity = base.GetParentEntity();
		if (!parentEntity.IsValid())
		{
			return null;
		}
		BasePlayer player = parentEntity.ToPlayer();
		if (player == null)
		{
			return null;
		}
		if (player.IsDead())
		{
			return null;
		}
		return player;
	}

	protected bool HasItemAmount()
	{
		Item ownerItem = this.GetOwnerItem();
		if (ownerItem == null)
		{
			return false;
		}
		return ownerItem.amount > 0;
	}

	private void InitOwnerPlayer()
	{
		BasePlayer ownerPlayer = this.GetOwnerPlayer();
		if (ownerPlayer == null)
		{
			this.ClearOwnerPlayer();
			return;
		}
		this.SetOwnerPlayer(ownerPlayer);
	}

	public bool IsDeployed()
	{
		return base.HasFlag(BaseEntity.Flags.Reserved4);
	}

	public bool LightsOn()
	{
		return base.HasFlag(BaseEntity.Flags.Reserved5);
	}

	public override void Load(BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (info.msg.heldEntity != null)
		{
			this.ownerItemUID = info.msg.heldEntity.itemUID;
		}
	}

	public virtual void OnHeldChanged()
	{
	}

	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning timeWarning = TimeWarning.New("HeldEntity.OnRpcMessage", 0.1f))
		{
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	public override void PostServerLoad()
	{
		this.InitOwnerPlayer();
	}

	public virtual void ReturnedFromCancelledCraft(Item item, BasePlayer crafter)
	{
	}

	public override void Save(BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		info.msg.heldEntity = Facepunch.Pool.Get<ProtoBuf.HeldEntity>();
		info.msg.heldEntity.itemUID = this.ownerItemUID;
	}

	public void SendPunch(Vector3 amount, float duration)
	{
		base.ClientRPCPlayer<Vector3, float>(null, this.GetOwnerPlayer(), "CL_Punch", amount, duration);
	}

	public virtual void ServerCommand(Item item, string command, BasePlayer player)
	{
	}

	public virtual void SetHeld(bool bHeld)
	{
		Assert.IsTrue(base.isServer, "Should be server!");
		base.SetFlag(BaseEntity.Flags.Reserved4, bHeld, false, true);
		if (!bHeld)
		{
			this.UpdateVisiblity_Invis();
		}
		base.limitNetworking = !bHeld;
		base.SetFlag(BaseEntity.Flags.Disabled, !bHeld, false, true);
		base.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
		this.OnHeldChanged();
	}

	public virtual void SetLightsOn(bool isOn)
	{
		base.SetFlag(BaseEntity.Flags.Reserved5, isOn, false, true);
	}

	public virtual void SetOwnerPlayer(BasePlayer player)
	{
		Assert.IsTrue(base.isServer, "Should be server!");
		Assert.IsTrue(player.isServer, "Player should be serverside!");
		base.gameObject.Identity();
		base.SetParent(player, this.handBone, false, false);
		this.SetHeld(false);
	}

	public virtual void SetupHeldEntity(Item item)
	{
		this.ownerItemUID = item.uid;
		this.InitOwnerPlayer();
	}

	public virtual void SetVisibleWhileHolstered(bool visible)
	{
		if (!this.holsterInfo.displayWhenHolstered)
		{
			return;
		}
		this.holsterVisible = visible;
		this.UpdateHeldItemVisibility();
	}

	public void UpdateHeldItemVisibility()
	{
		if (!this.GetOwnerPlayer())
		{
			return;
		}
		bool heldEntity = this.GetOwnerPlayer().GetHeldEntity() == this;
		bool flag = false;
		if (!ConVar.Server.showHolsteredItems && !heldEntity)
		{
			flag = this.UpdateVisiblity_Invis();
		}
		else if (!heldEntity)
		{
			flag = (!this.holsterVisible ? this.UpdateVisiblity_Invis() : this.UpdateVisiblity_Holster());
		}
		else
		{
			flag = this.UpdateVisibility_Hand();
		}
		if (flag)
		{
			base.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
		}
	}

	public bool UpdateVisibility_Hand()
	{
		if (this.currentVisState == HeldEntity.heldEntityVisState.Hand)
		{
			return false;
		}
		this.currentVisState = HeldEntity.heldEntityVisState.Hand;
		base.limitNetworking = false;
		base.SetFlag(BaseEntity.Flags.Disabled, false, false, true);
		base.SetParent(this.GetOwnerPlayer(), this.GetBone(this.handBone), false, false);
		return true;
	}

	public bool UpdateVisiblity_Holster()
	{
		if (this.currentVisState == HeldEntity.heldEntityVisState.Holster)
		{
			return false;
		}
		this.currentVisState = HeldEntity.heldEntityVisState.Holster;
		base.limitNetworking = false;
		base.SetFlag(BaseEntity.Flags.Disabled, false, false, true);
		base.SetParent(this.GetOwnerPlayer(), this.GetBone(this.holsterInfo.holsterBone), false, false);
		this.SetLightsOn(false);
		return true;
	}

	public bool UpdateVisiblity_Invis()
	{
		if (this.currentVisState == HeldEntity.heldEntityVisState.Invis)
		{
			return false;
		}
		this.currentVisState = HeldEntity.heldEntityVisState.Invis;
		base.SetParent(this.GetOwnerPlayer(), this.GetBone(this.handBone), false, false);
		base.limitNetworking = true;
		base.SetFlag(BaseEntity.Flags.Disabled, true, false, true);
		return true;
	}

	protected bool UseItemAmount(int iAmount)
	{
		if (iAmount <= 0)
		{
			return true;
		}
		Item ownerItem = this.GetOwnerItem();
		if (ownerItem == null)
		{
			this.DestroyThis();
			return true;
		}
		ownerItem.amount -= iAmount;
		ownerItem.MarkDirty();
		if (ownerItem.amount > 0)
		{
			return false;
		}
		this.DestroyThis();
		return true;
	}

	public static class HeldEntityFlags
	{
		public const BaseEntity.Flags Deployed = BaseEntity.Flags.Reserved4;

		public const BaseEntity.Flags LightsOn = BaseEntity.Flags.Reserved5;
	}

	public enum heldEntityVisState
	{
		UNSET,
		Invis,
		Hand,
		Holster
	}

	[Serializable]
	public class HolsterInfo
	{
		public HeldEntity.HolsterInfo.HolsterSlot slot;

		public bool displayWhenHolstered;

		public string holsterBone;

		public Vector3 holsterOffset;

		public Vector3 holsterRotationOffset;

		public HolsterInfo()
		{
		}

		public enum HolsterSlot
		{
			BACK,
			RIGHT_THIGH,
			LEFT_THIGH
		}
	}
}