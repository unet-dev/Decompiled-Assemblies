using Network;
using System;
using UnityEngine;

public class ServerProjectile : EntityComponent<BaseEntity>, IServerComponent
{
	public Vector3 initialVelocity;

	public float drag;

	public float gravityModifier = 1f;

	public float speed = 15f;

	public float scanRange;

	public Vector3 swimScale;

	public Vector3 swimSpeed;

	public float radius;

	private bool impacted;

	private float swimRandom;

	public Vector3 _currentVelocity = Vector3.zero;

	public ServerProjectile()
	{
	}

	public void DoMovement()
	{
		RaycastHit raycastHit;
		if (this.impacted)
		{
			return;
		}
		this._currentVelocity = this._currentVelocity + (((Physics.gravity * this.gravityModifier) * Time.fixedDeltaTime) * Time.timeScale);
		Vector3 vector3 = this._currentVelocity;
		if (this.swimScale != Vector3.zero)
		{
			if (this.swimRandom == 0f)
			{
				this.swimRandom = UnityEngine.Random.Range(0f, 20f);
			}
			float single = Time.time + this.swimRandom;
			Vector3 vector31 = new Vector3(Mathf.Sin(single * this.swimSpeed.x) * this.swimScale.x, Mathf.Cos(single * this.swimSpeed.y) * this.swimScale.y, Mathf.Sin(single * this.swimSpeed.z) * this.swimScale.z);
			vector31 = base.transform.InverseTransformDirection(vector31);
			vector3 += vector31;
		}
		float single1 = vector3.magnitude * Time.fixedDeltaTime;
		if (GamePhysics.Trace(new Ray(base.transform.position, vector3.normalized), this.radius, out raycastHit, single1 + this.scanRange, 1236478737, QueryTriggerInteraction.UseGlobal))
		{
			BaseEntity entity = raycastHit.GetEntity();
			if (!entity.IsValid() || !base.baseEntity.creatorEntity.IsValid() || entity.net.ID != base.baseEntity.creatorEntity.net.ID)
			{
				Transform transforms = base.transform;
				transforms.position = transforms.position + (base.transform.forward * raycastHit.distance);
				base.SendMessage("ProjectileImpact", raycastHit, SendMessageOptions.DontRequireReceiver);
				this.impacted = true;
				return;
			}
		}
		Transform transforms1 = base.transform;
		transforms1.position = transforms1.position + (base.transform.forward * single1);
		base.transform.rotation = Quaternion.LookRotation(vector3.normalized);
	}

	private void FixedUpdate()
	{
		if (base.baseEntity.isServer)
		{
			this.DoMovement();
		}
	}

	public void InitializeVelocity(Vector3 overrideVel)
	{
		base.transform.rotation = Quaternion.LookRotation(overrideVel.normalized);
		this.initialVelocity = overrideVel;
		this._currentVelocity = overrideVel;
	}
}