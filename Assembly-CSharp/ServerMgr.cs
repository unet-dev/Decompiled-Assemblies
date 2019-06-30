using ConVar;
using EasyAntiCheat.Server.Cerberus;
using EasyAntiCheat.Server.Hydra;
using Facepunch;
using Ionic.Crc;
using Network;
using Network.Visibility;
using Oxide.Core;
using ProtoBuf;
using Rust;
using Steamworks;
using Steamworks.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Runtime.CompilerServices;
using UnityEngine;

public class ServerMgr : SingletonComponent<ServerMgr>, IServerCallback
{
	private ConnectionAuth auth;

	private bool runFrameUpdate;

	private bool useQueryPort;

	public UserPersistance persistance;

	private string _AssemblyHash;

	private IEnumerator restartCoroutine;

	public ConnectionQueue connectionQueue = new ConnectionQueue();

	private Stopwatch queryTimer = Stopwatch.StartNew();

	private Dictionary<uint, int> unconnectedQueries = new Dictionary<uint, int>();

	private Stopwatch queriesPerSeconTimer = Stopwatch.StartNew();

	private int NumQueriesLastSecond;

	private MemoryStream queryBuffer = new MemoryStream();

	private string AssemblyHash
	{
		get
		{
			if (this._AssemblyHash == null)
			{
				byte[] numArray = File.ReadAllBytes(typeof(ServerMgr).Assembly.Location);
				CRC32 cRC32 = new CRC32();
				cRC32.SlurpBlock(numArray, 0, (int)numArray.Length);
				this._AssemblyHash = cRC32.Crc32Result.ToString("x");
			}
			return this._AssemblyHash;
		}
	}

	public static int AvailableSlots
	{
		get
		{
			return ConVar.Server.maxplayers - BasePlayer.activePlayerList.Count<BasePlayer>();
		}
	}

	public bool Restarting
	{
		get
		{
			return this.restartCoroutine != null;
		}
	}

	public ServerMgr()
	{
	}

	private void ClientReady(Message packet)
	{
		packet.connection.decryptIncoming = true;
		using (ClientReady clientReady = ClientReady.Deserialize(packet.read))
		{
			foreach (ClientReady.ClientInfo clientInfo in clientReady.clientInfo)
			{
				packet.connection.info.Set(clientInfo.name, clientInfo.@value);
			}
			this.connectionQueue.JoinedGame(packet.connection);
			Interface.CallHook("OnPlayerConnected", packet);
			using (TimeWarning timeWarning = TimeWarning.New("ClientReady", 0.1f))
			{
				using (TimeWarning timeWarning1 = TimeWarning.New("SpawnPlayerSleeping", 0.1f))
				{
					if (this.SpawnPlayerSleeping(packet.connection))
					{
						return;
					}
				}
				using (timeWarning1 = TimeWarning.New("SpawnNewPlayer", 0.1f))
				{
					this.SpawnNewPlayer(packet.connection);
				}
			}
		}
	}

	private void CloseConnection()
	{
		if (this.persistance != null)
		{
			this.persistance.Dispose();
			this.persistance = null;
		}
		EACServer.DoShutdown();
		Network.Net.sv.callbackHandler = null;
		using (TimeWarning timeWarning = TimeWarning.New("sv.Stop", 0.1f))
		{
			Network.Net.sv.Stop("Shutting Down");
		}
		using (timeWarning = TimeWarning.New("RCon.Shutdown", 0.1f))
		{
			RCon.Shutdown();
		}
		using (timeWarning = TimeWarning.New("Steamworks.GameServer.Shutdown", 0.1f))
		{
			if (SteamServer.IsValid)
			{
				UnityEngine.Debug.Log("Steamworks Shutting Down");
				SteamServer.Shutdown();
				UnityEngine.Debug.Log("Okay");
			}
		}
	}

	private void CreateImportantEntities()
	{
		this.CreateImportantEntity<EnvSync>("assets/bundled/prefabs/system/net_env.prefab");
		this.CreateImportantEntity<CommunityEntity>("assets/bundled/prefabs/system/server/community.prefab");
		this.CreateImportantEntity<ResourceDepositManager>("assets/bundled/prefabs/system/server/resourcedepositmanager.prefab");
		this.CreateImportantEntity<RelationshipManager>("assets/bundled/prefabs/system/server/relationship_manager.prefab");
	}

	private void CreateImportantEntity<T>(string prefabName)
	where T : BaseEntity
	{
		if (BaseNetworkable.serverEntities.Any<BaseNetworkable>((BaseNetworkable x) => x is T))
		{
			return;
		}
		UnityEngine.Debug.LogWarning(string.Concat("Missing ", typeof(T).Name, " - creating"));
		GameManager gameManager = GameManager.server;
		UnityEngine.Vector3 vector3 = new UnityEngine.Vector3();
		UnityEngine.Quaternion quaternion = new UnityEngine.Quaternion();
		BaseEntity baseEntity = gameManager.CreateEntity(prefabName, vector3, quaternion, true);
		if (baseEntity == null)
		{
			UnityEngine.Debug.LogWarning("Couldn't create");
			return;
		}
		baseEntity.Spawn();
	}

	private void DoHeartbeat()
	{
		ItemManager.Heartbeat();
	}

