using ConVar;
using Facepunch;
using Network;
using Network.Visibility;
using Oxide.Core;
using ProtoBuf;
using Rust;
using Rust.Registry;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using UnityEngine;
using UnityEngine.Assertions;

public abstract class BaseNetworkable : BaseMonoBehaviour, IEntity, NetworkHandler
{
	[Header("BaseNetworkable")]
	[ReadOnly]
	public uint prefabID;

	[Tooltip("If enabled the entity will send to everyone on the server - regardless of position")]
	public bool globalBroadcast;

	[NonSerialized]
	public Networkable net;

	private string _prefabName;

	private string _prefabNameWithoutExtension;

	public static BaseNetworkable.EntityRealm serverEntities;

	private const bool isServersideEntity = true;

	public bool _limitedNetworking;

	[NonSerialized]
	public EntityRef parentEntity;

	[NonSerialized]
	public List<BaseEntity> children = new List<BaseEntity>();

	public int creationFrame;

	public bool isSpawned;

	private MemoryStream _NetworkCache;

	public static Queue<MemoryStream> EntityMemoryStreamPool;

	private MemoryStream _SaveCache;

	public GameManager gameManager
	{
		get
		{
			if (!this.isServer)
			{
				throw new NotImplementedException("Missing gameManager path");
			}
			return GameManager.server;
		}
	}

	public static Group GlobalNetworkGroup
	{
		get
		{
			return Network.Net.sv.visibility.Get(0);
		}
	}

	public bool isClient
	{
		get
		{
			return false;
		}
	}

	public bool IsDestroyed
	{
		get
		{
			return JustDecompileGenerated_get_IsDestroyed();
		}
		set
		{
			JustDecompileGenerated_set_IsDestroyed(value);
		}
	}

	private bool JustDecompileGenerated_IsDestroyed_k__BackingField;

	public bool JustDecompileGenerated_get_IsDestroyed()
	{
		return this.JustDecompileGenerated_IsDestroyed_k__BackingField;
	}

	private void JustDecompileGenerated_set_IsDestroyed(bool value)
	{
		this.JustDecompileGenerated_IsDestroyed_k__BackingField = value;
	}

	public bool isServer
	{
		get
		{
			return true;
		}
	}

	public static Group LimboNetworkGroup
	{
		get
		{
			return Network.Net.sv.visibility.Get(1);
		}
	}

	public bool limitNetworking
	{
		get
		{
			return this._limitedNetworking;
		}
		set
		{
			if (value == this._limitedNetworking)
			{
				return;
			}
			this._limitedNetworking = value;
			if (this._limitedNetworking)
			{
				this.OnNetworkLimitStart();
				return;
			}
			this.OnNetworkLimitEnd();
		}
	}

	public PrefabAttribute.Library prefabAttribute
	{
		get
		{
			if (!this.isServer)
			{
				throw new NotImplementedException("Missing prefabAttribute path");
			}
			return PrefabAttribute.server;
		}
	}

	public string PrefabName
	{
		get
		{
			if (this._prefabName == null)
			{
				this._prefabName = StringPool.Get(this.prefabID);
			}
			return this._prefabName;
		}
	}

	public string ShortPrefabName
	{
		get
		{
			if (this._prefabNameWithoutExtension == null)
			{
				this._prefabNameWithoutExtension = Path.GetFileNameWithoutExtension(this.PrefabName);
			}
			return this._prefabNameWithoutExtension;
		}
	}

	static BaseNetworkable()
	{
		BaseNetworkable.serverEntities = new BaseNetworkable.EntityRealmServer();
		BaseNetworkable.EntityMemoryStreamPool = new Queue<MemoryStream>();
	}

	protected BaseNetworkable()
	{
	}

	public void AddChild(BaseEntity child)
	{
		if (this.children.Contains(child))
		{
			return;
		}
		this.children.Add(child);
	}

	public virtual bool CanUseNetworkCache(Connection connection)
	{
		return ConVar.Server.netcache;
	}

