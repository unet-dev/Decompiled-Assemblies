using ConVar;
using Network;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class WireTool : HeldEntity
{
	public static float maxWireLength;

	private const int maxLineNodes = 16;

	public GameObjectRef plugEffect;

	public GameObjectRef ioLine;

	public WireTool.PendingPlug_t pending;

	public Sprite InputSprite;

	public Sprite OutputSprite;

	public Sprite ClearSprite;

	static WireTool()
	{
		WireTool.maxWireLength = 30f;
	}

	public WireTool()
	{
	}

	[IsVisible(3f)]
	[RPC_Server]
	public void AddLine(BaseEntity.RPCMessage msg)
	{
		IOEntity component;
		IOEntity oEntity;
		if (!WireTool.CanPlayerUseWires(msg.player))
		{
			return;
		}
		int num = msg.read.Int32();
		if (num > 18)
		{
			return;
		}
		List<Vector3> vector3s = new List<Vector3>();
		for (int i = 0; i < num; i++)
		{
			vector3s.Add(msg.read.Vector3());
		}
		if (!this.ValidateLine(vector3s))
		{
			return;
		}
		uint num1 = msg.read.UInt32();
		int num2 = msg.read.Int32();
		uint num3 = msg.read.UInt32();
		int num4 = msg.read.Int32();
		BaseNetworkable baseNetworkable = BaseNetworkable.serverEntities.Find(num1);
		if (baseNetworkable == null)
		{
			component = null;
		}
		else
		{
			component = baseNetworkable.GetComponent<IOEntity>();
		}
		IOEntity oEntity1 = component;
		if (oEntity1 == null)
		{
			return;
		}
		BaseNetworkable baseNetworkable1 = BaseNetworkable.serverEntities.Find(num3);
		if (baseNetworkable1 == null)
		{
			oEntity = null;
		}
		else
		{
			oEntity = baseNetworkable1.GetComponent<IOEntity>();
		}
		IOEntity array = oEntity;
		if (array == null)
		{
			return;
		}
		if (num2 >= (int)oEntity1.inputs.Length)
		{
			return;
		}
		if (num4 >= (int)array.outputs.Length)
		{
			return;
		}
		if (oEntity1.inputs[num2].connectedTo.Get(true) != null)
		{
			return;
		}
		if (array.outputs[num4].connectedTo.Get(true) != null)
		{
			return;
		}
		array.outputs[num4].linePoints = vector3s.ToArray();
		array.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
	}

	public static bool CanPlayerUseWires(BasePlayer player)
	{
		if (!player.CanBuild())
		{
			return false;
		}
		return !GamePhysics.CheckSphere(player.eyes.position, 0.1f, 536870912, QueryTriggerInteraction.Collide);
	}

	public void ClearPendingPlug()
	{
		this.pending.ent = null;
		this.pending.index = -1;
	}

	public bool HasPendingPlug()
	{
		if (this.pending.ent == null)
		{
			return false;
		}
		return this.pending.index != -1;
	}

	[IsVisible(3f)]
	[RPC_Server]
	public void MakeConnection(BaseEntity.RPCMessage msg)
	{
		IOEntity component;
		IOEntity oEntity;
		if (!WireTool.CanPlayerUseWires(msg.player))
		{
			return;
		}
		uint num = msg.read.UInt32();
		int num1 = msg.read.Int32();
		uint num2 = msg.read.UInt32();
		int num3 = msg.read.Int32();
		BaseNetworkable baseNetworkable = BaseNetworkable.serverEntities.Find(num);
		if (baseNetworkable == null)
		{
			component = null;
		}
		else
		{
			component = baseNetworkable.GetComponent<IOEntity>();
		}
		IOEntity oEntity1 = component;
		if (oEntity1 == null)
		{
			return;
		}
		BaseNetworkable baseNetworkable1 = BaseNetworkable.serverEntities.Find(num2);
		if (baseNetworkable1 == null)
		{
			oEntity = null;
		}
		else
		{
			oEntity = baseNetworkable1.GetComponent<IOEntity>();
		}
		IOEntity oEntity2 = oEntity;
		if (oEntity2 == null)
		{
			return;
		}
		if (Vector3.Distance(baseNetworkable1.transform.position, baseNetworkable.transform.position) > WireTool.maxWireLength)
		{
			return;
		}
		if (num1 >= (int)oEntity1.inputs.Length)
		{
			return;
		}
		if (num3 >= (int)oEntity2.outputs.Length)
		{
			return;
		}
		if (oEntity1.inputs[num1].connectedTo.Get(true) != null)
		{
			return;
		}
		if (oEntity2.outputs[num3].connectedTo.Get(true) != null)
		{
			return;
		}
		if (oEntity1.inputs[num1].rootConnectionsOnly && !oEntity2.IsRootEntity())
		{
			return;
		}
		oEntity1.inputs[num1].connectedTo.Set(oEntity2);
		oEntity1.inputs[num1].connectedToSlot = num3;
		oEntity1.inputs[num1].connectedTo.Init();
		oEntity2.outputs[num3].connectedTo.Set(oEntity1);
		oEntity2.outputs[num3].connectedToSlot = num1;
		oEntity2.outputs[num3].connectedTo.Init();
		oEntity2.MarkDirtyForceUpdateOutputs();
		oEntity2.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
		oEntity1.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
	}

	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		bool flag;
		BaseEntity.RPCMessage rPCMessage;
		using (TimeWarning timeWarning = TimeWarning.New("WireTool.OnRpcMessage", 0.1f))
		{
			if (rpc == 678101026 && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log(string.Concat("SV_RPCMessage: ", player, " - AddLine "));
				}
				using (TimeWarning timeWarning1 = TimeWarning.New("AddLine", 0.1f))
				{
					using (TimeWarning timeWarning2 = TimeWarning.New("Conditions", 0.1f))
					{
						if (!BaseEntity.RPC_Server.IsVisible.Test("AddLine", this, player, 3f))
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
							this.AddLine(rPCMessage);
						}
					}
					catch (Exception exception)
					{
						player.Kick("RPC Error in AddLine");
						Debug.LogException(exception);
					}
				}
				flag = true;
			}
			else if (rpc == 40328523 && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log(string.Concat("SV_RPCMessage: ", player, " - MakeConnection "));
				}
				using (timeWarning1 = TimeWarning.New("MakeConnection", 0.1f))
				{
					using (timeWarning2 = TimeWarning.New("Conditions", 0.1f))
					{
						if (!BaseEntity.RPC_Server.IsVisible.Test("MakeConnection", this, player, 3f))
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
							this.MakeConnection(rPCMessage);
						}
					}
					catch (Exception exception1)
					{
						player.Kick("RPC Error in MakeConnection");
						Debug.LogException(exception1);
					}
				}
				flag = true;
			}
			else if (rpc == -1825127037 && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log(string.Concat("SV_RPCMessage: ", player, " - RequestClear "));
				}
				using (timeWarning1 = TimeWarning.New("RequestClear", 0.1f))
				{
					using (timeWarning2 = TimeWarning.New("Conditions", 0.1f))
					{
						if (!BaseEntity.RPC_Server.IsVisible.Test("RequestClear", this, player, 3f))
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
							this.RequestClear(rPCMessage);
						}
					}
					catch (Exception exception2)
					{
						player.Kick("RPC Error in RequestClear");
						Debug.LogException(exception2);
					}
				}
				flag = true;
			}
			else if (rpc == -1698508904 && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log(string.Concat("SV_RPCMessage: ", player, " - SetPlugged "));
				}
				using (timeWarning1 = TimeWarning.New("SetPlugged", 0.1f))
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
							this.SetPlugged(rPCMessage);
						}
					}
					catch (Exception exception3)
					{
						player.Kick("RPC Error in SetPlugged");
						Debug.LogException(exception3);
					}
				}
				flag = true;
			}
			else if (rpc != 210386477 || !(player != null))
			{
				return base.OnRpcMessage(player, rpc, msg);
			}
			else
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log(string.Concat("SV_RPCMessage: ", player, " - TryClear "));
				}
				using (timeWarning1 = TimeWarning.New("TryClear", 0.1f))
				{
					using (timeWarning2 = TimeWarning.New("Conditions", 0.1f))
					{
						if (!BaseEntity.RPC_Server.IsVisible.Test("TryClear", this, player, 3f))
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
							this.TryClear(rPCMessage);
						}
					}
					catch (Exception exception4)
					{
						player.Kick("RPC Error in TryClear");
						Debug.LogException(exception4);
					}
				}
				flag = true;
			}
		}
		return flag;
	}

	public bool PendingPlugIsInput()
	{
		if (!(this.pending.ent != null) || this.pending.index == -1)
		{
			return false;
		}
		return this.pending.input;
	}

	public bool PendingPlugIsOutput()
	{
		if (!(this.pending.ent != null) || this.pending.index == -1)
		{
			return false;
		}
		return !this.pending.input;
	}

	public bool PendingPlugRoot()
	{
		if (this.pending.ent == null)
		{
			return false;
		}
		return this.pending.ent.IsRootEntity();
	}

	[IsVisible(3f)]
	[RPC_Server]
	public void RequestClear(BaseEntity.RPCMessage msg)
	{
		IOEntity component;
		IOEntity.IOSlot oSlot;
		if (!WireTool.CanPlayerUseWires(msg.player))
		{
			return;
		}
		uint num = msg.read.UInt32();
		int num1 = msg.read.Int32();
		bool flag = msg.read.Bit();
		BaseNetworkable baseNetworkable = BaseNetworkable.serverEntities.Find(num);
		if (baseNetworkable == null)
		{
			component = null;
		}
		else
		{
			component = baseNetworkable.GetComponent<IOEntity>();
		}
		IOEntity oEntity = component;
		if (oEntity == null)
		{
			return;
		}
		if (num1 >= (flag ? (int)oEntity.inputs.Length : (int)oEntity.outputs.Length))
		{
			return;
		}
		IOEntity.IOSlot oSlot1 = (flag ? oEntity.inputs[num1] : oEntity.outputs[num1]);
		if (oSlot1.connectedTo.Get(true) == null)
		{
			return;
		}
		IOEntity oEntity1 = oSlot1.connectedTo.Get(true);
		oSlot = (flag ? oEntity1.outputs[oSlot1.connectedToSlot] : oEntity1.inputs[oSlot1.connectedToSlot]);
		if (flag)
		{
			oEntity.UpdateFromInput(0, num1);
		}
		else if (oEntity1)
		{
			oEntity1.UpdateFromInput(0, oSlot1.connectedToSlot);
		}
		oSlot1.Clear();
		oSlot.Clear();
		if (oEntity1)
		{
			oEntity1.MarkDirtyForceUpdateOutputs();
			oEntity1.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
		}
		oEntity.MarkDirtyForceUpdateOutputs();
		oEntity.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
	}

	[RPC_Server]
	public void SetPlugged(BaseEntity.RPCMessage msg)
	{
	}

	[IsVisible(3f)]
	[RPC_Server]
	public void TryClear(BaseEntity.RPCMessage msg)
	{
		IOEntity component;
		if (!WireTool.CanPlayerUseWires(msg.player))
		{
			return;
		}
		uint num = msg.read.UInt32();
		BaseNetworkable baseNetworkable = BaseNetworkable.serverEntities.Find(num);
		if (baseNetworkable == null)
		{
			component = null;
		}
		else
		{
			component = baseNetworkable.GetComponent<IOEntity>();
		}
		IOEntity oEntity = component;
		if (oEntity == null)
		{
			return;
		}
		oEntity.ClearConnections();
		oEntity.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
	}

	public bool ValidateLine(List<Vector3> lineList)
	{
		if (lineList.Count < 2)
		{
			return false;
		}
		Vector3 item = lineList[0];
		float single = 0f;
		for (int i = 1; i < lineList.Count; i++)
		{
			Vector3 vector3 = lineList[i];
			single += Vector3.Distance(item, vector3);
			if (single > WireTool.maxWireLength)
			{
				return false;
			}
			item = vector3;
		}
		return true;
	}

	public struct PendingPlug_t
	{
		public IOEntity ent;

		public bool input;

		public int index;

		public GameObject tempLine;
	}
}