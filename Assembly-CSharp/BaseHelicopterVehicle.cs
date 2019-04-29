using Rust;
using System;
using System.Collections.Generic;
using UnityEngine;

public class BaseHelicopterVehicle : BaseVehicle
{
	[Header("Helicopter")]
	public Rigidbody rigidBody;

	public float engineThrustMax;

	public Vector3 torqueScale;

	public Transform com;

	[Header("Effects")]
	public Transform[] GroundPoints;

	public Transform[] GroundEffects;

	public GameObjectRef explosionEffect;

	public GameObjectRef fireBall;

	public GameObjectRef impactEffectSmall;

	public GameObjectRef impactEffectLarge;

	private float avgTerrainHeight;

	public const BaseEntity.Flags Flag_InternalLights = BaseEntity.Flags.Reserved6;

	protected BaseHelicopterVehicle.HelicopterInputState currentInputState = new BaseHelicopterVehicle.HelicopterInputState();

	protected float lastPlayerInputTime;

	public float currentThrottle;

	public float avgThrust;

	public float liftDotMax = 0.75f;

	public float altForceDotMin = 0.85f;

	public float liftFraction = 0.25f;

	protected float hoverForceScale = 0.99f;

	public float thrustLerpSpeed = 1f;

	private float nextDamageTime;

	private float nextEffectTime;

	private float pendingImpactDamage;

	public BaseHelicopterVehicle()
	{
	}

	public virtual bool CollisionDamageEnabled()
	{
		return true;
	}

	public void DelayedImpactDamage()
	{
		this.Hurt(this.pendingImpactDamage * this.MaxHealth(), DamageType.Explosion, this, false);
		this.pendingImpactDamage = 0f;
	}

	public override Quaternion GetAngularVelocityServer()
	{
		return Quaternion.LookRotation(this.rigidBody.angularVelocity, base.transform.up);
	}

	public override Vector3 GetLocalVelocityServer()
	{
		return this.rigidBody.velocity;
	}

	public virtual float GetServiceCeiling()
	{
		return 1000f;
	}

	public virtual bool IsEnginePowered()
	{
		return true;
	}

	public override void LightToggle(BasePlayer player)
	{
		if (base.GetPlayerSeat(player) == 0)
		{
			base.SetFlag(BaseEntity.Flags.Reserved5, !base.HasFlag(BaseEntity.Flags.Reserved5), false, true);
		}
	}

	public override float MaxVelocity()
	{
		return 50f;
	}

	public float MouseToBinary(float amount)
	{
		return Mathf.Clamp(amount, -1f, 1f);
	}

	public virtual void MovementUpdate()
	{
		BaseHelicopterVehicle.HelicopterInputState helicopterInputState = this.currentInputState;
		this.currentThrottle = Mathf.Lerp(this.currentThrottle, helicopterInputState.throttle, 2f * Time.fixedDeltaTime);
		this.currentThrottle = Mathf.Clamp(this.currentThrottle, -0.8f, 1f);
		this.rigidBody.AddRelativeTorque(new Vector3(helicopterInputState.pitch * this.torqueScale.x, helicopterInputState.yaw * this.torqueScale.y, helicopterInputState.roll * this.torqueScale.z), ForceMode.Force);
		this.avgThrust = Mathf.Lerp(this.avgThrust, this.engineThrustMax * this.currentThrottle, Time.fixedDeltaTime * this.thrustLerpSpeed);
		float single = Mathf.Clamp01(Vector3.Dot(base.transform.up, Vector3.up));
		float single1 = Mathf.InverseLerp(this.liftDotMax, 1f, single);
		float serviceCeiling = this.GetServiceCeiling();
		this.avgTerrainHeight = Mathf.Lerp(this.avgTerrainHeight, TerrainMeta.HeightMap.GetHeight(base.transform.position), Time.deltaTime);
		float single2 = 1f - Mathf.InverseLerp(this.avgTerrainHeight + serviceCeiling - 20f, this.avgTerrainHeight + serviceCeiling, base.transform.position.y);
		single1 *= single2;
		float single3 = 1f - Mathf.InverseLerp(this.altForceDotMin, 1f, single);
		Vector3 vector3 = (((Vector3.up * this.engineThrustMax) * this.liftFraction) * this.currentThrottle) * single1;
		Vector3 vector31 = base.transform.up - Vector3.up;
		Vector3 vector32 = ((vector31.normalized * this.engineThrustMax) * this.currentThrottle) * single3;
		float single4 = this.rigidBody.mass * -Physics.gravity.y;
		this.rigidBody.AddForce(((base.transform.up * single4) * single1) * this.hoverForceScale, ForceMode.Force);
		this.rigidBody.AddForce(vector3, ForceMode.Force);
		this.rigidBody.AddForce(vector32, ForceMode.Force);
	}

	private void OnCollisionEnter(Collision collision)
	{
		this.ProcessCollision(collision);
	}

	public override void OnKilled(HitInfo info)
	{
		if (base.isClient)
		{
			base.OnKilled(info);
			return;
		}
		if (this.explosionEffect.isValid)
		{
			Effect.server.Run(this.explosionEffect.resourcePath, base.transform.position, Vector3.up, null, true);
		}
		Vector3 vector3 = this.rigidBody.velocity * 0.25f;
		List<ServerGib> serverGibs = null;
		if (this.serverGibs.isValid)
		{
			GameObject component = this.serverGibs.Get().GetComponent<ServerGib>()._gibSource;
			serverGibs = ServerGib.CreateGibs(this.serverGibs.resourcePath, base.gameObject, component, vector3, 3f);
		}
		if (this.fireBall.isValid)
		{
			for (int i = 0; i < 12; i++)
			{
				BaseEntity baseEntity = GameManager.server.CreateEntity(this.fireBall.resourcePath, base.transform.position, base.transform.rotation, true);
				if (baseEntity)
				{
					float single = 3f;
					float single1 = 10f;
					Vector3 vector31 = UnityEngine.Random.onUnitSphere;
					baseEntity.transform.position = (base.transform.position + new Vector3(0f, 1.5f, 0f)) + (vector31 * UnityEngine.Random.Range(-4f, 4f));
					Collider collider = baseEntity.GetComponent<Collider>();
					baseEntity.Spawn();
					baseEntity.SetVelocity(vector3 + (vector31 * UnityEngine.Random.Range(single, single1)));
					if (serverGibs != null)
					{
						foreach (ServerGib serverGib in serverGibs)
						{
							Physics.IgnoreCollision(collider, serverGib.GetCollider(), true);
						}
					}
				}
			}
		}
		base.OnKilled(info);
	}