	private void DoTick()
	{
		if (SteamServer.IsValid)
		{
			Interface.CallHook("OnTick");
			SteamServer.RunCallbacks();
		}
		RCon.Update();
		for (int i = 0; i < Network.Net.sv.connections.Count; i++)
		{
			Network.Connection item = Network.Net.sv.connections[i];
			if (!item.isAuthenticated && item.GetSecondsConnected() >= (float)ConVar.Server.authtimeout)
			{
				Network.Net.sv.Kick(item, "Authentication Timed Out");
			}
		}
	}

	private void EACUpdate()
	{
		EACServer.DoUpdate();
	}

	public static BasePlayer.SpawnPoint FindSpawnPoint()
	{
		RaycastHit raycastHit;
		if (SingletonComponent<SpawnHandler>.Instance != null)
		{
			BasePlayer.SpawnPoint spawnPoint = SpawnHandler.GetSpawnPoint();
			if (spawnPoint != null)
			{
				return spawnPoint;
			}
		}
		BasePlayer.SpawnPoint spawnPoint1 = new BasePlayer.SpawnPoint();
		GameObject[] gameObjectArray = GameObject.FindGameObjectsWithTag("spawnpoint");
		if (gameObjectArray.Length == 0)
		{
			UnityEngine.Debug.Log("Couldn't find an appropriate spawnpoint for the player - so spawning at camera");
			if (MainCamera.mainCamera != null)
			{
				spawnPoint1.pos = MainCamera.mainCamera.transform.position;
				spawnPoint1.rot = MainCamera.mainCamera.transform.rotation;
			}
		}
		else
		{
			GameObject gameObject = gameObjectArray[UnityEngine.Random.Range(0, (int)gameObjectArray.Length)];
			spawnPoint1.pos = gameObject.transform.position;
			spawnPoint1.rot = gameObject.transform.rotation;
		}
		if (UnityEngine.Physics.Raycast(new Ray(spawnPoint1.pos, UnityEngine.Vector3.down), out raycastHit, 32f, 1537286401))
		{
			spawnPoint1.pos = raycastHit.point;
		}
		return spawnPoint1;
	}

	public static string GamemodeDesc()
	{
		return "The default Rust survival gamemode";
	}

	public static string GamemodeImage()
	{
		return "https://files.facepunch.com/garry/3c96c182-ab06-40ff-b66e-f4a510053ca4.png";
	}

	public static string GamemodeName()
	{
		return "rust";
	}

	public static string GamemodeTitle()
	{
		return "Rust: Survival Mode";
	}

	public static string GamemodeUrl()
	{
		return "https://rust.facepunch.com";
	}

	public void Initialize(bool loadSave = true, string saveFile = "", bool allowOutOfDateSaves = false, bool skipInitialSpawn = false)
	{
		this.persistance = new UserPersistance(ConVar.Server.rootFolder);
		this.SpawnMapEntities();
		if (SingletonComponent<SpawnHandler>.Instance)
		{
			using (TimeWarning timeWarning = TimeWarning.New("SpawnHandler.UpdateDistributions", 0.1f))
			{
				SingletonComponent<SpawnHandler>.Instance.UpdateDistributions();
			}
		}
		if (loadSave)
		{
			skipInitialSpawn = SaveRestore.Load(saveFile, allowOutOfDateSaves);
		}
		if (SingletonComponent<SpawnHandler>.Instance)
		{
			if (!skipInitialSpawn)
			{
				using (timeWarning = TimeWarning.New("SpawnHandler.InitialSpawn", (long)200))
				{
					SingletonComponent<SpawnHandler>.Instance.InitialSpawn();
				}
			}
			using (timeWarning = TimeWarning.New("SpawnHandler.StartSpawnTick", (long)200))
			{
				SingletonComponent<SpawnHandler>.Instance.StartSpawnTick();
			}
		}
		this.CreateImportantEntities();
		this.auth = base.GetComponent<ConnectionAuth>();
	}

	public void JoinGame(Network.Connection connection)
	{
		using (Approval url = Facepunch.Pool.Get<Approval>())
		{
			uint num = (uint)ConVar.Server.encryption;
			if (num > 1 && connection.os == "editor" && DeveloperList.Contains(connection.ownerid))
			{
				num = 1;
			}
			url.level = UnityEngine.Application.loadedLevelName;
			url.levelUrl = World.Url;
			url.levelSeed = World.Seed;
			url.levelSize = World.Size;
			url.checksum = World.Checksum;
			url.hostname = ConVar.Server.hostname;
			url.official = ConVar.Server.official;
			url.encryption = num;
			if (Network.Net.sv.write.Start())
			{
				Network.Net.sv.write.PacketID(Message.Type.Approved);
				url.WriteToStream(Network.Net.sv.write);
				Network.Net.sv.write.Send(new SendInfo(connection));
			}
			connection.encryptionLevel = num;
			connection.encryptOutgoing = true;
		}
		connection.connected = true;
	}

	private void Log(Exception e)
	{
		if (ConVar.Global.developer > 0)
		{
			UnityEngine.Debug.LogException(e);
		}
	}

	private void OnApplicationQuit()
	{
		Rust.Application.isQuitting = true;
		this.CloseConnection();
	}

