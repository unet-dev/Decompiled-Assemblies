using Network;
using System;
using UnityEngine;

public class OreResourceEntity : StagedResourceEntity
{
	public GameObjectRef bonusPrefab;

	public GameObjectRef finishEffect;

	public GameObjectRef bonusFailEffect;

	public OreHotSpot _hotSpot;

	private int bonusesKilled;

	private int bonusesSpawned;

	private Vector3 lastNodeDir = Vector3.zero;

	public OreResourceEntity()
	{
	}

	public Vector3 ClampToCylinder(Vector3 localPos, Vector3 cylinderAxis, float cylinderDistance, float minHeight = 0f, float maxHeight = 0f)
	{
		return Vector3.zero;
	}

	public Vector3 ClampToHemisphere(Vector3 hemiInput, float degreesOffset, Vector3 inputVec)
	{
		degreesOffset = Mathf.Clamp(degreesOffset / 180f, -180f, 180f);
		Vector3 vector3 = hemiInput + (Vector3.one * degreesOffset);
		Vector3 vector31 = vector3.normalized;
		vector3 = hemiInput + (Vector3.one * -degreesOffset);
		Vector3 vector32 = vector3.normalized;
		for (int i = 0; i < 3; i++)
		{
			inputVec[i] = Mathf.Clamp(inputVec[i], vector32[i], vector31[i]);
		}
		return inputVec;
	}

	public void CleanupBonus()
	{
		if (this._hotSpot)
		{
			this._hotSpot.Kill(BaseNetworkable.DestroyMode.None);
		}
		this._hotSpot = null;
	}

	public void DelayedBonusSpawn()
	{
		base.CancelInvoke(new Action(this.RespawnBonus));
		base.Invoke(new Action(this.RespawnBonus), 0.25f);
	}

	public void FinishBonusAssigned()
	{
		Effect.server.Run(this.finishEffect.resourcePath, base.transform.position, base.transform.up, null, false);
	}

	public override void OnAttacked(HitInfo info)
	{
		if (base.isClient)
		{
			base.OnAttacked(info);
			return;
		}
		if (!info.DidGather && info.gatherScale > 0f && this._hotSpot)
		{
			if (Vector3.Distance(info.HitPositionWorld, this._hotSpot.transform.position) <= this._hotSpot.GetComponent<SphereCollider>().radius * 1.5f || info.Weapon is Jackhammer)
			{
				this.bonusesKilled++;
				info.gatherScale = 1f + Mathf.Clamp((float)this.bonusesKilled * 0.5f, 0f, 2f);
				this._hotSpot.FireFinishEffect();
				base.ClientRPC<int, Vector3>(null, "PlayBonusLevelSound", this.bonusesKilled, this._hotSpot.transform.position);
			}
			else if (this.bonusesKilled > 0)
			{
				this.bonusesKilled = 0;
				Effect.server.Run(this.bonusFailEffect.resourcePath, base.transform.position, base.transform.up, null, false);
			}
			if (this.bonusesKilled > 0)
			{
				this.CleanupBonus();
			}
		}
		if (this._hotSpot == null)
		{
			this.DelayedBonusSpawn();
		}
		base.OnAttacked(info);
	}

