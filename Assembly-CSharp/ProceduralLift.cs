using ConVar;
using Facepunch;
using Network;
using Oxide.Core;
using ProtoBuf;
using Rust;
using System;
using UnityEngine;
using UnityEngine.Assertions;

public class ProceduralLift : BaseEntity
{
	public float movementSpeed = 1f;

	public float resetDelay = 5f;

	public ProceduralLiftCabin cabin;

	public ProceduralLiftStop[] stops;

	public GameObjectRef triggerPrefab;

	public string triggerBone;

	private int floorIndex = -1;

	public ProceduralLift()
	{
	}

	public override void Load(BaseNetworkable.LoadInfo info)
	{
		if (info.msg.lift != null)
		{
			if (this.floorIndex != -1)
			{
				this.MoveToFloor(info.msg.lift.floor);
			}
			else
			{
				this.SnapToFloor(info.msg.lift.floor);
			}
		}
		base.Load(info);
	}

	private void MoveToFloor(int floor)
	{
		this.floorIndex = Mathf.Clamp(floor, 0, (int)this.stops.Length - 1);
		if (base.isServer)
		{
			base.SetFlag(BaseEntity.Flags.Busy, true, false, true);
			base.SendNetworkUpdateImmediate(false);
			base.CancelInvoke(new Action(this.ResetLift));
		}
	}

	private void OnFinishedMoving()
	{
		if (base.isServer)
		{
			base.SetFlag(BaseEntity.Flags.Busy, false, false, true);
			base.SendNetworkUpdateImmediate(false);
			if (this.floorIndex != 0)
			{
				base.Invoke(new Action(this.ResetLift), this.resetDelay);
			}
		}
	}

	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		bool flag;
		using (TimeWarning timeWarning = TimeWarning.New("ProceduralLift.OnRpcMessage", 0.1f))
		{
			if (rpc != -1637175855 || !(player != null))
			{
				return base.OnRpcMessage(player, rpc, msg);
			}
			else
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (ConVar.Global.developer > 2)
				{
					Debug.Log(string.Concat("SV_RPCMessage: ", player, " - RPC_UseLift "));
				}
				using (TimeWarning timeWarning1 = TimeWarning.New("RPC_UseLift", 0.1f))
				{
					using (TimeWarning timeWarning2 = TimeWarning.New("Conditions", 0.1f))
					{
						if (!BaseEntity.RPC_Server.MaxDistance.Test("RPC_UseLift", this, player, 3f))
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
							this.RPC_UseLift(rPCMessage);
						}
					}
					catch (Exception exception)
					{
						player.Kick("RPC Error in RPC_UseLift");
						Debug.LogException(exception);
					}
				}
				flag = true;
			}
		}
		return flag;
	}

	private void ResetLift()
	{
		this.MoveToFloor(0);
	}

	[MaxDistance(3f)]
	[RPC_Server]
	private void RPC_UseLift(BaseEntity.RPCMessage rpc)
	{
		if (!rpc.player.CanInteract())
		{
			return;
		}
		if (Interface.CallHook("OnLiftUse", this, rpc.player) != null)
		{
			return;
		}
		if (base.IsBusy())
		{
			return;
		}
		this.MoveToFloor((this.floorIndex + 1) % (int)this.stops.Length);
	}

	public override void Save(BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		info.msg.lift = Facepunch.Pool.Get<ProtoBuf.Lift>();
		info.msg.lift.floor = this.floorIndex;
	}

	public override void ServerInit()
	{
		base.ServerInit();
		this.SnapToFloor(0);
	}

	private void SnapToFloor(int floor)
	{
		this.floorIndex = Mathf.Clamp(floor, 0, (int)this.stops.Length - 1);
		ProceduralLiftStop proceduralLiftStop = this.stops[this.floorIndex];
		this.cabin.transform.position = proceduralLiftStop.transform.position;
		if (base.isServer)
		{
			base.SetFlag(BaseEntity.Flags.Busy, false, false, true);
			base.SendNetworkUpdateImmediate(false);
			base.CancelInvoke(new Action(this.ResetLift));
		}
	}

	public override void Spawn()
	{
		base.Spawn();
		if (!Rust.Application.isLoadingSave)
		{
			BaseEntity baseEntity = GameManager.server.CreateEntity(this.triggerPrefab.resourcePath, Vector3.zero, Quaternion.identity, true);
			baseEntity.Spawn();
			baseEntity.SetParent(this, this.triggerBone, false, false);
		}
	}

	protected void Update()
	{
		if (this.floorIndex < 0 || this.floorIndex > (int)this.stops.Length - 1)
		{
			return;
		}
		ProceduralLiftStop proceduralLiftStop = this.stops[this.floorIndex];
		if (this.cabin.transform.position == proceduralLiftStop.transform.position)
		{
			return;
		}
		this.cabin.transform.position = Vector3.MoveTowards(this.cabin.transform.position, proceduralLiftStop.transform.position, this.movementSpeed * UnityEngine.Time.deltaTime);
		if (this.cabin.transform.position == proceduralLiftStop.transform.position)
		{
			this.OnFinishedMoving();
		}
	}
}