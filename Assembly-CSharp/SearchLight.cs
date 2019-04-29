using ConVar;
using Network;
using ProtoBuf;
using System;
using UnityEngine;
using UnityEngine.Assertions;

public class SearchLight : StorageContainer
{
	public GameObject pitchObject;

	public GameObject yawObject;

	public GameObject eyePoint;

	public GameObject lightEffect;

	public SoundPlayer turnLoop;

	public ItemDefinition fuelType;

	public Vector3 aimDir = Vector3.zero;

	public BasePlayer mountedPlayer;

	public float secondsRemaining;

	public SearchLight()
	{
	}

	public void FuelUpdate()
	{
		if (base.IsOn())
		{
			this.secondsRemaining -= UnityEngine.Time.deltaTime;
			if (this.secondsRemaining <= 0f)
			{
				Item slot = this.inventory.GetSlot(0);
				if (slot == null || slot.info != this.inventory.onlyAllowedItem)
				{
					base.SetFlag(BaseEntity.Flags.On, false, false, true);
					return;
				}
				slot.UseItem(1);
				this.secondsRemaining += 20f;
			}
		}
	}

	public bool IsMounted()
	{
		return this.mountedPlayer != null;
	}

	public override void Load(BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (info.msg.autoturret != null)
		{
			this.aimDir = info.msg.autoturret.aimDir;
		}
	}

	public void MountedUpdate()
	{
		if (this.mountedPlayer == null || this.mountedPlayer.IsSleeping() || !this.mountedPlayer.IsAlive() || this.mountedPlayer.IsWounded() || Vector3.Distance(this.mountedPlayer.transform.position, base.transform.position) > 2f)
		{
			this.PlayerExit();
			return;
		}
		Vector3 vector3 = this.eyePoint.transform.position + (this.mountedPlayer.eyes.BodyForward() * 100f);
		this.SetTargetAimpoint(vector3);
		base.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
	}

	public override void OnKilled(HitInfo info)
	{
		base.SetFlag(BaseEntity.Flags.On, false, false, true);
		base.OnKilled(info);
	}

	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		bool flag;
		BaseEntity.RPCMessage rPCMessage;
		using (TimeWarning timeWarning = TimeWarning.New("SearchLight.OnRpcMessage", 0.1f))
		{
			if (rpc == -1251103440 && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log(string.Concat("SV_RPCMessage: ", player, " - RPC_Switch "));
				}
				using (TimeWarning timeWarning1 = TimeWarning.New("RPC_Switch", 0.1f))
				{
					using (TimeWarning timeWarning2 = TimeWarning.New("Conditions", 0.1f))
					{
						if (!BaseEntity.RPC_Server.MaxDistance.Test("RPC_Switch", this, player, 3f))
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
							this.RPC_Switch(rPCMessage);
						}
					}
					catch (Exception exception)
					{
						player.Kick("RPC Error in RPC_Switch");
						Debug.LogException(exception);
					}
				}
				flag = true;
			}
			else if (rpc != -683351494 || !(player != null))
			{
				return base.OnRpcMessage(player, rpc, msg);
			}
			else
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log(string.Concat("SV_RPCMessage: ", player, " - RPC_UseLight "));
				}
				using (timeWarning1 = TimeWarning.New("RPC_UseLight", 0.1f))
				{
					using (timeWarning2 = TimeWarning.New("Conditions", 0.1f))
					{
						if (!BaseEntity.RPC_Server.MaxDistance.Test("RPC_UseLight", this, player, 3f))
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
							this.RPC_UseLight(rPCMessage);
						}
					}
					catch (Exception exception1)
					{
						player.Kick("RPC Error in RPC_UseLight");
						Debug.LogException(exception1);
					}
				}
				flag = true;
			}
		}
		return flag;
	}

	public void PlayerEnter(BasePlayer player)
	{
		if (this.IsMounted() && player != this.mountedPlayer)
		{
			return;
		}
		this.PlayerExit();
		if (player != null)
		{
			this.mountedPlayer = player;
			base.SetFlag(BaseEntity.Flags.Reserved5, true, false, true);
		}
	}

	public void PlayerExit()
	{
		if (this.mountedPlayer)
		{
			this.mountedPlayer = null;
		}
		base.SetFlag(BaseEntity.Flags.Reserved5, false, false, true);
	}

	public override void ResetState()
	{
		this.aimDir = Vector3.zero;
	}

	[MaxDistance(3f)]
	[RPC_Server]
	public void RPC_Switch(BaseEntity.RPCMessage msg)
	{
		bool flag = msg.read.Bit();
		if (this.needsBuildingPrivilegeToUse && !msg.player.CanBuild())
		{
			return;
		}
		base.SetFlag(BaseEntity.Flags.On, flag, false, true);
		this.FuelUpdate();
	}

	[MaxDistance(3f)]
	[RPC_Server]
	public void RPC_UseLight(BaseEntity.RPCMessage msg)
	{
		BasePlayer basePlayer = msg.player;
		bool flag = msg.read.Bit();
		if (flag && this.IsMounted())
		{
			return;
		}
		if (this.needsBuildingPrivilegeToUse && !msg.player.CanBuild())
		{
			return;
		}
		if (!flag)
		{
			this.PlayerExit();
			return;
		}
		this.PlayerEnter(basePlayer);
	}

	public override void Save(BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		info.msg.autoturret = new ProtoBuf.AutoTurret()
		{
			aimDir = this.aimDir
		};
	}

	public void SetTargetAimpoint(Vector3 worldPos)
	{
		Vector3 vector3 = worldPos - this.eyePoint.transform.position;
		this.aimDir = vector3.normalized;
	}

	public void Update()
	{
		if (base.isServer)
		{
			if (this.IsMounted())
			{
				this.MountedUpdate();
			}
			this.FuelUpdate();
		}
	}

	public static class SearchLightFlags
	{
		public const BaseEntity.Flags PlayerUsing = BaseEntity.Flags.Reserved5;
	}
}