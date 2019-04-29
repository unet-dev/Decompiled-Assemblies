using Network;
using Rust;
using System;
using UnityEngine;

public class Effect : EffectData
{
	public Vector3 Up;

	public Vector3 worldPos;

	public Vector3 worldNrm;

	public bool attached;

	public Transform transform;

	public GameObject gameObject;

	public string pooledString;

	public bool broadcast;

	private static Effect reusableInstace;

	static Effect()
	{
		Effect.reusableInstace = new Effect();
	}

	public Effect()
	{
	}

	public Effect(string effectName, Vector3 posWorld, Vector3 normWorld, Connection sourceConnection = null)
	{
		this.Init(Effect.Type.Generic, posWorld, normWorld, sourceConnection);
		this.pooledString = effectName;
	}

	public Effect(string effectName, BaseEntity ent, uint boneID, Vector3 posLocal, Vector3 normLocal, Connection sourceConnection = null)
	{
		this.Init(Effect.Type.Generic, ent, boneID, posLocal, normLocal, sourceConnection);
		this.pooledString = effectName;
	}

	public void Clear()
	{
		this.worldPos = Vector3.zero;
		this.worldNrm = Vector3.zero;
		this.attached = false;
		this.transform = null;
		this.gameObject = null;
		this.pooledString = null;
		this.broadcast = false;
	}

	public void Init(Effect.Type fxtype, BaseEntity ent, uint boneID, Vector3 posLocal, Vector3 normLocal, Connection sourceConnection = null)
	{
		uint d;
		this.Clear();
		this.type = (uint)fxtype;
		this.attached = true;
		this.origin = posLocal;
		this.normal = normLocal;
		this.gameObject = null;
		this.Up = Vector3.zero;
		if (ent != null && !ent.IsValid())
		{
			Debug.LogWarning("Effect.Init - invalid entity");
		}
		if (ent.IsValid())
		{
			d = ent.net.ID;
		}
		else
		{
			d = 0;
		}
		this.entity = d;
		this.source = (sourceConnection != null ? sourceConnection.userid : (ulong)((long)0));
		this.bone = boneID;
	}

	public void Init(Effect.Type fxtype, Vector3 posWorld, Vector3 normWorld, Connection sourceConnection = null)
	{
		this.Clear();
		this.type = (uint)fxtype;
		this.attached = false;
		this.worldPos = posWorld;
		this.worldNrm = normWorld;
		this.gameObject = null;
		this.Up = Vector3.zero;
		this.entity = 0;
		this.origin = this.worldPos;
		this.normal = this.worldNrm;
		this.bone = 0;
		this.source = (sourceConnection != null ? sourceConnection.userid : (ulong)((long)0));
	}

	public static class client
	{
		public static void DoAdditiveImpactEffect(HitInfo info, string effectName)
		{
			if (!info.HitEntity.IsValid())
			{
				Effect.client.Run(effectName, info.HitPositionWorld + (info.HitNormalWorld * 0.1f), info.HitNormalWorld, new Vector3());
				return;
			}
			Effect.client.Run(effectName, info.HitEntity, info.HitBone, info.HitPositionLocal + (info.HitNormalLocal * 0.1f), info.HitNormalLocal);
		}

