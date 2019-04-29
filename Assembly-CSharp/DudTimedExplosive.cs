using ConVar;
using Facepunch;
using Network;
using ProtoBuf;
using System;
using UnityEngine;
using UnityEngine.Assertions;

public class DudTimedExplosive : TimedExplosive
{
	public GameObjectRef fizzleEffect;

	public GameObject wickSpark;

	public AudioSource wickSound;

	public float dudChance = 0.3f;

	[ItemSelector(ItemCategory.All)]
	public ItemDefinition itemToGive;

	[NonSerialized]
	private float explodeTime;

	public DudTimedExplosive()
	{
	}

	public virtual void BecomeDud()
	{
		bool flag;
		Vector3 vector3 = base.transform.position;
		Quaternion quaternion = base.transform.rotation;
		flag = (!this.parentEntity.IsValid(base.isServer) ? false : this.parentEntity.Get(base.isServer).syncPosition);
		if (flag)
		{
			base.SetParent(null, false, false);
		}
		base.transform.position = vector3;
		base.transform.rotation = quaternion;
		base.SetFlag(BaseEntity.Flags.On, false, false, true);
		this.SetCollisionEnabled(true);
		if (flag)
		{
			this.SetMotionEnabled(true);
		}
		Effect.server.Run("assets/bundled/prefabs/fx/impacts/blunt/concrete/concrete1.prefab", this, 0, Vector3.zero, Vector3.zero, null, false);
		base.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
		base.CancelInvoke(new Action(this.KillMessage));
		base.Invoke(new Action(this.KillMessage), 1200f);
	}

	public override bool CanStickTo(BaseEntity entity)
	{
		if (!base.CanStickTo(entity))
		{
			return false;
		}
		return this.IsWickBurning();
	}

	public override void Explode()
	{
		if (this.creatorEntity != null && this.creatorEntity.IsNpc)
		{
			base.Explode();
			return;
		}
		if (UnityEngine.Random.Range(0f, 1f) < this.dudChance)
		{
			this.BecomeDud();
			return;
		}
		base.Explode();
	}

	public override float GetRandomTimerTime()
	{
		float randomTimerTime = base.GetRandomTimerTime();
		float single = 1f;
		if (UnityEngine.Random.Range(0f, 1f) <= 0.15f)
		{
			single = 0.334f;
		}
		else if (UnityEngine.Random.Range(0f, 1f) <= 0.15f)
		{
			single = 3f;
		}
		return randomTimerTime * single;
	}

	private bool IsWickBurning()
	{
		return base.HasFlag(BaseEntity.Flags.On);
	}

	public override void Load(BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (info.msg.dudExplosive != null)
		{
			this.explodeTime = UnityEngine.Time.realtimeSinceStartup + info.msg.dudExplosive.fuseTimeLeft;
		}
	}

	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		bool flag;
		using (TimeWarning timeWarning = TimeWarning.New("DudTimedExplosive.OnRpcMessage", 0.1f))
		{
			if (rpc != -1858148972 || !(player != null))
			{
				return base.OnRpcMessage(player, rpc, msg);
			}
			else
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log(string.Concat("SV_RPCMessage: ", player, " - RPC_Pickup "));
				}
				using (TimeWarning timeWarning1 = TimeWarning.New("RPC_Pickup", 0.1f))
				{
					using (TimeWarning timeWarning2 = TimeWarning.New("Conditions", 0.1f))
					{
						if (!BaseEntity.RPC_Server.MaxDistance.Test("RPC_Pickup", this, player, 3f))
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
							this.RPC_Pickup(rPCMessage);
						}
					}
					catch (Exception exception)
					{
						player.Kick("RPC Error in RPC_Pickup");
						Debug.LogException(exception);
					}
				}
				flag = true;
			}
		}
		return flag;
	}

	[MaxDistance(3f)]
	[RPC_Server]
	public void RPC_Pickup(BaseEntity.RPCMessage msg)
	{
		if (this.IsWickBurning())
		{
			return;
		}
		BasePlayer basePlayer = msg.player;
		if (UnityEngine.Random.Range(0f, 1f) >= 0.5f && base.HasParent())
		{
			this.SetFuse(UnityEngine.Random.Range(2.5f, 3f));
			return;
		}
		basePlayer.GiveItem(ItemManager.Create(this.itemToGive, 1, (ulong)0), BaseEntity.GiveItemReason.Generic);
		base.Kill(BaseNetworkable.DestroyMode.None);
	}

	public override void Save(BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		info.msg.dudExplosive = Facepunch.Pool.Get<DudExplosive>();
		info.msg.dudExplosive.fuseTimeLeft = this.explodeTime - UnityEngine.Time.realtimeSinceStartup;
	}

	public override void SetFuse(float fuseLength)
	{
		base.SetFuse(fuseLength);
		this.explodeTime = UnityEngine.Time.realtimeSinceStartup + fuseLength;
		base.SetFlag(BaseEntity.Flags.On, true, false, true);
		base.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
		base.CancelInvoke(new Action(this.KillMessage));
	}
}