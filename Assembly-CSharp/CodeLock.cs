using ConVar;
using Facepunch;
using Network;
using Oxide.Core;
using ProtoBuf;
using Rust;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class CodeLock : BaseLock
{
	public GameObjectRef keyEnterDialog;

	public GameObjectRef effectUnlocked;

	public GameObjectRef effectLocked;

	public GameObjectRef effectDenied;

	public GameObjectRef effectCodeChanged;

	public GameObjectRef effectShock;

	public bool hasCode;

	public bool hasGuestCode;

	public string code = string.Empty;

	public string guestCode = string.Empty;

	public List<ulong> whitelistPlayers = new List<ulong>();

	public List<ulong> guestPlayers = new List<ulong>();

	public int wrongCodes;

	public float lastWrongTime = Single.NegativeInfinity;

	public CodeLock()
	{
	}

	internal void DoEffect(string effect)
	{
		Effect.server.Run(effect, this, 0, Vector3.zero, Vector3.forward, null, false);
	}

	public override void Load(BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (info.msg.codeLock != null)
		{
			this.hasCode = info.msg.codeLock.hasCode;
			this.hasGuestCode = info.msg.codeLock.hasGuestCode;
			if (info.msg.codeLock.pv != null)
			{
				this.code = info.msg.codeLock.pv.code;
				this.whitelistPlayers = info.msg.codeLock.pv.users;
				this.guestCode = info.msg.codeLock.pv.guestCode;
				this.guestPlayers = info.msg.codeLock.pv.guestUsers;
				if (this.guestCode == null || this.guestCode.Length != 4)
				{
					this.hasGuestCode = false;
					this.guestCode = string.Empty;
					this.guestPlayers.Clear();
				}
			}
		}
	}

	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		bool flag;
		BaseEntity.RPCMessage rPCMessage;
		using (TimeWarning timeWarning = TimeWarning.New("CodeLock.OnRpcMessage", 0.1f))
		{
			if (rpc == -281182935 && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (ConVar.Global.developer > 2)
				{
					Debug.Log(string.Concat("SV_RPCMessage: ", player, " - RPC_ChangeCode "));
				}
				using (TimeWarning timeWarning1 = TimeWarning.New("RPC_ChangeCode", 0.1f))
				{
					using (TimeWarning timeWarning2 = TimeWarning.New("Conditions", 0.1f))
					{
						if (!BaseEntity.RPC_Server.MaxDistance.Test("RPC_ChangeCode", this, player, 3f))
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
							this.RPC_ChangeCode(rPCMessage);
						}
					}
					catch (Exception exception)
					{
						player.Kick("RPC Error in RPC_ChangeCode");
						Debug.LogException(exception);
					}
				}
				flag = true;
			}
			else if (rpc == -1668899863 && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (ConVar.Global.developer > 2)
				{
					Debug.Log(string.Concat("SV_RPCMessage: ", player, " - TryLock "));
				}
				using (timeWarning1 = TimeWarning.New("TryLock", 0.1f))
				{
					using (timeWarning2 = TimeWarning.New("Conditions", 0.1f))
					{
						if (!BaseEntity.RPC_Server.MaxDistance.Test("TryLock", this, player, 3f))
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
							this.TryLock(rPCMessage);
						}
					}
					catch (Exception exception1)
					{
						player.Kick("RPC Error in TryLock");
						Debug.LogException(exception1);
					}
				}
				flag = true;
			}
			else if (rpc == 1718262 && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (ConVar.Global.developer > 2)
				{
					Debug.Log(string.Concat("SV_RPCMessage: ", player, " - TryUnlock "));
				}
				using (timeWarning1 = TimeWarning.New("TryUnlock", 0.1f))
				{
					using (timeWarning2 = TimeWarning.New("Conditions", 0.1f))
					{
						if (!BaseEntity.RPC_Server.MaxDistance.Test("TryUnlock", this, player, 3f))
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
							this.TryUnlock(rPCMessage);
						}
					}
					catch (Exception exception2)
					{
						player.Kick("RPC Error in TryUnlock");
						Debug.LogException(exception2);
					}
				}
				flag = true;
			}
			else if (rpc != 418605506 || !(player != null))
			{
				return base.OnRpcMessage(player, rpc, msg);
			}
			else
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (ConVar.Global.developer > 2)
				{
					Debug.Log(string.Concat("SV_RPCMessage: ", player, " - UnlockWithCode "));
				}
				using (timeWarning1 = TimeWarning.New("UnlockWithCode", 0.1f))
				{
					using (timeWarning2 = TimeWarning.New("Conditions", 0.1f))
					{
						if (!BaseEntity.RPC_Server.MaxDistance.Test("UnlockWithCode", this, player, 3f))
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
							this.UnlockWithCode(rPCMessage);
						}
					}
					catch (Exception exception3)
					{
						player.Kick("RPC Error in UnlockWithCode");
						Debug.LogException(exception3);
					}
				}
				flag = true;
			}
		}
		return flag;
	}

	public override bool OnTryToClose(BasePlayer player)
	{
		object obj = Interface.CallHook("CanUseLockedEntity", player, this);
		if (obj as bool)
		{
			return (bool)obj;
		}
		if (!base.IsLocked())
		{
			return true;
		}
		if (!this.whitelistPlayers.Contains(player.userID) && !this.guestPlayers.Contains(player.userID))
		{
			this.DoEffect(this.effectDenied.resourcePath);
			return false;
		}
		this.DoEffect(this.effectUnlocked.resourcePath);
		return true;
	}

	public override bool OnTryToOpen(BasePlayer player)
	{
		object obj = Interface.CallHook("CanUseLockedEntity", player, this);
		if (obj as bool)
		{
			return (bool)obj;
		}
		if (!base.IsLocked())
		{
			return true;
		}
		if (!this.whitelistPlayers.Contains(player.userID) && !this.guestPlayers.Contains(player.userID))
		{
			this.DoEffect(this.effectDenied.resourcePath);
			return false;
		}
		this.DoEffect(this.effectUnlocked.resourcePath);
		return true;
	}

	[MaxDistance(3f)]
	[RPC_Server]
	private void RPC_ChangeCode(BaseEntity.RPCMessage rpc)
	{
		if (!rpc.player.CanInteract())
		{
			return;
		}
		string str = rpc.read.String();
		bool flag = rpc.read.Bit();
		if (base.IsLocked())
		{
			return;
		}
		if (str.Length != 4)
		{
			return;
		}
		if (!this.hasCode & flag)
		{
			return;
		}
		if (!this.hasCode && !flag)
		{
			base.SetFlag(BaseEntity.Flags.Locked, true, false, true);
		}
		if (Interface.CallHook("CanChangeCode", rpc.player, this, str, flag) != null)
		{
			return;
		}
		if (flag)
		{
			this.guestCode = str;
			this.hasGuestCode = this.guestCode.Length > 0;
			this.guestPlayers.Clear();
			this.guestPlayers.Add(rpc.player.userID);
		}
		else
		{
			this.code = str;
			this.hasCode = this.code.Length > 0;
			this.whitelistPlayers.Clear();
			this.whitelistPlayers.Add(rpc.player.userID);
		}
		this.DoEffect(this.effectCodeChanged.resourcePath);
		base.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
	}

	public override void Save(BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		info.msg.codeLock = Facepunch.Pool.Get<ProtoBuf.CodeLock>();
		info.msg.codeLock.hasGuestCode = this.guestCode.Length > 0;
		info.msg.codeLock.hasCode = this.code.Length > 0;
		if (info.forDisk)
		{
			info.msg.codeLock.pv = Facepunch.Pool.Get<ProtoBuf.CodeLock.Private>();
			info.msg.codeLock.pv.code = this.code;
			info.msg.codeLock.pv.users = Facepunch.Pool.Get<List<ulong>>();
			info.msg.codeLock.pv.users.AddRange(this.whitelistPlayers);
			info.msg.codeLock.pv.guestCode = this.guestCode;
			info.msg.codeLock.pv.guestUsers = Facepunch.Pool.Get<List<ulong>>();
			info.msg.codeLock.pv.guestUsers.AddRange(this.guestPlayers);
		}
	}

	[MaxDistance(3f)]
	[RPC_Server]
	private void TryLock(BaseEntity.RPCMessage rpc)
	{
		if (!rpc.player.CanInteract())
		{
			return;
		}
		if (base.IsLocked())
		{
			return;
		}
		if (this.code.Length != 4)
		{
			return;
		}
		if (Interface.CallHook("CanLock", rpc.player, this) != null)
		{
			return;
		}
		if (!this.whitelistPlayers.Contains(rpc.player.userID))
		{
			return;
		}
		this.DoEffect(this.effectLocked.resourcePath);
		base.SetFlag(BaseEntity.Flags.Locked, true, false, true);
		base.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
	}

	[MaxDistance(3f)]
	[RPC_Server]
	private void TryUnlock(BaseEntity.RPCMessage rpc)
	{
		if (!rpc.player.CanInteract())
		{
			return;
		}
		if (!base.IsLocked())
		{
			return;
		}
		if (Interface.CallHook("CanUnlock", rpc.player, this) != null)
		{
			return;
		}
		if (!this.whitelistPlayers.Contains(rpc.player.userID))
		{
			base.ClientRPCPlayer(null, rpc.player, "EnterUnlockCode");
			return;
		}
		this.DoEffect(this.effectUnlocked.resourcePath);
		base.SetFlag(BaseEntity.Flags.Locked, false, false, true);
		base.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
	}

	[MaxDistance(3f)]
	[RPC_Server]
	private void UnlockWithCode(BaseEntity.RPCMessage rpc)
	{
		bool flag;
		if (!rpc.player.CanInteract())
		{
			return;
		}
		if (!base.IsLocked())
		{
			return;
		}
		string str = rpc.read.String();
		if (Interface.CallHook("OnCodeEntered", this, rpc.player, str) != null)
		{
			return;
		}
		bool flag1 = str == this.guestCode;
		bool flag2 = str == this.code;
		if (str == this.code)
		{
			flag = true;
		}
		else
		{
			flag = (!this.hasGuestCode ? false : str == this.guestCode);
		}
		if (!flag)
		{
			if (UnityEngine.Time.realtimeSinceStartup > this.lastWrongTime + 10f)
			{
				this.wrongCodes = 0;
			}
			this.DoEffect(this.effectDenied.resourcePath);
			this.DoEffect(this.effectShock.resourcePath);
			rpc.player.Hurt((float)(this.wrongCodes + 1) * 5f, DamageType.ElectricShock, this, false);
			this.wrongCodes++;
			this.lastWrongTime = UnityEngine.Time.realtimeSinceStartup;
			return;
		}
		base.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
		if (flag2)
		{
			if (!this.whitelistPlayers.Contains(rpc.player.userID))
			{
				this.DoEffect(this.effectCodeChanged.resourcePath);
				this.whitelistPlayers.Add(rpc.player.userID);
				return;
			}
		}
		else if (flag1 && !this.guestPlayers.Contains(rpc.player.userID))
		{
			this.DoEffect(this.effectCodeChanged.resourcePath);
			this.guestPlayers.Add(rpc.player.userID);
		}
	}
}