	public virtual void DestroyShared()
	{
	}

	private void DoEntityDestroy()
	{
		if (this.IsDestroyed)
		{
			return;
		}
		this.IsDestroyed = true;
		if (Rust.Application.isQuitting)
		{
			return;
		}
		this.DestroyShared();
		if (this.isServer)
		{
			this.DoServerDestroy();
		}
		using (TimeWarning timeWarning = TimeWarning.New("Registry.Entity.Unregister", 0.1f))
		{
			Rust.Registry.Entity.Unregister(base.gameObject);
		}
	}

	internal virtual void DoServerDestroy()
	{
		this.isSpawned = false;
	}

	private void EntityDestroy()
	{
		if (base.gameObject)
		{
			this.ResetState();
			this.gameManager.Retire(base.gameObject);
		}
	}

	public bool EqualNetID(BaseNetworkable other)
	{
		if (!(other != null) || other.net == null || this.net == null)
		{
			return false;
		}
		return other.net.ID == this.net.ID;
	}

	public static IEnumerable<Connection> GetConnectionsWithin(Vector3 position, float distance)
	{
		float single = distance * distance;
		List<Connection> globalNetworkGroup = BaseNetworkable.GlobalNetworkGroup.subscribers;
		for (int i = 0; i < globalNetworkGroup.Count; i++)
		{
			Connection item = globalNetworkGroup[i];
			if (item.active)
			{
				BasePlayer basePlayer = item.player as BasePlayer;
				if (!(basePlayer == null) && basePlayer.SqrDistance(position) <= single)
				{
					yield return item;
				}
			}
		}
	}

	public virtual Vector3 GetNetworkPosition()
	{
		return base.transform.localPosition;
	}

	public virtual Quaternion GetNetworkRotation()
	{
		return base.transform.localRotation;
	}

	public float GetNetworkTime()
	{
		if (this.PhysicsDriven())
		{
			return UnityEngine.Time.fixedTime;
		}
		return UnityEngine.Time.time;
	}

	public BaseEntity GetParentEntity()
	{
		return this.parentEntity.Get(this.isServer);
	}

	public MemoryStream GetSaveCache()
	{
		if (this._SaveCache == null)
		{
			if (BaseNetworkable.EntityMemoryStreamPool.Count <= 0)
			{
				this._SaveCache = new MemoryStream(8);
			}
			else
			{
				this._SaveCache = BaseNetworkable.EntityMemoryStreamPool.Dequeue();
			}
			BaseNetworkable.SaveInfo saveInfo = new BaseNetworkable.SaveInfo()
			{
				forDisk = true
			};
			this.ToStream(this._SaveCache, saveInfo);
			ConVar.Server.savecachesize += (int)this._SaveCache.Length;
		}
		return this._SaveCache;
	}

	protected List<Connection> GetSubscribers()
	{
		if (this.net == null)
		{
			return null;
		}
		if (this.net.@group == null)
		{
			return null;
		}
		return this.net.@group.subscribers;
	}

	public bool HasParent()
	{
		return this.parentEntity.IsValid(this.isServer);
	}

	public void InitLoad(uint entityID)
	{
		this.net = Network.Net.sv.CreateNetworkable(entityID);
		BaseNetworkable.serverEntities.RegisterID(this);
		this.PreServerLoad();
	}

	public virtual void InitShared()
	{
	}

	public void InvalidateNetworkCache()
	{
		using (TimeWarning timeWarning = TimeWarning.New("InvalidateNetworkCache", 0.1f))
		{
			if (this._SaveCache != null)
			{
				ConVar.Server.savecachesize -= (int)this._SaveCache.Length;
				this._SaveCache.SetLength((long)0);
				this._SaveCache.Position = (long)0;
				BaseNetworkable.EntityMemoryStreamPool.Enqueue(this._SaveCache);
				this._SaveCache = null;
			}
			if (this._NetworkCache != null)
			{
				ConVar.Server.netcachesize -= (int)this._NetworkCache.Length;
				this._NetworkCache.SetLength((long)0);
				this._NetworkCache.Position = (long)0;
				BaseNetworkable.EntityMemoryStreamPool.Enqueue(this._NetworkCache);
				this._NetworkCache = null;
			}
			base.LogEntry(BaseMonoBehaviour.LogEntryType.Network, 3, "InvalidateNetworkCache");
		}
	}