	public override void OnKilled(HitInfo info)
	{
		this.CleanupBonus();
		base.OnKilled(info);
	}

	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning timeWarning = TimeWarning.New("OreResourceEntity.OnRpcMessage", 0.1f))
		{
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	public Vector3 RandomCircle(float distance = 1f, bool allowInside = false)
	{
		Vector2 vector2 = (allowInside ? UnityEngine.Random.insideUnitCircle : UnityEngine.Random.insideUnitCircle.normalized);
		return new Vector3(vector2.x, 0f, vector2.y);
	}

	public static Vector3 RandomCylinderPointAroundVector(Vector3 input, float distance, float minHeight = 0f, float maxHeight = 0f, bool allowInside = false)
	{
		Vector2 vector2 = (allowInside ? UnityEngine.Random.insideUnitCircle : UnityEngine.Random.insideUnitCircle.normalized);
		Vector3 vector3 = new Vector3(vector2.x, 0f, vector2.y);
		Vector3 vector31 = vector3.normalized * distance;
		vector31.y = UnityEngine.Random.Range(minHeight, maxHeight);
		return vector31;
	}

	public Vector3 RandomHemisphereDirection(Vector3 input, float degreesOffset, bool allowInside = true, bool changeHeight = true)
	{
		degreesOffset = Mathf.Clamp(degreesOffset / 180f, -180f, 180f);
		Vector2 vector2 = (allowInside ? UnityEngine.Random.insideUnitCircle : UnityEngine.Random.insideUnitCircle.normalized);
		Vector3 vector3 = new Vector3(vector2.x * degreesOffset, (changeHeight ? UnityEngine.Random.Range(-1f, 1f) * degreesOffset : 0f), vector2.y * degreesOffset);
		return (input + vector3).normalized;
	}

	public void RespawnBonus()
	{
		this.CleanupBonus();
		this._hotSpot = this.SpawnBonusSpot(this.lastNodeDir);
	}

	public override void ServerInit()
	{
		base.ServerInit();
		this._hotSpot = this.SpawnBonusSpot(Vector3.zero);
	}

	public OreHotSpot SpawnBonusSpot(Vector3 lastDirection)
	{
		RaycastHit raycastHit;
		Vector3 vector3;
		if (base.isClient)
		{
			return null;
		}
		if (!this.bonusPrefab.isValid)
		{
			return null;
		}
		Vector2 vector2 = UnityEngine.Random.insideUnitCircle.normalized;
		Vector3 vector31 = Vector3.zero;
		MeshCollider stageComponent = base.GetStageComponent<MeshCollider>();
		Transform transforms = base.transform;
		Bounds bound = stageComponent.bounds;
		Vector3 vector32 = transforms.InverseTransformPoint(bound.center);
		if (lastDirection != Vector3.zero)
		{
			Vector3 vector33 = Vector3.Cross(this.lastNodeDir, Vector3.up);
			float single = UnityEngine.Random.Range(0.25f, 0.5f);
			float single1 = (UnityEngine.Random.Range(0, 2) == 0 ? -1f : 1f);
			vector3 = this.lastNodeDir + ((vector33 * single) * single1);
			Vector3 vector34 = vector3.normalized;
			this.lastNodeDir = vector34;
			vector31 = base.transform.position + (base.transform.TransformDirection(vector34) * 2f);
			float single2 = UnityEngine.Random.Range(1f, 1.5f);
			vector31 = vector31 + (base.transform.up * (vector32.y + single2));
		}
		else
		{
			Vector3 vector35 = this.RandomCircle(1f, false);
			this.lastNodeDir = vector35.normalized;
			Vector3 vector36 = base.transform.TransformDirection(vector35.normalized);
			vector35 = (base.transform.position + (base.transform.up * (vector32.y + 0.5f))) + (vector36.normalized * 2.5f);
			vector31 = vector35;
		}
		this.bonusesSpawned++;
		bound = stageComponent.bounds;
		vector3 = bound.center - vector31;
		Vector3 vector37 = vector3.normalized;
		if (!stageComponent.Raycast(new Ray(vector31, vector37), out raycastHit, 10f))
		{
			return null;
		}
		OreHotSpot oreHotSpot = GameManager.server.CreateEntity(this.bonusPrefab.resourcePath, raycastHit.point - (vector37 * 0.025f), Quaternion.LookRotation(raycastHit.normal, Vector3.up), true) as OreHotSpot;
		oreHotSpot.Spawn();
		oreHotSpot.SendMessage("OreOwner", this);
		return oreHotSpot;
	}

	protected override void UpdateNetworkStage()
	{
		int num = this.stage;
		base.UpdateNetworkStage();
		if (this.stage != num && this._hotSpot)
		{
			this.DelayedBonusSpawn();
		}
	}
}