	private void OnDisable()
	{
		if (Rust.Application.isQuitting)
		{
			return;
		}
		this.CloseConnection();
	}

	public void OnDisconnected(string strReason, Network.Connection connection)
	{
		this.connectionQueue.RemoveConnection(connection);
		ConnectionAuth.OnDisconnect(connection);
		SteamServer.EndSession(connection.userid);
		EACServer.OnLeaveGame(connection);
		BasePlayer basePlayer = connection.player as BasePlayer;
		if (basePlayer)
		{
			Interface.CallHook("OnPlayerDisconnected", basePlayer, strReason);
			basePlayer.OnDisconnected();
		}
	}

	public static void OnEnterVisibility(Network.Connection connection, Group group)
	{
		if (!Network.Net.sv.IsConnected())
		{
			return;
		}
		if (Network.Net.sv.write.Start())
		{
			Network.Net.sv.write.PacketID(Message.Type.GroupEnter);
			Network.Net.sv.write.GroupID(group.ID);
			Network.Net.sv.write.Send(new SendInfo(connection));
		}
	}

	private void OnGiveUserInformation(Message packet)
	{
		if (packet.connection.state != Network.Connection.State.Unconnected)
		{
			Network.Net.sv.Kick(packet.connection, "Invalid connection state");
			return;
		}
		packet.connection.state = Network.Connection.State.Connecting;
		if (packet.read.UInt8() != 228)
		{
			Network.Net.sv.Kick(packet.connection, "Invalid Connection Protocol");
			return;
		}
		packet.connection.userid = packet.read.UInt64();
		packet.connection.protocol = packet.read.UInt32();
		packet.connection.os = packet.read.String();
		packet.connection.username = packet.read.String();
		Interface.CallHook("OnClientAuth", packet.connection);
		if (string.IsNullOrEmpty(packet.connection.os))
		{
			throw new Exception("Invalid OS");
		}
		if (string.IsNullOrEmpty(packet.connection.username))
		{
			Network.Net.sv.Kick(packet.connection, "Invalid Username");
			return;
		}
		packet.connection.username = packet.connection.username.Replace('\n', ' ').Replace('\r', ' ').Replace('\t', ' ').Trim();
		if (string.IsNullOrEmpty(packet.connection.username))
		{
			Network.Net.sv.Kick(packet.connection, "Invalid Username");
			return;
		}
		string empty = string.Empty;
		string str = ConVar.Server.branch;
		if (packet.read.Unread >= 4)
		{
			empty = packet.read.String();
		}
		if (str != string.Empty && str != empty)
		{
			DebugEx.Log(string.Concat(new object[] { "Kicking ", packet.connection, " - their branch is '", empty, "' not '", str, "'" }), StackTraceLogType.None);
			Network.Net.sv.Kick(packet.connection, string.Concat("Wrong Steam Beta: Requires '", str, "' branch!"));
			return;
		}
		if (packet.connection.protocol > 2177)
		{
			DebugEx.Log(string.Concat(new object[] { "Kicking ", packet.connection, " - their protocol is ", packet.connection.protocol, " not ", 2177 }), StackTraceLogType.None);
			Network.Net.sv.Kick(packet.connection, "Wrong Connection Protocol: Server update required!");
			return;
		}
		if (packet.connection.protocol >= 2177)
		{
			packet.connection.token = packet.read.BytesWithSize();
			if (packet.connection.token != null && (int)packet.connection.token.Length >= 1)
			{
				this.auth.OnNewConnection(packet.connection);
				return;
			}
			Network.Net.sv.Kick(packet.connection, "Invalid Token");
			return;
		}
		DebugEx.Log(string.Concat(new object[] { "Kicking ", packet.connection, " - their protocol is ", packet.connection.protocol, " not ", 2177 }), StackTraceLogType.None);
		Network.Net.sv.Kick(packet.connection, "Wrong Connection Protocol: Client update required!");
	}

	private void OnInventoryDefinitionsUpdated()
	{
		ItemManager.InvalidateWorkshopSkinCache();
	}

	public static void OnLeaveVisibility(Network.Connection connection, Group group)
	{
		if (!Network.Net.sv.IsConnected())
		{
			return;
		}
		if (Network.Net.sv.write.Start())
		{
			Network.Net.sv.write.PacketID(Message.Type.GroupLeave);
			Network.Net.sv.write.GroupID(group.ID);
			Network.Net.sv.write.Send(new SendInfo(connection));
		}
		if (Network.Net.sv.write.Start())
		{
			Network.Net.sv.write.PacketID(Message.Type.GroupDestroy);
			Network.Net.sv.write.GroupID(group.ID);
			Network.Net.sv.write.Send(new SendInfo(connection));
		}
	}

