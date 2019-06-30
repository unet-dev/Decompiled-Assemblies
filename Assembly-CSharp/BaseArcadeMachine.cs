using ConVar;
using Facepunch;
using Network;
using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

public class BaseArcadeMachine : BaseVehicle
{
	public BaseArcadeGame arcadeGamePrefab;

	public BaseArcadeGame activeGame;

	public ArcadeNetworkTrigger networkTrigger;

	public float broadcastRadius = 8f;

	public Transform gameScreen;

	public RawImage RTImage;

	public Transform leftJoystick;

	public Transform rightJoystick;

	public SoundPlayer musicPlayer;

	public const BaseEntity.Flags Flag_P1 = BaseEntity.Flags.Reserved7;

	public const BaseEntity.Flags Flag_P2 = BaseEntity.Flags.Reserved8;

	public List<BaseArcadeMachine.ScoreEntry> scores = new List<BaseArcadeMachine.ScoreEntry>(10);

	private const int inputFrameRate = 60;

	private const int snapshotFrameRate = 15;

	private Connection.TimeAverageValue DestroyMessageFromHostPerSecond;

	private Connection.TimeAverageValue BroadcastEntityMessagePerSecond;

	private Connection.TimeAverageValue GetSnapshotFromClientPerSecond;

	public BaseArcadeMachine()
	{
	}

	public void AddScore(BasePlayer player, int score)
	{
		BaseArcadeMachine.ScoreEntry scoreEntry = new BaseArcadeMachine.ScoreEntry()
		{
			displayName = player.displayName,
			score = score,
			playerID = player.userID
		};
		this.scores.Add(scoreEntry);
		this.scores.Sort((BaseArcadeMachine.ScoreEntry a, BaseArcadeMachine.ScoreEntry b) => b.score.CompareTo(a.score));
		this.scores.TrimExcess();
		base.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
	}

	[IsVisible(3f)]
	[RPC_Server]
	public void BroadcastEntityMessage(BaseEntity.RPCMessage msg)
	{
		BasePlayer basePlayer = msg.player;
		if (basePlayer == null || base.GetDriver() != basePlayer)
		{
			return;
		}
		if (this.BroadcastEntityMessagePerSecond.Calculate() > (long)7)
		{
			return;
		}
		this.BroadcastEntityMessagePerSecond.Increment();
		if (this.networkTrigger.entityContents != null)
		{
			uint num = msg.read.UInt32();
			string str = msg.read.String();
			foreach (BaseEntity entityContent in this.networkTrigger.entityContents)
			{
				BasePlayer component = entityContent.GetComponent<BasePlayer>();
				base.ClientRPCPlayer<uint, string>(null, component, "GetEntityMessage", num, str);
			}
		}
	}

	[IsVisible(3f)]
	[RPC_Server]
	public void DestroyMessageFromHost(BaseEntity.RPCMessage msg)
	{
		BasePlayer basePlayer = msg.player;
		if (basePlayer == null || base.GetDriver() != basePlayer)
		{
			return;
		}
		if (this.networkTrigger.entityContents != null)
		{
			uint num = msg.read.UInt32();
			foreach (BaseEntity entityContent in this.networkTrigger.entityContents)
			{
				base.ClientRPCPlayer<uint>(null, entityContent.GetComponent<BasePlayer>(), "DestroyEntity", num);
			}
		}
	}

	[IsVisible(3f)]
	[RPC_Server]
	public void GetSnapshotFromClient(BaseEntity.RPCMessage msg)
	{
		BasePlayer basePlayer = msg.player;
		if (basePlayer == null || basePlayer != base.GetDriver())
		{
			return;
		}
		if (this.GetSnapshotFromClientPerSecond.Calculate() > (long)30)
		{
			return;
		}
		this.GetSnapshotFromClientPerSecond.Increment();
		ArcadeGame arcadeGame = Facepunch.Pool.Get<ArcadeGame>();
		arcadeGame = ArcadeGame.Deserialize(msg.read);
		Connection connection = null;
		if (this.networkTrigger.entityContents != null)
		{
			foreach (BaseEntity entityContent in this.networkTrigger.entityContents)
			{
				base.ClientRPCPlayer<ArcadeGame>(connection, entityContent.GetComponent<BasePlayer>(), "GetSnapshotFromServer", arcadeGame);
			}
		}
	}

	public override void Load(BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (info.msg.arcadeMachine != null && info.msg.arcadeMachine.scores != null)
		{
			this.scores.Clear();
			for (int i = 0; i < info.msg.arcadeMachine.scores.Count; i++)
			{
				BaseArcadeMachine.ScoreEntry scoreEntry = new BaseArcadeMachine.ScoreEntry()
				{
					displayName = info.msg.arcadeMachine.scores[i].displayName,
					score = info.msg.arcadeMachine.scores[i].score,
					playerID = info.msg.arcadeMachine.scores[i].playerID
				};
				this.scores.Add(scoreEntry);
			}
		}
	}

