using Network;
using Rust;
using System;
using UnityEngine;

public class TreeEntity : ResourceEntity
{
	public GameObjectRef prefab;

	public bool hasBonusGame = true;

	public GameObjectRef bonusHitEffect;

	public GameObjectRef bonusHitSound;

	public Collider serverCollider;

	public Collider clientCollider;

	public SoundDefinition smallCrackSoundDef;

	public SoundDefinition medCrackSoundDef;

	private float lastAttackDamage;

	[NonSerialized]
	protected BaseEntity xMarker;

	private int currentBonusLevel;

	private float lastDirection = -1f;

	private float lastHitTime;

	[Header("Falling")]
	public bool fallOnKilled = true;

	public float fallDuration = 1.5f;

	public GameObjectRef fallStartSound;

	public GameObjectRef fallImpactSound;

	public GameObjectRef fallImpactParticles;

	public SoundDefinition fallLeavesLoopDef;

	[NonSerialized]
	public bool[] usedHeights = new bool[20];

	public bool impactSoundPlayed;

	[NonSerialized]
	public float treeDistanceUponFalling;

	public TreeEntity()
	{
	}

	public bool BonusActive()
	{
		return this.xMarker != null;
	}

	public override float BoundsPadding()
	{
		return 1f;
	}

	public void CleanupMarker()
	{
		if (this.xMarker)
		{
			this.xMarker.Kill(BaseNetworkable.DestroyMode.None);
		}
		this.xMarker = null;
	}

	public void DelayedKill()
	{
		base.Kill(BaseNetworkable.DestroyMode.None);
	}

	public override void DestroyShared()
	{
		base.DestroyShared();
		if (base.isServer)
		{
			this.CleanupMarker();
		}
	}

	public bool DidHitMarker(HitInfo info)
	{
		if (this.xMarker == null)
		{
			return false;
		}
		if ((double)Vector3.Dot(Vector3Ex.Direction2D(base.transform.position, this.xMarker.transform.position), info.attackNormal) >= 0.3 && Vector3.Distance(this.xMarker.transform.position, info.HitPositionWorld) <= 0.2f)
		{
			return true;
		}
		return false;
	}

	public Collider GetCollider()
	{
		if (base.isServer)
		{
			if (this.serverCollider != null)
			{
				return this.serverCollider;
			}
			return base.GetComponentInChildren<CapsuleCollider>();
		}
		if (this.clientCollider != null)
		{
			return this.clientCollider;
		}
		return base.GetComponent<Collider>();
	}

	public override void OnAttacked(HitInfo info)
	{
		bool canGather = info.CanGather;
		float single = Time.time - this.lastHitTime;
		this.lastHitTime = Time.time;
		if (!this.hasBonusGame || !canGather || info.Initiator == null || this.BonusActive() && !this.DidHitMarker(info))
		{
			base.OnAttacked(info);
			return;
		}
		if (this.xMarker != null && !info.DidGather && info.gatherScale > 0f)
		{
			this.xMarker.ClientRPC<int>(null, "MarkerHit", this.currentBonusLevel);
			this.currentBonusLevel++;
			info.gatherScale = 1f + Mathf.Clamp((float)this.currentBonusLevel * 0.125f, 0f, 1f);
		}
		Vector3 vector3 = (this.xMarker != null ? this.xMarker.transform.position : info.HitPositionWorld);
		this.CleanupMarker();
		Vector3 vector31 = Vector3Ex.Direction2D(base.transform.position, vector3);
		Vector3 vector32 = Vector3.Cross(vector31, Vector3.up);
		float single1 = this.lastDirection;
		float single2 = UnityEngine.Random.Range(0.5f, 0.5f);
		Vector3 vector33 = Vector3.Lerp(-vector31, vector32 * single1, single2);
		Vector3 vector34 = base.transform.InverseTransformDirection(vector33.normalized) * 2.5f;
		vector34 = base.transform.InverseTransformPoint(this.GetCollider().ClosestPoint(base.transform.TransformPoint(vector34)));
		Vector3 vector35 = base.transform.TransformPoint(vector34);
		Vector3 vector36 = base.transform.InverseTransformPoint(info.HitPositionWorld);
		vector34.y = vector36.y;
		Vector3 vector37 = base.transform.InverseTransformPoint(info.Initiator.CenterPoint());
		float single3 = Mathf.Max(0.75f, vector37.y);
		float single4 = vector37.y + 0.5f;
		vector34.y = Mathf.Clamp(vector34.y + UnityEngine.Random.Range(0.1f, 0.2f) * (UnityEngine.Random.Range(0, 2) == 0 ? -1f : 1f), single3, single4);
		Vector3 vector38 = Vector3Ex.Direction2D(base.transform.position, vector35);
		Vector3 vector39 = vector38;
		vector38 = base.transform.InverseTransformDirection(vector38);
		Quaternion quaternion = QuaternionEx.LookRotationNormal(-vector38, Vector3.zero);
		vector34 = base.transform.TransformPoint(vector34);
		quaternion = QuaternionEx.LookRotationNormal(-vector39, Vector3.zero);
		this.xMarker = GameManager.server.CreateEntity("assets/content/nature/treesprefabs/trees/effects/tree_marking.prefab", vector34, quaternion, true);
		this.xMarker.Spawn();
		if (single > 5f)
		{
			this.StartBonusGame();
		}
		base.OnAttacked(info);
		if (this.health > 0f)
		{
			this.lastAttackDamage = info.damageTypes.Total();
			int num = Mathf.CeilToInt(this.health / this.lastAttackDamage);
			if (num < 2)
			{
				base.ClientRPC<int>(null, "CrackSound", 1);
				return;
			}
			if (num < 5)
			{
				base.ClientRPC<int>(null, "CrackSound", 0);
			}
		}
	}

	public override void OnKilled(HitInfo info)
	{
		if (this.isKilled)
		{
			return;
		}
		this.isKilled = true;
		this.CleanupMarker();
		if (!this.fallOnKilled)
		{
			this.DelayedKill();
			return;
		}
		Collider collider = this.GetCollider();
		if (collider)
		{
			collider.enabled = false;
		}
		base.ClientRPC<Vector3>(null, "TreeFall", info.attackNormal);
		base.Invoke(new Action(this.DelayedKill), this.fallDuration + 1f);
	}

	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning timeWarning = TimeWarning.New("TreeEntity.OnRpcMessage", 0.1f))
		{
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	public override void ServerInit()
	{
		base.ServerInit();
		this.lastDirection = (float)((UnityEngine.Random.Range(0, 2) == 0 ? -1 : 1));
	}

	public void StartBonusGame()
	{
		if (base.IsInvoking(new Action(this.StopBonusGame)))
		{
			base.CancelInvoke(new Action(this.StopBonusGame));
		}
		base.Invoke(new Action(this.StopBonusGame), 60f);
	}

	public void StopBonusGame()
	{
		this.CleanupMarker();
		this.lastHitTime = 0f;
		this.currentBonusLevel = 0;
	}
}