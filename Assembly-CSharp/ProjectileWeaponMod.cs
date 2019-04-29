using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;

public class ProjectileWeaponMod : BaseEntity
{
	[Header("Silencer")]
	public GameObjectRef defaultSilencerEffect;

	public bool isSilencer;

	[Header("Weapon Basics")]
	public ProjectileWeaponMod.Modifier repeatDelay;

	public ProjectileWeaponMod.Modifier projectileVelocity;

	public ProjectileWeaponMod.Modifier projectileDamage;

	public ProjectileWeaponMod.Modifier projectileDistance;

	[Header("Recoil")]
	public ProjectileWeaponMod.Modifier aimsway;

	public ProjectileWeaponMod.Modifier aimswaySpeed;

	public ProjectileWeaponMod.Modifier recoil;

	[Header("Aim Cone")]
	public ProjectileWeaponMod.Modifier sightAimCone;

	public ProjectileWeaponMod.Modifier hipAimCone;

	[Header("Light Effects")]
	public bool isLight;

	[Header("MuzzleBrake")]
	public bool isMuzzleBrake;

	[Header("MuzzleBoost")]
	public bool isMuzzleBoost;

	[Header("Scope")]
	public bool isScope;

	public float zoomAmountDisplayOnly;

	public bool needsOnForEffects;

	public ProjectileWeaponMod()
	{
	}

	public static float Average(BaseEntity parentEnt, Func<ProjectileWeaponMod, ProjectileWeaponMod.Modifier> selector_modifier, Func<ProjectileWeaponMod.Modifier, float> selector_value, float def)
	{
		if (parentEnt.children == null)
		{
			return def;
		}
		IEnumerable<float> mods = ProjectileWeaponMod.GetMods(parentEnt, selector_modifier, selector_value);
		if (mods.Count<float>() == 0)
		{
			return def;
		}
		return mods.Average();
	}

	public static IEnumerable<float> GetMods(BaseEntity parentEnt, Func<ProjectileWeaponMod, ProjectileWeaponMod.Modifier> selector_modifier, Func<ProjectileWeaponMod.Modifier, float> selector_value)
	{
		return parentEnt.children.Cast<ProjectileWeaponMod>().Where<ProjectileWeaponMod>((ProjectileWeaponMod x) => {
			if (x == null)
			{
				return false;
			}
			if (!x.needsOnForEffects)
			{
				return true;
			}
			return x.HasFlag(BaseEntity.Flags.On);
		}).Select<ProjectileWeaponMod, ProjectileWeaponMod.Modifier>(selector_modifier).Where<ProjectileWeaponMod.Modifier>((ProjectileWeaponMod.Modifier x) => x.enabled).Select<ProjectileWeaponMod.Modifier, float>(selector_value);
	}

	public static bool HasBrokenWeaponMod(BaseEntity parentEnt)
	{
		if (parentEnt.children == null)
		{
			return false;
		}
		if (parentEnt.children.Cast<ProjectileWeaponMod>().Any<ProjectileWeaponMod>((ProjectileWeaponMod x) => {
			if (x == null)
			{
				return false;
			}
			return x.IsBroken();
		}))
		{
			return true;
		}
		return false;
	}

	public static float Max(BaseEntity parentEnt, Func<ProjectileWeaponMod, ProjectileWeaponMod.Modifier> selector_modifier, Func<ProjectileWeaponMod.Modifier, float> selector_value, float def)
	{
		if (parentEnt.children == null)
		{
			return def;
		}
		IEnumerable<float> mods = ProjectileWeaponMod.GetMods(parentEnt, selector_modifier, selector_value);
		if (mods.Count<float>() == 0)
		{
			return def;
		}
		return mods.Max();
	}

	public static float Min(BaseEntity parentEnt, Func<ProjectileWeaponMod, ProjectileWeaponMod.Modifier> selector_modifier, Func<ProjectileWeaponMod.Modifier, float> selector_value, float def)
	{
		if (parentEnt.children == null)
		{
			return def;
		}
		IEnumerable<float> mods = ProjectileWeaponMod.GetMods(parentEnt, selector_modifier, selector_value);
		if (mods.Count<float>() == 0)
		{
			return def;
		}
		return mods.Min();
	}

	public override void PostServerLoad()
	{
		base.limitNetworking = base.HasFlag(BaseEntity.Flags.Disabled);
	}

	public override void ServerInit()
	{
		base.SetFlag(BaseEntity.Flags.Disabled, true, false, true);
		base.ServerInit();
	}

	public static float Sum(BaseEntity parentEnt, Func<ProjectileWeaponMod, ProjectileWeaponMod.Modifier> selector_modifier, Func<ProjectileWeaponMod.Modifier, float> selector_value, float def)
	{
		if (parentEnt.children == null)
		{
			return def;
		}
		IEnumerable<float> mods = ProjectileWeaponMod.GetMods(parentEnt, selector_modifier, selector_value);
		if (mods.Count<float>() == 0)
		{
			return def;
		}
		return mods.Sum();
	}

	[Serializable]
	public struct Modifier
	{
		public bool enabled;

		[Tooltip("1 means no change. 0.5 is half.")]
		public float scalar;

		[Tooltip("Added after the scalar is applied.")]
		public float offset;
	}
}