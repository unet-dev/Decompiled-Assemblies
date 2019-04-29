using ConVar;
using Facepunch;
using Facepunch.Extend;
using Network;
using Network.Visibility;
using ProtoBuf;
using Rust;
using Rust.Ai;
using Rust.Workshop;
using Spatial;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;

public class BaseEntity : BaseNetworkable, IOnParentSpawning, IPrefabPreProcess
{
	[Header("BaseEntity")]
	public Bounds bounds;

	public GameObjectRef impactEffect;

	public bool enableSaving = true;

	public bool syncPosition;

	public Model model;

	[InspectorFlags]
	public BaseEntity.Flags flags;

	[NonSerialized]
	public uint parentBone;

	[NonSerialized]
	public ulong skinID;

	private EntityComponentBase[] _components;

	[NonSerialized]
	public string _name;

	private static Queue<BaseEntity> globalBroadcastQueue;

	private static uint globalBroadcastProtocol;

	private uint broadcastProtocol;

	private List<EntityLink> links = new List<EntityLink>();

	private bool linkedToNeighbours;

	private Spawnable _spawnable;

	public static HashSet<BaseEntity> saveList;

	[NonSerialized]
	public BaseEntity creatorEntity;

	private int doneMovingWithoutARigidBodyCheck = 1;

	private bool isCallingUpdateNetworkGroup;

	private EntityRef[] entitySlots = new EntityRef[7];

	protected List<TriggerBase> triggers;

	protected bool isVisible = true;

	protected bool isAnimatorVisible = true;

	protected bool isShadowVisible = true;

	protected OccludeeSphere localOccludee = new OccludeeSphere(-1);

	public EntityComponentBase[] Components
	{
		get
		{
			EntityComponentBase[] entityComponentBaseArray = this._components;
			if (entityComponentBaseArray == null)
			{
				EntityComponentBase[] componentsInChildren = base.GetComponentsInChildren<EntityComponentBase>(true);
				EntityComponentBase[] entityComponentBaseArray1 = componentsInChildren;
				this._components = componentsInChildren;
				entityComponentBaseArray = entityComponentBaseArray1;
			}
			return entityComponentBaseArray;
		}
	}

	public float currentTemperature
	{
		get
		{
			float temperature = Climate.GetTemperature(base.transform.position);
			if (this.triggers == null)
			{
				return temperature;
			}
			for (int i = 0; i < this.triggers.Count; i++)
			{
				TriggerTemperature item = this.triggers[i] as TriggerTemperature;
				if (item != null)
				{
					temperature = item.WorkoutTemperature(this.GetNetworkPosition(), temperature);
				}
			}
			return temperature;
		}
	}

	public virtual bool IsNpc
	{
		get
		{
			return false;
		}
	}

	public ulong OwnerID
	{
		get;
		set;
	}

	protected virtual float PositionTickRate
	{
		get
		{
			return 0.1f;
		}
	}

	public float radiationLevel
	{
		get
		{
			if (this.triggers == null)
			{
				return 0f;
			}
			float single = 0f;
			for (int i = 0; i < this.triggers.Count; i++)
			{
				TriggerRadiation item = this.triggers[i] as TriggerRadiation;
				if (item != null)
				{
					Vector3 networkPosition = this.GetNetworkPosition();
					BaseEntity parentEntity = base.GetParentEntity();
					if (parentEntity != null)
					{
						networkPosition = parentEntity.transform.TransformPoint(networkPosition);
					}
					single = Mathf.Max(single, item.GetRadiation(networkPosition, this.RadiationProtection()));
				}
			}
			return single;
		}
	}

	public virtual Vector3 ServerPosition
	{
		get
		{
			return base.transform.localPosition;
		}
		set
		{
			if (base.transform.localPosition == value)
			{
				return;
			}
			base.transform.localPosition = value;
			base.transform.hasChanged = true;
		}
	}

	public virtual Quaternion ServerRotation
	{
		get
		{
			return base.transform.localRotation;
		}
		set
		{
			if (base.transform.localRotation == value)
			{
				return;
			}
			base.transform.localRotation = value;
			base.transform.hasChanged = true;
		}
	}

	public virtual BaseEntity.TraitFlag Traits
	{
		get
		{
			return BaseEntity.TraitFlag.None;
		}
	}

	static BaseEntity()
	{
		BaseEntity.globalBroadcastQueue = new Queue<BaseEntity>();
		BaseEntity.globalBroadcastProtocol = 0;
		BaseEntity.saveList = new HashSet<BaseEntity>();
	}

	public BaseEntity()
	{
	}

	public virtual void AttackerInfo(PlayerLifeStory.DeathInfo info)
	{
		info.attackerName = base.ShortPrefabName;
		info.attackerSteamID = (ulong)0;
		info.inflictorName = "";
	}

	public virtual float BoundsPadding()
	{
		return 0.1f;
	}

	public void BroadcastEntityMessage(string msg, float radius = 20f, int layerMask = 1218652417)
	{
		if (base.isClient)
		{
			return;
		}
		List<BaseEntity> list = Facepunch.Pool.GetList<BaseEntity>();
		Vis.Entities<BaseEntity>(base.transform.position, radius, list, layerMask, QueryTriggerInteraction.Collide);
		foreach (BaseEntity baseEntity in list)
		{
			if (!baseEntity.isServer)
			{
				continue;
			}
			baseEntity.OnEntityMessage(this, msg);
		}
		Facepunch.Pool.FreeList<BaseEntity>(ref list);
	}

	[FromOwner]
	[RPC_Server]
	private void BroadcastSignalFromClient(BaseEntity.RPCMessage msg)
	{
		BaseEntity.Signal signal = (BaseEntity.Signal)msg.read.Int32();
		this.SignalBroadcast(signal, msg.read.String(), msg.connection);
	}

	public virtual bool CanBeLooted(BasePlayer player)
	{
		return true;
	}

	public virtual string Categorize()
	{
		return "entity";
	}

	public Vector3 CenterPoint()
	{
		return this.WorldSpaceBounds().position;
	}

