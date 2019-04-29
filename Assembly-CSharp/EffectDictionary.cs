using Rust;
using System;
using System.Collections.Generic;
using UnityEngine;

public class EffectDictionary
{
	private static Dictionary<string, string[]> effectDictionary;

	static EffectDictionary()
	{
	}

	public EffectDictionary()
	{
	}

	public static string GetDecal(string impactType, string materialName)
	{
		return EffectDictionary.LookupEffect("decals", impactType, materialName);
	}

	public static string GetDecal(DamageType damageType, string materialName)
	{
		switch (damageType)
		{
			case DamageType.Bullet:
			{
				return EffectDictionary.GetDecal("bullet", materialName);
			}
			case DamageType.Slash:
			{
				return EffectDictionary.GetDecal("slash", materialName);
			}
			case DamageType.Blunt:
			{
				return EffectDictionary.GetDecal("blunt", materialName);
			}
			case DamageType.Fall:
			case DamageType.Radiation:
			case DamageType.Bite:
			{
				return EffectDictionary.GetDecal("blunt", materialName);
			}
			case DamageType.Stab:
			{
				return EffectDictionary.GetDecal("stab", materialName);
			}
			default:
			{
				if (damageType == DamageType.Arrow)
				{
					return EffectDictionary.GetDecal("bullet", materialName);
				}
				return EffectDictionary.GetDecal("blunt", materialName);
			}
		}
	}

	public static string GetDisplacement(string impactType, string materialName)
	{
		return EffectDictionary.LookupEffect("displacement", impactType, materialName);
	}

	public static string GetParticle(string impactType, string materialName)
	{
		return EffectDictionary.LookupEffect("impacts", impactType, materialName);
	}

	public static string GetParticle(DamageType damageType, string materialName)
	{
		switch (damageType)
		{
			case DamageType.Bullet:
			{
				return EffectDictionary.GetParticle("bullet", materialName);
			}
			case DamageType.Slash:
			{
				return EffectDictionary.GetParticle("slash", materialName);
			}
			case DamageType.Blunt:
			{
				return EffectDictionary.GetParticle("blunt", materialName);
			}
			case DamageType.Fall:
			case DamageType.Radiation:
			case DamageType.Bite:
			{
				return EffectDictionary.GetParticle("blunt", materialName);
			}
			case DamageType.Stab:
			{
				return EffectDictionary.GetParticle("stab", materialName);
			}
			default:
			{
				if (damageType == DamageType.Arrow)
				{
					return EffectDictionary.GetParticle("bullet", materialName);
				}
				return EffectDictionary.GetParticle("blunt", materialName);
			}
		}
	}

	private static string LookupEffect(string category, string effect, string material)
	{
		string[] strArrays;
		if (EffectDictionary.effectDictionary == null)
		{
			EffectDictionary.effectDictionary = GameManifest.LoadEffectDictionary();
		}
		string str = "assets/bundled/prefabs/fx/{0}/{1}/{2}";
		if (!EffectDictionary.effectDictionary.TryGetValue(StringFormatCache.Get(str, category, effect, material), out strArrays) && !EffectDictionary.effectDictionary.TryGetValue(StringFormatCache.Get(str, category, effect, "generic"), out strArrays))
		{
			return string.Empty;
		}
		return strArrays[UnityEngine.Random.Range(0, (int)strArrays.Length)];
	}
}