	public string InvokeString()
	{
		StringBuilder stringBuilder = new StringBuilder();
		List<InvokeAction> list = Facepunch.Pool.GetList<InvokeAction>();
		InvokeHandler.FindInvokes(this, list);
		foreach (InvokeAction invokeAction in list)
		{
			if (stringBuilder.Length > 0)
			{
				stringBuilder.Append(", ");
			}
			stringBuilder.Append(invokeAction.action.Method.Name);
		}
		Facepunch.Pool.FreeList<InvokeAction>(ref list);
		return stringBuilder.ToString();
	}

	public bool IsFullySpawned()
	{
		return this.isSpawned;
	}

	public void Kill(BaseNetworkable.DestroyMode mode = 0)
	{
		if (this.IsDestroyed)
		{
			UnityEngine.Debug.LogWarning(string.Concat("Calling kill - but already IsDestroyed!? ", this));
			return;
		}
		Interface.CallHook("OnEntityKill", this);
		base.gameObject.BroadcastOnParentDestroying();
		this.DoEntityDestroy();
		this.TerminateOnClient(mode);
		this.TerminateOnServer();
		this.EntityDestroy();
	}

	public void KillMessage()
	{
		this.Kill(BaseNetworkable.DestroyMode.None);
	}

	public virtual void Load(BaseNetworkable.LoadInfo info)
	{
		if (info.msg.baseNetworkable == null)
		{
			return;
		}
		ProtoBuf.BaseNetworkable baseNetworkable = info.msg.baseNetworkable;
		if (this.prefabID != baseNetworkable.prefabID)
		{
			UnityEngine.Debug.LogError(string.Concat(new object[] { "Prefab IDs don't match! ", this.prefabID, "/", baseNetworkable.prefabID, " -> ", base.gameObject }), base.gameObject);
		}
	}

	public BaseEntity LookupPrefab()
	{
		return this.gameManager.FindPrefab(this.PrefabName).ToBaseEntity();
	}

	public void OnNetworkGroupChange()
	{
		if (this.children != null)
		{
			foreach (BaseEntity child in this.children)
			{
				if (!child.ShouldInheritNetworkGroup())
				{
					continue;
				}
				child.net.SwitchGroup(this.net.@group);
			}
		}
	}

	public virtual void OnNetworkGroupEnter(Group group)
	{
	}

	public virtual void OnNetworkGroupLeave(Group group)
	{
	}

	private void OnNetworkLimitEnd()
	{
		base.LogEntry(BaseMonoBehaviour.LogEntryType.Network, 2, "OnNetworkLimitEnd");
		List<Connection> subscribers = this.GetSubscribers();
		if (subscribers == null)
		{
			return;
		}
		this.OnNetworkSubscribersEnter(subscribers);
		if (this.children != null)
		{
			foreach (BaseEntity child in this.children)
			{
				child.OnNetworkLimitEnd();
			}
		}
	}

	private void OnNetworkLimitStart()
	{
		base.LogEntry(BaseMonoBehaviour.LogEntryType.Network, 2, "OnNetworkLimitStart");
		List<Connection> subscribers = this.GetSubscribers();
		if (subscribers == null)
		{
			return;
		}
		subscribers = subscribers.ToList<Connection>();
		subscribers.RemoveAll((Connection x) => this.ShouldNetworkTo(x.player as BasePlayer));
		this.OnNetworkSubscribersLeave(subscribers);
		if (this.children != null)
		{
			foreach (BaseEntity child in this.children)
			{
				child.OnNetworkLimitStart();
			}
		}
	}

