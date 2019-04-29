using ConVar;
using Network;
using ProtoBuf;
using System;
using UnityEngine;
using UnityEngine.Assertions;

public class ElectricalBranch : IOEntity
{
	public int branchAmount = 2;

	public GameObjectRef branchPanelPrefab;

	private float nextChangeTime;

	public ElectricalBranch()
	{
	}

	public override void Load(BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (info.msg.ioEntity != null)
		{
			this.branchAmount = info.msg.ioEntity.genericInt1;
		}
	}

	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		bool flag;
		using (TimeWarning timeWarning = TimeWarning.New("ElectricalBranch.OnRpcMessage", 0.1f))
		{
			if (rpc != 643124146 || !(player != null))
			{
				return base.OnRpcMessage(player, rpc, msg);
			}
			else
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log(string.Concat("SV_RPCMessage: ", player, " - SetBranchOffPower "));
				}
				using (TimeWarning timeWarning1 = TimeWarning.New("SetBranchOffPower", 0.1f))
				{
					using (TimeWarning timeWarning2 = TimeWarning.New("Conditions", 0.1f))
					{
						if (!BaseEntity.RPC_Server.IsVisible.Test("SetBranchOffPower", this, player, 3f))
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
							this.SetBranchOffPower(rPCMessage);
						}
					}
					catch (Exception exception)
					{
						player.Kick("RPC Error in SetBranchOffPower");
						Debug.LogException(exception);
					}
				}
				flag = true;
			}
		}
		return flag;
	}

	public override void Save(BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		info.msg.ioEntity.genericInt1 = this.branchAmount;
	}

	public void SetBranchAmount(int newAmount)
	{
		newAmount = Mathf.Clamp(newAmount, 2, 100000000);
		this.branchAmount = newAmount;
	}

	[IsVisible(3f)]
	[RPC_Server]
	public void SetBranchOffPower(BaseEntity.RPCMessage msg)
	{
		BasePlayer basePlayer = msg.player;
		if (basePlayer == null || !basePlayer.CanBuild())
		{
			return;
		}
		if (UnityEngine.Time.time < this.nextChangeTime)
		{
			return;
		}
		this.nextChangeTime = UnityEngine.Time.time + 1f;
		int num = msg.read.Int32();
		this.branchAmount = Mathf.Clamp(num, 2, 10000000);
		Debug.Log(string.Concat("new branch power : ", this.branchAmount));
		base.MarkDirtyForceUpdateOutputs();
		base.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
	}

	public override void UpdateOutputs()
	{
		if (this.outputs.Length == 0)
		{
			this.ensureOutputsUpdated = false;
			return;
		}
		if (this.ensureOutputsUpdated)
		{
			if (this.outputs[0].connectedTo.Get(true) != null)
			{
				this.outputs[0].connectedTo.Get(true).UpdateFromInput(Mathf.Clamp(this.GetPassthroughAmount(0) - this.branchAmount, 0, this.currentEnergy), this.outputs[0].connectedToSlot);
			}
			if (this.outputs[1].connectedTo.Get(true) != null)
			{
				this.outputs[1].connectedTo.Get(true).UpdateFromInput(Mathf.Min(this.GetPassthroughAmount(0), this.branchAmount), this.outputs[1].connectedToSlot);
			}
		}
	}
}