		public static void ImpactEffect(HitInfo info)
		{
			Vector3 vector3;
			string str = StringPool.Get(info.HitMaterial);
			string particle = EffectDictionary.GetParticle(info.damageTypes.GetMajorityDamageType(), str);
			string decal = EffectDictionary.GetDecal(info.damageTypes.GetMajorityDamageType(), str);
			if (TerrainMeta.WaterMap != null && info.HitMaterial != Projectile.WaterMaterialID() && info.HitMaterial != Projectile.FleshMaterialID() && info.HitPositionWorld.y < TerrainMeta.WaterMap.GetHeight(info.HitPositionWorld))
			{
				return;
			}
			if (!info.HitEntity.IsValid())
			{
				Vector3 hitPositionWorld = info.HitPositionWorld;
				Vector3 hitNormalWorld = info.HitNormalWorld;
				vector3 = new Vector3();
				Effect.client.Run(particle, hitPositionWorld, hitNormalWorld, vector3);
				Vector3 hitPositionWorld1 = info.HitPositionWorld;
				Vector3 hitNormalWorld1 = info.HitNormalWorld;
				vector3 = new Vector3();
				Effect.client.Run(decal, hitPositionWorld1, hitNormalWorld1, vector3);
			}
			else
			{
				GameObjectRef impactEffect = info.HitEntity.GetImpactEffect(info);
				if (impactEffect.isValid)
				{
					particle = impactEffect.resourcePath;
				}
				Effect.client.Run(particle, info.HitEntity, info.HitBone, info.HitPositionLocal, info.HitNormalLocal);
				if (info.DoDecals)
				{
					Effect.client.Run(decal, info.HitEntity, info.HitBone, info.HitPositionLocal, info.HitNormalLocal);
				}
			}
			if (info.WeaponPrefab)
			{
				BaseMelee weaponPrefab = info.WeaponPrefab as BaseMelee;
				if (weaponPrefab != null)
				{
					string strikeEffectPath = weaponPrefab.GetStrikeEffectPath(str);
					if (!info.HitEntity.IsValid())
					{
						Vector3 vector31 = info.HitPositionWorld;
						Vector3 hitNormalWorld2 = info.HitNormalWorld;
						vector3 = new Vector3();
						Effect.client.Run(strikeEffectPath, vector31, hitNormalWorld2, vector3);
					}
					else
					{
						Effect.client.Run(strikeEffectPath, info.HitEntity, info.HitBone, info.HitPositionLocal, info.HitNormalLocal);
					}
				}
			}
			if (info.damageTypes.Has(DamageType.Explosion))
			{
				Effect.client.DoAdditiveImpactEffect(info, "assets/bundled/prefabs/fx/impacts/additive/explosion.prefab");
			}
			if (info.damageTypes.Has(DamageType.Heat))
			{
				Effect.client.DoAdditiveImpactEffect(info, "assets/bundled/prefabs/fx/impacts/additive/fire.prefab");
			}
		}

		public static void Run(Effect.Type fxtype, BaseEntity ent, uint boneID, Vector3 posLocal, Vector3 normLocal)
		{
		}

		public static void Run(string strName, BaseEntity ent, uint boneID, Vector3 posLocal, Vector3 normLocal)
		{
			string.IsNullOrEmpty(strName);
		}

		public static void Run(Effect.Type fxtype, Vector3 posWorld, Vector3 normWorld, Vector3 up = null)
		{
		}

		public static void Run(string strName, Vector3 posWorld = null, Vector3 normWorld = null, Vector3 up = null)
		{
			string.IsNullOrEmpty(strName);
		}

		public static void Run(string strName, GameObject obj)
		{
			string.IsNullOrEmpty(strName);
		}
	}

	public static class server
	{
		public static void DoAdditiveImpactEffect(HitInfo info, string effectName)
		{
			if (!info.HitEntity.IsValid())
			{
				Effect.server.Run(effectName, info.HitPositionWorld, info.HitNormalWorld, info.Predicted, false);
				return;
			}
			Effect.server.Run(effectName, info.HitEntity, info.HitBone, info.HitPositionLocal, info.HitNormalLocal, info.Predicted, false);
		}

