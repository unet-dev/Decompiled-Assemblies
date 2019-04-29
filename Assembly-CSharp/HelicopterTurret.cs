using ConVar;
using Oxide.Core;
using System;
using System.Collections.Generic;
using UnityEngine;

public class HelicopterTurret : MonoBehaviour
{
	public PatrolHelicopterAI _heliAI;

	public float fireRate = 0.125f;

	public float burstLength = 3f;

	public float timeBetweenBursts = 3f;

	public float maxTargetRange = 300f;

	public float loseTargetAfter = 5f;

	public Transform gun_yaw;

	public Transform gun_pitch;

	public Transform muzzleTransform;

	public bool left;

	public BaseCombatEntity _target;

	private float lastBurstTime = Single.NegativeInfinity;

	private float lastFireTime = Single.NegativeInfinity;

	private float lastSeenTargetTime = Single.NegativeInfinity;

	private bool targetVisible;

	public HelicopterTurret()
	{
	}

	public float AngleToTarget(BaseCombatEntity potentialtarget)
	{
		Vector3 positionForEntity = (this.GetPositionForEntity(potentialtarget) - this.muzzleTransform.position).normalized;
		return Vector3.Angle((this.left ? -this._heliAI.transform.right : this._heliAI.transform.right), positionForEntity);
	}

	public void ClearTarget()
	{
		this._target = null;
		this.targetVisible = false;
	}

	public void FireGun()
	{
		this._heliAI.FireGun(this._target.transform.position + new Vector3(0f, 0.25f, 0f), PatrolHelicopter.bulletAccuracy, this.left);
	}

	public Vector3 GetPositionForEntity(BaseCombatEntity potentialtarget)
	{
		return potentialtarget.transform.position;
	}

	public bool HasTarget()
	{
		return this._target != null;
	}

	public bool InFiringArc(BaseCombatEntity potentialtarget)
	{
		object obj = Interface.CallHook("CanBeTargeted", potentialtarget, this);
		if (obj as bool)
		{
			return (bool)obj;
		}
		return this.AngleToTarget(potentialtarget) < 80f;
	}

	public bool NeedsNewTarget()
	{
		if (!this.HasTarget())
		{
			return true;
		}
		if (this.targetVisible)
		{
			return false;
		}
		return this.TimeSinceTargetLastSeen() > this.loseTargetAfter;
	}

	public void SetTarget(BaseCombatEntity newTarget)
	{
		if (Interface.CallHook("OnHelicopterTarget", this, newTarget) != null)
		{
			return;
		}
		this._target = newTarget;
		this.UpdateTargetVisibility();
	}

	public bool TargetVisible()
	{
		this.UpdateTargetVisibility();
		return this.targetVisible;
	}

	public float TimeSinceTargetLastSeen()
	{
		return UnityEngine.Time.realtimeSinceStartup - this.lastSeenTargetTime;
	}

	public void TurretThink()
	{
		if (this.HasTarget() && this.TimeSinceTargetLastSeen() > this.loseTargetAfter * 2f)
		{
			this.ClearTarget();
		}
		if (!this.HasTarget())
		{
			return;
		}
		if (UnityEngine.Time.time - this.lastBurstTime > this.burstLength + this.timeBetweenBursts && this.TargetVisible())
		{
			this.lastBurstTime = UnityEngine.Time.time;
		}
		if (UnityEngine.Time.time < this.lastBurstTime + this.burstLength && UnityEngine.Time.time - this.lastFireTime >= this.fireRate && this.InFiringArc(this._target))
		{
			this.lastFireTime = UnityEngine.Time.time;
			this.FireGun();
		}
	}

	public bool UpdateTargetFromList(List<PatrolHelicopterAI.targetinfo> newTargetList)
	{
		int num = UnityEngine.Random.Range(0, newTargetList.Count);
		int count = newTargetList.Count;
		while (count >= 0)
		{
			count--;
			PatrolHelicopterAI.targetinfo item = newTargetList[num];
			if (item != null && item.ent != null && item.IsVisible() && this.InFiringArc(item.ply))
			{
				this.SetTarget(item.ply);
				return true;
			}
			num++;
			if (num < newTargetList.Count)
			{
				continue;
			}
			num = 0;
		}
		return false;
	}

	public void UpdateTargetVisibility()
	{
		RaycastHit raycastHit;
		if (!this.HasTarget())
		{
			return;
		}
		Vector3 vector3 = this._target.transform.position;
		BasePlayer basePlayer = this._target as BasePlayer;
		if (basePlayer)
		{
			vector3 = basePlayer.eyes.position;
		}
		bool flag = false;
		float single = Vector3.Distance(vector3, this.muzzleTransform.position);
		Vector3 vector31 = (vector3 - this.muzzleTransform.position).normalized;
		if (single < this.maxTargetRange && this.InFiringArc(this._target) && GamePhysics.Trace(new Ray(this.muzzleTransform.position + (vector31 * 6f), vector31), 0f, out raycastHit, single * 1.1f, 1218652417, QueryTriggerInteraction.UseGlobal) && raycastHit.collider.gameObject.ToBaseEntity() == this._target)
		{
			flag = true;
		}
		if (flag)
		{
			this.lastSeenTargetTime = UnityEngine.Time.realtimeSinceStartup;
		}
		this.targetVisible = flag;
	}
}