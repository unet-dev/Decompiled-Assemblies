using ConVar;
using Network;
using Oxide.Core;
using Rust;
using System;
using UnityEngine;
using UnityEngine.Assertions;

public class BaseMountable : BaseCombatEntity
{
	public BasePlayer _mounted;

	[Header("View")]
	public Transform eyeOverride;

	public Vector2 pitchClamp = new Vector2(-80f, 50f);

	public Vector2 yawClamp = new Vector2(-80f, 80f);

	public bool canWieldItems = true;

	[Header("Mounting")]
	public PlayerModel.MountPoses mountPose;

	public float maxMountDistance = 1.5f;

	public Transform mountAnchor;

	public Transform dismountAnchor;

	public Transform[] dismountPositions;

	public Transform dismountCheckEyes;

	public SoundDefinition mountSoundDef;

	public SoundDefinition dismountSoundDef;

	public bool isMobile;

	public const float playerHeight = 1.8f;

	public const float playerRadius = 0.5f;

	public readonly static Vector3 DISMOUNT_POS_INVALID;

	protected override float PositionTickRate
	{
		get
		{
			return 0.05f;
		}
	}

	static BaseMountable()
	{
		BaseMountable.DISMOUNT_POS_INVALID = new Vector3(0f, 5000f, 0f);
	}

	public BaseMountable()
	{
	}

	public virtual bool AttemptDismount(BasePlayer player)
	{
		if (player != this._mounted)
		{
			return false;
		}
		this.DismountPlayer(player, false);
		return true;
	}

	public virtual void AttemptMount(BasePlayer player)
	{
		if (this._mounted != null)
		{
			return;
		}
		if (!this.HasValidDismountPosition(player))
		{
			return;
		}
		this.MountPlayer(player);
	}

	public virtual bool CanHoldItems()
	{
		return this.canWieldItems;
	}

	public override bool CanPickup(BasePlayer player)
	{
		if (!base.CanPickup(player))
		{
			return false;
		}
		return !this.IsMounted();
	}

	public static Vector3 ConvertVector(Vector3 vec)
	{
		// 
		// Current member / type: UnityEngine.Vector3 BaseMountable::ConvertVector(UnityEngine.Vector3)
		// File path: D:\GameServers\Rust\RustDedicated_Data\Managed\Assembly-CSharp.dll
		// 
		// Product version: 2019.1.118.0
		// Exception in: UnityEngine.Vector3 ConvertVector(UnityEngine.Vector3)
		// 
		// Managed pointer usage not in SSA
		//    at ..(BinaryExpression ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Steps\ManagedPointersRemovalStep.cs:line 100
		//    at ..(BinaryExpression ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Steps\ManagedPointersRemovalStep.cs:line 76
		//    at ..Visit(ICodeNode ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Ast\BaseCodeVisitor.cs:line 141
		//    at ..() in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Steps\ManagedPointersRemovalStep.cs:line 38
		//    at ..(DecompilationContext ,  ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Steps\ManagedPointersRemovalStep.cs:line 20
		//    at ..(MethodBody ,  , ILanguage ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\DecompilationPipeline.cs:line 88
		//    at ..(MethodBody , ILanguage ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\DecompilationPipeline.cs:line 70
		//    at Telerik.JustDecompiler.Decompiler.Extensions.( , ILanguage , MethodBody , DecompilationContext& ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\Extensions.cs:line 95
		//    at Telerik.JustDecompiler.Decompiler.Extensions.(MethodBody , ILanguage , DecompilationContext& ,  ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\Extensions.cs:line 58
		//    at ..(ILanguage , MethodDefinition ,  ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\WriterContextServices\BaseWriterContextService.cs:line 117
		// 
		// mailto: JustDecompilePublicFeedback@telerik.com

	}

	public virtual bool DirectlyMountable()
	{
		return true;
	}

	public virtual void DismountAllPlayers()
	{
		if (this._mounted)
		{
			this.DismountPlayer(this._mounted, false);
		}
	}