	public void OnNetworkSubscribersEnter(List<Connection> connections)
	{
		if (!Network.Net.sv.IsConnected())
		{
			return;
		}
		foreach (Connection connection in connections)
		{
			BasePlayer basePlayer = connection.player as BasePlayer;
			if (basePlayer == null)
			{
				continue;
			}
			basePlayer.QueueUpdate(BasePlayer.NetworkQueue.Update, this as BaseEntity);
		}
	}

	public void OnNetworkSubscribersLeave(List<Connection> connections)
	{
		if (!Network.Net.sv.IsConnected())
		{
			return;
		}
		base.LogEntry(BaseMonoBehaviour.LogEntryType.Network, 2, "LeaveVisibility");
		if (Network.Net.sv.write.Start())
		{
			Network.Net.sv.write.PacketID(Message.Type.EntityDestroy);
			Network.Net.sv.write.EntityID(this.net.ID);
			Network.Net.sv.write.UInt8(0);
			Network.Net.sv.write.Send(new SendInfo(connections));
		}
	}

	public virtual bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		return false;
	}

	public virtual bool PhysicsDriven()
	{
		return false;
	}

	public virtual void PostInitShared()
	{
	}

	public virtual void PostSave(BaseNetworkable.SaveInfo info)
	{
	}

	public virtual void PostServerLoad()
	{
		base.gameObject.SendOnSendNetworkUpdate(this as BaseEntity);
	}

	public virtual void PreInitShared()
	{
	}

	public virtual void PreServerLoad()
	{
	}

	public void RemoveChild(BaseEntity child)
	{
		this.children.Remove(child);
	}

	public virtual void ResetState()
	{
		if (this.children.Count > 0)
		{
			this.children.Clear();
		}
	}

	public virtual void Save(BaseNetworkable.SaveInfo info)
	{
		if (this.prefabID == 0)
		{
			UnityEngine.Debug.LogError(string.Concat("PrefabID is 0! ", base.transform.GetRecursiveName("")), base.gameObject);
		}
		info.msg.baseNetworkable = Facepunch.Pool.Get<ProtoBuf.BaseNetworkable>();
		info.msg.baseNetworkable.uid = this.net.ID;
		info.msg.baseNetworkable.prefabID = this.prefabID;
		if (this.net.@group != null)
		{
			info.msg.baseNetworkable.@group = this.net.@group.ID;
		}
		if (!info.forDisk)
		{
			info.msg.createdThisFrame = this.creationFrame == UnityEngine.Time.frameCount;
		}
	}

	protected void SendAsSnapshot(Connection connection, bool justCreated = false)
	{
		if (Network.Net.sv.write.Start())
		{
			connection.validate.entityUpdates++;
			BaseNetworkable.SaveInfo saveInfo = new BaseNetworkable.SaveInfo()
			{
				forConnection = connection,
				forDisk = false
			};
			BaseNetworkable.SaveInfo saveInfo1 = saveInfo;
			Network.Net.sv.write.PacketID(Message.Type.Entities);
			Network.Net.sv.write.UInt32(connection.validate.entityUpdates);
			this.ToStreamForNetwork(Network.Net.sv.write, saveInfo1);
			Network.Net.sv.write.Send(new SendInfo(connection));
		}
	}

	protected void SendNetworkGroupChange()
	{
		if (!this.isSpawned)
		{
			return;
		}
		if (!Network.Net.sv.IsConnected())
		{
			return;
		}
		if (this.net.@group == null)
		{
			UnityEngine.Debug.LogWarning(string.Concat(this.ToString(), " changed its network group to null"));
			return;
		}
		if (Network.Net.sv.write.Start())
		{
			Network.Net.sv.write.PacketID(Message.Type.GroupChange);
			Network.Net.sv.write.EntityID(this.net.ID);
			Network.Net.sv.write.GroupID(this.net.@group.ID);
			Network.Net.sv.write.Send(new SendInfo(this.net.@group.subscribers));
		}
	}

	public void SendNetworkUpdate(BasePlayer.NetworkQueue queue = 0)
	{
		if (Rust.Application.isLoading)
		{
			return;
		}
		if (Rust.Application.isLoadingSave)
		{
			return;
		}
		if (this.IsDestroyed)
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
		using (TimeWarning timeWarning = TimeWarning.New("SendNetworkUpdate", 0.1f))
		{
			base.LogEntry(BaseMonoBehaviour.LogEntryType.Network, 2, "SendNetworkUpdate");
			this.InvalidateNetworkCache();
			List<Connection> subscribers = this.GetSubscribers();
			if (subscribers != null && subscribers.Count > 0)
			{
				for (int i = 0; i < subscribers.Count; i++)
				{
					BasePlayer item = subscribers[i].player as BasePlayer;
					if (!(item == null) && this.ShouldNetworkTo(item))
					{
						item.QueueUpdate(queue, this);
					}
				}
			}
		}
		base.gameObject.SendOnSendNetworkUpdate(this as BaseEntity);
	}

	protected void SendNetworkUpdate_Position()
	{
		if (Rust.Application.isLoading)
		{
			return;
		}
		if (Rust.Application.isLoadingSave)
		{
			return;
		}
		if (this.IsDestroyed)
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
		List<Connection> subscribers = this.GetSubscribers();
		if (subscribers == null || subscribers.Count == 0)
		{
			return;
		}
		base.LogEntry(BaseMonoBehaviour.LogEntryType.Network, 2, "SendNetworkUpdate_Position");
		if (Network.Net.sv.write.Start())
		{
			Network.Net.sv.write.PacketID(Message.Type.EntityPosition);
			Network.Net.sv.write.EntityID(this.net.ID);
			Network.Net.sv.write.Vector3(this.GetNetworkPosition());
			Network.Net.sv.write.Vector3(this.GetNetworkRotation().eulerAngles);
			Network.Net.sv.write.Float(this.GetNetworkTime());
			uint num = this.parentEntity.uid;
			if (num > 0)
			{
				Network.Net.sv.write.EntityID(num);
			}
			Write write = Network.Net.sv.write;
			SendInfo sendInfo = new SendInfo(subscribers)
			{
				method = SendMethod.ReliableUnordered,
				priority = Priority.Immediate
			};
			write.Send(sendInfo);
		}
	}

	public void SendNetworkUpdateImmediate(bool justCreated = false)
	{
		if (Rust.Application.isLoading)
		{
			return;
		}
		if (Rust.Application.isLoadingSave)
		{
			return;
		}
		if (this.IsDestroyed)
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
		using (TimeWarning timeWarning = TimeWarning.New("SendNetworkUpdateImmediate", 0.1f))
		{
			base.LogEntry(BaseMonoBehaviour.LogEntryType.Network, 2, "SendNetworkUpdateImmediate");
			this.InvalidateNetworkCache();
			List<Connection> subscribers = this.GetSubscribers();
			if (subscribers != null && subscribers.Count > 0)
			{
				for (int i = 0; i < subscribers.Count; i++)
				{
					Connection item = subscribers[i];
					BasePlayer basePlayer = item.player as BasePlayer;
					if (!(basePlayer == null) && this.ShouldNetworkTo(basePlayer))
					{
						this.SendAsSnapshot(item, justCreated);
					}
				}
			}
		}
		base.gameObject.SendOnSendNetworkUpdate(this as BaseEntity);
	}

	public virtual void ServerInit()
	{
		BaseNetworkable.serverEntities.RegisterID(this);
		if (this.net != null)
		{
			this.net.handler = this;
		}
	}

	public virtual bool ShouldNetworkTo(BasePlayer player)
	{
		object obj = Interface.CallHook("CanNetworkTo", this, player);
		if (!(obj as bool))
		{
			return true;
		}
		return (bool)obj;
	}

	public virtual void Spawn()
	{
		this.SpawnShared();
		if (this.net == null)
		{
			this.net = Network.Net.sv.CreateNetworkable();
		}
		this.creationFrame = UnityEngine.Time.frameCount;
		this.PreInitShared();
		this.InitShared();
		this.ServerInit();
		this.PostInitShared();
		this.UpdateNetworkGroup();
		this.isSpawned = true;
		Interface.CallHook("OnEntitySpawned", this);
		this.SendNetworkUpdateImmediate(true);
		if (Rust.Application.isLoading && !Rust.Application.isLoadingSave)
		{
			base.gameObject.SendOnSendNetworkUpdate(this as BaseEntity);
		}
	}

	private void SpawnShared()
	{
		this.IsDestroyed = false;
		using (TimeWarning timeWarning = TimeWarning.New("Registry.Entity.Register", 0.1f))
		{
			Rust.Registry.Entity.Register(base.gameObject, this);
		}
	}

	private void TerminateOnClient(BaseNetworkable.DestroyMode mode)
	{
		if (this.net == null)
		{
			return;
		}
		if (this.net.@group == null)
		{
			return;
		}
		if (!Network.Net.sv.IsConnected())
		{
			return;
		}
		base.LogEntry(BaseMonoBehaviour.LogEntryType.Network, 2, "Term {0}", mode);
		if (Network.Net.sv.write.Start())
		{
			Network.Net.sv.write.PacketID(Message.Type.EntityDestroy);
			Network.Net.sv.write.EntityID(this.net.ID);
			Network.Net.sv.write.UInt8((byte)mode);
			Network.Net.sv.write.Send(new SendInfo(this.net.@group.subscribers));
		}
	}

	private void TerminateOnServer()
	{
		if (this.net == null)
		{
			return;
		}
		this.InvalidateNetworkCache();
		BaseNetworkable.serverEntities.UnregisterID(this);
		Network.Net.sv.DestroyNetworkable(ref this.net);
		base.StopAllCoroutines();
		base.gameObject.SetActive(false);
	}

	public T ToServer<T>()
	where T : BaseNetworkable
	{
		if (this.isServer)
		{
			return (T)(this as T);
		}
		return default(T);
	}

	private void ToStream(Stream stream, BaseNetworkable.SaveInfo saveInfo)
	{
		ProtoBuf.Entity entity = Facepunch.Pool.Get<ProtoBuf.Entity>();
		ProtoBuf.Entity entity1 = entity;
		saveInfo.msg = entity;
		using (entity1)
		{
			this.Save(saveInfo);
			if (saveInfo.msg.baseEntity == null)
			{
				UnityEngine.Debug.LogError(string.Concat(this, ": ToStream - no BaseEntity!?"));
			}
			if (saveInfo.msg.baseNetworkable == null)
			{
				UnityEngine.Debug.LogError(string.Concat(this, ": ToStream - no baseNetworkable!?"));
			}
			saveInfo.msg.ToProto(stream);
			this.PostSave(saveInfo);
		}
	}

	public void ToStreamForNetwork(Stream stream, BaseNetworkable.SaveInfo saveInfo)
	{
		MemoryStream memoryStream;
		if (!this.CanUseNetworkCache(saveInfo.forConnection))
		{
			this.ToStream(stream, saveInfo);
			return;
		}
		if (this._NetworkCache == null)
		{
			if (BaseNetworkable.EntityMemoryStreamPool.Count > 0)
			{
				MemoryStream memoryStream1 = BaseNetworkable.EntityMemoryStreamPool.Dequeue();
				MemoryStream memoryStream2 = memoryStream1;
				this._NetworkCache = memoryStream1;
				memoryStream = memoryStream2;
			}
			else
			{
				memoryStream = new MemoryStream(8);
			}
			this._NetworkCache = memoryStream;
			this.ToStream(this._NetworkCache, saveInfo);
			ConVar.Server.netcachesize += (int)this._NetworkCache.Length;
		}
		this._NetworkCache.WriteTo(stream);
	}

	public virtual void UpdateNetworkGroup()
	{
		UnityEngine.Assertions.Assert.IsTrue(this.isServer, "UpdateNetworkGroup called on clientside entity!");
		if (this.net == null)
		{
			return;
		}
		using (TimeWarning timeWarning = TimeWarning.New("UpdateGroups", 0.1f))
		{
			if (this.net.UpdateGroups(base.transform.position))
			{
				this.SendNetworkGroupChange();
			}
		}
	}

	public enum DestroyMode : byte
	{
		None,
		Gib
	}

	public abstract class EntityRealm : IEnumerable<BaseNetworkable>, IEnumerable
	{
		private ListDictionary<uint, BaseNetworkable> entityList;

		public int Count
		{
			get
			{
				return this.entityList.Count;
			}
		}

		protected abstract Manager visibilityManager
		{
			get;
		}

		protected EntityRealm()
		{
		}

		public BaseNetworkable Find(uint uid)
		{
			BaseNetworkable baseNetworkable = null;
			if (!this.entityList.TryGetValue(uid, out baseNetworkable))
			{
				return null;
			}
			return baseNetworkable;
		}

		public Group FindGroup(uint uid)
		{
			Manager manager = this.visibilityManager;
			if (manager == null)
			{
				return null;
			}
			return manager.Get(uid);
		}

		public void FindInGroup(uint uid, List<BaseNetworkable> list)
		{
			Group group = this.TryFindGroup(uid);
			if (group == null)
			{
				return;
			}
			int count = group.networkables.Values.Count;
			Networkable[] buffer = group.networkables.Values.Buffer;
			for (int i = 0; i < count; i++)
			{
				BaseNetworkable baseNetworkable = this.Find(buffer[i].ID);
				if (!(baseNetworkable == null) && baseNetworkable.net != null && baseNetworkable.net.@group != null)
				{
					if (baseNetworkable.net.@group.ID == uid)
					{
						list.Add(baseNetworkable);
					}
					else
					{
						UnityEngine.Debug.LogWarning(string.Concat("Group ID mismatch: ", baseNetworkable.ToString()));
					}
				}
			}
		}

		public IEnumerator<BaseNetworkable> GetEnumerator()
		{
			return this.entityList.Values.GetEnumerator();
		}

		public void RegisterID(BaseNetworkable ent)
		{
			if (ent.net != null)
			{
				if (this.entityList.Contains(ent.net.ID))
				{
					this.entityList[ent.net.ID] = ent;
					return;
				}
				this.entityList.Add(ent.net.ID, ent);
			}
		}

		IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		public Group TryFindGroup(uint uid)
		{
			Manager manager = this.visibilityManager;
			if (manager == null)
			{
				return null;
			}
			return manager.TryGet(uid);
		}

		public void UnregisterID(BaseNetworkable ent)
		{
			if (ent.net != null)
			{
				this.entityList.Remove(ent.net.ID);
			}
		}
	}

	public class EntityRealmServer : BaseNetworkable.EntityRealm
	{
		protected override Manager visibilityManager
		{
			get
			{
				if (Network.Net.sv == null)
				{
					return null;
				}
				return Network.Net.sv.visibility;
			}
		}

		public EntityRealmServer()
		{
		}
	}

	public struct LoadInfo
	{
		public ProtoBuf.Entity msg;

		public bool fromDisk;
	}

	public struct SaveInfo
	{
		public ProtoBuf.Entity msg;

		public bool forDisk;

		public Connection forConnection;

		internal bool SendingTo(Connection ownerConnection)
		{
			if (ownerConnection == null)
			{
				return false;
			}
			if (this.forConnection == null)
			{
				return false;
			}
			return this.forConnection == ownerConnection;
		}
	}
}