		public static void ImpactEffect(HitInfo info)
		{
			if (!info.DoHitEffects)
			{
				return;
			}
			string str = StringPool.Get(info.HitMaterial);
			if (TerrainMeta.WaterMap != null && info.HitMaterial != Projectile.WaterMaterialID() && info.HitMaterial != Projectile.FleshMaterialID() && info.HitPositionWorld.y < WaterLevel.GetWaterDepth(info.HitPositionWorld))
			{
				return;
			}
			string particle = EffectDictionary.GetParticle(info.damageTypes.GetMajorityDamageType(), str);
			string decal = EffectDictionary.GetDecal(info.damageTypes.GetMajorityDamageType(), str);
			if (!info.HitEntity.IsValid())
			{
				Effect.server.Run(particle, info.HitPositionWorld, info.HitNormalWorld, info.Predicted, false);
				Effect.server.Run(decal, info.HitPositionWorld, info.HitNormalWorld, info.Predicted, false);
			}
			else
			{
				GameObjectRef impactEffect = info.HitEntity.GetImpactEffect(info);
				if (impactEffect.isValid)
				{
					particle = impactEffect.resourcePath;
				}
				Effect.server.Run(particle, info.HitEntity, info.HitBone, info.HitPositionLocal, info.HitNormalLocal, info.Predicted, false);
				Effect.server.Run(decal, info.HitEntity, info.HitBone, info.HitPositionLocal, info.HitNormalLocal, info.Predicted, false);
			}
			if (info.WeaponPrefab)
			{
				BaseMelee weaponPrefab = info.WeaponPrefab as BaseMelee;
				if (weaponPrefab != null)
				{
					string strikeEffectPath = weaponPrefab.GetStrikeEffectPath(str);
					if (!info.HitEntity.IsValid())
					{
						Effect.server.Run(strikeEffectPath, info.HitPositionWorld, info.HitNormalWorld, info.Predicted, false);
					}
					else
					{
						Effect.server.Run(strikeEffectPath, info.HitEntity, info.HitBone, info.HitPositionLocal, info.HitNormalLocal, info.Predicted, false);
					}
				}
			}
			if (info.damageTypes.Has(DamageType.Explosion))
			{
				Effect.server.DoAdditiveImpactEffect(info, "assets/bundled/prefabs/fx/impacts/additive/explosion.prefab");
			}
			if (info.damageTypes.Has(DamageType.Heat))
			{
				Effect.server.DoAdditiveImpactEffect(info, "assets/bundled/prefabs/fx/impacts/additive/fire.prefab");
			}
		}

		public static void Run(Effect.Type fxtype, BaseEntity ent, uint boneID, Vector3 posLocal, Vector3 normLocal, Connection sourceConnection = null, bool broadcast = false)
		{
			Effect.reusableInstace.Init(fxtype, ent, boneID, posLocal, normLocal, sourceConnection);
			Effect.reusableInstace.broadcast = broadcast;
			EffectNetwork.Send(Effect.reusableInstace);
		}

		public static void Run(string strName, BaseEntity ent, uint boneID, Vector3 posLocal, Vector3 normLocal, Connection sourceConnection = null, bool broadcast = false)
		{
			if (string.IsNullOrEmpty(strName))
			{
				return;
			}
			Effect.reusableInstace.Init(Effect.Type.Generic, ent, boneID, posLocal, normLocal, sourceConnection);
			Effect.reusableInstace.pooledString = strName;
			Effect.reusableInstace.broadcast = broadcast;
			EffectNetwork.Send(Effect.reusableInstace);
		}

		public static void Run(Effect.Type fxtype, Vector3 posWorld, Vector3 normWorld, Connection sourceConnection = null, bool broadcast = false)
		{
			Effect.reusableInstace.Init(fxtype, posWorld, normWorld, sourceConnection);
			Effect.reusableInstace.broadcast = broadcast;
			EffectNetwork.Send(Effect.reusableInstace);
		}

		public static void Run(string strName, Vector3 posWorld = null, Vector3 normWorld = null, Connection sourceConnection = null, bool broadcast = false)
		{
			if (string.IsNullOrEmpty(strName))
			{
				return;
			}
			Effect.reusableInstace.Init(Effect.Type.Generic, posWorld, normWorld, sourceConnection);
			Effect.reusableInstace.pooledString = strName;
			Effect.reusableInstace.broadcast = broadcast;
			EffectNetwork.Send(Effect.reusableInstace);
		}
	}

	public enum Type : uint
	{
		Generic,
		Projectile
	}
}