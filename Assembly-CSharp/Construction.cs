using Facepunch;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Construction : PrefabAttribute
{
	public BaseEntity.Menu.Option info;

	public bool canBypassBuildingPermission;

	public bool canRotate;

	public bool checkVolumeOnRotate;

	public bool checkVolumeOnUpgrade;

	public bool canPlaceAtMaxDistance;

	public Vector3 rotationAmount = new Vector3(0f, 90f, 0f);

	[Range(0f, 10f)]
	public float healthMultiplier = 1f;

	[Range(0f, 10f)]
	public float costMultiplier = 1f;

	[Range(1f, 50f)]
	public float maxplaceDistance = 4f;

	public Mesh guideMesh;

	[NonSerialized]
	public Socket_Base[] allSockets;

	[NonSerialized]
	public BuildingProximity[] allProximities;

	[NonSerialized]
	public ConstructionGrade defaultGrade;

	[NonSerialized]
	public SocketHandle socketHandle;

	[NonSerialized]
	public Bounds bounds;

	[NonSerialized]
	public bool isBuildingPrivilege;

	[NonSerialized]
	public ConstructionGrade[] grades;

	[NonSerialized]
	public Deployable deployable;

	[NonSerialized]
	public ConstructionPlaceholder placeholder;

	public static string lastPlacementError;

	public Construction()
	{
	}

	protected override void AttributeSetup(GameObject rootObj, string name, bool serverside, bool clientside, bool bundling)
	{
		base.AttributeSetup(rootObj, name, serverside, clientside, bundling);
		this.isBuildingPrivilege = rootObj.GetComponent<BuildingPrivlidge>();
		this.bounds = rootObj.GetComponent<BaseEntity>().bounds;
		this.deployable = base.GetComponent<Deployable>();
		this.placeholder = base.GetComponentInChildren<ConstructionPlaceholder>();
		this.allSockets = base.GetComponentsInChildren<Socket_Base>(true);
		this.allProximities = base.GetComponentsInChildren<BuildingProximity>(true);
		this.socketHandle = base.GetComponentsInChildren<SocketHandle>(true).FirstOrDefault<SocketHandle>();
		ConstructionGrade[] components = rootObj.GetComponents<ConstructionGrade>();
		this.grades = new ConstructionGrade[5];
		ConstructionGrade[] constructionGradeArray = components;
		for (int i = 0; i < (int)constructionGradeArray.Length; i++)
		{
			ConstructionGrade constructionGrade = constructionGradeArray[i];
			constructionGrade.construction = this;
			this.grades[(int)constructionGrade.gradeBase.type] = constructionGrade;
		}
		for (int j = 0; j < (int)this.grades.Length; j++)
		{
			if (this.grades[j] != null)
			{
				this.defaultGrade = this.grades[j];
				return;
			}
		}
	}

	public BaseEntity CreateConstruction(Construction.Target target, bool bNeedsValidPlacement = false)
	{
		GameObject gameObject = GameManager.server.CreatePrefab(this.fullName, Vector3.zero, Quaternion.identity, false);
		bool flag = this.UpdatePlacement(gameObject.transform, this, ref target);
		BaseEntity baseEntity = gameObject.ToBaseEntity();
		if (bNeedsValidPlacement && !flag)
		{
			if (!baseEntity.IsValid())
			{
				GameManager.Destroy(gameObject, 0f);
			}
			else
			{
				baseEntity.Kill(BaseNetworkable.DestroyMode.None);
			}
			return null;
		}
		DecayEntity decayEntity = baseEntity as DecayEntity;
		if (decayEntity)
		{
			decayEntity.AttachToBuilding(target.entity as DecayEntity);
		}
		return baseEntity;
	}

	public void FindMaleSockets(Construction.Target target, List<Socket_Base> sockets)
	{
		Socket_Base[] socketBaseArray = this.allSockets;
		for (int i = 0; i < (int)socketBaseArray.Length; i++)
		{
			Socket_Base socketBase = socketBaseArray[i];
			if (socketBase.male && !socketBase.maleDummy && socketBase.TestTarget(target))
			{
				sockets.Add(socketBase);
			}
		}
	}

	protected override Type GetIndexedType()
	{
		return typeof(Construction);
	}

	public bool HasMaleSockets(Construction.Target target)
	{
		Socket_Base[] socketBaseArray = this.allSockets;
		for (int i = 0; i < (int)socketBaseArray.Length; i++)
		{
			Socket_Base socketBase = socketBaseArray[i];
			if (socketBase.male && !socketBase.maleDummy && socketBase.TestTarget(target))
			{
				return true;
			}
		}
		return false;
	}

	private bool TestPlacingThroughRock(ref Construction.Placement placement, Construction.Target target)
	{
		RaycastHit raycastHit;
		OBB oBB = new OBB(placement.position, Vector3.one, placement.rotation, this.bounds);
		Vector3 center = target.player.GetCenter(true);
		Vector3 vector3 = target.ray.origin;
		if (Physics.Linecast(center, vector3, 65536, QueryTriggerInteraction.Ignore))
		{
			return false;
		}
		if (Physics.Linecast(vector3, (oBB.Trace(target.ray, out raycastHit, Single.PositiveInfinity) ? raycastHit.point : oBB.ClosestPoint(vector3)), 65536, QueryTriggerInteraction.Ignore))
		{
			return false;
		}
		return true;
	}

	private static bool TestPlacingThroughWall(ref Construction.Placement placement, Transform transform, Construction common, Construction.Target target)
	{
		RaycastHit raycastHit;
		Vector3 vector3 = placement.position - target.ray.origin;
		if (!Physics.Raycast(target.ray.origin, vector3.normalized, out raycastHit, vector3.magnitude, 2097152))
		{
			return true;
		}
		StabilityEntity entity = raycastHit.GetEntity() as StabilityEntity;
		if (entity != null && target.entity == entity)
		{
			return true;
		}
		if (vector3.magnitude - raycastHit.distance < 0.2f)
		{
			return true;
		}
		Construction.lastPlacementError = "object in placement path";
		transform.position = raycastHit.point;
		transform.rotation = placement.rotation;
		return false;
	}

	public bool UpdatePlacement(Transform transform, Construction common, ref Construction.Target target)
	{
		bool flag;
		if (!target.valid)
		{
			return false;
		}
		if (!common.canBypassBuildingPermission && !target.player.CanBuild())
		{
			Construction.lastPlacementError = "Player doesn't have permission";
			return false;
		}
		List<Socket_Base> list = Pool.GetList<Socket_Base>();
		common.FindMaleSockets(target, list);
		List<Socket_Base>.Enumerator enumerator = list.GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				Socket_Base current = enumerator.Current;
				Construction.Placement placement = null;
				if (target.entity != null && target.socket != null && target.entity.IsOccupied(target.socket))
				{
					continue;
				}
				if (placement == null)
				{
					placement = current.DoPlacement(target);
				}
				if (placement == null)
				{
					continue;
				}
				if (!current.CheckSocketMods(placement))
				{
					transform.position = placement.position;
					transform.rotation = placement.rotation;
				}
				else if (!this.TestPlacingThroughRock(ref placement, target))
				{
					transform.position = placement.position;
					transform.rotation = placement.rotation;
					Construction.lastPlacementError = "Placing through rock";
				}
				else if (!Construction.TestPlacingThroughWall(ref placement, transform, common, target))
				{
					transform.position = placement.position;
					transform.rotation = placement.rotation;
					Construction.lastPlacementError = "Placing through wall";
				}
				else if (Vector3.Distance(placement.position, target.player.eyes.position) <= common.maxplaceDistance + 1f)
				{
					DeployVolume[] deployVolumeArray = PrefabAttribute.server.FindAll<DeployVolume>(this.prefabID);
					if (DeployVolume.Check(placement.position, placement.rotation, deployVolumeArray, -1))
					{
						transform.position = placement.position;
						transform.rotation = placement.rotation;
						Construction.lastPlacementError = "Not enough space";
					}
					else if (BuildingProximity.Check(target.player, this, placement.position, placement.rotation))
					{
						transform.position = placement.position;
						transform.rotation = placement.rotation;
					}
					else if (!common.isBuildingPrivilege || target.player.CanPlaceBuildingPrivilege(placement.position, placement.rotation, common.bounds))
					{
						bool flag1 = target.player.IsBuildingBlocked(placement.position, placement.rotation, common.bounds);
						if (common.canBypassBuildingPermission || !flag1)
						{
							target.inBuildingPrivilege = flag1;
							transform.position = placement.position;
							transform.rotation = placement.rotation;
							Pool.FreeList<Socket_Base>(ref list);
							flag = true;
							return flag;
						}
						else
						{
							transform.position = placement.position;
							transform.rotation = placement.rotation;
							Construction.lastPlacementError = "Building privilege";
						}
					}
					else
					{
						transform.position = placement.position;
						transform.rotation = placement.rotation;
						Construction.lastPlacementError = "Cannot stack building privileges";
					}
				}
				else
				{
					transform.position = placement.position;
					transform.rotation = placement.rotation;
					Construction.lastPlacementError = "Too far away";
				}
			}
			Pool.FreeList<Socket_Base>(ref list);
			return false;
		}
		finally
		{
			((IDisposable)enumerator).Dispose();
		}
		return flag;
	}

	public class Grade
	{
		public BuildingGrade grade;

		public float maxHealth;

		public List<ItemAmount> costToBuild;

		public ProtectionProperties damageProtecton
		{
			get
			{
				return this.grade.damageProtecton;
			}
		}

		public PhysicMaterial physicMaterial
		{
			get
			{
				return this.grade.physicMaterial;
			}
		}

		public Grade()
		{
		}
	}

	public class Placement
	{
		public Vector3 position;

		public Quaternion rotation;

		public Placement()
		{
		}
	}

	public struct Target
	{
		public bool valid;

		public Ray ray;

		public BaseEntity entity;

		public Socket_Base socket;

		public bool onTerrain;

		public Vector3 position;

		public Vector3 normal;

		public Vector3 rotation;

		public BasePlayer player;

		public bool inBuildingPrivilege;

		public Vector3 GetWorldPosition()
		{
			Matrix4x4 matrix4x4 = this.entity.transform.localToWorldMatrix;
			return matrix4x4.MultiplyPoint3x4(this.socket.position);
		}

		public Quaternion GetWorldRotation(bool female)
		{
			Quaternion quaternion = this.socket.rotation;
			if ((!this.socket.male ? false : this.socket.female) & female)
			{
				quaternion = this.socket.rotation * Quaternion.Euler(180f, 0f, 180f);
			}
			return this.entity.transform.rotation * quaternion;
		}
	}
}