	public void NearbyClientMessage(string msg)
	{
		if (this.networkTrigger.entityContents != null)
		{
			foreach (BaseEntity entityContent in this.networkTrigger.entityContents)
			{
				base.ClientRPCPlayer(null, entityContent.GetComponent<BasePlayer>(), msg);
			}
		}
	}

	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		bool flag;
		BaseEntity.RPCMessage rPCMessage;
		using (TimeWarning timeWarning = TimeWarning.New("BaseArcadeMachine.OnRpcMessage", 0.1f))
		{
			if (rpc == 271542211 && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log(string.Concat("SV_RPCMessage: ", player, " - BroadcastEntityMessage "));
				}
				using (TimeWarning timeWarning1 = TimeWarning.New("BroadcastEntityMessage", 0.1f))
				{
					using (TimeWarning timeWarning2 = TimeWarning.New("Conditions", 0.1f))
					{
						if (!BaseEntity.RPC_Server.IsVisible.Test("BroadcastEntityMessage", this, player, 3f))
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
							this.BroadcastEntityMessage(rPCMessage);
						}
					}
					catch (Exception exception)
					{
						player.Kick("RPC Error in BroadcastEntityMessage");
						Debug.LogException(exception);
					}
				}
				flag = true;
			}
			else if (rpc == 1365277306 && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log(string.Concat("SV_RPCMessage: ", player, " - DestroyMessageFromHost "));
				}
				using (timeWarning1 = TimeWarning.New("DestroyMessageFromHost", 0.1f))
				{
					using (timeWarning2 = TimeWarning.New("Conditions", 0.1f))
					{
						if (!BaseEntity.RPC_Server.IsVisible.Test("DestroyMessageFromHost", this, player, 3f))
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
							this.DestroyMessageFromHost(rPCMessage);
						}
					}
					catch (Exception exception1)
					{
						player.Kick("RPC Error in DestroyMessageFromHost");
						Debug.LogException(exception1);
					}
				}
				flag = true;
			}
			else if (rpc == -1827114908 && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log(string.Concat("SV_RPCMessage: ", player, " - GetSnapshotFromClient "));
				}
				using (timeWarning1 = TimeWarning.New("GetSnapshotFromClient", 0.1f))
				{
					using (timeWarning2 = TimeWarning.New("Conditions", 0.1f))
					{
						if (!BaseEntity.RPC_Server.IsVisible.Test("GetSnapshotFromClient", this, player, 3f))
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
							this.GetSnapshotFromClient(rPCMessage);
						}
					}
					catch (Exception exception2)
					{
						player.Kick("RPC Error in GetSnapshotFromClient");
						Debug.LogException(exception2);
					}
				}
				flag = true;
			}
			else if (rpc != -1304095661 || !(player != null))
			{
				return base.OnRpcMessage(player, rpc, msg);
			}
			else
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log(string.Concat("SV_RPCMessage: ", player, " - RequestAddScore "));
				}
				using (timeWarning1 = TimeWarning.New("RequestAddScore", 0.1f))
				{
					using (timeWarning2 = TimeWarning.New("Conditions", 0.1f))
					{
						if (!BaseEntity.RPC_Server.IsVisible.Test("RequestAddScore", this, player, 3f))
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
							this.RequestAddScore(rPCMessage);
						}
					}
					catch (Exception exception3)
					{
						player.Kick("RPC Error in RequestAddScore");
						Debug.LogException(exception3);
					}
				}
				flag = true;
			}
		}
		return flag;
	}

	public override void PlayerDismounted(BasePlayer player, BaseMountable seat)
	{
		base.PlayerDismounted(player, seat);
		base.ClientRPCPlayer(null, player, "EndHosting");
		base.SetFlag(BaseEntity.Flags.Reserved7, false, true, true);
		if (!base.AnyMounted())
		{
			this.NearbyClientMessage("NoHost");
		}
	}

	public override void PlayerMounted(BasePlayer player, BaseMountable seat)
	{
		base.PlayerMounted(player, seat);
		base.ClientRPCPlayer(null, player, "BeginHosting");
		base.SetFlag(BaseEntity.Flags.Reserved7, true, true, true);
	}

	public override void PlayerServerInput(InputState inputState, BasePlayer player)
	{
	}

	[IsVisible(3f)]
	[RPC_Server]
	public void RequestAddScore(BaseEntity.RPCMessage msg)
	{
		BasePlayer basePlayer = msg.player;
		if (basePlayer == null)
		{
			return;
		}
		if (!basePlayer.isMounted || basePlayer.GetMountedVehicle() != this)
		{
			return;
		}
		this.AddScore(basePlayer, msg.read.Int32());
	}

	public override void Save(BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		info.msg.arcadeMachine = Facepunch.Pool.Get<ArcadeMachine>();
		info.msg.arcadeMachine.scores = Facepunch.Pool.GetList<ArcadeMachine.ScoreEntry>();
		for (int i = 0; i < this.scores.Count; i++)
		{
			ArcadeMachine.ScoreEntry item = Facepunch.Pool.Get<ArcadeMachine.ScoreEntry>();
			item.displayName = this.scores[i].displayName;
			item.playerID = this.scores[i].playerID;
			item.score = this.scores[i].score;
			info.msg.arcadeMachine.scores.Add(item);
		}
	}

	public class ScoreEntry
	{
		public ulong playerID;

		public int score;

		public string displayName;

		public ScoreEntry()
		{
		}
	}
}