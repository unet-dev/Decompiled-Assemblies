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

public class BuildingBlock : StabilityEntity
{
	[NonSerialized]
	public Construction blockDefinition;

	private static Vector3[] outsideLookupOffsets;

	private bool forceSkinRefresh;

	public int modelState;

	private int lastModelState;

	public BuildingGrade.Enum grade;

	public BuildingGrade.Enum lastGrade = BuildingGrade.Enum.None;

	public ConstructionSkin currentSkin;

	private DeferredAction skinChange;

	private MeshRenderer placeholderRenderer;

	private MeshCollider placeholderCollider;

	public static BuildingBlock.UpdateSkinWorkQueue updateSkinQueueServer;

	public ConstructionGrade currentGrade
	{
		get
		{
			ConstructionGrade grade = this.GetGrade(this.grade);
			if (grade != null)
			{
				return grade;
			}
			for (int i = 0; i < (int)this.blockDefinition.grades.Length; i++)
			{
				if (this.blockDefinition.grades[i] != null)
				{
					return this.blockDefinition.grades[i];
				}
			}
			Debug.LogWarning(string.Concat("Building block grade not found: ", this.grade));
			return null;
		}
	}

	static BuildingBlock()
	{
		Vector3[] vector3Array = new Vector3[5];
		Vector3 vector3 = new Vector3(0f, 1f, 0f);
		vector3Array[0] = vector3.normalized;
		vector3 = new Vector3(1f, 1f, 0f);
		vector3Array[1] = vector3.normalized;
		vector3 = new Vector3(-1f, 1f, 0f);
		vector3Array[2] = vector3.normalized;
		vector3 = new Vector3(0f, 1f, 1f);
		vector3Array[3] = vector3.normalized;
		vector3 = new Vector3(0f, 1f, -1f);
		vector3Array[4] = vector3.normalized;
		BuildingBlock.outsideLookupOffsets = vector3Array;
		BuildingBlock.updateSkinQueueServer = new BuildingBlock.UpdateSkinWorkQueue();
	}

	public BuildingBlock()
	{
	}

	public override float BoundsPadding()
	{
		return 1f;
	}

	public override List<ItemAmount> BuildCost()
	{
		return this.currentGrade.costToBuild;
	}