	public void DismountPlayer(BasePlayer player, bool lite = false)
	{
		if (this._mounted == null)
		{
			return;
		}
		if (this._mounted != player)
		{
			return;
		}
		if (Interface.CallHook("CanDismountEntity", player, this) != null)
		{
			return;
		}
		if (lite)
		{
			this._mounted.DismountObject();
			this._mounted = null;
			base.SetFlag(BaseEntity.Flags.Busy, false, false, true);
			return;
		}
		Vector3 dismountPosition = this.GetDismountPosition(player);
		if (dismountPosition == BaseMountable.DISMOUNT_POS_INVALID)
		{
			dismountPosition = player.transform.position;
			this._mounted.DismountObject();
			this._mounted.MovePosition(dismountPosition);
			this._mounted.ClientRPCPlayer<Vector3>(null, this._mounted, "ForcePositionTo", dismountPosition);
			BasePlayer basePlayer = this._mounted;
			this._mounted = null;
			Debug.LogWarning(string.Concat(new object[] { "Killing player due to invalid dismount point :", player.displayName, " / ", player.userID, " on obj : ", base.gameObject.name }));
			basePlayer.Hurt(1000f, DamageType.Suicide, basePlayer, false);
			base.SetFlag(BaseEntity.Flags.Busy, false, false, true);
			return;
		}
		this._mounted.DismountObject();
		this._mounted.transform.rotation = Quaternion.LookRotation(Vector3.forward, Vector3.up);
		this._mounted.MovePosition(dismountPosition);
		this._mounted.SendNetworkUpdateImmediate(false);
		this._mounted = null;
		base.SetFlag(BaseEntity.Flags.Busy, false, false, true);
		player.ForceUpdateTriggers(true, true, true);
		if (!player.GetParentEntity())
		{
			player.ClientRPCPlayer<Vector3>(null, player, "ForcePositionTo", dismountPosition);
			Interface.CallHook("OnEntityDismounted", this, player);
			return;
		}
		BaseEntity parentEntity = player.GetParentEntity();
		player.ClientRPCPlayer<Vector3, uint>(null, player, "ForcePositionToParentOffset", parentEntity.transform.InverseTransformPoint(dismountPosition), parentEntity.net.ID);
	}

	public Vector3 DismountVisCheckOrigin()
	{
		if (this.dismountCheckEyes != null)
		{
			return this.dismountCheckEyes.position;
		}
		return base.transform.position + new Vector3(0f, 0.3f, 0f);
	}

	public virtual Vector3 EyePositionForPlayer(BasePlayer player)
	{
		if (player.GetMounted() != this)
		{
			return Vector3.zero;
		}
		return this.eyeOverride.transform.position;
	}

	public void FixedUpdate()
	{
		if (base.isClient)
		{
			return;
		}
		if (!this.isMobile)
		{
			return;
		}
		this.VehicleFixedUpdate();
		if (this._mounted)
		{
			this._mounted.transform.rotation = this.mountAnchor.transform.rotation;
			this._mounted.ServerRotation = this.mountAnchor.transform.rotation;
			this._mounted.MovePosition(this.mountAnchor.transform.position);
		}
	}

	public virtual float GetComfort()
	{
		return 0f;
	}

	public virtual Vector3 GetDismountPosition(BasePlayer player)
	{
		BaseVehicle baseVehicle = this.VehicleParent();
		if (baseVehicle != null)
		{
			return baseVehicle.GetDismountPosition(player);
		}
		int num = 0;
		Transform[] transformArrays = this.dismountPositions;
		for (int i = 0; i < (int)transformArrays.Length; i++)
		{
			Transform transforms = transformArrays[i];
			if (this.ValidDismountPosition(transforms.transform.position))
			{
				return transforms.transform.position;
			}
			num++;
		}
		Debug.LogWarning(string.Concat(new object[] { "Failed to find dismount position for player :", player.displayName, " / ", player.userID, " on obj : ", base.gameObject.name }));
		return BaseMountable.DISMOUNT_POS_INVALID;
	}

	public BasePlayer GetMounted()
	{
		return this._mounted;
	}

	public virtual float GetSteering(BasePlayer player)
	{
		return 0f;
	}

	public virtual bool HasValidDismountPosition(BasePlayer player)
	{
		BaseVehicle baseVehicle = this.VehicleParent();
		if (baseVehicle != null)
		{
			return baseVehicle.HasValidDismountPosition(player);
		}
		Transform[] transformArrays = this.dismountPositions;
		for (int i = 0; i < (int)transformArrays.Length; i++)
		{
			if (this.ValidDismountPosition(transformArrays[i].transform.position))
			{
				return true;
			}
		}
		return false;
	}

	public virtual bool IsMounted()
	{
		return this._mounted != null;
	}

	public virtual void LightToggle(BasePlayer player)
	{
	}

	public override float MaxVelocity()
	{
		BaseEntity parentEntity = base.GetParentEntity();
		if (parentEntity)
		{
			return parentEntity.MaxVelocity();
		}
		return base.MaxVelocity();
	}

	public virtual void MounteeTookDamage(BasePlayer mountee, HitInfo info)
	{
	}

	public void MountPlayer(BasePlayer player)
	{
		if (this._mounted != null)
		{
			return;
		}
		if (Interface.CallHook("CanMountEntity", player, this) != null)
		{
			return;
		}
		player.EnsureDismounted();
		this._mounted = player;
		TriggerParent triggerParent = player.FindTrigger<TriggerParent>();
		if (triggerParent)
		{
			triggerParent.OnTriggerExit(player.GetComponent<Collider>());
		}
		player.MountObject(this, 0);
		player.MovePosition(this.mountAnchor.transform.position);
		player.transform.rotation = this.mountAnchor.transform.rotation;
		player.ServerRotation = this.mountAnchor.transform.rotation;
		player.OverrideViewAngles(this.mountAnchor.transform.rotation.eulerAngles);
		this._mounted.eyes.NetworkUpdate(this.mountAnchor.transform.rotation);
		player.ClientRPCPlayer<Vector3>(null, player, "ForcePositionTo", player.transform.position);
		base.SetFlag(BaseEntity.Flags.Busy, true, false, true);
		Interface.CallHook("OnEntityMounted", this, player);
	}