	public virtual void PilotInput(InputState inputState, BasePlayer player)
	{
		this.currentInputState.Reset();
		this.currentInputState.throttle = (inputState.IsDown(BUTTON.FORWARD) ? 1f : 0f);
		BaseHelicopterVehicle.HelicopterInputState helicopterInputState = this.currentInputState;
		helicopterInputState.throttle = helicopterInputState.throttle - (inputState.IsDown(BUTTON.BACKWARD) || inputState.IsDown(BUTTON.DUCK) ? 1f : 0f);
		this.currentInputState.pitch = inputState.current.mouseDelta.y;
		this.currentInputState.roll = -inputState.current.mouseDelta.x;
		this.currentInputState.yaw = (inputState.IsDown(BUTTON.RIGHT) ? 1f : 0f);
		BaseHelicopterVehicle.HelicopterInputState helicopterInputState1 = this.currentInputState;
		helicopterInputState1.yaw = helicopterInputState1.yaw - (inputState.IsDown(BUTTON.LEFT) ? 1f : 0f);
		this.currentInputState.pitch = this.MouseToBinary(this.currentInputState.pitch);
		this.currentInputState.roll = this.MouseToBinary(this.currentInputState.roll);
		this.lastPlayerInputTime = Time.time;
	}

	public override void PlayerServerInput(InputState inputState, BasePlayer player)
	{
		if (base.GetPlayerSeat(player) == 0)
		{
			this.PilotInput(inputState, player);
		}
	}

	public void ProcessCollision(Collision collision)
	{
		if (base.isClient)
		{
			return;
		}
		if (!this.CollisionDamageEnabled())
		{
			return;
		}
		if (Time.time < this.nextDamageTime)
		{
			return;
		}
		float single = collision.relativeVelocity.magnitude;
		if (collision.gameObject && (1 << (collision.collider.gameObject.layer & 31) & 1218519297) <= 0)
		{
			return;
		}
		float single1 = Mathf.InverseLerp(5f, 30f, single);
		if (single1 > 0f)
		{
			this.pendingImpactDamage += Mathf.Max(single1, 0.15f);
			if (Vector3.Dot(base.transform.up, Vector3.up) < 0.5f)
			{
				this.pendingImpactDamage *= 5f;
			}
			if (Time.time > this.nextEffectTime)
			{
				this.nextEffectTime = Time.time + 0.25f;
				if (this.impactEffectSmall.isValid)
				{
					Vector3 contact = collision.GetContact(0).point;
					contact = contact + ((base.transform.position - contact) * 0.25f);
					Effect.server.Run(this.impactEffectSmall.resourcePath, contact, base.transform.up, null, false);
				}
			}
			Rigidbody rigidbody = this.rigidBody;
			ContactPoint contactPoint = collision.GetContact(0);
			Vector3 vector3 = contactPoint.normal * (1f + 3f * single1);
			contactPoint = collision.GetContact(0);
			rigidbody.AddForceAtPosition(vector3, contactPoint.point, ForceMode.VelocityChange);
			this.nextDamageTime = Time.time + 0.333f;
			base.Invoke(new Action(this.DelayedImpactDamage), 0.015f);
		}
	}

	public override void ServerInit()
	{
		base.ServerInit();
		this.rigidBody.centerOfMass = this.com.localPosition;
	}

	public virtual void SetDefaultInputState()
	{
		this.currentInputState.Reset();
		if (!this.IsMounted())
		{
			this.currentInputState.throttle = -1f;
		}
		else
		{
			float single = Vector3.Dot(Vector3.up, base.transform.right);
			float single1 = Vector3.Dot(Vector3.up, base.transform.forward);
			this.currentInputState.roll = (single < 0f ? 1f : 0f);
			BaseHelicopterVehicle.HelicopterInputState helicopterInputState = this.currentInputState;
			helicopterInputState.roll = helicopterInputState.roll - (single > 0f ? 1f : 0f);
			if (single1 < 0f)
			{
				this.currentInputState.pitch = -1f;
				return;
			}
			if (single1 > 0f)
			{
				this.currentInputState.pitch = 1f;
				return;
			}
		}
	}

	public override void VehicleFixedUpdate()
	{
		base.VehicleFixedUpdate();
		if (base.isClient)
		{
			return;
		}
		if (Time.time > this.lastPlayerInputTime + 0.5f)
		{
			this.SetDefaultInputState();
		}
		this.MovementUpdate();
		base.SetFlag(BaseEntity.Flags.Reserved6, TOD_Sky.Instance.IsNight, false, true);
	}

	public class HelicopterInputState
	{
		public float throttle;

		public float roll;

		public float yaw;

		public float pitch;

		public bool groundControl;

		public HelicopterInputState()
		{
		}

		public void Reset()
		{
			this.throttle = 0f;
			this.roll = 0f;
			this.yaw = 0f;
			this.pitch = 0f;
			this.groundControl = false;
		}
	}
}