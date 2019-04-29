using ConVar;
using Network;
using Oxide.Core;
using Rust;
using System;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

public class HackableLockedCrate : LootContainer
{
	public const BaseEntity.Flags Flag_Hacking = BaseEntity.Flags.Reserved1;

	public const BaseEntity.Flags Flag_FullyHacked = BaseEntity.Flags.Reserved2;

	public Text timerText;

	[ServerVar(Help="How many seconds for the crate to unlock")]
	public static float requiredHackSeconds;

	[ServerVar(Help="How many seconds until the crate is destroyed without any hack attempts")]
	public static float decaySeconds;

	public SoundPlayer hackProgressBeep;

	public float hackSeconds;

	public GameObjectRef shockEffect;

	public GameObjectRef mapMarkerEntityPrefab;

	public GameObjectRef landEffect;

	private BaseEntity mapMarkerInstance;

	public bool hasLanded;

	public bool wasDropped;

	static HackableLockedCrate()
	{
		HackableLockedCrate.requiredHackSeconds = 900f;
		HackableLockedCrate.decaySeconds = 7200f;
	}

	public HackableLockedCrate()
	{
	}

	public void CreateMapMarker(float durationMinutes)
	{
		if (this.mapMarkerInstance)
		{
			this.mapMarkerInstance.Kill(BaseNetworkable.DestroyMode.None);
		}
		BaseEntity baseEntity = GameManager.server.CreateEntity(this.mapMarkerEntityPrefab.resourcePath, base.transform.position, Quaternion.identity, true);
		baseEntity.Spawn();
		baseEntity.SetParent(this, false, false);
		baseEntity.transform.localPosition = Vector3.zero;
		baseEntity.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
		this.mapMarkerInstance = baseEntity;
	}

	public void DelayedDestroy()
	{
		base.Kill(BaseNetworkable.DestroyMode.None);
	}

	public override void DestroyShared()
	{
		if (base.isServer && this.mapMarkerInstance)
		{
			this.mapMarkerInstance.Kill(BaseNetworkable.DestroyMode.None);
		}
		base.DestroyShared();
	}

	public void HackProgress()
	{
		this.hackSeconds += 1f;
		if (this.hackSeconds > HackableLockedCrate.requiredHackSeconds)
		{
			Interface.CallHook("OnCrateHackEnd", this);
			this.RefreshDecay();
			base.SetFlag(BaseEntity.Flags.Reserved2, true, false, true);
			this.isLootable = true;
			base.CancelInvoke(new Action(this.HackProgress));
		}
		base.ClientRPC<int, int>(null, "UpdateHackProgress", (int)this.hackSeconds, (int)HackableLockedCrate.requiredHackSeconds);
	}

	public bool IsBeingHacked()
	{
		return base.HasFlag(BaseEntity.Flags.Reserved1);
	}

	public bool IsFullyHacked()
	{
		return base.HasFlag(BaseEntity.Flags.Reserved2);
	}

	public void LandCheck()
	{
		RaycastHit raycastHit;
		if (this.hasLanded)
		{
			Interface.CallHook("OnCrateLanded", this);
			return;
		}
		if (UnityEngine.Physics.Raycast(new Ray(base.transform.position + (Vector3.up * 0.5f), Vector3.down), out raycastHit, 1f, 1218511105))
		{
			Effect.server.Run(this.landEffect.resourcePath, raycastHit.point, Vector3.up, null, false);
			this.hasLanded = true;
			base.CancelInvoke(new Action(this.LandCheck));
		}
	}

	public override void OnAttacked(HitInfo info)
	{
		if (base.isServer)
		{
			if (StringPool.Get(info.HitBone) == "laptopcollision")
			{
				Effect.server.Run(this.shockEffect.resourcePath, info.HitPositionWorld, Vector3.up, null, false);
				this.hackSeconds = this.hackSeconds - 8f * (info.damageTypes.Total() / 50f);
				if (this.hackSeconds < 0f)
				{
					this.hackSeconds = 0f;
				}
			}
			this.RefreshDecay();
		}
		base.OnAttacked(info);
	}

	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		bool flag;
		using (TimeWarning timeWarning = TimeWarning.New("HackableLockedCrate.OnRpcMessage", 0.1f))
		{
			if (rpc != 888500940 || !(player != null))
			{
				return base.OnRpcMessage(player, rpc, msg);
			}
			else
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (ConVar.Global.developer > 2)
				{
					Debug.Log(string.Concat("SV_RPCMessage: ", player, " - RPC_Hack "));
				}
				using (TimeWarning timeWarning1 = TimeWarning.New("RPC_Hack", 0.1f))
				{
					using (TimeWarning timeWarning2 = TimeWarning.New("Conditions", 0.1f))
					{
						if (!BaseEntity.RPC_Server.IsVisible.Test("RPC_Hack", this, player, 3f))
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
							this.RPC_Hack(rPCMessage);
						}
					}
					catch (Exception exception)
					{
						player.Kick("RPC Error in RPC_Hack");
						Debug.LogException(exception);
					}
				}
				flag = true;
			}
		}
		return flag;
	}

	public override void PostServerLoad()
	{
		base.PostServerLoad();
		base.SetFlag(BaseEntity.Flags.Reserved1, false, false, true);
	}

	public void RefreshDecay()
	{
		base.CancelInvoke(new Action(this.DelayedDestroy));
		base.Invoke(new Action(this.DelayedDestroy), HackableLockedCrate.decaySeconds);
	}

	[IsVisible(3f)]
	[RPC_Server]
	public void RPC_Hack(BaseEntity.RPCMessage msg)
	{
		if (this.IsBeingHacked())
		{
			return;
		}
		if (Interface.CallHook("CanHackCrate", msg.player, this) != null)
		{
			return;
		}
		this.StartHacking();
	}

	public override void ServerInit()
	{
		base.ServerInit();
		if (base.isClient)
		{
			return;
		}
		if (!Rust.Application.isLoadingSave)
		{
			base.SetFlag(BaseEntity.Flags.Reserved1, false, false, true);
			base.SetFlag(BaseEntity.Flags.Reserved2, false, false, true);
			if (this.wasDropped)
			{
				base.InvokeRepeating(new Action(this.LandCheck), 0f, 0.015f);
			}
		}
		this.RefreshDecay();
		this.isLootable = this.IsFullyHacked();
		this.CreateMapMarker(120f);
	}

	public void SetWasDropped()
	{
		this.wasDropped = true;
		Interface.CallHook("OnCrateDropped", this);
	}

	public void StartHacking()
	{
		Interface.CallHook("OnCrateHack", this);
		base.BroadcastEntityMessage("HackingStarted", 20f, 256);
		base.SetFlag(BaseEntity.Flags.Reserved1, true, false, true);
		base.InvokeRepeating(new Action(this.HackProgress), 1f, 1f);
		base.ClientRPC<int, int>(null, "UpdateHackProgress", 0, (int)HackableLockedCrate.requiredHackSeconds);
		this.RefreshDecay();
	}
}