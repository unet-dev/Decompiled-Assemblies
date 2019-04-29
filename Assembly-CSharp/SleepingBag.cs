using ConVar;
using Facepunch;
using Network;
using Oxide.Core;
using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Assertions;

public class SleepingBag : DecayEntity
{
	[NonSerialized]
	public ulong deployerUserID;

	public GameObject renameDialog;

	public GameObject assignDialog;

	public float secondsBetweenReuses = 300f;

	public string niceName = "Unnamed Bag";

	public Vector3 spawnOffset = Vector3.zero;

	public bool canBePublic;

	public float unlockTime;

	public static List<SleepingBag> sleepingBags;

	public float unlockSeconds
	{
		get
		{
			if (this.unlockTime < UnityEngine.Time.realtimeSinceStartup)
			{
				return 0f;
			}
			return this.unlockTime - UnityEngine.Time.realtimeSinceStartup;
		}
	}

	static SleepingBag()
	{
		SleepingBag.sleepingBags = new List<SleepingBag>();
	}

	public SleepingBag()
	{
	}

	[MaxDistance(3f)]
	[RPC_Server]
	public void AssignToFriend(BaseEntity.RPCMessage msg)
	{
		if (!msg.player.CanInteract())
		{
			return;
		}
		if (this.deployerUserID != msg.player.userID)
		{
			return;
		}
		ulong num = msg.read.UInt64();
		if (num == 0)
		{
			return;
		}
		if (Interface.CallHook("CanAssignBed", msg.player, this, num) != null)
		{
			return;
		}
		this.deployerUserID = num;
		base.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
	}

	public override bool CanPickup(BasePlayer player)
	{
		if (!base.CanPickup(player))
		{
			return false;
		}
		return player.userID == this.deployerUserID;
	}

	public static bool DestroyBag(BasePlayer player, uint sleepingBag)
	{
		SleepingBag sleepingBag1 = SleepingBag.FindForPlayer(player.userID, sleepingBag, false);
		if (sleepingBag1 == null)
		{
			return false;
		}
		if (!sleepingBag1.canBePublic)
		{
			sleepingBag1.Kill(BaseNetworkable.DestroyMode.None);
		}
		else
		{
			sleepingBag1.SetPublic(true);
			sleepingBag1.deployerUserID = (ulong)0;
		}
		player.SendRespawnOptions();
		return true;
	}

	internal override void DoServerDestroy()
	{
		base.DoServerDestroy();
		SleepingBag.sleepingBags.RemoveAll((SleepingBag x) => x == this);
	}

	public static SleepingBag[] FindForPlayer(ulong playerID, bool ignoreTimers)
	{
		return SleepingBag.sleepingBags.Where<SleepingBag>((SleepingBag x) => {
			if (x.deployerUserID != playerID)
			{
				return false;
			}
			if (ignoreTimers)
			{
				return true;
			}
			return x.unlockTime < UnityEngine.Time.realtimeSinceStartup;
		}).ToArray<SleepingBag>();
	}

	public static SleepingBag FindForPlayer(ulong playerID, uint sleepingBagID, bool ignoreTimers)
	{
		return SleepingBag.sleepingBags.FirstOrDefault<SleepingBag>((SleepingBag x) => {
			if (x.deployerUserID != playerID || x.net.ID != sleepingBagID)
			{
				return false;
			}
			if (ignoreTimers)
			{
				return true;
			}
			return x.unlockTime < UnityEngine.Time.realtimeSinceStartup;
		});
	}

	public bool IsPublic()
	{
		return base.HasFlag(BaseEntity.Flags.Reserved3);
	}

