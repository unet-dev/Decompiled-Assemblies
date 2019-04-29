using Facepunch;
using Network;
using ProtoBuf;
using Rust;
using System;
using System.Collections.Generic;
using UnityEngine;

public class BaseHelicopter : BaseCombatEntity
{
	public GameObject rotorPivot;

	public GameObject mainRotor;

	public GameObject mainRotor_blades;

	public GameObject mainRotor_blur;

	public GameObject tailRotor;

	public GameObject tailRotor_blades;

	public GameObject tailRotor_blur;

	public GameObject rocket_tube_left;

	public GameObject rocket_tube_right;

	public GameObject left_gun_yaw;

	public GameObject left_gun_pitch;

	public GameObject left_gun_muzzle;

	public GameObject right_gun_yaw;

	public GameObject right_gun_pitch;

	public GameObject right_gun_muzzle;

	public GameObject spotlight_rotation;

	public GameObjectRef rocket_fire_effect;

	public GameObjectRef gun_fire_effect;

	public GameObjectRef bulletEffect;

	public GameObjectRef explosionEffect;

	public GameObjectRef fireBall;

	public GameObjectRef crateToDrop;

	public int maxCratesToSpawn = 4;

	public float bulletSpeed = 250f;

	public float bulletDamage = 20f;

	public GameObjectRef servergibs;

	public GameObjectRef debrisFieldMarker;

	public SoundDefinition rotorWashSoundDef;

	public SoundDefinition engineSoundDef;

	public SoundDefinition rotorSoundDef;

	private Sound _engineSound;

	private Sound _rotorSound;

	private Sound _rotorWashSound;

	public float spotlightJitterAmount = 5f;

	public float spotlightJitterSpeed = 5f;

	public GameObject[] nightLights;

	public Vector3 spotlightTarget;

	public float engineSpeed = 1f;

	public float targetEngineSpeed = 1f;

	public float blur_rotationScale = 0.05f;

	public ParticleSystem[] _rotorWashParticles;

	private PatrolHelicopterAI myAI;

	private float lastNetworkUpdate = Single.NegativeInfinity;

	private const float networkUpdateRate = 0.25f;

	public BaseHelicopter.weakspot[] weakspots;

	public BaseHelicopter()
	{
	}

	public void CreateExplosionMarker(float durationMinutes)
	{
		BaseEntity baseEntity = GameManager.server.CreateEntity(this.debrisFieldMarker.resourcePath, base.transform.position, Quaternion.identity, true);
		baseEntity.Spawn();
		baseEntity.SendMessage("SetDuration", durationMinutes, SendMessageOptions.DontRequireReceiver);
	}

	public override void Hurt(HitInfo info)
	{
		bool flag = false;
		if (info.damageTypes.Total() >= base.health)
		{
			base.health = 1000000f;
			this.myAI.CriticalDamage();
			flag = true;
		}
		base.Hurt(info);
		if (!flag)
		{
			BaseHelicopter.weakspot[] weakspotArray = this.weakspots;
			for (int i = 0; i < (int)weakspotArray.Length; i++)
			{
				BaseHelicopter.weakspot _weakspot = weakspotArray[i];
				string[] strArrays = _weakspot.bonenames;
				for (int j = 0; j < (int)strArrays.Length; j++)
				{
					string str = strArrays[j];
					if (info.HitBone == StringPool.Get(str))
					{
						_weakspot.Hurt(info.damageTypes.Total(), info);
						this.myAI.WeakspotDamaged(_weakspot, info);
					}
				}
			}
		}
	}

	public void InitalizeWeakspots()
	{
		BaseHelicopter.weakspot[] weakspotArray = this.weakspots;
		for (int i = 0; i < (int)weakspotArray.Length; i++)
		{
			weakspotArray[i].body = this;
		}
	}

	public override void InitShared()
	{
		base.InitShared();
		this.InitalizeWeakspots();
	}

	public override void Load(BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (info.msg.helicopter != null)
		{
			this.spotlightTarget = info.msg.helicopter.spotlightVec;
		}
	}

	public override float MaxVelocity()
	{
		return 100f;
	}

	public override void OnAttacked(HitInfo info)
	{
		base.OnAttacked(info);
		if (base.isServer)
		{
			this.myAI.WasAttacked(info);
		}
	}