	public void OnNetworkMessage(Message packet)
	{
		Message.Type type = packet.type;
		if (type == Message.Type.Ready)
		{
			if (!packet.connection.isAuthenticated)
			{
				return;
			}
			if (packet.connection.GetPacketsPerSecond(packet.type) > (long)1)
			{
				Network.Net.sv.Kick(packet.connection, "Packet Flooding: Client Ready");
				return;
			}
			using (TimeWarning timeWarning = TimeWarning.New("ClientReady", (long)20))
			{
				try
				{
					this.ClientReady(packet);
				}
				catch (Exception exception)
				{
					this.Log(exception);
					Network.Net.sv.Kick(packet.connection, "Invalid Packet: Client Ready");
				}
			}
			packet.connection.AddPacketsPerSecond(packet.type);
			return;
		}
		switch (type)
		{
			case Message.Type.RPCMessage:
			{
				if (!packet.connection.isAuthenticated)
				{
					return;
				}
				if (packet.connection.GetPacketsPerSecond(packet.type) > (long)ConVar.Server.maxrpcspersecond)
				{
					Network.Net.sv.Kick(packet.connection, "Paket Flooding: RPC Message");
					return;
				}
				using (timeWarning = TimeWarning.New("OnRPCMessage", (long)20))
				{
					try
					{
						this.OnRPCMessage(packet);
					}
					catch (Exception exception1)
					{
						this.Log(exception1);
						Network.Net.sv.Kick(packet.connection, "Invalid Packet: RPC Message");
					}
				}
				packet.connection.AddPacketsPerSecond(packet.type);
				return;
			}
			case Message.Type.EntityPosition:
			case Message.Type.ConsoleMessage:
			case Message.Type.Effect:
			{
				this.ProcessUnhandledPacket(packet);
				return;
			}
			case Message.Type.ConsoleCommand:
			{
				if (!packet.connection.isAuthenticated)
				{
					return;
				}
				if (packet.connection.GetPacketsPerSecond(packet.type) > (long)ConVar.Server.maxcommandspersecond)
				{
					Network.Net.sv.Kick(packet.connection, "Packet Flooding: Client Command");
					return;
				}
				using (timeWarning = TimeWarning.New("OnClientCommand", (long)20))
				{
					try
					{
						ConsoleNetwork.OnClientCommand(packet);
					}
					catch (Exception exception2)
					{
						this.Log(exception2);
						Network.Net.sv.Kick(packet.connection, "Invalid Packet: Client Command");
					}
				}
				packet.connection.AddPacketsPerSecond(packet.type);
				return;
			}
			case Message.Type.DisconnectReason:
			{
				if (!packet.connection.isAuthenticated)
				{
					return;
				}
				if (packet.connection.GetPacketsPerSecond(packet.type) > (long)1)
				{
					Network.Net.sv.Kick(packet.connection, "Packet Flooding: Disconnect Reason");
					return;
				}
				using (timeWarning = TimeWarning.New("ReadDisconnectReason", (long)20))
				{
					try
					{
						this.ReadDisconnectReason(packet);
					}
					catch (Exception exception3)
					{
						this.Log(exception3);
						Network.Net.sv.Kick(packet.connection, "Invalid Packet: Disconnect Reason");
					}
				}
				packet.connection.AddPacketsPerSecond(packet.type);
				return;
			}
			case Message.Type.Tick:
			{
				if (!packet.connection.isAuthenticated)
				{
					return;
				}
				if (packet.connection.GetPacketsPerSecond(packet.type) > (long)ConVar.Server.maxtickspersecond)
				{
					Network.Net.sv.Kick(packet.connection, "Packet Flooding: Player Tick");
					return;
				}
				using (timeWarning = TimeWarning.New("OnPlayerTick", (long)20))
				{
					try
					{
						this.OnPlayerTick(packet);
					}
					catch (Exception exception4)
					{
						this.Log(exception4);
						Network.Net.sv.Kick(packet.connection, "Invalid Packet: Player Tick");
					}
				}
				packet.connection.AddPacketsPerSecond(packet.type);
				return;
			}
			default:
			{
				switch (type)
				{
					case Message.Type.GiveUserInformation:
					{
						if (packet.connection.GetPacketsPerSecond(packet.type) > (long)1)
						{
							Network.Net.sv.Kick(packet.connection, "Packet Flooding: User Information");
							return;
						}
						using (timeWarning = TimeWarning.New("GiveUserInformation", (long)20))
						{
							try
							{
								this.OnGiveUserInformation(packet);
							}
							catch (Exception exception5)
							{
								this.Log(exception5);
								Network.Net.sv.Kick(packet.connection, "Invalid Packet: User Information");
							}
						}
						packet.connection.AddPacketsPerSecond(packet.type);
						return;
					}
					case Message.Type.GroupEnter:
					case Message.Type.GroupLeave:
					{
						this.ProcessUnhandledPacket(packet);
						return;
					}
					case Message.Type.VoiceData:
					{
						if (!packet.connection.isAuthenticated)
						{
							return;
						}
						if (packet.connection.GetPacketsPerSecond(packet.type) > (long)100)
						{
							Network.Net.sv.Kick(packet.connection, "Packet Flooding: Disconnect Reason");
							return;
						}
						using (timeWarning = TimeWarning.New("OnPlayerVoice", (long)20))
						{
							try
							{
								this.OnPlayerVoice(packet);
							}
							catch (Exception exception6)
							{
								this.Log(exception6);
								Network.Net.sv.Kick(packet.connection, "Invalid Packet: Player Voice");
							}
						}
						packet.connection.AddPacketsPerSecond(packet.type);
						return;
					}
					case Message.Type.EAC:
					{
						using (timeWarning = TimeWarning.New("OnEACMessage", (long)20))
						{
							try
							{
								EACServer.OnMessageReceived(packet);
							}
							catch (Exception exception7)
							{
								this.Log(exception7);
								Network.Net.sv.Kick(packet.connection, "Invalid Packet: EAC");
							}
						}
						return;
					}
					default:
					{
						this.ProcessUnhandledPacket(packet);
						return;
					}
				}
				break;
			}
		}
	}

