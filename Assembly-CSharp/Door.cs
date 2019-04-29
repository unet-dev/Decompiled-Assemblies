using ConVar;
using Network;
using Oxide.Core;
using System;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Assertions;

public class Door : AnimatedBuildingBlock
{
	public GameObjectRef knockEffect;

	public bool canTakeLock = true;

	public bool hasHatch;

	public bool canTakeCloser;

	public bool canTakeKnocker;

	public bool canNpcOpen = true;

	public bool canHandOpen = true;

	public bool isSecurityDoor;

	private float decayResetTimeLast = Single.NegativeInfinity;

	public NavMeshModifierVolume NavMeshVolumeAnimals;

	public NavMeshModifierVolume NavMeshVolumeHumanoids;

	public UnityEngine.AI.NavMeshLink NavMeshLink;

	public NPCDoorTriggerBox NpcTriggerBox;

	private static int nonWalkableArea;

	private static int animalAgentTypeId;

	private static int humanoidAgentTypeId;

	private float nextKnockTime = Single.NegativeInfinity;

	static Door()
	{
		Door.nonWalkableArea = -1;
		Door.animalAgentTypeId = -1;
		Door.humanoidAgentTypeId = -1;
	}

	public Door()
	{
	}

	public override float BoundsPadding()
	{
		return 2f;
	}

	public override bool CanPickup(BasePlayer player)
	{
		if (!base.IsOpen())
		{
			return false;
		}
		if (base.GetSlot(BaseEntity.Slot.Lock))
		{
			return false;
		}
		if (base.GetSlot(BaseEntity.Slot.UpperModifier))
		{
			return false;
		}
		if (base.GetSlot(BaseEntity.Slot.CenterDecoration))
		{
			return false;
		}
		if (base.GetSlot(BaseEntity.Slot.LowerCenterDecoration))
		{
			return false;
		}
		return base.CanPickup(player);
	}

	public void CloseRequest()
	{
		this.SetOpen(false);
	}

	public bool GetPlayerLockPermission(BasePlayer player)
	{
		BaseLock slot = base.GetSlot(BaseEntity.Slot.Lock) as BaseLock;
		if (slot == null)
		{
			return true;
		}
		return slot.GetPlayerLockPermission(player);
	}

	public override bool HasSlot(BaseEntity.Slot slot)
	{
		if (slot == BaseEntity.Slot.Lock && this.canTakeLock)
		{
			return true;
		}
		if (slot == BaseEntity.Slot.UpperModifier)
		{
			return true;
		}
		if (slot == BaseEntity.Slot.CenterDecoration && this.canTakeCloser)
		{
			return true;
		}
		if (slot == BaseEntity.Slot.LowerCenterDecoration && this.canTakeKnocker)
		{
			return true;
		}
		return base.HasSlot(slot);
	}

