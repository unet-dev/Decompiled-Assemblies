using System;
using UnityEngine;

[CreateAssetMenu(menuName="Rust/MaterialEffect")]
public class MaterialEffect : ScriptableObject
{
	public GameObjectRef DefaultEffect;

	public SoundDefinition DefaultSoundDefinition;

	public MaterialEffect.Entry[] Entries;

	public int waterFootstepIndex = -1;

	public MaterialEffect.Entry deepWaterEntry;

	public float deepWaterDepth = -1f;

	public MaterialEffect.Entry submergedWaterEntry;

	public float submergedWaterDepth = -1f;

	public bool ScaleVolumeWithSpeed;

	public AnimationCurve SpeedGainCurve;

	public MaterialEffect()
	{
	}

	public MaterialEffect.Entry GetEntryFromMaterial(PhysicMaterial mat)
	{
		MaterialEffect.Entry[] entries = this.Entries;
		for (int i = 0; i < (int)entries.Length; i++)
		{
			MaterialEffect.Entry entry = entries[i];
			if (entry.Material == mat)
			{
				return entry;
			}
		}
		return null;
	}

	public MaterialEffect.Entry GetWaterEntry()
	{
		if (this.waterFootstepIndex == -1)
		{
			int num = 0;
			while (num < (int)this.Entries.Length)
			{
				if (this.Entries[num].Material.name != "Water")
				{
					num++;
				}
				else
				{
					this.waterFootstepIndex = num;
					break;
				}
			}
		}
		if (this.waterFootstepIndex != -1)
		{
			return this.Entries[this.waterFootstepIndex];
		}
		Debug.LogWarning(string.Concat("Unable to find water effect for :", base.name));
		return null;
	}

	public void PlaySound(SoundDefinition definition, Vector3 position, float velocity = 0f)
	{
	}

	public void SpawnOnRay(Ray ray, int mask, float length = 0.5f, Vector3 forward = null, float speed = 0f)
	{
		RaycastHit raycastHit;
		if (!GamePhysics.Trace(ray, 0f, out raycastHit, length, mask, QueryTriggerInteraction.UseGlobal))
		{
			Effect.client.Run(this.DefaultEffect.resourcePath, ray.origin, ray.direction * -1f, forward);
			if (this.DefaultSoundDefinition != null)
			{
				this.PlaySound(this.DefaultSoundDefinition, raycastHit.point, speed);
			}
			return;
		}
		WaterLevel.WaterInfo waterInfo = WaterLevel.GetWaterInfo(ray.origin);
		if (waterInfo.isValid)
		{
			Vector3 vector3 = new Vector3(ray.origin.x, WaterSystem.GetHeight(ray.origin), ray.origin.z);
			MaterialEffect.Entry waterEntry = this.GetWaterEntry();
			if (this.submergedWaterDepth > 0f && waterInfo.currentDepth >= this.submergedWaterDepth)
			{
				waterEntry = this.submergedWaterEntry;
			}
			else if (this.deepWaterDepth > 0f && waterInfo.currentDepth >= this.deepWaterDepth)
			{
				waterEntry = this.deepWaterEntry;
			}
			if (waterEntry == null)
			{
				return;
			}
			Effect.client.Run(waterEntry.Effect.resourcePath, vector3, Vector3.up, new Vector3());
			if (waterEntry.SoundDefinition != null)
			{
				this.PlaySound(waterEntry.SoundDefinition, vector3, speed);
			}
			return;
		}
		PhysicMaterial materialAt = raycastHit.collider.GetMaterialAt(raycastHit.point);
		MaterialEffect.Entry entryFromMaterial = this.GetEntryFromMaterial(materialAt);
		if (entryFromMaterial != null)
		{
			Effect.client.Run(entryFromMaterial.Effect.resourcePath, raycastHit.point, raycastHit.normal, forward);
			if (entryFromMaterial.SoundDefinition != null)
			{
				this.PlaySound(entryFromMaterial.SoundDefinition, raycastHit.point, speed);
			}
		}
		else
		{
			Effect.client.Run(this.DefaultEffect.resourcePath, raycastHit.point, raycastHit.normal, forward);
			if (this.DefaultSoundDefinition != null)
			{
				this.PlaySound(this.DefaultSoundDefinition, raycastHit.point, speed);
				return;
			}
		}
	}

	[Serializable]
	public class Entry
	{
		public PhysicMaterial Material;

		public GameObjectRef Effect;

		public SoundDefinition SoundDefinition;

		public Entry()
		{
		}
	}
}