	private void OnPlayerTick(Message packet)
	{
		BasePlayer basePlayer = packet.Player();
		if (basePlayer == null)
		{
			return;
		}
		basePlayer.OnReceivedTick(packet.read);
	}

	private void OnPlayerVoice(Message packet)
	{
		BasePlayer basePlayer = packet.Player();
		if (basePlayer == null)
		{
			return;
		}
		basePlayer.OnReceivedVoice(packet.read.BytesWithSize());
	}

	private void OnRPCMessage(Message packet)
	{
		uint num = packet.read.UInt32();
		uint num1 = packet.read.UInt32();
		BaseEntity baseEntity = BaseNetworkable.serverEntities.Find(num) as BaseEntity;
		if (baseEntity == null)
		{
			return;
		}
		baseEntity.SV_RPCMessage(num1, packet);
	}

	private void OnSteamConnected()
	{
		UnityEngine.Debug.Log("SteamServer Connected");
		base.Invoke("UpdateServerInformation", 1f);
	}

	private void OnSteamConnectionFailure(Result result, bool stilltrying)
	{
		UnityEngine.Debug.LogWarning(string.Format("SteamServer Connection Failure ({0})", result));
	}

	private void OnSteamServersDisconnected(Result result)
	{
		UnityEngine.Debug.LogWarning(string.Format("SteamServer Disconnected ({0})", result));
	}

	public bool OnUnconnectedMessage(int type, NetRead read, uint ip, int port)
	{
		if (this.useQueryPort)
		{
			return false;
		}
		if (type != 255)
		{
			return false;
		}
		if (this.queriesPerSeconTimer.Elapsed.TotalSeconds > 1)
		{
			this.queriesPerSeconTimer.Reset();
			this.queriesPerSeconTimer.Start();
			this.NumQueriesLastSecond = 0;
		}
		if (this.NumQueriesLastSecond > ConVar.Server.queriesPerSecond)
		{
			return false;
		}
		if (read.UInt8() != 255)
		{
			return false;
		}
		if (read.UInt8() != 255)
		{
			return false;
		}
		if (read.UInt8() != 255)
		{
			return false;
		}
		if (this.queryTimer.Elapsed.TotalSeconds > 60)
		{
			this.queryTimer.Reset();
			this.queryTimer.Start();
			this.unconnectedQueries.Clear();
		}
		if (!this.unconnectedQueries.ContainsKey(ip))
		{
			this.unconnectedQueries.Add(ip, 0);
		}
		int item = this.unconnectedQueries[ip] + 1;
		this.unconnectedQueries[ip] = item;
		if (item > ConVar.Server.ipQueriesPerMin)
		{
			return true;
		}
		this.NumQueriesLastSecond++;
		read.Position = (long)0;
		int unread = read.Unread;
		if (unread > 4096)
		{
			return true;
		}
		if (this.queryBuffer.Capacity < unread)
		{
			this.queryBuffer.Capacity = unread;
		}
		int num = read.Read(this.queryBuffer.GetBuffer(), 0, unread);
		SteamServer.HandleIncomingPacket(this.queryBuffer.GetBuffer(), num, ip, (ushort)port);
		return true;
	}

	private void OnValidateAuthTicketResponse(Steamworks.SteamId SteamId, Steamworks.SteamId OwnerId, AuthResponse Status)
	{
		if (Auth_Steam.ValidateConnecting(SteamId, OwnerId, Status))
		{
			return;
		}
		Network.Connection str = Network.Net.sv.connections.FirstOrDefault<Network.Connection>((Network.Connection x) => x.userid == SteamId);
		if (str == null)
		{
			UnityEngine.Debug.LogWarning(string.Format("Steam gave us a {0} ticket response for unconnected id {1}", Status, SteamId));
			return;
		}
		if (Status == AuthResponse.OK)
		{
			UnityEngine.Debug.LogWarning(string.Format("Steam gave us a 'ok' ticket response for already connected id {0}", SteamId));
			return;
		}
		if (Status == AuthResponse.VACCheckTimedOut)
		{
			return;
		}
		str.authStatus = Status.ToString();
		Network.Net.sv.Kick(str, string.Concat("Steam: ", Status.ToString()));
	}

	public void OpenConnection()
	{
		this.useQueryPort = (ConVar.Server.queryport <= 0 ? false : ConVar.Server.queryport != ConVar.Server.port);
		Network.Net.sv.ip = ConVar.Server.ip;
		Network.Net.sv.port = ConVar.Server.port;
		if (!Network.Net.sv.Start())
		{
			UnityEngine.Debug.LogWarning("Couldn't Start Server.");
			return;
		}
		this.StartSteamServer();
		Network.Net.sv.callbackHandler = this;
		Network.Net.sv.cryptography = new NetworkCryptographyServer();
		EACServer.DoStartup();
		base.InvokeRepeating("EACUpdate", 1f, 1f);
		base.InvokeRepeating("DoTick", 1f, 1f / (float)ConVar.Server.tickrate);
		base.InvokeRepeating("DoHeartbeat", 1f, 1f);
		this.runFrameUpdate = true;
		Interface.CallHook("IOnServerInitialized");
	}

