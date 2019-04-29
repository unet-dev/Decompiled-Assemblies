using ConVar;
using Facepunch;
using Network;
using Oxide.Core;
using ProtoBuf;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class MapEntity : HeldEntity
{
	[NonSerialized]
	public uint[] fogImages = new uint[1];

	[NonSerialized]
	public uint[] paintImages = new uint[144];

	public MapEntity()
	{
	}

	[FromOwner]
	[RPC_Server]
	public void ImageUpdate(BaseEntity.RPCMessage msg)
	{
		byte num = msg.read.UInt8();
		byte num1 = msg.read.UInt8();
		uint num2 = msg.read.UInt32();
		if (num == 0 && this.fogImages[num1] == num2)
		{
			return;
		}
		if (num == 1 && this.paintImages[num1] == num2)
		{
			return;
		}
		uint num3 = (uint)(num * 1000 + num1);
		byte[] numArray = msg.read.BytesWithSize();
		if (numArray == null)
		{
			return;
		}
		FileStorage.server.RemoveEntityNum(this.net.ID, num3);
		uint num4 = FileStorage.server.Store(numArray, FileStorage.Type.png, this.net.ID, num3);
		if (num == 0)
		{
			this.fogImages[num1] = num4;
		}
		if (num == 1)
		{
			this.paintImages[num1] = num4;
		}
		base.InvalidateNetworkCache();
		Interface.CallHook("OnMapImageUpdated");
	}

	public override void Load(BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (info.msg.mapEntity != null)
		{
			if (info.msg.mapEntity.fogImages.Count == (int)this.fogImages.Length)
			{
				this.fogImages = info.msg.mapEntity.fogImages.ToArray();
			}
			if (info.msg.mapEntity.paintImages.Count == (int)this.paintImages.Length)
			{
				this.paintImages = info.msg.mapEntity.paintImages.ToArray();
			}
		}
	}

	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		bool flag;
		using (TimeWarning timeWarning = TimeWarning.New("MapEntity.OnRpcMessage", 0.1f))
		{
			if (rpc != 1443560440 || !(player != null))
			{
				return base.OnRpcMessage(player, rpc, msg);
			}
			else
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log(string.Concat("SV_RPCMessage: ", player, " - ImageUpdate "));
				}
				using (TimeWarning timeWarning1 = TimeWarning.New("ImageUpdate", 0.1f))
				{
					using (TimeWarning timeWarning2 = TimeWarning.New("Conditions", 0.1f))
					{
						if (!BaseEntity.RPC_Server.FromOwner.Test("ImageUpdate", this, player))
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
							this.ImageUpdate(rPCMessage);
						}
					}
					catch (Exception exception)
					{
						player.Kick("RPC Error in ImageUpdate");
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
		info.msg.mapEntity = Facepunch.Pool.Get<ProtoBuf.MapEntity>();
		info.msg.mapEntity.fogImages = Facepunch.Pool.Get<List<uint>>();
		info.msg.mapEntity.fogImages.AddRange(this.fogImages);
		info.msg.mapEntity.paintImages = Facepunch.Pool.Get<List<uint>>();
		info.msg.mapEntity.paintImages.AddRange(this.paintImages);
	}
}