	public bool NearMountPoint(BasePlayer player)
	{
		RaycastHit raycastHit;
		if (Vector3.Distance(player.transform.position, this.mountAnchor.position) <= this.maxMountDistance)
		{
			if (!UnityEngine.Physics.SphereCast(player.eyes.HeadRay(), 0.25f, out raycastHit, 2f, 1218652417))
			{
				return false;
			}
			if (raycastHit.GetEntity() && raycastHit.GetEntity().net.ID == this.net.ID)
			{
				return true;
			}
		}
		return false;
	}

	public override void OnKilled(HitInfo info)
	{
		this.DismountAllPlayers();
		base.OnKilled(info);
	}

	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		BaseEntity.RPCMessage rPCMessage;
		bool flag;
		using (TimeWarning timeWarning = TimeWarning.New("BaseMountable.OnRpcMessage", 0.1f))
		{
			if (rpc == 1735799362 && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (ConVar.Global.developer > 2)
				{
					Debug.Log(string.Concat("SV_RPCMessage: ", player, " - RPC_WantsDismount "));
				}
				using (TimeWarning timeWarning1 = TimeWarning.New("RPC_WantsDismount", 0.1f))
				{
					try
					{
						using (TimeWarning timeWarning2 = TimeWarning.New("Call", 0.1f))
						{
							rPCMessage = new BaseEntity.RPCMessage()
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.RPC_WantsDismount(rPCMessage);
						}
					}
					catch (Exception exception)
					{
						player.Kick("RPC Error in RPC_WantsDismount");
						Debug.LogException(exception);
					}
				}
				flag = true;
			}
			else if (rpc != -280666344 || !(player != null))
			{
				return base.OnRpcMessage(player, rpc, msg);
			}
			else
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (ConVar.Global.developer > 2)
				{
					Debug.Log(string.Concat("SV_RPCMessage: ", player, " - RPC_WantsMount "));
				}
				using (timeWarning1 = TimeWarning.New("RPC_WantsMount", 0.1f))
				{
					using (timeWarning2 = TimeWarning.New("Conditions", 0.1f))
					{
						if (!BaseEntity.RPC_Server.IsVisible.Test("RPC_WantsMount", this, player, 3f))
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
							this.RPC_WantsMount(rPCMessage);
						}
					}
					catch (Exception exception1)
					{
						player.Kick("RPC Error in RPC_WantsMount");
						Debug.LogException(exception1);
					}
				}
				flag = true;
			}
		}
		return flag;
	}

	public virtual void PlayerServerInput(InputState inputState, BasePlayer player)
	{
		bool flag = player != this._mounted;
	}

	public override void PostServerLoad()
	{
		base.PostServerLoad();
		base.SetFlag(BaseEntity.Flags.Busy, false, false, true);
	}

	[RPC_Server]
	public void RPC_WantsDismount(BaseEntity.RPCMessage msg)
	{
		BasePlayer basePlayer = msg.player;
		if (!this.HasValidDismountPosition(basePlayer))
		{
			return;
		}
		this.AttemptDismount(basePlayer);
	}

	[IsVisible(3f)]
	[RPC_Server]
	public void RPC_WantsMount(BaseEntity.RPCMessage msg)
	{
		BasePlayer basePlayer = msg.player;
		if (!this.DirectlyMountable())
		{
			return;
		}
		this.AttemptMount(basePlayer);
	}

	public override void ServerInit()
	{
		base.ServerInit();
	}

	public virtual bool ValidDismountPosition(Vector3 disPos)
	{
		if (!UnityEngine.Physics.CheckCapsule(disPos + new Vector3(0f, 0.5f, 0f), disPos + new Vector3(0f, 1.3f, 0f), 0.5f, 1537286401))
		{
			Vector3 vector3 = this.DismountVisCheckOrigin();
			Vector3 vector31 = disPos + (base.transform.up * 0.5f);
			if (base.IsVisible(vector31, Single.PositiveInfinity) && !UnityEngine.Physics.Linecast(vector3, vector31, 1486946561))
			{
				return true;
			}
		}
		return false;
	}

	public virtual void VehicleFixedUpdate()
	{
	}

	public BaseVehicle VehicleParent()
	{
		return base.GetParentEntity() as BaseVehicle;
	}

	public virtual float WaterFactorForPlayer(BasePlayer player)
	{
		return WaterLevel.Factor(player.WorldSpaceBounds().ToBounds());
	}
}