	public void ProcessUnhandledPacket(Message packet)
	{
		if (ConVar.Global.developer > 0)
		{
			UnityEngine.Debug.LogWarning(string.Concat("[SERVER][UNHANDLED] ", packet.type));
		}
		Network.Net.sv.Kick(packet.connection, "Sent Unhandled Message");
	}

	public void ReadDisconnectReason(Message packet)
	{
		string str = packet.read.String();
		string str1 = packet.connection.ToString();
		if (!string.IsNullOrEmpty(str) && !string.IsNullOrEmpty(str1))
		{
			DebugEx.Log(string.Concat(str1, " disconnecting: ", str), StackTraceLogType.None);
		}
	}

	public static void RestartServer(string strNotice, int iSeconds)
	{
		if (SingletonComponent<ServerMgr>.Instance == null)
		{
			return;
		}
		if (SingletonComponent<ServerMgr>.Instance.restartCoroutine != null)
		{
			ConsoleNetwork.BroadcastToAllClients("chat.add", new object[] { 0, "<color=#fff>SERVER</color> Restart interrupted!" });
			SingletonComponent<ServerMgr>.Instance.StopCoroutine(SingletonComponent<ServerMgr>.Instance.restartCoroutine);
			SingletonComponent<ServerMgr>.Instance.restartCoroutine = null;
		}
		SingletonComponent<ServerMgr>.Instance.restartCoroutine = SingletonComponent<ServerMgr>.Instance.ServerRestartWarning(strNotice, iSeconds);
		SingletonComponent<ServerMgr>.Instance.StartCoroutine(SingletonComponent<ServerMgr>.Instance.restartCoroutine);
		SingletonComponent<ServerMgr>.Instance.UpdateServerInformation();
	}

	private IEnumerator ServerRestartWarning(string info, int iSeconds)
	{
		int j = 0;
		if (iSeconds < 0)
		{
			yield break;
		}
		if (!string.IsNullOrEmpty(info))
		{
			object[] objArray = new object[] { 0, string.Concat("<color=#fff>SERVER</color> Restarting: ", info) };
			ConsoleNetwork.BroadcastToAllClients("chat.add", objArray);
		}
		for (int i = iSeconds; i > 0; i = j - 1)
		{
			if (i == iSeconds || i % 60 == 0 || i < 300 && i % 30 == 0 || i < 60 && i % 10 == 0 || i < 10)
			{
				object[] objArray1 = new object[] { 0, string.Format("<color=#fff>SERVER</color> Restarting in {0} seconds!", i) };
				ConsoleNetwork.BroadcastToAllClients("chat.add", objArray1);
				UnityEngine.Debug.Log(string.Format("Restarting in {0} seconds", i));
			}
			yield return CoroutineEx.waitForSeconds(1f);
			j = i;
		}
		object[] objArray2 = new object[] { 0, "<color=#fff>SERVER</color> Restarting" };
		ConsoleNetwork.BroadcastToAllClients("chat.add", objArray2);
		yield return CoroutineEx.waitForSeconds(2f);
		BasePlayer[] array = BasePlayer.activePlayerList.ToArray();
		for (j = 0; j < (int)array.Length; j++)
		{
			array[j].Kick("Server Restarting");
		}
		yield return CoroutineEx.waitForSeconds(1f);
		ConsoleSystem.Run(ConsoleSystem.Option.Server, "quit", Array.Empty<object>());
	}

	internal void Shutdown()
	{
		Interface.CallHook("OnServerShutdown");
		BasePlayer[] array = BasePlayer.activePlayerList.ToArray();
		for (int i = 0; i < (int)array.Length; i++)
		{
			array[i].Kick("Server Shutting Down");
		}
		ConsoleSystem.Run(ConsoleSystem.Option.Server, "server.save", Array.Empty<object>());
		ConsoleSystem.Run(ConsoleSystem.Option.Server, "server.writecfg", Array.Empty<object>());
	}

	internal void SpawnMapEntities()
	{
		int i;
		PrefabPreProcess prefabPreProcess = new PrefabPreProcess(false, true, false);
		BaseEntity[] baseEntityArray = UnityEngine.Object.FindObjectsOfType<BaseEntity>();
		BaseEntity[] baseEntityArray1 = baseEntityArray;
		for (i = 0; i < (int)baseEntityArray1.Length; i++)
		{
			BaseEntity baseEntity = baseEntityArray1[i];
			if (prefabPreProcess.NeedsProcessing(baseEntity.gameObject))
			{
				prefabPreProcess.ProcessObject(null, baseEntity.gameObject, false);
			}
			baseEntity.SpawnAsMapEntity();
		}
		DebugEx.Log(string.Format("Map Spawned {0} entities", (int)baseEntityArray.Length), StackTraceLogType.None);
		baseEntityArray1 = baseEntityArray;
		for (i = 0; i < (int)baseEntityArray1.Length; i++)
		{
			BaseEntity baseEntity1 = baseEntityArray1[i];
			if (baseEntity1 != null)
			{
				baseEntity1.PostMapEntitySpawn();
			}
		}
	}