	public override void Load(BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (info.msg.sleepingBag != null)
		{
			this.niceName = info.msg.sleepingBag.name;
			this.deployerUserID = info.msg.sleepingBag.deployerID;
		}
	}

	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		bool flag;
		BaseEntity.RPCMessage rPCMessage;
		using (TimeWarning timeWarning = TimeWarning.New("SleepingBag.OnRpcMessage", 0.1f))
		{
			if (rpc == -1237911508 && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log(string.Concat("SV_RPCMessage: ", player, " - AssignToFriend "));
				}
				using (TimeWarning timeWarning1 = TimeWarning.New("AssignToFriend", 0.1f))
				{
					using (TimeWarning timeWarning2 = TimeWarning.New("Conditions", 0.1f))
					{
						if (!BaseEntity.RPC_Server.MaxDistance.Test("AssignToFriend", this, player, 3f))
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
							this.AssignToFriend(rPCMessage);
						}
					}
					catch (Exception exception)
					{
						player.Kick("RPC Error in AssignToFriend");
						Debug.LogException(exception);
					}
				}
				flag = true;
			}
			else if (rpc == 1335950295 && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log(string.Concat("SV_RPCMessage: ", player, " - Rename "));
				}
				using (timeWarning1 = TimeWarning.New("Rename", 0.1f))
				{
					using (timeWarning2 = TimeWarning.New("Conditions", 0.1f))
					{
						if (!BaseEntity.RPC_Server.MaxDistance.Test("Rename", this, player, 3f))
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
							this.Rename(rPCMessage);
						}
					}
					catch (Exception exception1)
					{
						player.Kick("RPC Error in Rename");
						Debug.LogException(exception1);
					}
				}
				flag = true;
			}
			else if (rpc == 42669546 && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log(string.Concat("SV_RPCMessage: ", player, " - RPC_MakeBed "));
				}
				using (timeWarning1 = TimeWarning.New("RPC_MakeBed", 0.1f))
				{
					using (timeWarning2 = TimeWarning.New("Conditions", 0.1f))
					{
						if (!BaseEntity.RPC_Server.MaxDistance.Test("RPC_MakeBed", this, player, 3f))
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
							this.RPC_MakeBed(rPCMessage);
						}
					}
					catch (Exception exception2)
					{
						player.Kick("RPC Error in RPC_MakeBed");
						Debug.LogException(exception2);
					}
				}
				flag = true;
			}
			else if (rpc != 393812086 || !(player != null))
			{
				return base.OnRpcMessage(player, rpc, msg);
			}
			else
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log(string.Concat("SV_RPCMessage: ", player, " - RPC_MakePublic "));
				}
				using (timeWarning1 = TimeWarning.New("RPC_MakePublic", 0.1f))
				{
					using (timeWarning2 = TimeWarning.New("Conditions", 0.1f))
					{
						if (!BaseEntity.RPC_Server.MaxDistance.Test("RPC_MakePublic", this, player, 3f))
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
							this.RPC_MakePublic(rPCMessage);
						}
					}
					catch (Exception exception3)
					{
						player.Kick("RPC Error in RPC_MakePublic");
						Debug.LogException(exception3);
					}
				}
				flag = true;
			}
		}
		return flag;
	}

	[MaxDistance(3f)]
	[RPC_Server]
	public void Rename(BaseEntity.RPCMessage msg)
	{
		if (!msg.player.CanInteract())
		{
			return;
		}
		string str = msg.read.String();
		if (Interface.CallHook("CanRenameBed", msg.player, this, str) != null)
		{
			return;
		}
		str = WordFilter.Filter(str);
		if (string.IsNullOrEmpty(str))
		{
			str = "Unnamed Sleeping Bag";
		}
		if (str.Length > 24)
		{
			str = string.Concat(str.Substring(0, 22), "..");
		}
		this.niceName = str;
		base.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
	}

	[MaxDistance(3f)]
	[RPC_Server]
	public void RPC_MakeBed(BaseEntity.RPCMessage msg)
	{
		if (!this.canBePublic || !this.IsPublic())
		{
			return;
		}
		if (!msg.player.CanInteract())
		{
			return;
		}
		this.deployerUserID = msg.player.userID;
		base.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
	}

	[MaxDistance(3f)]
	[RPC_Server]
	public void RPC_MakePublic(BaseEntity.RPCMessage msg)
	{
		if (!this.canBePublic)
		{
			return;
		}
		if (!msg.player.CanInteract())
		{
			return;
		}
		if (this.deployerUserID != msg.player.userID && !msg.player.CanBuild())
		{
			return;
		}
		bool flag = msg.read.Bit();
		if (flag == this.IsPublic())
		{
			return;
		}
		if (Interface.CallHook("CanSetBedPublic", msg.player, this) != null)
		{
			return;
		}
		this.SetPublic(flag);
		if (!this.IsPublic())
		{
			this.deployerUserID = msg.player.userID;
		}
		base.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
	}

	public override void Save(BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		info.msg.sleepingBag = Facepunch.Pool.Get<ProtoBuf.SleepingBag>();
		info.msg.sleepingBag.name = this.niceName;
		info.msg.sleepingBag.deployerID = this.deployerUserID;
	}

	public override void ServerInit()
	{
		base.ServerInit();
		if (!SleepingBag.sleepingBags.Contains(this))
		{
			SleepingBag.sleepingBags.Add(this);
		}
	}

	private void SetDeployedBy(BasePlayer player)
	{
		if (player == null)
		{
			return;
		}
		this.deployerUserID = player.userID;
		float single = UnityEngine.Time.realtimeSinceStartup;
		SleepingBag[] array = SleepingBag.sleepingBags.Where<SleepingBag>((SleepingBag x) => {
			if (x.deployerUserID != player.userID)
			{
				return false;
			}
			return x.unlockTime > UnityEngine.Time.realtimeSinceStartup;
		}).ToArray<SleepingBag>();
		for (int i = 0; i < (int)array.Length; i++)
		{
			SleepingBag sleepingBag = array[i];
			if (sleepingBag.unlockTime > single && Vector3.Distance(sleepingBag.transform.position, base.transform.position) <= ConVar.Server.respawnresetrange)
			{
				single = sleepingBag.unlockTime;
			}
		}
		this.unlockTime = Mathf.Max(single, UnityEngine.Time.realtimeSinceStartup + this.secondsBetweenReuses);
		base.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
	}

	public void SetPublic(bool isPublic)
	{
		base.SetFlag(BaseEntity.Flags.Reserved3, isPublic, false, true);
	}

	public static bool SpawnPlayer(BasePlayer player, uint sleepingBag)
	{
		SleepingBag[] sleepingBagArray = SleepingBag.FindForPlayer(player.userID, true);
		SleepingBag sleepingBag1 = sleepingBagArray.FirstOrDefault<SleepingBag>((SleepingBag x) => {
			if (x.deployerUserID != player.userID || x.net.ID != sleepingBag)
			{
				return false;
			}
			return x.unlockTime < UnityEngine.Time.realtimeSinceStartup;
		});
		if (sleepingBag1 == null)
		{
			return false;
		}
		Vector3 vector3 = sleepingBag1.transform.position + sleepingBag1.spawnOffset;
		Quaternion quaternion = sleepingBag1.transform.rotation;
		Quaternion quaternion1 = Quaternion.Euler(0f, quaternion.eulerAngles.y, 0f);
		player.RespawnAt(vector3, quaternion1);
		SleepingBag[] sleepingBagArray1 = sleepingBagArray;
		for (int i = 0; i < (int)sleepingBagArray1.Length; i++)
		{
			SleepingBag sleepingBag2 = sleepingBagArray1[i];
			if (Vector3.Distance(vector3, sleepingBag2.transform.position) <= ConVar.Server.respawnresetrange)
			{
				sleepingBag2.unlockTime = UnityEngine.Time.realtimeSinceStartup + sleepingBag2.secondsBetweenReuses;
			}
		}
		return true;
	}
}