	public void ClientRPC<T1, T2, T3, T4, T5>(Connection sourceConnection, string funcName, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
	{
		if (!Network.Net.sv.IsConnected())
		{
			return;
		}
		if (this.net == null)
		{
			return;
		}
		if (this.net.@group == null)
		{
			return;
		}
		this.ClientRPCEx<T1, T2, T3, T4, T5>(new SendInfo(this.net.@group.subscribers), sourceConnection, funcName, arg1, arg2, arg3, arg4, arg5);
	}

	public void ClientRPC<T1, T2, T3, T4>(Connection sourceConnection, string funcName, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
	{
		if (!Network.Net.sv.IsConnected())
		{
			return;
		}
		if (this.net == null)
		{
			return;
		}
		if (this.net.@group == null)
		{
			return;
		}
		this.ClientRPCEx<T1, T2, T3, T4>(new SendInfo(this.net.@group.subscribers), sourceConnection, funcName, arg1, arg2, arg3, arg4);
	}

	public void ClientRPC<T1, T2, T3>(Connection sourceConnection, string funcName, T1 arg1, T2 arg2, T3 arg3)
	{
		if (!Network.Net.sv.IsConnected())
		{
			return;
		}
		if (this.net == null)
		{
			return;
		}
		if (this.net.@group == null)
		{
			return;
		}
		this.ClientRPCEx<T1, T2, T3>(new SendInfo(this.net.@group.subscribers), sourceConnection, funcName, arg1, arg2, arg3);
	}

	public void ClientRPC<T1, T2>(Connection sourceConnection, string funcName, T1 arg1, T2 arg2)
	{
		if (!Network.Net.sv.IsConnected())
		{
			return;
		}
		if (this.net == null)
		{
			return;
		}
		if (this.net.@group == null)
		{
			return;
		}
		this.ClientRPCEx<T1, T2>(new SendInfo(this.net.@group.subscribers), sourceConnection, funcName, arg1, arg2);
	}

	public void ClientRPC<T1>(Connection sourceConnection, string funcName, T1 arg1)
	{
		if (!Network.Net.sv.IsConnected())
		{
			return;
		}
		if (this.net == null)
		{
			return;
		}
		if (this.net.@group == null)
		{
			return;
		}
		this.ClientRPCEx<T1>(new SendInfo(this.net.@group.subscribers), sourceConnection, funcName, arg1);
	}

	public void ClientRPC(Connection sourceConnection, string funcName)
	{
		if (!Network.Net.sv.IsConnected())
		{
			return;
		}
		if (this.net == null)
		{
			return;
		}
		if (this.net.@group == null)
		{
			return;
		}
		this.ClientRPCEx(new SendInfo(this.net.@group.subscribers), sourceConnection, funcName);
	}

	public void ClientRPCEx<T1, T2, T3, T4, T5>(SendInfo sendInfo, Connection sourceConnection, string funcName, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
	{
		if (!Network.Net.sv.IsConnected())
		{
			return;
		}
		if (this.net == null)
		{
			return;
		}
		if (this.ClientRPCStart(sourceConnection, funcName))
		{
			this.ClientRPCWrite<T1>(arg1);
			this.ClientRPCWrite<T2>(arg2);
			this.ClientRPCWrite<T3>(arg3);
			this.ClientRPCWrite<T4>(arg4);
			this.ClientRPCWrite<T5>(arg5);
			this.ClientRPCSend(sendInfo);
		}
	}

	public void ClientRPCEx<T1, T2, T3, T4>(SendInfo sendInfo, Connection sourceConnection, string funcName, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
	{
		if (!Network.Net.sv.IsConnected())
		{
			return;
		}
		if (this.net == null)
		{
			return;
		}
		if (this.ClientRPCStart(sourceConnection, funcName))
		{
			this.ClientRPCWrite<T1>(arg1);
			this.ClientRPCWrite<T2>(arg2);
			this.ClientRPCWrite<T3>(arg3);
			this.ClientRPCWrite<T4>(arg4);
			this.ClientRPCSend(sendInfo);
		}
	}

	public void ClientRPCEx<T1, T2, T3>(SendInfo sendInfo, Connection sourceConnection, string funcName, T1 arg1, T2 arg2, T3 arg3)
	{
		if (!Network.Net.sv.IsConnected())
		{
			return;
		}
		if (this.net == null)
		{
			return;
		}
		if (this.ClientRPCStart(sourceConnection, funcName))
		{
			this.ClientRPCWrite<T1>(arg1);
			this.ClientRPCWrite<T2>(arg2);
			this.ClientRPCWrite<T3>(arg3);
			this.ClientRPCSend(sendInfo);
		}
	}

	public void ClientRPCEx<T1, T2>(SendInfo sendInfo, Connection sourceConnection, string funcName, T1 arg1, T2 arg2)
	{
		if (!Network.Net.sv.IsConnected())
		{
			return;
		}
		if (this.net == null)
		{
			return;
		}
		if (this.ClientRPCStart(sourceConnection, funcName))
		{
			this.ClientRPCWrite<T1>(arg1);
			this.ClientRPCWrite<T2>(arg2);
			this.ClientRPCSend(sendInfo);
		}
	}

	public void ClientRPCEx<T1>(SendInfo sendInfo, Connection sourceConnection, string funcName, T1 arg1)
	{
		if (!Network.Net.sv.IsConnected())
		{
			return;
		}
		if (this.net == null)
		{
			return;
		}
		if (this.ClientRPCStart(sourceConnection, funcName))
		{
			this.ClientRPCWrite<T1>(arg1);
			this.ClientRPCSend(sendInfo);
		}
	}

	public void ClientRPCEx(SendInfo sendInfo, Connection sourceConnection, string funcName)
	{
		if (!Network.Net.sv.IsConnected())
		{
			return;
		}
		if (this.net == null)
		{
			return;
		}
		if (this.ClientRPCStart(sourceConnection, funcName))
		{
			this.ClientRPCSend(sendInfo);
		}
	}

	public void ClientRPCPlayer<T1, T2, T3, T4, T5>(Connection sourceConnection, BasePlayer player, string funcName, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
	{
		if (!Network.Net.sv.IsConnected())
		{
			return;
		}
		if (this.net == null)
		{
			return;
		}
		if (player.net.connection == null)
		{
			return;
		}
		this.ClientRPCEx<T1, T2, T3, T4, T5>(new SendInfo(player.net.connection), sourceConnection, funcName, arg1, arg2, arg3, arg4, arg5);
	}

	public void ClientRPCPlayer<T1, T2, T3, T4>(Connection sourceConnection, BasePlayer player, string funcName, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
	{
		if (!Network.Net.sv.IsConnected())
		{
			return;
		}
		if (this.net == null)
		{
			return;
		}
		if (player.net.connection == null)
		{
			return;
		}
		this.ClientRPCEx<T1, T2, T3, T4>(new SendInfo(player.net.connection), sourceConnection, funcName, arg1, arg2, arg3, arg4);
	}

	public void ClientRPCPlayer<T1, T2, T3>(Connection sourceConnection, BasePlayer player, string funcName, T1 arg1, T2 arg2, T3 arg3)
	{
		if (!Network.Net.sv.IsConnected())
		{
			return;
		}
		if (this.net == null)
		{
			return;
		}
		if (player.net.connection == null)
		{
			return;
		}
		this.ClientRPCEx<T1, T2, T3>(new SendInfo(player.net.connection), sourceConnection, funcName, arg1, arg2, arg3);
	}

	public void ClientRPCPlayer<T1, T2>(Connection sourceConnection, BasePlayer player, string funcName, T1 arg1, T2 arg2)
	{
		if (!Network.Net.sv.IsConnected())
		{
			return;
		}
		if (this.net == null)
		{
			return;
		}
		if (player.net.connection == null)
		{
			return;
		}
		this.ClientRPCEx<T1, T2>(new SendInfo(player.net.connection), sourceConnection, funcName, arg1, arg2);
	}

	public void ClientRPCPlayer<T1>(Connection sourceConnection, BasePlayer player, string funcName, T1 arg1)
	{
		if (!Network.Net.sv.IsConnected())
		{
			return;
		}
		if (this.net == null)
		{
			return;
		}
		if (player.net.connection == null)
		{
			return;
		}
		this.ClientRPCEx<T1>(new SendInfo(player.net.connection), sourceConnection, funcName, arg1);
	}

	public void ClientRPCPlayer(Connection sourceConnection, BasePlayer player, string funcName)
	{
		if (!Network.Net.sv.IsConnected())
		{
			return;
		}
		if (this.net == null)
		{
			return;
		}
		if (player.net.connection == null)
		{
			return;
		}
		this.ClientRPCEx(new SendInfo(player.net.connection), sourceConnection, funcName);
	}

	private void ClientRPCSend(SendInfo sendInfo)
	{
		Network.Net.sv.write.Send(sendInfo);
	}

	private bool ClientRPCStart(Connection sourceConnection, string funcName)
	{
		if (!Network.Net.sv.write.Start())
		{
			return false;
		}
		Network.Net.sv.write.PacketID(Message.Type.RPCMessage);
		Network.Net.sv.write.UInt32(this.net.ID);
		Network.Net.sv.write.UInt32(StringPool.Get(funcName));
		Network.Net.sv.write.UInt64((sourceConnection == null ? (ulong)((long)0) : sourceConnection.userid));
		return true;
	}

	private void ClientRPCWrite<T>(T arg)
	{
		Network.Net.sv.write.WriteObject<T>(arg);
	}

	public Vector3 ClosestPoint(Vector3 position)
	{
		return this.WorldSpaceBounds().ClosestPoint(position);
	}

	public virtual void DebugServer(int rep, float time)
	{
		uint d;
		Vector3 vector3 = base.transform.position + (Vector3.up * 1f);
		if (this.net != null)
		{
			d = this.net.ID;
		}
		else
		{
			d = 0;
		}
		this.DebugText(vector3, string.Format("{0}: {1}\n{2}", d, base.name, this.DebugText()), Color.white, time);
	}

	public virtual string DebugText()
	{
		return "";
	}

	protected void DebugText(Vector3 pos, string str, Color color, float time)
	{
		if (base.isServer)
		{
			ConsoleNetwork.BroadcastToAllClients("ddraw.text", new object[] { time, color, pos, str });
		}
	}

	public void DestroyOnClient(Connection connection)
	{
		if (this.children != null)
		{
			foreach (BaseEntity child in this.children)
			{
				child.DestroyOnClient(connection);
			}
		}
		if (Network.Net.sv.IsConnected() && Network.Net.sv.write.Start())
		{
			Network.Net.sv.write.PacketID(Message.Type.EntityDestroy);
			Network.Net.sv.write.EntityID(this.net.ID);
			Network.Net.sv.write.UInt8(0);
			Network.Net.sv.write.Send(new SendInfo(connection));
			base.LogEntry(BaseMonoBehaviour.LogEntryType.Network, 2, "EntityDestroy");
		}
	}

	public override void DestroyShared()
	{
		base.DestroyShared();
		this.FreeEntityLinks();
	}

	public float Distance(Vector3 position)
	{
		return (this.ClosestPoint(position) - position).magnitude;
	}

	public float Distance(BaseEntity other)
	{
		return this.Distance(other.transform.position);
	}

	public float Distance2D(Vector3 position)
	{
		return (this.ClosestPoint(position) - position).Magnitude2D();
	}

	public float Distance2D(BaseEntity other)
	{
		return this.Distance(other.transform.position);
	}

	public void DoMovingWithoutARigidBodyCheck()
	{
		if (this.doneMovingWithoutARigidBodyCheck > 10)
		{
			return;
		}
		this.doneMovingWithoutARigidBodyCheck++;
		if (this.doneMovingWithoutARigidBodyCheck < 10)
		{
			return;
		}
		if (base.GetComponent<Collider>() == null)
		{
			return;
		}
		if (base.GetComponent<Rigidbody>() == null)
		{
			Debug.LogWarning(string.Concat("Entity moving without a rigid body! (", base.gameObject, ")"), this);
		}
	}

	internal override void DoServerDestroy()
	{
		base.CancelInvoke(new Action(this.NetworkPositionTick));
		BaseEntity.saveList.Remove(this);
		this.RemoveFromTriggers();
		if (this.children != null)
		{
			BaseEntity[] array = this.children.ToArray();
			for (int i = 0; i < (int)array.Length; i++)
			{
				array[i].OnParentRemoved();
			}
		}
		this.SetParent(null, false, false);
		BaseEntity.Query.Server.Remove(this, false);
		base.DoServerDestroy();
	}

	public BaseCorpse DropCorpse(string strCorpsePrefab)
	{
		Assert.IsTrue(base.isServer, "DropCorpse called on client!");
		if (!ConVar.Server.corpses)
		{
			return null;
		}
		if (string.IsNullOrEmpty(strCorpsePrefab))
		{
			return null;
		}
		GameManager gameManager = GameManager.server;
		Vector3 vector3 = new Vector3();
		Quaternion quaternion = new Quaternion();
		BaseCorpse baseCorpse = gameManager.CreateEntity(strCorpsePrefab, vector3, quaternion, true) as BaseCorpse;
		if (baseCorpse != null)
		{
			baseCorpse.InitCorpse(this);
			return baseCorpse;
		}
		Debug.LogWarning(string.Concat(new object[] { "Error creating corpse: ", base.gameObject, " - ", strCorpsePrefab }));
		return null;
	}

	public virtual void Eat(BaseNpc baseNpc, float timeSpent)
	{
		baseNpc.AddCalories(100f);
	}

	public virtual bool EnterTrigger(TriggerBase trigger)
	{
		if (this.triggers == null)
		{
			this.triggers = Facepunch.Pool.Get<List<TriggerBase>>();
		}
		this.triggers.Add(trigger);
		return true;
	}

	public void EntityLinkBroadcast<T>(Action<T> action)
	where T : BaseEntity
	{
		BaseEntity.globalBroadcastProtocol++;
		BaseEntity.globalBroadcastQueue.Clear();
		this.broadcastProtocol = BaseEntity.globalBroadcastProtocol;
		BaseEntity.globalBroadcastQueue.Enqueue(this);
		if (this is T)
		{
			action((T)(this as T));
		}
		while (BaseEntity.globalBroadcastQueue.Count > 0)
		{
			List<EntityLink> entityLinks = BaseEntity.globalBroadcastQueue.Dequeue().GetEntityLinks(true);
			for (int i = 0; i < entityLinks.Count; i++)
			{
				EntityLink item = entityLinks[i];
				for (int j = 0; j < item.connections.Count; j++)
				{
					BaseEntity baseEntity = item.connections[j].owner;
					if (baseEntity.broadcastProtocol != BaseEntity.globalBroadcastProtocol)
					{
						baseEntity.broadcastProtocol = BaseEntity.globalBroadcastProtocol;
						BaseEntity.globalBroadcastQueue.Enqueue(baseEntity);
						if (baseEntity is T)
						{
							action((T)(baseEntity as T));
						}
					}
				}
			}
		}
	}

	public void EntityLinkBroadcast()
	{
		BaseEntity.globalBroadcastProtocol++;
		BaseEntity.globalBroadcastQueue.Clear();
		this.broadcastProtocol = BaseEntity.globalBroadcastProtocol;
		BaseEntity.globalBroadcastQueue.Enqueue(this);
		while (BaseEntity.globalBroadcastQueue.Count > 0)
		{
			List<EntityLink> entityLinks = BaseEntity.globalBroadcastQueue.Dequeue().GetEntityLinks(true);
			for (int i = 0; i < entityLinks.Count; i++)
			{
				EntityLink item = entityLinks[i];
				for (int j = 0; j < item.connections.Count; j++)
				{
					BaseEntity baseEntity = item.connections[j].owner;
					if (baseEntity.broadcastProtocol != BaseEntity.globalBroadcastProtocol)
					{
						baseEntity.broadcastProtocol = BaseEntity.globalBroadcastProtocol;
						BaseEntity.globalBroadcastQueue.Enqueue(baseEntity);
					}
				}
			}
		}
	}

	public void EntityLinkMessage<T>(Action<T> action)
	where T : BaseEntity
	{
		List<EntityLink> entityLinks = this.GetEntityLinks(true);
		for (int i = 0; i < entityLinks.Count; i++)
		{
			EntityLink item = entityLinks[i];
			for (int j = 0; j < item.connections.Count; j++)
			{
				EntityLink entityLink = item.connections[j];
				if (entityLink.owner is T)
				{
					action((T)(entityLink.owner as T));
				}
			}
		}
	}

	public virtual Transform FindBone(string strName)
	{
		if (!this.model)
		{
			return base.transform;
		}
		return this.model.FindBone(strName);
	}

	public virtual Transform FindClosestBone(Vector3 worldPos)
	{
		if (!this.model)
		{
			return base.transform;
		}
		return this.model.FindClosestBone(worldPos);
	}

	public EntityLink FindLink(Socket_Base socket)
	{
		List<EntityLink> entityLinks = this.GetEntityLinks(true);
		for (int i = 0; i < entityLinks.Count; i++)
		{
			if (entityLinks[i].socket == socket)
			{
				return entityLinks[i];
			}
		}
		return null;
	}

	public EntityLink FindLink(string socketName)
	{
		List<EntityLink> entityLinks = this.GetEntityLinks(true);
		for (int i = 0; i < entityLinks.Count; i++)
		{
			if (entityLinks[i].socket.socketName == socketName)
			{
				return entityLinks[i];
			}
		}
		return null;
	}

	public T FindLinkedEntity<T>()
	where T : BaseEntity
	{
		List<EntityLink> entityLinks = this.GetEntityLinks(true);
		for (int i = 0; i < entityLinks.Count; i++)
		{
			EntityLink item = entityLinks[i];
			for (int j = 0; j < item.connections.Count; j++)
			{
				EntityLink entityLink = item.connections[j];
				if (entityLink.owner is T)
				{
					return (T)(entityLink.owner as T);
				}
			}
		}
		return default(T);
	}

	public T FindTrigger<T>()
	where T : TriggerBase
	{
		T t;
		if (this.triggers == null)
		{
			t = default(T);
			return t;
		}
		List<TriggerBase>.Enumerator enumerator = this.triggers.GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				TriggerBase current = enumerator.Current;
				if ((T)(current as T) == null)
				{
					continue;
				}
				t = (T)(current as T);
				return t;
			}
			return default(T);
		}
		finally
		{
			((IDisposable)enumerator).Dispose();
		}
		return t;
	}

	private void FreeEntityLinks()
	{
		using (TimeWarning timeWarning = TimeWarning.New("FreeEntityLinks", 0.1f))
		{
			this.links.FreeLinks();
			this.linkedToNeighbours = false;
		}
	}

	public Quaternion GetAngularVelocity()
	{
		if (!base.isServer)
		{
			return Quaternion.identity;
		}
		return this.GetAngularVelocityServer();
	}

	public virtual Quaternion GetAngularVelocityServer()
	{
		return Quaternion.identity;
	}

	public virtual Transform[] GetBones()
	{
		return null;
	}

	public virtual BuildingPrivlidge GetBuildingPrivilege()
	{
		return this.GetBuildingPrivilege(this.WorldSpaceBounds());
	}

	public BuildingPrivlidge GetBuildingPrivilege(OBB obb)
	{
		BuildingBlock buildingBlock = null;
		BuildingPrivlidge buildingPrivlidge = null;
		List<BuildingBlock> list = Facepunch.Pool.GetList<BuildingBlock>();
		Vis.Entities<BuildingBlock>(obb.position, 16f + obb.extents.magnitude, list, 2097152, QueryTriggerInteraction.Collide);
		for (int i = 0; i < list.Count; i++)
		{
			BuildingBlock item = list[i];
			if (item.isServer == base.isServer && item.IsOlderThan(buildingBlock) && obb.Distance(item.WorldSpaceBounds()) <= 16f)
			{
				BuildingManager.Building building = item.GetBuilding();
				if (building != null)
				{
					BuildingPrivlidge dominatingBuildingPrivilege = building.GetDominatingBuildingPrivilege();
					if (dominatingBuildingPrivilege != null)
					{
						buildingBlock = item;
						buildingPrivlidge = dominatingBuildingPrivilege;
					}
				}
			}
		}
		Facepunch.Pool.FreeList<BuildingBlock>(ref list);
		return buildingPrivlidge;
	}

	public virtual Vector3 GetDropPosition()
	{
		return base.transform.position;
	}

	public virtual Vector3 GetDropVelocity()
	{
		return this.GetInheritedDropVelocity() + Vector3.up;
	}

	public virtual BaseEntity GetEntity()
	{
		return this;
	}

	public List<EntityLink> GetEntityLinks(bool linkToNeighbours = true)
	{
		if (Rust.Application.isLoadingSave)
		{
			return this.links;
		}
		if (!this.linkedToNeighbours & linkToNeighbours)
		{
			this.LinkToNeighbours();
		}
		return this.links;
	}

	public virtual GameObjectRef GetImpactEffect(HitInfo info)
	{
		return this.impactEffect;
	}

	public Vector3 GetInheritedDropVelocity()
	{
		BaseEntity baseEntity = this.parentEntity.Get(base.isServer);
		if (baseEntity == null)
		{
			return Vector3.zero;
		}
		return baseEntity.GetWorldVelocity();
	}

	public Vector3 GetInheritedProjectileVelocity()
	{
		BaseEntity baseEntity = this.parentEntity.Get(base.isServer);
		if (baseEntity == null)
		{
			return Vector3.zero;
		}
		return this.GetParentVelocity() * baseEntity.InheritedVelocityScale();
	}

	public Vector3 GetInheritedThrowVelocity()
	{
		return this.GetParentVelocity();
	}

	public virtual Item GetItem()
	{
		return null;
	}

	public virtual Item GetItem(uint itemId)
	{
		return null;
	}

	public Vector3 GetLocalVelocity()
	{
		if (!base.isServer)
		{
			return Vector3.zero;
		}
		return this.GetLocalVelocityServer();
	}

	public virtual Vector3 GetLocalVelocityServer()
	{
		return Vector3.zero;
	}

	public override string GetLogColor()
	{
		if (base.isServer)
		{
			return "cyan";
		}
		return "yellow";
	}

	public Model GetModel()
	{
		return this.model;
	}

	public Vector3 GetParentVelocity()
	{
		BaseEntity baseEntity = this.parentEntity.Get(base.isServer);
		if (baseEntity == null)
		{
			return Vector3.zero;
		}
		return baseEntity.GetWorldVelocity() + ((baseEntity.GetAngularVelocity() * base.transform.localPosition) - base.transform.localPosition);
	}

	public BaseEntity GetSlot(BaseEntity.Slot slot)
	{
		return this.entitySlots[(int)slot].Get(base.isServer);
	}

	public string GetSlotAnchorName(BaseEntity.Slot slot)
	{
		return slot.ToString().ToLower();
	}

	public Vector3 GetWorldVelocity()
	{
		BaseEntity baseEntity = this.parentEntity.Get(base.isServer);
		if (baseEntity == null)
		{
			return this.GetLocalVelocity();
		}
		return (baseEntity.GetWorldVelocity() + ((baseEntity.GetAngularVelocity() * base.transform.localPosition) - base.transform.localPosition)) + baseEntity.transform.TransformDirection(this.GetLocalVelocity());
	}

	public virtual void GiveItem(Item item, BaseEntity.GiveItemReason reason = 0)
	{
		item.Remove(0f);
	}

	public bool HasAnySlot()
	{
		for (int i = 0; i < (int)this.entitySlots.Length; i++)
		{
			if (this.entitySlots[i].IsValid(base.isServer))
			{
				return true;
			}
		}
		return false;
	}

	public bool HasAnyTrait(BaseEntity.TraitFlag f)
	{
		return (this.Traits & f) != BaseEntity.TraitFlag.None;
	}

	public bool HasChild(BaseEntity c)
	{
		if (c == this)
		{
			return true;
		}
		BaseEntity parentEntity = c.GetParentEntity();
		if (parentEntity == null)
		{
			return false;
		}
		return this.HasChild(parentEntity);
	}

	public bool HasFlag(BaseEntity.Flags f)
	{
		return (this.flags & f) == f;
	}

	public virtual bool HasSlot(BaseEntity.Slot slot)
	{
		return false;
	}

	public bool HasTrait(BaseEntity.TraitFlag f)
	{
		return (this.Traits & f) == f;
	}

	public virtual float Health()
	{
		return 0f;
	}

	public virtual float InheritedVelocityScale()
	{
		return 0f;
	}

	private void InitEntityLinks()
	{
		using (TimeWarning timeWarning = TimeWarning.New("InitEntityLinks", 0.1f))
		{
			if (base.isServer)
			{
				this.links.AddLinks(this, PrefabAttribute.server.FindAll<Socket_Base>(this.prefabID));
			}
		}
	}

	public override void InitShared()
	{
		base.InitShared();
		this.InitEntityLinks();
	}

	public bool IsBroken()
	{
		return this.HasFlag(BaseEntity.Flags.Broken);
	}

	public bool IsBusy()
	{
		return this.HasFlag(BaseEntity.Flags.Busy);
	}

	public override bool IsDebugging()
	{
		return this.HasFlag(BaseEntity.Flags.Debugging);
	}

	public bool IsDisabled()
	{
		if (this.HasFlag(BaseEntity.Flags.Disabled))
		{
			return true;
		}
		return this.ParentHasFlag(BaseEntity.Flags.Disabled);
	}

	public bool IsLocked()
	{
		return this.HasFlag(BaseEntity.Flags.Locked);
	}

	public bool IsOccupied(Socket_Base socket)
	{
		EntityLink entityLink = this.FindLink(socket);
		if (entityLink == null)
		{
			return false;
		}
		return entityLink.IsOccupied();
	}

	public bool IsOccupied(string socketName)
	{
		EntityLink entityLink = this.FindLink(socketName);
		if (entityLink == null)
		{
			return false;
		}
		return entityLink.IsOccupied();
	}

	public bool IsOlderThan(BaseEntity other)
	{
		uint d;
		uint num;
		if (other == null)
		{
			return true;
		}
		if (this.net == null)
		{
			num = 0;
		}
		else
		{
			num = this.net.ID;
		}
		if (other.net == null)
		{
			d = 0;
		}
		else
		{
			d = other.net.ID;
		}
		return num < d;
	}

	public bool IsOn()
	{
		return this.HasFlag(BaseEntity.Flags.On);
	}

	public bool IsOnFire()
	{
		return this.HasFlag(BaseEntity.Flags.OnFire);
	}

	public bool IsOpen()
	{
		return this.HasFlag(BaseEntity.Flags.Open);
	}

	public virtual bool IsOutside()
	{
		OBB oBB = this.WorldSpaceBounds();
		Vector3 vector3 = oBB.position + (oBB.up * oBB.extents.y);
		return this.IsOutside(vector3);
	}

	public bool IsOutside(Vector3 position)
	{
		return !UnityEngine.Physics.Raycast(position, Vector3.up, 100f, 1101070337);
	}

	public bool IsVisible(Ray ray, float maxDistance = Single.PositiveInfinity)
	{
		RaycastHit raycastHit;
		RaycastHit raycastHit1;
		if (ray.origin.IsNaNOrInfinity())
		{
			return false;
		}
		if (ray.direction.IsNaNOrInfinity())
		{
			return false;
		}
		if (ray.direction == Vector3.zero)
		{
			return false;
		}
		if (!this.WorldSpaceBounds().Trace(ray, out raycastHit, maxDistance))
		{
			return false;
		}
		if (GamePhysics.Trace(ray, 0f, out raycastHit1, maxDistance, 1218519041, QueryTriggerInteraction.UseGlobal))
		{
			BaseEntity entity = raycastHit1.GetEntity();
			if (entity == this)
			{
				return true;
			}
			if (entity != null && base.GetParentEntity() && base.GetParentEntity().EqualNetID(entity) && raycastHit1.collider != null && raycastHit1.collider.gameObject.layer == 13)
			{
				return true;
			}
			if (raycastHit1.distance <= raycastHit.distance)
			{
				return false;
			}
		}
		return true;
	}

	public bool IsVisible(Vector3 position, Vector3 target, float maxDistance = Single.PositiveInfinity)
	{
		Vector3 vector3 = target - position;
		float single = vector3.magnitude;
		if (single < Mathf.Epsilon)
		{
			return true;
		}
		Vector3 vector31 = vector3 / single;
		Vector3 vector32 = vector31 * Mathf.Min(single, 0.01f);
		return this.IsVisible(new Ray(position + vector32, vector31), maxDistance);
	}

	public bool IsVisible(Vector3 position, float maxDistance = Single.PositiveInfinity)
	{
		if (this.IsVisible(position, this.CenterPoint(), maxDistance))
		{
			return true;
		}
		return this.IsVisible(position, this.ClosestPoint(position), maxDistance);
	}

	public virtual void LeaveTrigger(TriggerBase trigger)
	{
		if (this.triggers == null)
		{
			return;
		}
		this.triggers.Remove(trigger);
		if (this.triggers.Count == 0)
		{
			Facepunch.Pool.FreeList<TriggerBase>(ref this.triggers);
		}
	}

	private void LinkToEntity(BaseEntity other)
	{
		if (this == other)
		{
			return;
		}
		if (this.links.Count == 0 || other.links.Count == 0)
		{
			return;
		}
		using (TimeWarning timeWarning = TimeWarning.New("LinkToEntity", 0.1f))
		{
			for (int i = 0; i < this.links.Count; i++)
			{
				EntityLink item = this.links[i];
				for (int j = 0; j < other.links.Count; j++)
				{
					EntityLink entityLink = other.links[j];
					if (item.CanConnect(entityLink))
					{
						if (!item.Contains(entityLink))
						{
							item.Add(entityLink);
						}
						if (!entityLink.Contains(item))
						{
							entityLink.Add(item);
						}
					}
				}
			}
		}
	}

	private void LinkToNeighbours()
	{
		if (this.links.Count == 0)
		{
			return;
		}
		this.linkedToNeighbours = true;
		using (TimeWarning timeWarning = TimeWarning.New("LinkToNeighbours", 0.1f))
		{
			List<BaseEntity> list = Facepunch.Pool.GetList<BaseEntity>();
			OBB oBB = this.WorldSpaceBounds();
			Vis.Entities<BaseEntity>(oBB.position, oBB.extents.magnitude + 1f, list, -1, QueryTriggerInteraction.Collide);
			for (int i = 0; i < list.Count; i++)
			{
				BaseEntity item = list[i];
				if (item.isServer == base.isServer)
				{
					this.LinkToEntity(item);
				}
			}
			Facepunch.Pool.FreeList<BaseEntity>(ref list);
		}
	}

	public override void Load(BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (info.msg.baseEntity != null)
		{
			ProtoBuf.BaseEntity baseEntity = info.msg.baseEntity;
			BaseEntity.Flags flag = this.flags;
			this.flags = (BaseEntity.Flags)baseEntity.flags;
			this.OnFlagsChanged(flag, this.flags);
			this.OnSkinChanged(this.skinID, info.msg.baseEntity.skinid);
			if (info.fromDisk)
			{
				if (baseEntity.pos.IsNaNOrInfinity())
				{
					Debug.LogWarning(string.Concat(this.ToString(), " has broken position - ", baseEntity.pos));
					baseEntity.pos = Vector3.zero;
				}
				base.transform.localPosition = baseEntity.pos;
				base.transform.localRotation = Quaternion.Euler(baseEntity.rot);
			}
		}
		if (info.msg.entitySlots != null)
		{
			this.entitySlots[0].uid = info.msg.entitySlots.slotLock;
			this.entitySlots[1].uid = info.msg.entitySlots.slotFireMod;
			this.entitySlots[2].uid = info.msg.entitySlots.slotUpperModification;
			this.entitySlots[5].uid = info.msg.entitySlots.centerDecoration;
			this.entitySlots[6].uid = info.msg.entitySlots.lowerCenterDecoration;
		}
		if (info.msg.parent == null)
		{
			this.parentEntity.uid = 0;
			this.parentBone = 0;
		}
		else
		{
			if (base.isServer)
			{
				BaseEntity baseEntity1 = BaseNetworkable.serverEntities.Find(info.msg.parent.uid) as BaseEntity;
				this.SetParent(baseEntity1, info.msg.parent.bone, false, false);
			}
			this.parentEntity.uid = info.msg.parent.uid;
			this.parentBone = info.msg.parent.bone;
		}
		if (info.msg.ownerInfo != null)
		{
			this.OwnerID = info.msg.ownerInfo.steamid;
		}
		if (this._spawnable)
		{
			this._spawnable.Load(info);
		}
	}

	public void Log(string str)
	{
		if (base.isClient)
		{
			Debug.Log(string.Concat(new string[] { "<color=#ffa>[", this.ToString(), "] ", str, "</color>" }), base.gameObject);
			return;
		}
		Debug.Log(string.Concat(new string[] { "<color=#aff>[", this.ToString(), "] ", str, "</color>" }), base.gameObject);
	}

	public virtual float MaxHealth()
	{
		return 0f;
	}

	public virtual float MaxVelocity()
	{
		return 0f;
	}

	protected void NetworkPositionTick()
	{
		if (!base.transform.hasChanged)
		{
			if (this.ShouldInheritNetworkGroup())
			{
				return;
			}
			if (!base.HasParent())
			{
				return;
			}
		}
		this.TransformChanged();
		base.transform.hasChanged = false;
	}

	public virtual void OnAttacked(HitInfo info)
	{
	}

	public virtual void OnCollision(Collision collision, BaseEntity hitEntity)
	{
		throw new NotImplementedException();
	}

	public void OnDebugStart()
	{
		EntityDebug component = base.gameObject.GetComponent<EntityDebug>();
		if (component == null)
		{
			component = base.gameObject.AddComponent<EntityDebug>();
		}
		component.enabled = true;
	}

	public virtual void OnDeployed(BaseEntity parent)
	{
	}

	public virtual void OnEntityMessage(BaseEntity from, string msg)
	{
	}

	public virtual void OnFlagsChanged(BaseEntity.Flags old, BaseEntity.Flags next)
	{
		if (this.IsDebugging() && (old & BaseEntity.Flags.Debugging) != (next & BaseEntity.Flags.Debugging))
		{
			this.OnDebugStart();
		}
	}

	public virtual void OnInvalidPosition()
	{
		Debug.Log(string.Concat(new object[] { "Invalid Position: ", this, " ", base.transform.position, " (destroying)" }));
		base.Kill(BaseNetworkable.DestroyMode.None);
	}

	public virtual void OnParentChanging(BaseEntity oldParent, BaseEntity newParent)
	{
		Rigidbody component = base.GetComponent<Rigidbody>();
		if (component)
		{
			if (oldParent != null)
			{
				Rigidbody worldVelocity = component;
				worldVelocity.velocity = worldVelocity.velocity + oldParent.GetWorldVelocity();
			}
			if (newParent != null)
			{
				Rigidbody rigidbody = component;
				rigidbody.velocity = rigidbody.velocity - newParent.GetWorldVelocity();
			}
		}
	}

	internal virtual void OnParentRemoved()
	{
		base.Kill(BaseNetworkable.DestroyMode.None);
	}

	public void OnParentSpawning()
	{
		BaseEntity componentInParent;
		if (this.net != null)
		{
			return;
		}
		if (base.IsDestroyed)
		{
			return;
		}
		if (Rust.Application.isLoadingSave)
		{
			UnityEngine.Object.Destroy(base.gameObject);
			return;
		}
		if (base.transform.parent != null)
		{
			componentInParent = base.transform.parent.GetComponentInParent<BaseEntity>();
		}
		else
		{
			componentInParent = null;
		}
		BaseEntity baseEntity = componentInParent;
		this.Spawn();
		if (baseEntity != null)
		{
			this.SetParent(baseEntity, true, false);
		}
	}

	public virtual void OnPositionalNetworkUpdate()
	{
	}

	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		bool flag;
		BaseEntity.RPCMessage rPCMessage;
		using (TimeWarning timeWarning = TimeWarning.New("BaseEntity.OnRpcMessage", 0.1f))
		{
			if (rpc == 1552640099 && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (ConVar.Global.developer > 2)
				{
					Debug.Log(string.Concat("SV_RPCMessage: ", player, " - BroadcastSignalFromClient "));
				}
				using (TimeWarning timeWarning1 = TimeWarning.New("BroadcastSignalFromClient", 0.1f))
				{
					using (TimeWarning timeWarning2 = TimeWarning.New("Conditions", 0.1f))
					{
						if (!BaseEntity.RPC_Server.FromOwner.Test("BroadcastSignalFromClient", this, player))
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
							this.BroadcastSignalFromClient(rPCMessage);
						}
					}
					catch (Exception exception)
					{
						player.Kick("RPC Error in BroadcastSignalFromClient");
						Debug.LogException(exception);
					}
				}
				flag = true;
			}
			else if (rpc != -649820255 || !(player != null))
			{
				return base.OnRpcMessage(player, rpc, msg);
			}
			else
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (ConVar.Global.developer > 2)
				{
					Debug.Log(string.Concat("SV_RPCMessage: ", player, " - SV_RequestFile "));
				}
				using (timeWarning1 = TimeWarning.New("SV_RequestFile", 0.1f))
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
							this.SV_RequestFile(rPCMessage);
						}
					}
					catch (Exception exception1)
					{
						player.Kick("RPC Error in SV_RequestFile");
						Debug.LogException(exception1);
					}
				}
				flag = true;
			}
		}
		return flag;
	}

	public virtual void OnSensation(Sensation sensation)
	{
	}

	private void OnSkinChanged(ulong oldSkinID, ulong newSkinID)
	{
		if (oldSkinID == newSkinID)
		{
			return;
		}
		this.skinID = newSkinID;
	}

	public virtual bool OnStartBeingLooted(BasePlayer baseEntity)
	{
		return true;
	}

	public bool ParentHasFlag(BaseEntity.Flags f)
	{
		BaseEntity parentEntity = base.GetParentEntity();
		if (parentEntity == null)
		{
			return false;
		}
		return parentEntity.HasFlag(f);
	}

	public virtual float PenetrationResistance(HitInfo info)
	{
		return 1f;
	}

	public Vector3 PivotPoint()
	{
		return base.transform.position;
	}

	public virtual void PostMapEntitySpawn()
	{
	}

	public virtual void PreProcess(IPrefabProcessor preProcess, GameObject rootObj, string name, bool serverside, bool clientside, bool bundling)
	{
		if (clientside && Skinnable.All != null && Skinnable.FindForEntity(name) != null)
		{
			Rust.Workshop.WorkshopSkin.Prepare(rootObj);
			MaterialReplacement.Prepare(rootObj);
		}
	}

	public virtual void Push(Vector3 velocity)
	{
		this.SetVelocity(velocity);
	}

	public virtual float RadiationExposureFraction()
	{
		return 1f;
	}

	public virtual float RadiationProtection()
	{
		return 0f;
	}

	protected void ReceiveCollisionMessages(bool b)
	{
		if (!b)
		{
			base.gameObject.transform.RemoveComponent<EntityCollisionMessage>();
			return;
		}
		base.gameObject.transform.GetOrAddComponent<EntityCollisionMessage>();
	}

	public bool ReceivedEntityLinkBroadcast()
	{
		return this.broadcastProtocol == BaseEntity.globalBroadcastProtocol;
	}

	public void RefreshEntityLinks()
	{
		using (TimeWarning timeWarning = TimeWarning.New("RefreshEntityLinks", 0.1f))
		{
			this.links.ClearLinks();
			this.LinkToNeighbours();
		}
	}

	public void RemoveFromTriggers()
	{
		if (this.triggers == null)
		{
			return;
		}
		using (TimeWarning timeWarning = TimeWarning.New("RemoveFromTriggers", 0.1f))
		{
			TriggerBase[] array = this.triggers.ToArray();
			for (int i = 0; i < (int)array.Length; i++)
			{
				TriggerBase triggerBase = array[i];
				if (triggerBase)
				{
					triggerBase.RemoveEntity(this);
				}
			}
			if (this.triggers != null && this.triggers.Count == 0)
			{
				Facepunch.Pool.FreeList<TriggerBase>(ref this.triggers);
			}
		}
	}

	public override void ResetState()
	{
		base.ResetState();
		this.parentBone = 0;
		this.OwnerID = (ulong)0;
		this.flags = (BaseEntity.Flags)0;
		this.parentEntity = new EntityRef();
		this._spawnable = null;
	}

	public override void Save(BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		info.msg.baseEntity = Facepunch.Pool.Get<ProtoBuf.BaseEntity>();
		if (!info.forDisk)
		{
			info.msg.baseEntity.pos = this.GetNetworkPosition();
			info.msg.baseEntity.rot = this.GetNetworkRotation().eulerAngles;
			info.msg.baseEntity.time = base.GetNetworkTime();
		}
		else
		{
			info.msg.baseEntity.pos = base.transform.localPosition;
			info.msg.baseEntity.rot = base.transform.localRotation.eulerAngles;
		}
		info.msg.baseEntity.flags = (int)this.flags;
		info.msg.baseEntity.skinid = this.skinID;
		if (this.parentEntity.IsValid(base.isServer))
		{
			info.msg.parent = Facepunch.Pool.Get<ParentInfo>();
			info.msg.parent.uid = this.parentEntity.uid;
			info.msg.parent.bone = this.parentBone;
		}
		if (this.HasAnySlot())
		{
			info.msg.entitySlots = Facepunch.Pool.Get<EntitySlots>();
			info.msg.entitySlots.slotLock = this.entitySlots[0].uid;
			info.msg.entitySlots.slotFireMod = this.entitySlots[1].uid;
			info.msg.entitySlots.slotUpperModification = this.entitySlots[2].uid;
			info.msg.entitySlots.centerDecoration = this.entitySlots[5].uid;
			info.msg.entitySlots.lowerCenterDecoration = this.entitySlots[6].uid;
		}
		if (info.forDisk && this._spawnable)
		{
			this._spawnable.Save(info);
		}
		if (this.OwnerID != 0 && (info.forDisk || this.ShouldNetworkOwnerInfo()))
		{
			info.msg.ownerInfo = Facepunch.Pool.Get<OwnerInfo>();
			info.msg.ownerInfo.steamid = this.OwnerID;
		}
	}

	private void SendChildrenNetworkUpdate()
	{
		if (this.children == null)
		{
			return;
		}
		foreach (BaseEntity child in this.children)
		{
			child.UpdateNetworkGroup();
			child.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
		}
	}

	private void SendChildrenNetworkUpdateImmediate()
	{
		if (this.children == null)
		{
			return;
		}
		foreach (BaseEntity child in this.children)
		{
			child.UpdateNetworkGroup();
			child.SendNetworkUpdateImmediate(false);
		}
	}

	protected void SendNetworkUpdate_Flags()
	{
		if (Rust.Application.isLoading)
		{
			return;
		}
		if (Rust.Application.isLoadingSave)
		{
			return;
		}
		if (base.IsDestroyed)
		{
			return;
		}
		if (this.net == null)
		{
			return;
		}
		if (!this.isSpawned)
		{
			return;
		}
		List<Connection> subscribers = base.GetSubscribers();
		if (subscribers == null || subscribers.Count == 0)
		{
			return;
		}
		base.LogEntry(BaseMonoBehaviour.LogEntryType.Network, 2, "SendNetworkUpdate_Flags");
		if (Network.Net.sv.write.Start())
		{
			Network.Net.sv.write.PacketID(Message.Type.EntityFlags);
			Network.Net.sv.write.EntityID(this.net.ID);
			Network.Net.sv.write.Int32((int)this.flags);
			Network.Net.sv.write.Send(new SendInfo(subscribers));
		}
	}

	public override void ServerInit()
	{
		this._spawnable = base.GetComponent<Spawnable>();
		base.ServerInit();
		if (this.enableSaving)
		{
			Assert.IsTrue(!BaseEntity.saveList.Contains(this), "Already in save list - server Init being called twice?");
			BaseEntity.saveList.Add(this);
		}
		if ((int)this.flags != 0)
		{
			this.OnFlagsChanged(0, this.flags);
		}
		if (this.syncPosition && this.PositionTickRate >= 0f)
		{
			base.InvokeRandomized(new Action(this.NetworkPositionTick), this.PositionTickRate, this.PositionTickRate - this.PositionTickRate * 0.05f, this.PositionTickRate * 0.05f);
		}
		BaseEntity.Query.Server.Add(this);
	}

	public virtual void SetAngularVelocity(Vector3 velocity)
	{
		Rigidbody component = base.GetComponent<Rigidbody>();
		if (component)
		{
			component.angularVelocity = velocity;
		}
	}

	public void SetFlag(BaseEntity.Flags f, bool b, bool recursive = false, bool networkupdate = true)
	{
		BaseEntity.Flags flag = this.flags;
		if (!b)
		{
			if (!this.HasFlag(f))
			{
				return;
			}
			this.flags &= ~f;
		}
		else
		{
			if (this.HasFlag(f))
			{
				return;
			}
			this.flags |= f;
		}
		this.OnFlagsChanged(flag, this.flags);
		if (networkupdate)
		{
			base.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
		}
		if (recursive && this.children != null)
		{
			for (int i = 0; i < this.children.Count; i++)
			{
				this.children[i].SetFlag(f, b, true, true);
			}
		}
	}

	public void SetModel(Model mdl)
	{
		if (this.model == mdl)
		{
			return;
		}
		this.model = mdl;
	}

	public void SetParent(BaseEntity entity, bool worldPositionStays = false, bool sendImmediate = false)
	{
		this.SetParent(entity, 0, worldPositionStays, sendImmediate);
	}

	public void SetParent(BaseEntity entity, string strBone, bool worldPositionStays = false, bool sendImmediate = false)
	{
		uint num;
		BaseEntity baseEntity = entity;
		if (string.IsNullOrEmpty(strBone))
		{
			num = 0;
		}
		else
		{
			num = StringPool.Get(strBone);
		}
		this.SetParent(baseEntity, num, worldPositionStays, sendImmediate);
	}

	public void SetParent(BaseEntity entity, uint boneID, bool worldPositionStays = false, bool sendImmediate = false)
	{
		if (entity != null)
		{
			if (entity == this)
			{
				Debug.LogError(string.Concat("Trying to parent to self ", this), base.gameObject);
				return;
			}
			if (this.HasChild(entity))
			{
				Debug.LogError(string.Concat("Trying to parent to child ", this), base.gameObject);
				return;
			}
		}
		base.LogEntry(BaseMonoBehaviour.LogEntryType.Hierarchy, 2, "SetParent {0} {1}", entity, boneID);
		BaseEntity parentEntity = base.GetParentEntity();
		if (parentEntity)
		{
			parentEntity.RemoveChild(this);
		}
		if (base.limitNetworking && parentEntity != null && parentEntity != entity)
		{
			BasePlayer basePlayer = parentEntity as BasePlayer;
			if (basePlayer.IsValid())
			{
				this.DestroyOnClient(basePlayer.net.connection);
			}
		}
		if (entity == null)
		{
			if (this.parentEntity.IsValid(base.isServer))
			{
				this.OnParentChanging(parentEntity, null);
				this.parentEntity.Set(null);
				base.transform.SetParent(null, worldPositionStays);
				this.parentBone = 0;
				this.UpdateNetworkGroup();
				if (sendImmediate)
				{
					base.SendNetworkUpdateImmediate(false);
					this.SendChildrenNetworkUpdateImmediate();
					return;
				}
				base.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
				this.SendChildrenNetworkUpdate();
			}
			return;
		}
		Debug.Assert(entity.isServer, "SetParent - child should be a SERVER entity");
		Debug.Assert(entity.net != null, "Setting parent to entity that hasn't spawned yet! (net is null)");
		Debug.Assert(entity.net.ID != 0, "Setting parent to entity that hasn't spawned yet! (id = 0)");
		entity.AddChild(this);
		this.OnParentChanging(parentEntity, entity);
		this.parentEntity.Set(entity);
		if (boneID == 0 || boneID == StringPool.closest)
		{
			base.transform.SetParent(entity.transform, worldPositionStays);
		}
		else
		{
			base.transform.SetParent(entity.FindBone(StringPool.Get(boneID)), worldPositionStays);
		}
		this.parentBone = boneID;
		this.UpdateNetworkGroup();
		if (sendImmediate)
		{
			base.SendNetworkUpdateImmediate(false);
			this.SendChildrenNetworkUpdateImmediate();
			return;
		}
		base.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
		this.SendChildrenNetworkUpdate();
	}

	public void SetSlot(BaseEntity.Slot slot, BaseEntity ent)
	{
		this.entitySlots[(int)slot].Set(ent);
		base.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
	}

	public virtual void SetVelocity(Vector3 velocity)
	{
		Rigidbody component = base.GetComponent<Rigidbody>();
		if (component)
		{
			component.velocity = velocity;
		}
	}

	public virtual bool ShouldBlockProjectiles()
	{
		return true;
	}

	public virtual bool ShouldInheritNetworkGroup()
	{
		return true;
	}

	public virtual bool ShouldNetworkOwnerInfo()
	{
		return false;
	}

	public override bool ShouldNetworkTo(BasePlayer player)
	{
		if (player == this)
		{
			return true;
		}
		BaseEntity parentEntity = base.GetParentEntity();
		if (base.limitNetworking)
		{
			if (parentEntity == null)
			{
				return false;
			}
			if (parentEntity != player)
			{
				return false;
			}
		}
		if (parentEntity != null)
		{
			return parentEntity.ShouldNetworkTo(player);
		}
		return base.ShouldNetworkTo(player);
	}

	public void SignalBroadcast(BaseEntity.Signal signal, string arg, Connection sourceConnection = null)
	{
		if (this.net == null)
		{
			return;
		}
		if (this.net.@group == null)
		{
			return;
		}
		SendInfo sendInfo = new SendInfo(this.net.@group.subscribers)
		{
			method = SendMethod.Unreliable,
			priority = Priority.Immediate
		};
		this.ClientRPCEx<int, string>(sendInfo, sourceConnection, "SignalFromServerEx", signal, arg);
	}

	public void SignalBroadcast(BaseEntity.Signal signal, Connection sourceConnection = null)
	{
		if (this.net == null)
		{
			return;
		}
		if (this.net.@group == null)
		{
			return;
		}
		SendInfo sendInfo = new SendInfo(this.net.@group.subscribers)
		{
			method = SendMethod.Unreliable,
			priority = Priority.Immediate
		};
		this.ClientRPCEx<int>(sendInfo, sourceConnection, "SignalFromServer", signal);
	}

	public override void Spawn()
	{
		base.Spawn();
		base.gameObject.BroadcastOnParentSpawning();
	}

	public void SpawnAsMapEntity()
	{
		UnityEngine.Object componentInParent;
		if (this.net != null)
		{
			return;
		}
		if (base.IsDestroyed)
		{
			return;
		}
		if (base.transform.parent != null)
		{
			componentInParent = base.transform.parent.GetComponentInParent<BaseEntity>();
		}
		else
		{
			componentInParent = null;
		}
		if (componentInParent == null)
		{
			base.transform.parent = null;
			SceneManager.MoveGameObjectToScene(base.gameObject, Rust.Server.EntityScene);
			base.gameObject.SetActive(true);
			this.Spawn();
		}
	}

	public float SqrDistance(Vector3 position)
	{
		return (this.ClosestPoint(position) - position).sqrMagnitude;
	}

	public float SqrDistance(BaseEntity other)
	{
		return this.SqrDistance(other.transform.position);
	}

	public float SqrDistance2D(Vector3 position)
	{
		return (this.ClosestPoint(position) - position).SqrMagnitude2D();
	}

	public float SqrDistance2D(BaseEntity other)
	{
		return this.SqrDistance(other.transform.position);
	}

	public virtual bool SupportsChildDeployables()
	{
		return true;
	}

	[RPC_Server]
	public void SV_RequestFile(BaseEntity.RPCMessage msg)
	{
		uint num = msg.read.UInt32();
		FileStorage.Type type = (FileStorage.Type)msg.read.UInt8();
		string str = StringPool.Get(msg.read.UInt32());
		byte[] numArray = FileStorage.server.Get(num, type, this.net.ID);
		if (numArray == null)
		{
			return;
		}
		SendInfo sendInfo = new SendInfo(msg.connection)
		{
			channel = 2,
			method = SendMethod.Reliable
		};
		this.ClientRPCEx<uint, uint, byte[]>(sendInfo, null, str, num, (uint)numArray.Length, numArray);
	}

	public void SV_RPCMessage(uint nameID, Message message)
	{
		Assert.IsTrue(base.isServer, "Should be server!");
		BasePlayer basePlayer = message.Player();
		if (!basePlayer.IsValid())
		{
			if (ConVar.Global.developer > 0)
			{
				Debug.Log(string.Concat("SV_RPCMessage: From invalid player ", basePlayer));
			}
			return;
		}
		if (basePlayer.isStalled)
		{
			if (ConVar.Global.developer > 0)
			{
				Debug.Log(string.Concat("SV_RPCMessage: player is stalled ", basePlayer));
			}
			return;
		}
		if (this.OnRpcMessage(basePlayer, nameID, message))
		{
			return;
		}
		for (int i = 0; i < (int)this.Components.Length; i++)
		{
			if (this.Components[i].OnRpcMessage(basePlayer, nameID, message))
			{
				return;
			}
		}
	}

	public virtual void SwitchParent(BaseEntity ent)
	{
		this.Log(string.Concat("SwitchParent Missed ", ent));
	}

	public virtual BasePlayer ToPlayer()
	{
		return null;
	}

	public override string ToString()
	{
		uint d;
		if (this._name == null)
		{
			if (!base.isServer)
			{
				this._name = base.ShortPrefabName;
			}
			else
			{
				if (this.net != null)
				{
					d = this.net.ID;
				}
				else
				{
					d = 0;
				}
				this._name = string.Format("{1}[{0}]", d, base.ShortPrefabName);
			}
		}
		return this._name;
	}

	public void TransformChanged()
	{
		if (BaseEntity.Query.Server != null)
		{
			BaseEntity.Query.Server.Move(this);
		}
		if (this.net == null)
		{
			return;
		}
		base.InvalidateNetworkCache();
		if (!this.globalBroadcast && !ValidBounds.Test(base.transform.position))
		{
			this.OnInvalidPosition();
			return;
		}
		if (this.syncPosition)
		{
			if (!this.isCallingUpdateNetworkGroup)
			{
				BaseEntity baseEntity = this;
				base.Invoke(new Action(baseEntity.UpdateNetworkGroup), 5f);
				this.isCallingUpdateNetworkGroup = true;
			}
			base.SendNetworkUpdate_Position();
			this.OnPositionalNetworkUpdate();
		}
	}

	public override void UpdateNetworkGroup()
	{
		this.isCallingUpdateNetworkGroup = false;
		if (this.net == null)
		{
			return;
		}
		if (Network.Net.sv == null)
		{
			return;
		}
		if (Network.Net.sv.visibility == null)
		{
			return;
		}
		using (TimeWarning timeWarning = TimeWarning.New("UpdateNetworkGroup", 0.1f))
		{
			if (this.globalBroadcast)
			{
				if (this.net.SwitchGroup(BaseNetworkable.GlobalNetworkGroup))
				{
					base.SendNetworkGroupChange();
					return;
				}
			}
			else if (!this.ShouldInheritNetworkGroup() || !this.parentEntity.IsSet())
			{
				base.UpdateNetworkGroup();
			}
			else
			{
				BaseEntity parentEntity = base.GetParentEntity();
				if (!parentEntity.IsValid())
				{
					Debug.LogWarning(string.Concat("UpdateNetworkGroup: Missing parent entity ", this.parentEntity.uid));
					BaseEntity baseEntity = this;
					base.Invoke(new Action(baseEntity.UpdateNetworkGroup), 2f);
					this.isCallingUpdateNetworkGroup = true;
					return;
				}
				else if (parentEntity == null)
				{
					Debug.LogWarning(string.Concat(base.gameObject, ": has parent id - but couldn't find parent! ", this.parentEntity));
					return;
				}
				else if (this.net.SwitchGroup(parentEntity.net.@group))
				{
					base.SendNetworkGroupChange();
					return;
				}
			}
		}
	}

	public virtual float WaterFactor()
	{
		return WaterLevel.Factor(this.WorldSpaceBounds().ToBounds());
	}

	public OBB WorldSpaceBounds()
	{
		return new OBB(base.transform.position, base.transform.lossyScale, base.transform.rotation, this.bounds);
	}

	[Flags]
	public enum Flags
	{
		Placeholder = 1,
		On = 2,
		OnFire = 4,
		Open = 8,
		Locked = 16,
		Debugging = 32,
		Disabled = 64,
		Reserved1 = 128,
		Reserved2 = 256,
		Reserved3 = 512,
		Reserved4 = 1024,
		Reserved5 = 2048,
		Broken = 4096,
		Busy = 8192,
		Reserved6 = 16384,
		Reserved7 = 32768,
		Reserved8 = 65536
	}

	public enum GiveItemReason
	{
		Generic,
		ResourceHarvested,
		PickedUp,
		Crafted
	}

	public class Menu : Attribute
	{
		public string TitleToken;

		public string TitleEnglish;

		public string UseVariable;

		public int Order;

		public string ProxyFunction;

		public float Time;

		public string OnStart;

		public string OnProgress;

		public bool LongUseOnly;

		public Menu()
		{
		}

		public Menu(string menuTitleToken, string menuTitleEnglish)
		{
			this.TitleToken = menuTitleToken;
			this.TitleEnglish = menuTitleEnglish;
		}

		public class Description : Attribute
		{
			public string token;

			public string english;

			public Description(string t, string e)
			{
				this.token = t;
				this.english = e;
			}
		}

		public class Icon : Attribute
		{
			public string icon;

			public Icon(string i)
			{
				this.icon = i;
			}
		}

		[Serializable]
		public struct Option
		{
			public Translate.Phrase name;

			public Translate.Phrase description;

			public Sprite icon;

			public int order;
		}

		public class ShowIf : Attribute
		{
			public string functionName;

			public ShowIf(string testFunc)
			{
				this.functionName = testFunc;
			}
		}
	}

	[Serializable]
	public struct MovementModify
	{
		public float drag;
	}

	public static class Query
	{
		public static BaseEntity.Query.EntityTree Server;

		public class EntityTree
		{
			private Grid<BaseEntity> Grid;

			private Grid<BasePlayer> PlayerGrid;

			public EntityTree(float worldSize)
			{
				this.Grid = new Grid<BaseEntity>(32, worldSize);
				this.PlayerGrid = new Grid<BasePlayer>(32, worldSize);
			}

			public void Add(BaseEntity ent)
			{
				Vector3 vector3 = ent.transform.position;
				this.Grid.Add(ent, vector3.x, vector3.z);
			}

			public void AddPlayer(BasePlayer player)
			{
				Vector3 vector3 = player.transform.position;
				this.PlayerGrid.Add(player, vector3.x, vector3.z);
			}

			public int GetInSphere(Vector3 position, float distance, BaseEntity[] results, Func<BaseEntity, bool> filter = null)
			{
				return this.Grid.Query(position.x, position.z, distance, results, filter);
			}

			public int GetPlayersInSphere(Vector3 position, float distance, BasePlayer[] results, Func<BasePlayer, bool> filter = null)
			{
				return this.PlayerGrid.Query(position.x, position.z, distance, results, filter);
			}

			public void Move(BaseEntity ent)
			{
				Vector3 vector3 = ent.transform.position;
				this.Grid.Move(ent, vector3.x, vector3.z);
				BasePlayer basePlayer = ent as BasePlayer;
				if (basePlayer != null)
				{
					this.MovePlayer(basePlayer);
				}
			}

			public void MovePlayer(BasePlayer player)
			{
				Vector3 vector3 = player.transform.position;
				this.PlayerGrid.Move(player, vector3.x, vector3.z);
			}

			public void Remove(BaseEntity ent, bool isPlayer = false)
			{
				this.Grid.Remove(ent);
				if (isPlayer)
				{
					BasePlayer basePlayer = ent as BasePlayer;
					if (basePlayer != null)
					{
						this.PlayerGrid.Remove(basePlayer);
					}
				}
			}

			public void RemovePlayer(BasePlayer player)
			{
				this.PlayerGrid.Remove(player);
			}
		}
	}

	public class RPC_Server : BaseEntity.RPC_Shared
	{
		public RPC_Server()
		{
		}

		public abstract class Conditional : Attribute
		{
			protected Conditional()
			{
			}

			public virtual string GetArgs()
			{
				return null;
			}
		}

		public class FromOwner : BaseEntity.RPC_Server.Conditional
		{
			public FromOwner()
			{
			}

			public static bool Test(string debugName, BaseEntity ent, BasePlayer player)
			{
				if (ent == null || player == null)
				{
					return false;
				}
				if (ent.net == null || player.net == null)
				{
					return false;
				}
				if (ent.net.ID == player.net.ID)
				{
					return true;
				}
				if (ent.parentEntity.uid != player.net.ID)
				{
					return false;
				}
				return true;
			}
		}

		public class IsActiveItem : BaseEntity.RPC_Server.Conditional
		{
			public IsActiveItem()
			{
			}

			public static bool Test(string debugName, BaseEntity ent, BasePlayer player)
			{
				if (ent == null || player == null)
				{
					return false;
				}
				if (ent.net == null || player.net == null)
				{
					return false;
				}
				if (ent.net.ID == player.net.ID)
				{
					return true;
				}
				if (ent.parentEntity.uid != player.net.ID)
				{
					return false;
				}
				Item activeItem = player.GetActiveItem();
				if (activeItem == null)
				{
					return false;
				}
				if (activeItem.GetHeldEntity() != ent)
				{
					return false;
				}
				return true;
			}
		}

		public class IsVisible : BaseEntity.RPC_Server.Conditional
		{
			private float maximumDistance;

			public IsVisible(float maxDist)
			{
				this.maximumDistance = maxDist;
			}

			public override string GetArgs()
			{
				return this.maximumDistance.ToString("0.00f");
			}

			public static bool Test(string debugName, BaseEntity ent, BasePlayer player, float maximumDistance)
			{
				if (ent == null || player == null)
				{
					return false;
				}
				if (!GamePhysics.LineOfSight(player.eyes.center, player.eyes.position, 2162688, 0f))
				{
					return false;
				}
				if (ent.IsVisible(player.eyes.HeadRay(), maximumDistance))
				{
					return true;
				}
				return ent.IsVisible(player.eyes.position, maximumDistance);
			}
		}

		public class MaxDistance : BaseEntity.RPC_Server.Conditional
		{
			private float maximumDistance;

			public MaxDistance(float maxDist)
			{
				this.maximumDistance = maxDist;
			}

			public override string GetArgs()
			{
				return this.maximumDistance.ToString("0.00f");
			}

			public static bool Test(string debugName, BaseEntity ent, BasePlayer player, float maximumDistance)
			{
				if (ent == null || player == null)
				{
					return false;
				}
				return ent.Distance(player.eyes.position) <= maximumDistance;
			}
		}
	}

	public class RPC_Shared : Attribute
	{
		public RPC_Shared()
		{
		}
	}

	public struct RPCMessage
	{
		public Connection connection;

		public BasePlayer player;

		public Read read;
	}

	public enum Signal
	{
		Attack,
		Alt_Attack,
		DryFire,
		Reload,
		Deploy,
		Flinch_Head,
		Flinch_Chest,
		Flinch_Stomach,
		Flinch_RearHead,
		Flinch_RearTorso,
		Throw,
		Relax,
		Gesture,
		PhysImpact,
		Eat,
		Startled
	}

	public enum Slot
	{
		Lock,
		FireMod,
		UpperModifier,
		MiddleModifier,
		LowerModifier,
		CenterDecoration,
		LowerCenterDecoration,
		Count
	}

	[Flags]
	public enum TraitFlag
	{
		None = 0,
		Alive = 1,
		Animal = 2,
		Human = 4,
		Interesting = 8,
		Food = 16,
		Meat = 32,
		Water = 32
	}

	public static class Util
	{
		public static BaseEntity[] FindTargets(string strFilter, bool onlyPlayers)
		{
			return BaseNetworkable.serverEntities.Where<BaseNetworkable>((BaseNetworkable x) => {
				if (!(x is BasePlayer))
				{
					if (onlyPlayers)
					{
						return false;
					}
					if (string.IsNullOrEmpty(strFilter))
					{
						return false;
					}
					if (x.ShortPrefabName.Contains(strFilter))
					{
						return true;
					}
					return false;
				}
				BasePlayer basePlayer = x as BasePlayer;
				if (string.IsNullOrEmpty(strFilter))
				{
					return true;
				}
				if (strFilter == "!alive" && basePlayer.IsAlive())
				{
					return true;
				}
				if (strFilter == "!sleeping" && basePlayer.IsSleeping())
				{
					return true;
				}
				if (strFilter[0] != '!' && !basePlayer.displayName.Contains(strFilter, CompareOptions.IgnoreCase) && !basePlayer.UserIDString.Contains(strFilter))
				{
					return false;
				}
				return true;
			}).Select<BaseNetworkable, BaseEntity>((BaseNetworkable x) => x as BaseEntity).ToArray<BaseEntity>();
		}
	}
}