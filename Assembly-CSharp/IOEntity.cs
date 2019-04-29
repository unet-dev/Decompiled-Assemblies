using ConVar;
using Facepunch;
using Network;
using ProtoBuf;
using Rust;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class IOEntity : BaseCombatEntity
{
	[Header("IOEntity")]
	public Transform debugOrigin;

	public ItemDefinition sourceItem;

	[NonSerialized]
	public int lastResetIndex;

	[Help("How many miliseconds to budget for processing io entities per server frame")]
	[ServerVar]
	public static float framebudgetms;

	[ServerVar]
	public static float responsetime;

	[ServerVar]
	public static int backtracking;

	public const BaseEntity.Flags Flag_ShortCircuit = BaseEntity.Flags.Reserved7;

	public const BaseEntity.Flags Flag_HasPower = BaseEntity.Flags.Reserved8;

	public IOEntity.IOSlot[] inputs;

	public IOEntity.IOSlot[] outputs;

	public IOEntity.IOType ioType;

	public static Queue<IOEntity> _processQueue;

	private int cachedOutputsUsed;

	private int lastPassthroughEnergy;

	private int lastEnergy;

	protected int currentEnergy;

	private float lastUpdateTime;

	protected bool ensureOutputsUpdated;

	static IOEntity()
	{
		IOEntity.framebudgetms = 1f;
		IOEntity.responsetime = 0.1f;
		IOEntity.backtracking = 8;
		IOEntity._processQueue = new Queue<IOEntity>();
	}

	public IOEntity()
	{
	}

	public void ClearConnections()
	{
		int i;
		IOEntity.IOSlot[] oSlotArray;
		int j;
		List<IOEntity> oEntities = new List<IOEntity>();
		IOEntity.IOSlot[] oSlotArray1 = this.inputs;
		for (i = 0; i < (int)oSlotArray1.Length; i++)
		{
			IOEntity.IOSlot oSlot = oSlotArray1[i];
			IOEntity oEntity = null;
			if (oSlot.connectedTo.Get(true) != null)
			{
				oEntity = oSlot.connectedTo.Get(true);
				oSlotArray = oSlot.connectedTo.Get(true).outputs;
				for (j = 0; j < (int)oSlotArray.Length; j++)
				{
					IOEntity.IOSlot oSlot1 = oSlotArray[j];
					if (oSlot1.connectedTo.Get(true) != null && oSlot1.connectedTo.Get(true).EqualNetID(this))
					{
						oSlot1.Clear();
					}
				}
			}
			oSlot.Clear();
			if (oEntity)
			{
				oEntity.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
			}
		}
		oSlotArray1 = this.outputs;
		for (i = 0; i < (int)oSlotArray1.Length; i++)
		{
			IOEntity.IOSlot oSlot2 = oSlotArray1[i];
			if (oSlot2.connectedTo.Get(true) != null)
			{
				oEntities.Add(oSlot2.connectedTo.Get(true));
				oSlotArray = oSlot2.connectedTo.Get(true).inputs;
				for (j = 0; j < (int)oSlotArray.Length; j++)
				{
					IOEntity.IOSlot oSlot3 = oSlotArray[j];
					if (oSlot3.connectedTo.Get(true) != null && oSlot3.connectedTo.Get(true).EqualNetID(this))
					{
						oSlot3.Clear();
					}
				}
			}
			if (oSlot2.connectedTo.Get(true))
			{
				oSlot2.connectedTo.Get(true).UpdateFromInput(0, oSlot2.connectedToSlot);
			}
			oSlot2.Clear();
		}
		base.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
		foreach (IOEntity oEntity1 in oEntities)
		{
			if (oEntity1 == null)
			{
				continue;
			}
			oEntity1.MarkDirty();
			oEntity1.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
		}
		for (int k = 0; k < (int)this.inputs.Length; k++)
		{
			this.UpdateFromInput(0, k);
		}
	}

	public virtual int ConsumptionAmount()
	{
		return 1;
	}

	internal override void DoServerDestroy()
	{
		if (base.isServer)
		{
			this.Shutdown();
		}
		base.DoServerDestroy();
	}

	public virtual int GetCurrentEnergy()
	{
		return Mathf.Clamp(this.currentEnergy - this.ConsumptionAmount(), 0, this.currentEnergy);
	}

	public string GetDisplayName()
	{
		if (this.sourceItem == null)
		{
			return base.ShortPrefabName;
		}
		return this.sourceItem.displayName.translated;
	}

	public virtual int GetPassthroughAmount(int outputSlot = 0)
	{
		return Mathf.Clamp(this.GetCurrentEnergy(), 0, this.GetCurrentEnergy());
	}

	public virtual void Init()
	{
		for (int i = 0; i < (int)this.outputs.Length; i++)
		{
			IOEntity.IOSlot oSlot = this.outputs[i];
			oSlot.connectedTo.Init();
			if (oSlot.connectedTo.Get(true) != null)
			{
				int num = oSlot.connectedToSlot;
				if (num < 0 || num >= (int)oSlot.connectedTo.Get(true).inputs.Length)
				{
					Debug.LogError(string.Concat(new object[] { "Slot IOR Error: ", base.name, " setting up inputs for ", oSlot.connectedTo.Get(true).name, " slot : ", oSlot.connectedToSlot }));
				}
				else
				{
					oSlot.connectedTo.Get(true).inputs[oSlot.connectedToSlot].connectedTo.Set(this);
					oSlot.connectedTo.Get(true).inputs[oSlot.connectedToSlot].connectedToSlot = i;
					oSlot.connectedTo.Get(true).inputs[oSlot.connectedToSlot].connectedTo.Init();
				}
			}
		}
		if (this.IsRootEntity())
		{
			base.Invoke(new Action(this.MarkDirtyForceUpdateOutputs), UnityEngine.Random.Range(1f, 1f));
		}
	}

	public virtual float IOInput(IOEntity from, IOEntity.IOType inputType, float inputAmount, int slot = 0)
	{
		IOEntity.IOSlot[] oSlotArray = this.outputs;
		for (int i = 0; i < (int)oSlotArray.Length; i++)
		{
			IOEntity.IOSlot oSlot = oSlotArray[i];
			if (oSlot.connectedTo.Get(true) != null)
			{
				inputAmount = oSlot.connectedTo.Get(true).IOInput(this, oSlot.type, inputAmount, oSlot.connectedToSlot);
			}
		}
		return inputAmount;
	}

	public virtual void IOStateChanged(int inputAmount, int inputSlot)
	{
	}

	public bool IsConnectedTo(IOEntity entity, int slot, int depth, bool defaultReturn = false)
	{
		if (depth > 0 && slot < (int)this.inputs.Length)
		{
			IOEntity.IOSlot oSlot = this.inputs[slot];
			if (oSlot.mainPowerSlot)
			{
				IOEntity oEntity = oSlot.connectedTo.Get(true);
				if (oEntity != null)
				{
					if (oEntity == entity)
					{
						return true;
					}
					if (oEntity.IsConnectedTo(entity, depth - 1, defaultReturn))
					{
						return true;
					}
				}
			}
		}
		return false;
	}

	public bool IsConnectedTo(IOEntity entity, int depth, bool defaultReturn = false)
	{
		if (depth <= 0)
		{
			return defaultReturn;
		}
		for (int i = 0; i < (int)this.inputs.Length; i++)
		{
			IOEntity.IOSlot oSlot = this.inputs[i];
			if (oSlot.mainPowerSlot)
			{
				IOEntity oEntity = oSlot.connectedTo.Get(true);
				if (oEntity != null)
				{
					if (oEntity == entity)
					{
						return true;
					}
					if (oEntity.IsConnectedTo(entity, depth - 1, defaultReturn))
					{
						return true;
					}
				}
			}
		}
		return false;
	}

	public virtual bool IsPowered()
	{
		return base.HasFlag(BaseEntity.Flags.Reserved8);
	}

	public virtual bool IsRootEntity()
	{
		return false;
	}

	public override void Load(BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (info.msg.ioEntity == null)
		{
			return;
		}
		if (!info.fromDisk && info.msg.ioEntity.inputs != null)
		{
			int count = info.msg.ioEntity.inputs.Count;
			if ((int)this.inputs.Length != count)
			{
				this.inputs = new IOEntity.IOSlot[count];
			}
			for (int i = 0; i < count; i++)
			{
				if (this.inputs[i] == null)
				{
					this.inputs[i] = new IOEntity.IOSlot();
				}
				ProtoBuf.IOEntity.IOConnection item = info.msg.ioEntity.inputs[i];
				this.inputs[i].connectedTo = new IOEntity.IORef();
				this.inputs[i].connectedTo.entityRef.uid = item.connectedID;
				if (base.isClient)
				{
					this.inputs[i].connectedTo.InitClient();
				}
				this.inputs[i].connectedToSlot = item.connectedToSlot;
				this.inputs[i].niceName = item.niceName;
				this.inputs[i].type = (IOEntity.IOType)item.type;
			}
		}
		if (info.msg.ioEntity.outputs != null)
		{
			if (!info.fromDisk && base.isClient)
			{
				IOEntity.IOSlot[] oSlotArray = this.outputs;
				for (int j = 0; j < (int)oSlotArray.Length; j++)
				{
					oSlotArray[j].Clear();
				}
			}
			int num = info.msg.ioEntity.outputs.Count;
			IOEntity.IOSlot[] oSlotArray1 = null;
			if ((int)this.outputs.Length != num && num > 0)
			{
				oSlotArray1 = this.outputs;
				this.outputs = new IOEntity.IOSlot[num];
				for (int k = 0; k < (int)oSlotArray1.Length; k++)
				{
					if (k < num)
					{
						this.outputs[k] = oSlotArray1[k];
					}
				}
			}
			for (int l = 0; l < num; l++)
			{
				if (this.outputs[l] == null)
				{
					this.outputs[l] = new IOEntity.IOSlot();
				}
				ProtoBuf.IOEntity.IOConnection oConnection = info.msg.ioEntity.outputs[l];
				this.outputs[l].connectedTo = new IOEntity.IORef();
				this.outputs[l].connectedTo.entityRef.uid = oConnection.connectedID;
				if (base.isClient)
				{
					this.outputs[l].connectedTo.InitClient();
				}
				this.outputs[l].connectedToSlot = oConnection.connectedToSlot;
				this.outputs[l].niceName = oConnection.niceName;
				this.outputs[l].type = (IOEntity.IOType)oConnection.type;
				if (info.fromDisk || base.isClient)
				{
					List<ProtoBuf.IOEntity.IOConnection.LineVec> lineVecs = oConnection.linePointList;
					if (this.outputs[l].linePoints == null || (int)this.outputs[l].linePoints.Length != lineVecs.Count)
					{
						this.outputs[l].linePoints = new Vector3[lineVecs.Count];
					}
					for (int m = 0; m < lineVecs.Count; m++)
					{
						this.outputs[l].linePoints[m] = lineVecs[m].vec;
					}
				}
			}
		}
	}

	public virtual void MarkDirty()
	{
		if (base.isClient)
		{
			return;
		}
		this.cachedOutputsUsed = 0;
		IOEntity.IOSlot[] oSlotArray = this.outputs;
		for (int i = 0; i < (int)oSlotArray.Length; i++)
		{
			if (oSlotArray[i].connectedTo.Get(true) != null)
			{
				this.cachedOutputsUsed++;
			}
		}
		this.TouchIOState();
	}

	public void MarkDirtyForceUpdateOutputs()
	{
		this.ensureOutputsUpdated = true;
		this.MarkDirty();
	}

	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		bool flag;
		using (TimeWarning timeWarning = TimeWarning.New("IOEntity.OnRpcMessage", 0.1f))
		{
			if (rpc != -133425730 || !(player != null))
			{
				return base.OnRpcMessage(player, rpc, msg);
			}
			else
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (ConVar.Global.developer > 2)
				{
					Debug.Log(string.Concat("SV_RPCMessage: ", player, " - Server_RequestData "));
				}
				using (TimeWarning timeWarning1 = TimeWarning.New("Server_RequestData", 0.1f))
				{
					using (TimeWarning timeWarning2 = TimeWarning.New("Conditions", 0.1f))
					{
						if (!BaseEntity.RPC_Server.IsVisible.Test("Server_RequestData", this, player, 6f))
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
							this.Server_RequestData(rPCMessage);
						}
					}
					catch (Exception exception)
					{
						player.Kick("RPC Error in Server_RequestData");
						Debug.LogException(exception);
					}
				}
				flag = true;
			}
		}
		return flag;
	}

	public override void PostMapEntitySpawn()
	{
		base.PostMapEntitySpawn();
		this.Init();
	}

	public override void PostServerLoad()
	{
		base.PostServerLoad();
		this.Init();
	}

	public static void ProcessQueue()
	{
		float single = UnityEngine.Time.realtimeSinceStartup;
		float single1 = (float)IOEntity.framebudgetms / 1000f;
		while (IOEntity._processQueue.Count > 0 && UnityEngine.Time.realtimeSinceStartup < single + single1)
		{
			IOEntity oEntity = IOEntity._processQueue.Dequeue();
			if (oEntity == null)
			{
				continue;
			}
			oEntity.UpdateOutputs();
		}
	}

	public virtual void ResetIOState()
	{
	}

	public override void ResetState()
	{
		this.lastResetIndex = 0;
		this.cachedOutputsUsed = 0;
		this.lastPassthroughEnergy = 0;
		this.lastEnergy = 0;
		this.currentEnergy = 0;
		this.lastUpdateTime = 0f;
		this.ensureOutputsUpdated = false;
	}

	public override void Save(BaseNetworkable.SaveInfo info)
	{
		int i;
		base.Save(info);
		info.msg.ioEntity = Facepunch.Pool.Get<ProtoBuf.IOEntity>();
		info.msg.ioEntity.inputs = Facepunch.Pool.GetList<ProtoBuf.IOEntity.IOConnection>();
		info.msg.ioEntity.outputs = Facepunch.Pool.GetList<ProtoBuf.IOEntity.IOConnection>();
		IOEntity.IOSlot[] oSlotArray = this.inputs;
		for (i = 0; i < (int)oSlotArray.Length; i++)
		{
			IOEntity.IOSlot oSlot = oSlotArray[i];
			ProtoBuf.IOEntity.IOConnection oConnection = Facepunch.Pool.Get<ProtoBuf.IOEntity.IOConnection>();
			oConnection.connectedID = oSlot.connectedTo.entityRef.uid;
			oConnection.connectedToSlot = oSlot.connectedToSlot;
			oConnection.niceName = oSlot.niceName;
			oConnection.type = (int)oSlot.type;
			oConnection.inUse = oConnection.connectedID != 0;
			info.msg.ioEntity.inputs.Add(oConnection);
		}
		oSlotArray = this.outputs;
		for (i = 0; i < (int)oSlotArray.Length; i++)
		{
			IOEntity.IOSlot oSlot1 = oSlotArray[i];
			ProtoBuf.IOEntity.IOConnection list = Facepunch.Pool.Get<ProtoBuf.IOEntity.IOConnection>();
			list.connectedID = oSlot1.connectedTo.entityRef.uid;
			list.connectedToSlot = oSlot1.connectedToSlot;
			list.niceName = oSlot1.niceName;
			list.type = (int)oSlot1.type;
			list.inUse = list.connectedID != 0;
			if (oSlot1.linePoints != null)
			{
				list.linePointList = Facepunch.Pool.GetList<ProtoBuf.IOEntity.IOConnection.LineVec>();
				list.linePointList.Clear();
				Vector3[] vector3Array = oSlot1.linePoints;
				for (int j = 0; j < (int)vector3Array.Length; j++)
				{
					Vector3 vector3 = vector3Array[j];
					ProtoBuf.IOEntity.IOConnection.LineVec lineVec = Facepunch.Pool.Get<ProtoBuf.IOEntity.IOConnection.LineVec>();
					lineVec.vec = vector3;
					list.linePointList.Add(lineVec);
				}
			}
			info.msg.ioEntity.outputs.Add(list);
		}
	}

	public virtual void SendAdditionalData(BasePlayer player)
	{
		float single = 0f;
		float single1 = 0f;
		base.ClientRPCPlayer<int, int, float, float>(null, player, "Client_ReceiveAdditionalData", this.currentEnergy, this.GetPassthroughAmount(0), single, single1);
	}

	public virtual void SendIONetworkUpdate()
	{
		base.SendNetworkUpdate_Flags();
	}

	[IsVisible(6f)]
	[RPC_Server]
	private void Server_RequestData(BaseEntity.RPCMessage msg)
	{
		this.SendAdditionalData(msg.player);
	}

	public void Shutdown()
	{
		this.ClearConnections();
	}

	public override void Spawn()
	{
		base.Spawn();
		if (!Rust.Application.isLoadingSave)
		{
			this.Init();
		}
	}

	public void TouchInternal()
	{
		int passthroughAmount = this.GetPassthroughAmount(0);
		bool flag = this.lastPassthroughEnergy != passthroughAmount;
		this.lastPassthroughEnergy = passthroughAmount;
		if (flag)
		{
			this.IOStateChanged(this.currentEnergy, 0);
			this.ensureOutputsUpdated = true;
		}
		IOEntity._processQueue.Enqueue(this);
	}

	public virtual void TouchIOState()
	{
		if (base.isClient)
		{
			return;
		}
		this.TouchInternal();
	}

	public virtual void UpdateFromInput(int inputAmount, int inputSlot)
	{
		this.UpdateHasPower(inputAmount, inputSlot);
		this.lastEnergy = this.currentEnergy;
		this.currentEnergy = inputAmount;
		int passthroughAmount = this.GetPassthroughAmount(0);
		bool flag = this.lastPassthroughEnergy != passthroughAmount;
		this.lastPassthroughEnergy = passthroughAmount;
		if (this.currentEnergy != this.lastEnergy | flag)
		{
			this.IOStateChanged(inputAmount, inputSlot);
			this.ensureOutputsUpdated = true;
		}
		IOEntity._processQueue.Enqueue(this);
	}

	public virtual void UpdateHasPower(int inputAmount, int inputSlot)
	{
		base.SetFlag(BaseEntity.Flags.Reserved8, inputAmount >= this.ConsumptionAmount(), false, false);
	}

	public virtual void UpdateOutputs()
	{
		if (UnityEngine.Time.realtimeSinceStartup - this.lastUpdateTime < IOEntity.responsetime)
		{
			IOEntity._processQueue.Enqueue(this);
			return;
		}
		this.lastUpdateTime = UnityEngine.Time.realtimeSinceStartup;
		this.SendIONetworkUpdate();
		if (this.outputs.Length == 0)
		{
			this.ensureOutputsUpdated = false;
			return;
		}
		if (this.ensureOutputsUpdated)
		{
			int num = (this.cachedOutputsUsed == 0 ? 1 : this.cachedOutputsUsed);
			int num1 = Mathf.FloorToInt((float)this.GetPassthroughAmount(0) / (float)num);
			this.ensureOutputsUpdated = false;
			IOEntity.IOSlot[] oSlotArray = this.outputs;
			for (int i = 0; i < (int)oSlotArray.Length; i++)
			{
				IOEntity.IOSlot oSlot = oSlotArray[i];
				if (oSlot.connectedTo.Get(true) != null)
				{
					oSlot.connectedTo.Get(true).UpdateFromInput(num1, oSlot.connectedToSlot);
				}
			}
		}
	}

	public virtual bool WantsPower()
	{
		return true;
	}

	[Serializable]
	public class IORef
	{
		public EntityRef entityRef;

		public IOEntity ioEnt;

		public IORef()
		{
		}

		public void Clear()
		{
			this.ioEnt = null;
			this.entityRef.Set(null);
		}

		public IOEntity Get(bool isServer = true)
		{
			if (this.ioEnt == null && this.entityRef.IsValid(isServer))
			{
				this.ioEnt = this.entityRef.Get(isServer).GetComponent<IOEntity>();
			}
			return this.ioEnt;
		}

		public void Init()
		{
			if (this.ioEnt != null && !this.entityRef.IsValid(true))
			{
				this.entityRef.Set(this.ioEnt);
			}
			if (this.entityRef.IsValid(true))
			{
				this.ioEnt = this.entityRef.Get(true).GetComponent<IOEntity>();
			}
		}

		public void InitClient()
		{
			if (this.entityRef.IsValid(false) && this.ioEnt == null)
			{
				this.ioEnt = this.entityRef.Get(false).GetComponent<IOEntity>();
			}
		}

		public void Set(IOEntity newIOEnt)
		{
			this.entityRef.Set(newIOEnt);
		}
	}

	[Serializable]
	public class IOSlot
	{
		public string niceName;

		public IOEntity.IOType type;

		public IOEntity.IORef connectedTo;

		public int connectedToSlot;

		public Vector3[] linePoints;

		public ClientIOLine line;

		public Vector3 handlePosition;

		public bool rootConnectionsOnly;

		public bool mainPowerSlot;

		public IOSlot()
		{
		}

		public void Clear()
		{
			this.connectedTo.Clear();
			this.connectedToSlot = 0;
			this.linePoints = null;
		}
	}

	public enum IOType
	{
		Electric,
		Fluidic,
		Kinetic,
		Generic
	}
}