	private void SpawnNewPlayer(Network.Connection connection)
	{
		BasePlayer.SpawnPoint spawnPoint = ServerMgr.FindSpawnPoint();
		BasePlayer player = GameManager.server.CreateEntity("assets/prefabs/player/player.prefab", spawnPoint.pos, spawnPoint.rot, true).ToPlayer();
		if (Interface.CallHook("OnPlayerSpawn", player) != null)
		{
			return;
		}
		player.health = 0f;
		player.lifestate = BaseCombatEntity.LifeState.Dead;
		player.ResetLifeStateOnSpawn = false;
		player.limitNetworking = true;
		player.Spawn();
		player.limitNetworking = false;
		player.PlayerInit(connection);
		if (SleepingBag.FindForPlayer(player.userID, true).Length != 0 || player.hasPreviousLife)
		{
			player.SendRespawnOptions();
		}
		else
		{
			player.Respawn();
		}
		DebugEx.Log(string.Concat(new object[] { player.net.connection.ToString(), " joined [", player.net.connection.os, "/", player.net.connection.ownerid, "]" }), StackTraceLogType.None);
	}

	private bool SpawnPlayerSleeping(Network.Connection connection)
	{
		BasePlayer basePlayer = BasePlayer.FindSleeping(connection.userid);
		if (basePlayer == null)
		{
			return false;
		}
		if (!basePlayer.IsSleeping())
		{
			UnityEngine.Debug.LogWarning("Player spawning into sleeper that isn't sleeping!");
			basePlayer.Kill(BaseNetworkable.DestroyMode.None);
			return false;
		}
		basePlayer.PlayerInit(connection);
		basePlayer.inventory.SendSnapshot();
		DebugEx.Log(string.Concat(new object[] { basePlayer.net.connection.ToString(), " joined [", basePlayer.net.connection.os, "/", basePlayer.net.connection.ownerid, "]" }), StackTraceLogType.None);
		return true;
	}

	private void StartSteamServer()
	{
		if (SteamServer.IsValid)
		{
			return;
		}
		IPAddress pAddress = null;
		if (!string.IsNullOrEmpty(ConVar.Server.ip))
		{
			pAddress = IPAddress.Parse(ConVar.Server.ip);
		}
		SteamServerInit steamServerInit = new SteamServerInit("rust", "Rust")
		{
			IpAddress = pAddress,
			GamePort = (ushort)Network.Net.sv.port,
			Secure = ConVar.Server.secure,
			VersionString = 2177.ToString()
		};
		if (!this.useQueryPort)
		{
			steamServerInit = steamServerInit.WithQueryShareGamePort();
		}
		else
		{
			steamServerInit.QueryPort = (ushort)ConVar.Server.queryport;
		}
		SteamServer.OnCallbackException = (Exception e) => {
			UnityEngine.Debug.LogError(e.Message);
			UnityEngine.Debug.LogError(e.StackTrace);
		};
		try
		{
			SteamServer.Init(Rust.Defines.appID, steamServerInit);
		}
		catch (Exception exception1)
		{
			Exception exception = exception1;
			UnityEngine.Debug.LogWarning(string.Concat("Couldn't initialize Steam Server (", exception.Message, ")"));
			UnityEngine.Application.Quit();
			return;
		}
		SteamServer.OnValidateAuthTicketResponse += new Action<SteamId, SteamId, AuthResponse>(this.OnValidateAuthTicketResponse);
		SteamServer.OnSteamServerConnectFailure += new Action<Result, bool>(this.OnSteamConnectionFailure);
		SteamServer.OnSteamServersDisconnected += new Action<Result>(this.OnSteamServersDisconnected);
		SteamServer.OnSteamServersConnected += new Action(this.OnSteamConnected);
		Steamworks.SteamInventory.OnDefinitionsUpdated += new Action(this.OnInventoryDefinitionsUpdated);
		SteamServer.DedicatedServer = true;
		SteamServer.LogOnAnonymous();
		base.InvokeRepeating("UpdateServerInformation", 2f, 30f);
		base.InvokeRepeating("UpdateItemDefinitions", 10f, 3600f);
		DebugEx.Log("SteamServer Initialized", StackTraceLogType.None);
	}

	private void SteamQueryResponse()
	{
		OutgoingPacket outgoingPacket;
		if (!SteamServer.IsValid)
		{
			return;
		}
		using (TimeWarning timeWarning = TimeWarning.New("SteamGameServer.GetNextOutgoingPacket", 0.1f))
		{
			while (SteamServer.GetOutgoingPacket(out outgoingPacket))
			{
				Network.Net.sv.SendUnconnected(outgoingPacket.Address, outgoingPacket.Port, outgoingPacket.Data, outgoingPacket.Size);
			}
		}
	}