	private bool CanAffordUpgrade(BuildingGrade.Enum iGrade, BasePlayer player)
	{
		bool flag;
		object obj = Interface.CallHook("CanAffordUpgrade", player, this, iGrade);
		if (obj as bool)
		{
			return (bool)obj;
		}
		List<ItemAmount>.Enumerator enumerator = this.GetGrade(iGrade).costToBuild.GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				ItemAmount current = enumerator.Current;
				if ((float)player.inventory.GetAmount(current.itemid) >= current.amount)
				{
					continue;
				}
				flag = false;
				return flag;
			}
			return true;
		}
		finally
		{
			((IDisposable)enumerator).Dispose();
		}
		return flag;
	}

	private bool CanChangeToGrade(BuildingGrade.Enum iGrade, BasePlayer player)
	{
		object obj = Interface.CallHook("CanChangeGrade", player, this, iGrade);
		if (obj as bool)
		{
			return (bool)obj;
		}
		if (!this.HasUpgradePrivilege(iGrade, player))
		{
			return false;
		}
		return !this.IsUpgradeBlocked();
	}

	public bool CanDemolish(BasePlayer player)
	{
		object obj = Interface.CallHook("CanDemolish", player, this);
		if (obj as bool)
		{
			return (bool)obj;
		}
		if (!this.IsDemolishable())
		{
			return false;
		}
		return this.HasDemolishPrivilege(player);
	}

	public bool CanRotate(BasePlayer player)
	{
		if (!this.IsRotatable() || !this.HasRotationPrivilege(player))
		{
			return false;
		}
		return !this.IsRotationBlocked();
	}

	public override string Categorize()
	{
		return "building";
	}

	private void ChangeSkin()
	{
		if (base.IsDestroyed)
		{
			return;
		}
		ConstructionGrade constructionGrade = this.currentGrade;
		if (constructionGrade.skinObject.isValid)
		{
			this.ChangeSkin(constructionGrade.skinObject);
			return;
		}
		ConstructionGrade[] constructionGradeArray = this.blockDefinition.grades;
		for (int i = 0; i < (int)constructionGradeArray.Length; i++)
		{
			ConstructionGrade constructionGrade1 = constructionGradeArray[i];
			if (constructionGrade1.skinObject.isValid)
			{
				this.ChangeSkin(constructionGrade1.skinObject);
				return;
			}
		}
		Debug.LogWarning(string.Concat("No skins found for ", base.gameObject));
	}

	public void ChangeSkin(GameObjectRef prefab)
	{
		bool flag = this.lastGrade != this.grade;
		this.lastGrade = this.grade;
		if (flag)
		{
			if (this.currentSkin != null)
			{
				this.DestroySkin();
			}
			else
			{
				this.UpdatePlaceholder(false);
			}
			GameObject gameObject = base.gameManager.CreatePrefab(prefab.resourcePath, base.transform, true);
			this.currentSkin = gameObject.GetComponent<ConstructionSkin>();
			Model component = this.currentSkin.GetComponent<Model>();
			base.SetModel(component);
			Assert.IsTrue(this.model == component, "Didn't manage to set model successfully!");
		}
		if (base.isServer)
		{
			this.modelState = this.currentSkin.DetermineConditionalModelState(this);
		}
		bool flag1 = this.lastModelState != this.modelState;
		this.lastModelState = this.modelState;
		if (flag | flag1 || this.forceSkinRefresh)
		{
			this.currentSkin.Refresh(this);
			this.forceSkinRefresh = false;
		}
		if (base.isServer)
		{
			if (flag)
			{
				this.RefreshNeighbours(true);
			}
			if (flag1)
			{
				base.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
			}
		}
	}

	public override void DestroyShared()
	{
		if (base.isServer)
		{
			this.RefreshNeighbours(false);
		}
		base.DestroyShared();
	}

	private void DestroySkin()
	{
		if (this.currentSkin != null)
		{
			this.currentSkin.Destroy(this);
			this.currentSkin = null;
		}
	}

	[MaxDistance(3f)]
	[RPC_Server]
	private void DoDemolish(BaseEntity.RPCMessage msg)
	{
		if (!msg.player.CanInteract())
		{
			return;
		}
		if (!this.CanDemolish(msg.player))
		{
			return;
		}
		if (Interface.CallHook("OnStructureDemolish", this, msg.player, false) != null)
		{
			return;
		}
		base.Kill(BaseNetworkable.DestroyMode.Gib);
	}

	[MaxDistance(3f)]
	[RPC_Server]
	private void DoImmediateDemolish(BaseEntity.RPCMessage msg)
	{
		if (!msg.player.CanInteract())
		{
			return;
		}
		if (!msg.player.IsAdmin)
		{
			return;
		}
		if (Interface.CallHook("OnStructureDemolish", this, msg.player, true) != null)
		{
			return;
		}
		base.Kill(BaseNetworkable.DestroyMode.Gib);
	}

	[MaxDistance(3f)]
	[RPC_Server]
	private void DoRotation(BaseEntity.RPCMessage msg)
	{
		if (!msg.player.CanInteract())
		{
			return;
		}
		if (!this.CanRotate(msg.player))
		{
			return;
		}
		if (!this.blockDefinition.canRotate)
		{
			return;
		}
		if (Interface.CallHook("OnStructureRotate", this, msg.player) != null)
		{
			return;
		}
		Transform transforms = base.transform;
		transforms.localRotation = transforms.localRotation * Quaternion.Euler(this.blockDefinition.rotationAmount);
		base.RefreshEntityLinks();
		base.UpdateSurroundingEntities();
		this.UpdateSkin(true);
		base.SendNetworkUpdateImmediate(false);
		base.ClientRPC(null, "RefreshSkin");
	}

	[MaxDistance(3f)]
	[RPC_Server]
	private void DoUpgradeToGrade(BaseEntity.RPCMessage msg)
	{
		if (!msg.player.CanInteract())
		{
			return;
		}
		BuildingGrade.Enum @enum = (BuildingGrade.Enum)msg.read.Int32();
		ConstructionGrade grade = this.GetGrade(@enum);
		if (grade == null)
		{
			return;
		}
		if (!this.CanChangeToGrade(@enum, msg.player))
		{
			return;
		}
		if (!this.CanAffordUpgrade(@enum, msg.player))
		{
			return;
		}
		if (base.SecondsSinceAttacked < 30f)
		{
			return;
		}
		if (Interface.CallHook("OnStructureUpgrade", this, msg.player, @enum) != null)
		{
			return;
		}
		this.PayForUpgrade(grade, msg.player);
		this.SetGrade(@enum);
		this.SetHealthToMax();
		this.StartBeingRotatable();
		base.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
		this.UpdateSkin(false);
		base.ResetUpkeepTime();
		BuildingManager.Building building = BuildingManager.server.GetBuilding(this.buildingID);
		if (building != null)
		{
			building.Dirty();
		}
		Effect.server.Run(string.Concat("assets/bundled/prefabs/fx/build/promote_", @enum.ToString().ToLower(), ".prefab"), this, 0, Vector3.zero, Vector3.zero, null, false);
	}

	public bool GetConditionalModel(int index)
	{
		return (this.modelState & 1 << (index & 31)) != 0;
	}

	private ConstructionGrade GetGrade(BuildingGrade.Enum iGrade)
	{
		if ((int)this.grade < (int)this.blockDefinition.grades.Length)
		{
			return this.blockDefinition.grades[(int)iGrade];
		}
		Debug.LogWarning(string.Concat(new object[] { "Grade out of range ", base.gameObject, " ", this.grade, " / ", (int)this.blockDefinition.grades.Length }));
		return this.blockDefinition.defaultGrade;
	}

	private bool HasDemolishPrivilege(BasePlayer player)
	{
		return player.IsBuildingAuthed(base.transform.position, base.transform.rotation, this.bounds);
	}

	private bool HasRotationPrivilege(BasePlayer player)
	{
		return !player.IsBuildingBlocked(base.transform.position, base.transform.rotation, this.bounds);
	}

	private bool HasUpgradePrivilege(BuildingGrade.Enum iGrade, BasePlayer player)
	{
		if (iGrade == this.grade)
		{
			return false;
		}
		if ((int)iGrade >= (int)this.blockDefinition.grades.Length)
		{
			return false;
		}
		if (iGrade < BuildingGrade.Enum.Twigs)
		{
			return false;
		}
		if (iGrade < this.grade)
		{
			return false;
		}
		return !player.IsBuildingBlocked(base.transform.position, base.transform.rotation, this.bounds);
	}

	public override void Hurt(HitInfo info)
	{
		if (!ConVar.Server.pve || !info.Initiator || !(info.Initiator is BasePlayer))
		{
			base.Hurt(info);
			return;
		}
		(info.Initiator as BasePlayer).Hurt(info.damageTypes.Total(), DamageType.Generic, null, true);
	}

	public override void InitShared()
	{
		base.InitShared();
		this.placeholderRenderer = base.GetComponent<MeshRenderer>();
		this.placeholderCollider = base.GetComponent<MeshCollider>();
	}

	private bool IsDemolishable()
	{
		if (!ConVar.Server.pve && !base.HasFlag(BaseEntity.Flags.Reserved2))
		{
			return false;
		}
		return true;
	}

	public override bool IsOutside()
	{
		float outsideTestRange = ConVar.Decay.outside_test_range;
		Vector3 vector3 = base.PivotPoint();
		for (int i = 0; i < (int)BuildingBlock.outsideLookupOffsets.Length; i++)
		{
			Vector3 vector31 = BuildingBlock.outsideLookupOffsets[i];
			if (!UnityEngine.Physics.Raycast(new Ray(vector3 + (vector31 * outsideTestRange), -vector31), outsideTestRange - 0.5f, 2097152))
			{
				return true;
			}
		}
		return false;
	}

	private bool IsRotatable()
	{
		if (this.blockDefinition.grades == null)
		{
			return false;
		}
		if (!this.blockDefinition.canRotate)
		{
			return false;
		}
		if (!base.HasFlag(BaseEntity.Flags.Reserved1))
		{
			return false;
		}
		return true;
	}

	private bool IsRotationBlocked()
	{
		if (!this.blockDefinition.checkVolumeOnRotate)
		{
			return false;
		}
		DeployVolume[] deployVolumeArray = PrefabAttribute.server.FindAll<DeployVolume>(this.prefabID);
		return DeployVolume.Check(base.transform.position, base.transform.rotation, deployVolumeArray, ~(1 << (base.gameObject.layer & 31)));
	}

	private bool IsUpgradeBlocked()
	{
		if (!this.blockDefinition.checkVolumeOnUpgrade)
		{
			return false;
		}
		DeployVolume[] deployVolumeArray = PrefabAttribute.server.FindAll<DeployVolume>(this.prefabID);
		return DeployVolume.Check(base.transform.position, base.transform.rotation, deployVolumeArray, ~(1 << (base.gameObject.layer & 31)));
	}

	public override void Load(BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (info.msg.buildingBlock != null)
		{
			this.SetConditionalModel(info.msg.buildingBlock.model);
			this.SetGrade((BuildingGrade.Enum)info.msg.buildingBlock.grade);
		}
		if (info.fromDisk)
		{
			base.SetFlag(BaseEntity.Flags.Reserved2, false, false, true);
			base.SetFlag(BaseEntity.Flags.Reserved1, false, false, true);
			this.UpdateSkin(false);
		}
	}

	public override float MaxHealth()
	{
		return this.currentGrade.maxHealth;
	}

	private bool NeedsSkinChange()
	{
		if (this.currentSkin == null || this.forceSkinRefresh || this.lastGrade != this.grade)
		{
			return true;
		}
		return this.lastModelState != this.modelState;
	}

	private void OnHammered()
	{
	}

	public override void OnHealthChanged(float oldvalue, float newvalue)
	{
		base.OnHealthChanged(oldvalue, newvalue);
		if (Mathf.RoundToInt(oldvalue) == Mathf.RoundToInt(newvalue))
		{
			return;
		}
		base.SendNetworkUpdate(BasePlayer.NetworkQueue.UpdateDistance);
	}

	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		bool flag;
		BaseEntity.RPCMessage rPCMessage;
		using (TimeWarning timeWarning = TimeWarning.New("BuildingBlock.OnRpcMessage", 0.1f))
		{
			if (rpc == -1436904883 && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (ConVar.Global.developer > 2)
				{
					Debug.Log(string.Concat("SV_RPCMessage: ", player, " - DoDemolish "));
				}
				using (TimeWarning timeWarning1 = TimeWarning.New("DoDemolish", 0.1f))
				{
					using (TimeWarning timeWarning2 = TimeWarning.New("Conditions", 0.1f))
					{
						if (!BaseEntity.RPC_Server.MaxDistance.Test("DoDemolish", this, player, 3f))
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
							this.DoDemolish(rPCMessage);
						}
					}
					catch (Exception exception)
					{
						player.Kick("RPC Error in DoDemolish");
						Debug.LogException(exception);
					}
				}
				flag = true;
			}
			else if (rpc == 216608990 && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (ConVar.Global.developer > 2)
				{
					Debug.Log(string.Concat("SV_RPCMessage: ", player, " - DoImmediateDemolish "));
				}
				using (timeWarning1 = TimeWarning.New("DoImmediateDemolish", 0.1f))
				{
					using (timeWarning2 = TimeWarning.New("Conditions", 0.1f))
					{
						if (!BaseEntity.RPC_Server.MaxDistance.Test("DoImmediateDemolish", this, player, 3f))
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
							this.DoImmediateDemolish(rPCMessage);
						}
					}
					catch (Exception exception1)
					{
						player.Kick("RPC Error in DoImmediateDemolish");
						Debug.LogException(exception1);
					}
				}
				flag = true;
			}
			else if (rpc == 1956645865 && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (ConVar.Global.developer > 2)
				{
					Debug.Log(string.Concat("SV_RPCMessage: ", player, " - DoRotation "));
				}
				using (timeWarning1 = TimeWarning.New("DoRotation", 0.1f))
				{
					using (timeWarning2 = TimeWarning.New("Conditions", 0.1f))
					{
						if (!BaseEntity.RPC_Server.MaxDistance.Test("DoRotation", this, player, 3f))
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
							this.DoRotation(rPCMessage);
						}
					}
					catch (Exception exception2)
					{
						player.Kick("RPC Error in DoRotation");
						Debug.LogException(exception2);
					}
				}
				flag = true;
			}
			else if (rpc != -548679239 || !(player != null))
			{
				return base.OnRpcMessage(player, rpc, msg);
			}
			else
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (ConVar.Global.developer > 2)
				{
					Debug.Log(string.Concat("SV_RPCMessage: ", player, " - DoUpgradeToGrade "));
				}
				using (timeWarning1 = TimeWarning.New("DoUpgradeToGrade", 0.1f))
				{
					using (timeWarning2 = TimeWarning.New("Conditions", 0.1f))
					{
						if (!BaseEntity.RPC_Server.MaxDistance.Test("DoUpgradeToGrade", this, player, 3f))
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
							this.DoUpgradeToGrade(rPCMessage);
						}
					}
					catch (Exception exception3)
					{
						player.Kick("RPC Error in DoUpgradeToGrade");
						Debug.LogException(exception3);
					}
				}
				flag = true;
			}
		}
		return flag;
	}

	private void PayForUpgrade(ConstructionGrade g, BasePlayer player)
	{
		List<Item> items = new List<Item>();
		foreach (ItemAmount itemAmount in g.costToBuild)
		{
			player.inventory.Take(items, itemAmount.itemid, (int)itemAmount.amount);
			player.Command(string.Concat(new object[] { "note.inv ", itemAmount.itemid, " ", itemAmount.amount * -1f }), Array.Empty<object>());
		}
		foreach (Item item in items)
		{
			item.Remove(0f);
		}
	}

	public override void PostInitShared()
	{
		this.baseProtection = this.currentGrade.gradeBase.damageProtecton;
		this.grade = this.currentGrade.gradeBase.type;
		base.PostInitShared();
	}

	private void RefreshNeighbours(bool linkToNeighbours)
	{
		List<EntityLink> entityLinks = base.GetEntityLinks(linkToNeighbours);
		for (int i = 0; i < entityLinks.Count; i++)
		{
			EntityLink item = entityLinks[i];
			for (int j = 0; j < item.connections.Count; j++)
			{
				BuildingBlock buildingBlock = item.connections[j].owner as BuildingBlock;
				if (buildingBlock != null)
				{
					if (!Rust.Application.isLoading)
					{
						BuildingBlock.updateSkinQueueServer.Add(buildingBlock);
					}
					else
					{
						buildingBlock.UpdateSkin(true);
					}
				}
			}
		}
	}

	public override float RepairCostFraction()
	{
		return 1f;
	}

	public override void ResetState()
	{
		base.ResetState();
		this.blockDefinition = null;
		this.forceSkinRefresh = false;
		this.modelState = 0;
		this.lastModelState = 0;
		this.grade = BuildingGrade.Enum.Twigs;
		this.lastGrade = BuildingGrade.Enum.None;
		this.DestroySkin();
		this.UpdatePlaceholder(true);
	}

	public override void Save(BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		info.msg.buildingBlock = Facepunch.Pool.Get<ProtoBuf.BuildingBlock>();
		info.msg.buildingBlock.model = this.modelState;
		info.msg.buildingBlock.grade = (int)this.grade;
	}

	public override void ServerInit()
	{
		this.blockDefinition = PrefabAttribute.server.Find<Construction>(this.prefabID);
		if (this.blockDefinition == null)
		{
			Debug.LogError(string.Concat("Couldn't find Construction for prefab ", this.prefabID));
		}
		base.ServerInit();
		this.UpdateSkin(false);
		if (base.HasFlag(BaseEntity.Flags.Reserved1) || !Rust.Application.isLoadingSave)
		{
			this.StartBeingRotatable();
		}
		if (base.HasFlag(BaseEntity.Flags.Reserved2) || !Rust.Application.isLoadingSave)
		{
			this.StartBeingDemolishable();
		}
	}

	public void SetConditionalModel(int state)
	{
		this.modelState = state;
	}

	public void SetGrade(BuildingGrade.Enum iGradeID)
	{
		if (this.blockDefinition.grades == null || (int)iGradeID >= (int)this.blockDefinition.grades.Length)
		{
			Debug.LogError(string.Concat("Tried to set to undefined grade! ", this.blockDefinition.fullName), base.gameObject);
			return;
		}
		this.grade = iGradeID;
		this.grade = this.currentGrade.gradeBase.type;
		this.UpdateGrade();
	}

	public void SetHealthToMax()
	{
		base.health = this.MaxHealth();
	}

	public override bool ShouldBlockProjectiles()
	{
		return this.grade != BuildingGrade.Enum.Twigs;
	}

	public void StartBeingDemolishable()
	{
		base.SetFlag(BaseEntity.Flags.Reserved2, true, false, true);
		base.Invoke(new Action(this.StopBeingDemolishable), 600f);
	}

	public void StartBeingRotatable()
	{
		if (this.blockDefinition.grades == null)
		{
			return;
		}
		if (!this.blockDefinition.canRotate)
		{
			return;
		}
		base.SetFlag(BaseEntity.Flags.Reserved1, true, false, true);
		base.Invoke(new Action(this.StopBeingRotatable), 600f);
	}

	public void StopBeingDemolishable()
	{
		base.SetFlag(BaseEntity.Flags.Reserved2, false, false, true);
		base.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
	}

	public void StopBeingRotatable()
	{
		base.SetFlag(BaseEntity.Flags.Reserved1, false, false, true);
		base.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
	}

	private void UpdateGrade()
	{
		this.baseProtection = this.currentGrade.gradeBase.damageProtecton;
	}

	private void UpdatePlaceholder(bool state)
	{
		if (this.placeholderRenderer)
		{
			this.placeholderRenderer.enabled = state;
		}
		if (this.placeholderCollider)
		{
			this.placeholderCollider.enabled = state;
		}
	}

	public void UpdateSkin(bool force = false)
	{
		if (force)
		{
			this.forceSkinRefresh = true;
		}
		if (!this.NeedsSkinChange())
		{
			return;
		}
		if (this.cachedStability <= 0f || base.isServer)
		{
			this.ChangeSkin();
			return;
		}
		if (!this.skinChange)
		{
			this.skinChange = new DeferredAction(this, new Action(this.ChangeSkin), ActionPriority.Medium);
		}
		if (!this.skinChange.Idle)
		{
			return;
		}
		this.skinChange.Invoke();
	}

	public static class BlockFlags
	{
		public const BaseEntity.Flags CanRotate = BaseEntity.Flags.Reserved1;

		public const BaseEntity.Flags CanDemolish = BaseEntity.Flags.Reserved2;
	}

	public class UpdateSkinWorkQueue : ObjectWorkQueue<BuildingBlock>
	{
		public UpdateSkinWorkQueue()
		{
		}

		protected override void RunJob(BuildingBlock entity)
		{
			if (!this.ShouldAdd(entity))
			{
				return;
			}
			entity.UpdateSkin(true);
		}

		protected override bool ShouldAdd(BuildingBlock entity)
		{
			return entity.IsValid();
		}
	}
}