	public override void OnKilled(HitInfo info)
	{
		if (base.isClient)
		{
			return;
		}
		this.CreateExplosionMarker(10f);
		Effect.server.Run(this.explosionEffect.resourcePath, base.transform.position, Vector3.up, null, true);
		Vector3 lastMoveDir = (this.myAI.GetLastMoveDir() * this.myAI.GetMoveSpeed()) * 0.75f;
		GameObject component = this.servergibs.Get().GetComponent<ServerGib>()._gibSource;
		List<ServerGib> serverGibs = ServerGib.CreateGibs(this.servergibs.resourcePath, base.gameObject, component, lastMoveDir, 3f);
		for (int i = 0; i < 12 - this.maxCratesToSpawn; i++)
		{
			BaseEntity vector3 = GameManager.server.CreateEntity(this.fireBall.resourcePath, base.transform.position, base.transform.rotation, true);
			if (vector3)
			{
				float single = 3f;
				float single1 = 10f;
				Vector3 vector31 = UnityEngine.Random.onUnitSphere;
				vector3.transform.position = (base.transform.position + new Vector3(0f, 1.5f, 0f)) + (vector31 * UnityEngine.Random.Range(-4f, 4f));
				Collider collider = vector3.GetComponent<Collider>();
				vector3.Spawn();
				vector3.SetVelocity(lastMoveDir + (vector31 * UnityEngine.Random.Range(single, single1)));
				foreach (ServerGib serverGib in serverGibs)
				{
					Physics.IgnoreCollision(collider, serverGib.GetCollider(), true);
				}
			}
		}
		for (int j = 0; j < this.maxCratesToSpawn; j++)
		{
			Vector3 vector32 = UnityEngine.Random.onUnitSphere;
			Vector3 vector33 = (base.transform.position + new Vector3(0f, 1.5f, 0f)) + (vector32 * UnityEngine.Random.Range(2f, 3f));
			BaseEntity baseEntity = GameManager.server.CreateEntity(this.crateToDrop.resourcePath, vector33, Quaternion.LookRotation(vector32), true);
			baseEntity.Spawn();
			LootContainer lootContainer = baseEntity as LootContainer;
			if (lootContainer)
			{
				lootContainer.Invoke(new Action(lootContainer.RemoveMe), 1800f);
			}
			Collider component1 = baseEntity.GetComponent<Collider>();
			Rigidbody rigidbody = baseEntity.gameObject.AddComponent<Rigidbody>();
			rigidbody.useGravity = true;
			rigidbody.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
			rigidbody.mass = 2f;
			rigidbody.interpolation = RigidbodyInterpolation.Interpolate;
			rigidbody.velocity = lastMoveDir + (vector32 * UnityEngine.Random.Range(1f, 3f));
			rigidbody.angularVelocity = Vector3Ex.Range(-1.75f, 1.75f);
			rigidbody.drag = 0.5f * (rigidbody.mass / 5f);
			rigidbody.angularDrag = 0.2f * (rigidbody.mass / 5f);
			GameManager gameManager = GameManager.server;
			string str = this.fireBall.resourcePath;
			Vector3 vector34 = new Vector3();
			Quaternion quaternion = new Quaternion();
			FireBall fireBall = gameManager.CreateEntity(str, vector34, quaternion, true) as FireBall;
			if (fireBall)
			{
				fireBall.SetParent(baseEntity, false, false);
				fireBall.Spawn();
				fireBall.GetComponent<Rigidbody>().isKinematic = true;
				fireBall.GetComponent<Collider>().enabled = false;
			}
			baseEntity.SendMessage("SetLockingEnt", fireBall.gameObject, SendMessageOptions.DontRequireReceiver);
			foreach (ServerGib serverGib1 in serverGibs)
			{
				Physics.IgnoreCollision(component1, serverGib1.GetCollider(), true);
			}
		}
		base.OnKilled(info);
	}

	public override void OnPositionalNetworkUpdate()
	{
		base.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
		base.OnPositionalNetworkUpdate();
	}

	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning timeWarning = TimeWarning.New("BaseHelicopter.OnRpcMessage", 0.1f))
		{
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	public override void Save(BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		info.msg.helicopter = Pool.Get<Helicopter>();
		info.msg.helicopter.tiltRot = this.rotorPivot.transform.localRotation.eulerAngles;
		info.msg.helicopter.spotlightVec = this.spotlightTarget;
		info.msg.helicopter.weakspothealths = Pool.Get<List<float>>();
		for (int i = 0; i < (int)this.weakspots.Length; i++)
		{
			info.msg.helicopter.weakspothealths.Add(this.weakspots[i].health);
		}
	}

	public override void ServerInit()
	{
		base.ServerInit();
		this.myAI = base.GetComponent<PatrolHelicopterAI>();
		if (!this.myAI.hasInterestZone)
		{
			this.myAI.SetInitialDestination(Vector3.zero, 1.25f);
			this.myAI.targetThrottleSpeed = 1f;
			this.myAI.ExitCurrentState();
			this.myAI.State_Patrol_Enter();
		}
	}

	public void Update()
	{
		if (base.isServer && Time.realtimeSinceStartup - this.lastNetworkUpdate >= 0.25f)
		{
			base.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
			this.lastNetworkUpdate = Time.realtimeSinceStartup;
		}
	}

	[Serializable]
	public class weakspot
	{
		[NonSerialized]
		public BaseHelicopter body;

		public string[] bonenames;

		public float maxHealth;

		public float health;

		public float healthFractionOnDestroyed;

		public GameObjectRef destroyedParticles;

		public GameObjectRef damagedParticles;

		public GameObject damagedEffect;

		public GameObject destroyedEffect;

		public List<BasePlayer> attackers;

		private bool isDestroyed;

		public weakspot()
		{
		}

		public void Heal(float amount)
		{
			this.health += amount;
		}

		public float HealthFraction()
		{
			return this.health / this.maxHealth;
		}

		public void Hurt(float amount, HitInfo info)
		{
			if (this.isDestroyed)
			{
				return;
			}
			this.health -= amount;
			Effect.server.Run(this.damagedParticles.resourcePath, this.body, StringPool.Get(this.bonenames[UnityEngine.Random.Range(0, (int)this.bonenames.Length)]), Vector3.zero, Vector3.up, null, true);
			if (this.health <= 0f)
			{
				this.health = 0f;
				this.WeakspotDestroyed();
			}
		}

		public void WeakspotDestroyed()
		{
			this.isDestroyed = true;
			Effect.server.Run(this.destroyedParticles.resourcePath, this.body, StringPool.Get(this.bonenames[UnityEngine.Random.Range(0, (int)this.bonenames.Length)]), Vector3.zero, Vector3.up, null, true);
			this.body.Hurt(this.body.MaxHealth() * this.healthFractionOnDestroyed, DamageType.Generic, null, false);
		}
	}
}