	private void Update()
	{
		if (!this.runFrameUpdate)
		{
			return;
		}
		using (TimeWarning timeWarning = TimeWarning.New("ServerMgr.Update", (long)500))
		{
			if (EACServer.playerTracker != null)
			{
				EACServer.playerTracker.BeginFrame();
			}
			try
			{
				using (TimeWarning timeWarning1 = TimeWarning.New("Net.sv.Cycle", (long)100))
				{
					Network.Net.sv.Cycle();
				}
			}
			catch (Exception exception)
			{
				UnityEngine.Debug.LogWarning("Server Exception: Network Cycle");
				UnityEngine.Debug.LogException(exception, this);
			}
			try
			{
				using (timeWarning1 = TimeWarning.New("ServerBuildingManager.Cycle", 0.1f))
				{
					BuildingManager.server.Cycle();
				}
			}
			catch (Exception exception1)
			{
				UnityEngine.Debug.LogWarning("Server Exception: Building Manager");
				UnityEngine.Debug.LogException(exception1, this);
			}
			try
			{
				using (timeWarning1 = TimeWarning.New("BasePlayer.ServerCycle", 0.1f))
				{
					BasePlayer.ServerCycle(UnityEngine.Time.deltaTime);
				}
			}
			catch (Exception exception2)
			{
				UnityEngine.Debug.LogWarning("Server Exception: Player Update");
				UnityEngine.Debug.LogException(exception2, this);
			}
			try
			{
				using (timeWarning1 = TimeWarning.New("SteamQueryResponse", 0.1f))
				{
					this.SteamQueryResponse();
				}
			}
			catch (Exception exception3)
			{
				UnityEngine.Debug.LogWarning("Server Exception: Steam Query");
				UnityEngine.Debug.LogException(exception3, this);
			}
			try
			{
				using (timeWarning1 = TimeWarning.New("connectionQueue.Cycle", 0.1f))
				{
					this.connectionQueue.Cycle(ServerMgr.AvailableSlots);
				}
			}
			catch (Exception exception4)
			{
				UnityEngine.Debug.LogWarning("Server Exception: Connection Queue");
				UnityEngine.Debug.LogException(exception4, this);
			}
			try
			{
				using (timeWarning1 = TimeWarning.New("IOEntity.ProcessQueue", 0.1f))
				{
					IOEntity.ProcessQueue();
				}
			}
			catch (Exception exception5)
			{
				UnityEngine.Debug.LogWarning("Server Exception: IOEntity.ProcessQueue");
				UnityEngine.Debug.LogException(exception5, this);
			}
			try
			{
				using (timeWarning1 = TimeWarning.New("AIThinkManager.ProcessQueue", 0.1f))
				{
					AIThinkManager.ProcessQueue();
				}
			}
			catch (Exception exception6)
			{
				UnityEngine.Debug.LogWarning("Server Exception: AIThinkManager.ProcessQueue");
				UnityEngine.Debug.LogException(exception6, this);
			}
			if (EACServer.playerTracker != null)
			{
				EACServer.playerTracker.EndFrame();
			}
		}
	}

	private void UpdateItemDefinitions()
	{
		UnityEngine.Debug.Log("Checking for new Steam Item Definitions..");
		Steamworks.SteamInventory.LoadItemDefinitions();
	}

	private void UpdateServerInformation()
	{
		// 
		// Current member / type: System.Void ServerMgr::UpdateServerInformation()
		// File path: D:\GameServers\Rust\RustDedicated_Data\Managed\Assembly-CSharp.dll
		// 
		// Product version: 2019.1.118.0
		// Exception in: System.Void UpdateServerInformation()
		// 
		// Two paths with different stack states encountered.
		//    at ..( ,  ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\DefineUseAnalysis\StackUsageAnalysis.cs:line 139
		//    at ..( , Int32[] ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\DefineUseAnalysis\StackUsageAnalysis.cs:line 132
		//    at ..( , Int32[] ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\DefineUseAnalysis\StackUsageAnalysis.cs:line 128
		//    at ..( , Int32[] ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\DefineUseAnalysis\StackUsageAnalysis.cs:line 128
		//    at ..( , Int32[] ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\DefineUseAnalysis\StackUsageAnalysis.cs:line 128
		//    at ..( , Int32[] ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\DefineUseAnalysis\StackUsageAnalysis.cs:line 128
		//    at ..( , Int32[] ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\DefineUseAnalysis\StackUsageAnalysis.cs:line 128
		//    at ..( , Int32[] ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\DefineUseAnalysis\StackUsageAnalysis.cs:line 128
		//    at ..( , Int32[] ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\DefineUseAnalysis\StackUsageAnalysis.cs:line 128
		//    at ..() in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\DefineUseAnalysis\StackUsageAnalysis.cs:line 74
		//    at ..(DecompilationContext ,  ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\DefineUseAnalysis\StackUsageAnalysis.cs:line 56
		//    at ..(MethodBody ,  , ILanguage ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\DecompilationPipeline.cs:line 88
		//    at ..(MethodBody , ILanguage ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\DecompilationPipeline.cs:line 70
		//    at Telerik.JustDecompiler.Decompiler.Extensions.( , ILanguage , MethodBody , DecompilationContext& ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\Extensions.cs:line 95
		//    at Telerik.JustDecompiler.Decompiler.Extensions.(MethodBody , ILanguage , DecompilationContext& ,  ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\Extensions.cs:line 58
		//    at ..(ILanguage , MethodDefinition ,  ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\WriterContextServices\BaseWriterContextService.cs:line 117
		// 
		// mailto: JustDecompilePublicFeedback@telerik.com

	}
}