	public override void OnFlagsChanged(BaseEntity.Flags old, BaseEntity.Flags next)
	{
		base.OnFlagsChanged(old, next);
		BaseEntity slot = base.GetSlot(BaseEntity.Slot.UpperModifier);
		if (slot)
		{
			slot.SendMessage("Think");
		}
	}

	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		bool flag;
		BaseEntity.RPCMessage rPCMessage;
		using (TimeWarning timeWarning = TimeWarning.New("Door.OnRpcMessage", 0.1f))
		{
			if (rpc == -295458617 && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log(string.Concat("SV_RPCMessage: ", player, " - RPC_CloseDoor "));
				}
				using (TimeWarning timeWarning1 = TimeWarning.New("RPC_CloseDoor", 0.1f))
				{
					using (TimeWarning timeWarning2 = TimeWarning.New("Conditions", 0.1f))
					{
						if (!BaseEntity.RPC_Server.MaxDistance.Test("RPC_CloseDoor", this, player, 3f))
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
							this.RPC_CloseDoor(rPCMessage);
						}
					}
					catch (Exception exception)
					{
						player.Kick("RPC Error in RPC_CloseDoor");
						Debug.LogException(exception);
					}
				}
				flag = true;
			}
			else if (rpc == 1487779344 && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log(string.Concat("SV_RPCMessage: ", player, " - RPC_KnockDoor "));
				}
				using (timeWarning1 = TimeWarning.New("RPC_KnockDoor", 0.1f))
				{
					using (timeWarning2 = TimeWarning.New("Conditions", 0.1f))
					{
						if (!BaseEntity.RPC_Server.MaxDistance.Test("RPC_KnockDoor", this, player, 3f))
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
							this.RPC_KnockDoor(rPCMessage);
						}
					}
					catch (Exception exception1)
					{
						player.Kick("RPC Error in RPC_KnockDoor");
						Debug.LogException(exception1);
					}
				}
				flag = true;
			}
			else if (rpc == -980606731 && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log(string.Concat("SV_RPCMessage: ", player, " - RPC_OpenDoor "));
				}
				using (timeWarning1 = TimeWarning.New("RPC_OpenDoor", 0.1f))
				{
					using (timeWarning2 = TimeWarning.New("Conditions", 0.1f))
					{
						if (!BaseEntity.RPC_Server.MaxDistance.Test("RPC_OpenDoor", this, player, 3f))
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
							this.RPC_OpenDoor(rPCMessage);
						}
					}
					catch (Exception exception2)
					{
						player.Kick("RPC Error in RPC_OpenDoor");
						Debug.LogException(exception2);
					}
				}
				flag = true;
			}
			else if (rpc != -1294476695 || !(player != null))
			{
				return base.OnRpcMessage(player, rpc, msg);
			}
			else
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log(string.Concat("SV_RPCMessage: ", player, " - RPC_ToggleHatch "));
				}
				using (timeWarning1 = TimeWarning.New("RPC_ToggleHatch", 0.1f))
				{
					using (timeWarning2 = TimeWarning.New("Conditions", 0.1f))
					{
						if (!BaseEntity.RPC_Server.MaxDistance.Test("RPC_ToggleHatch", this, player, 3f))
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
							this.RPC_ToggleHatch(rPCMessage);
						}
					}
					catch (Exception exception3)
					{
						player.Kick("RPC Error in RPC_ToggleHatch");
						Debug.LogException(exception3);
					}
				}
				flag = true;
			}
		}
		return flag;
	}

	public override void ResetState()
	{
		base.ResetState();
		this.decayResetTimeLast = Single.NegativeInfinity;
		if (this.isSecurityDoor && this.NavMeshLink != null)
		{
			this.SetNavMeshLinkEnabled(false);
		}
	}

	[MaxDistance(3f)]
	[RPC_Server]
	private void RPC_CloseDoor(BaseEntity.RPCMessage rpc)
	{
		if (!rpc.player.CanInteract())
		{
			return;
		}
		if (!this.canHandOpen)
		{
			return;
		}
		if (!base.IsOpen())
		{
			return;
		}
		if (base.IsBusy())
		{
			return;
		}
		if (base.IsLocked())
		{
			return;
		}
		BaseLock slot = base.GetSlot(BaseEntity.Slot.Lock) as BaseLock;
		if (slot != null && !slot.OnTryToClose(rpc.player))
		{
			return;
		}
		base.SetFlag(BaseEntity.Flags.Open, false, false, true);
		base.SendNetworkUpdateImmediate(false);
		if (this.isSecurityDoor && this.NavMeshLink != null)
		{
			this.SetNavMeshLinkEnabled(false);
		}
		Interface.CallHook("OnDoorClosed", this, rpc.player);
	}

	[MaxDistance(3f)]
	[RPC_Server]
	private void RPC_KnockDoor(BaseEntity.RPCMessage rpc)
	{
		if (!rpc.player.CanInteract())
		{
			return;
		}
		if (!this.knockEffect.isValid)
		{
			return;
		}
		if (UnityEngine.Time.realtimeSinceStartup < this.nextKnockTime)
		{
			return;
		}
		this.nextKnockTime = UnityEngine.Time.realtimeSinceStartup + 0.5f;
		BaseEntity slot = base.GetSlot(BaseEntity.Slot.LowerCenterDecoration);
		if (slot != null)
		{
			DoorKnocker component = slot.GetComponent<DoorKnocker>();
			if (component)
			{
				component.Knock(rpc.player);
				return;
			}
		}
		Effect.server.Run(this.knockEffect.resourcePath, this, 0, Vector3.zero, Vector3.zero, null, false);
		Interface.CallHook("OnDoorKnocked", this, rpc.player);
	}

	[MaxDistance(3f)]
	[RPC_Server]
	private void RPC_OpenDoor(BaseEntity.RPCMessage rpc)
	{
		if (!rpc.player.CanInteract())
		{
			return;
		}
		if (!this.canHandOpen)
		{
			return;
		}
		if (base.IsOpen())
		{
			return;
		}
		if (base.IsBusy())
		{
			return;
		}
		if (base.IsLocked())
		{
			return;
		}
		BaseLock slot = base.GetSlot(BaseEntity.Slot.Lock) as BaseLock;
		if (slot != null)
		{
			if (!slot.OnTryToOpen(rpc.player))
			{
				return;
			}
			if (slot.IsLocked() && UnityEngine.Time.realtimeSinceStartup - this.decayResetTimeLast > 60f)
			{
				BuildingBlock buildingBlock = base.FindLinkedEntity<BuildingBlock>();
				if (!buildingBlock)
				{
					Decay.RadialDecayTouch(base.transform.position, 40f, 2097408);
				}
				else
				{
					Decay.BuildingDecayTouch(buildingBlock);
				}
				this.decayResetTimeLast = UnityEngine.Time.realtimeSinceStartup;
			}
		}
		base.SetFlag(BaseEntity.Flags.Open, true, false, true);
		base.SendNetworkUpdateImmediate(false);
		if (this.isSecurityDoor && this.NavMeshLink != null)
		{
			this.SetNavMeshLinkEnabled(true);
		}
		Interface.CallHook("OnDoorOpened", this, rpc.player);
	}

	[MaxDistance(3f)]
	[RPC_Server]
	private void RPC_ToggleHatch(BaseEntity.RPCMessage rpc)
	{
		if (!rpc.player.CanInteract())
		{
			return;
		}
		if (!this.hasHatch)
		{
			return;
		}
		BaseLock slot = base.GetSlot(BaseEntity.Slot.Lock) as BaseLock;
		if (!slot || slot.OnTryToOpen(rpc.player))
		{
			base.SetFlag(BaseEntity.Flags.Reserved3, !base.HasFlag(BaseEntity.Flags.Reserved3), false, true);
		}
	}

	public override void ServerInit()
	{
		NavMeshBuildSettings settingsByIndex;
		base.ServerInit();
		if (Door.nonWalkableArea < 0)
		{
			Door.nonWalkableArea = NavMesh.GetAreaFromName("Not Walkable");
		}
		if (Door.animalAgentTypeId < 0)
		{
			settingsByIndex = NavMesh.GetSettingsByIndex(1);
			Door.animalAgentTypeId = settingsByIndex.agentTypeID;
		}
		if (this.NavMeshVolumeAnimals == null)
		{
			this.NavMeshVolumeAnimals = base.gameObject.AddComponent<NavMeshModifierVolume>();
			this.NavMeshVolumeAnimals.area = Door.nonWalkableArea;
			this.NavMeshVolumeAnimals.AddAgentType(Door.animalAgentTypeId);
			this.NavMeshVolumeAnimals.center = Vector3.zero;
			this.NavMeshVolumeAnimals.size = Vector3.one;
		}
		if (this.HasSlot(BaseEntity.Slot.Lock))
		{
			this.canNpcOpen = false;
		}
		if (!this.canNpcOpen)
		{
			if (Door.humanoidAgentTypeId < 0)
			{
				settingsByIndex = NavMesh.GetSettingsByIndex(0);
				Door.humanoidAgentTypeId = settingsByIndex.agentTypeID;
			}
			if (this.NavMeshVolumeHumanoids == null)
			{
				this.NavMeshVolumeHumanoids = base.gameObject.AddComponent<NavMeshModifierVolume>();
				this.NavMeshVolumeHumanoids.area = Door.nonWalkableArea;
				this.NavMeshVolumeHumanoids.AddAgentType(Door.humanoidAgentTypeId);
				this.NavMeshVolumeHumanoids.center = Vector3.zero;
				this.NavMeshVolumeHumanoids.size = (Vector3.one + Vector3.up) + Vector3.forward;
			}
		}
		else if (this.NpcTriggerBox == null)
		{
			if (this.isSecurityDoor)
			{
				NavMeshObstacle navMeshObstacle = base.gameObject.AddComponent<NavMeshObstacle>();
				navMeshObstacle.carving = true;
				navMeshObstacle.center = Vector3.zero;
				navMeshObstacle.size = Vector3.one;
				navMeshObstacle.shape = NavMeshObstacleShape.Box;
			}
			this.NpcTriggerBox = (new GameObject("NpcTriggerBox")).AddComponent<NPCDoorTriggerBox>();
			this.NpcTriggerBox.Setup(this);
		}
		AIInformationZone forPoint = AIInformationZone.GetForPoint(base.transform.position, null);
		if (forPoint != null && this.NavMeshLink == null)
		{
			this.NavMeshLink = forPoint.GetClosestNavMeshLink(base.transform.position);
		}
	}

	public void SetLocked(bool locked)
	{
		base.SetFlag(BaseEntity.Flags.Locked, false, false, true);
		base.SendNetworkUpdateImmediate(false);
	}

	public void SetNavMeshLinkEnabled(bool wantsOn)
	{
		if (this.NavMeshLink != null)
		{
			if (wantsOn)
			{
				this.NavMeshLink.gameObject.SetActive(true);
				this.NavMeshLink.enabled = true;
				return;
			}
			this.NavMeshLink.enabled = false;
			this.NavMeshLink.gameObject.SetActive(false);
		}
	}

	public void SetOpen(bool open)
	{
		base.SetFlag(BaseEntity.Flags.Open, open, false, true);
		base.SendNetworkUpdateImmediate(false);
		if (this.isSecurityDoor && this.NavMeshLink != null)
		{
			this.SetNavMeshLinkEnabled(open);
		}
	}
}