using Rust;
using System;
using UnityEngine;
using UnityEngine.AI;

public class Barricade : DecayEntity
{
	public float reflectDamage = 5f;

	public GameObjectRef reflectEffect;

	public bool canNpcSmash = true;

	public NavMeshModifierVolume NavMeshVolumeAnimals;

	public NavMeshModifierVolume NavMeshVolumeHumanoids;

	public NPCBarricadeTriggerBox NpcTriggerBox;

	private static int nonWalkableArea;

	private static int animalAgentTypeId;

	private static int humanoidAgentTypeId;

	static Barricade()
	{
		Barricade.nonWalkableArea = -1;
		Barricade.animalAgentTypeId = -1;
		Barricade.humanoidAgentTypeId = -1;
	}

	public Barricade()
	{
	}

	public override void OnAttacked(HitInfo info)
	{
		if (base.isServer && info.WeaponPrefab is BaseMelee && !info.IsProjectile())
		{
			BasePlayer initiator = info.Initiator as BasePlayer;
			if (initiator && this.reflectDamage > 0f)
			{
				initiator.Hurt(this.reflectDamage * UnityEngine.Random.Range(0.75f, 1.25f), DamageType.Stab, this, true);
				if (this.reflectEffect.isValid)
				{
					Effect.server.Run(this.reflectEffect.resourcePath, initiator, StringPool.closest, base.transform.position, Vector3.up, null, false);
				}
			}
		}
		base.OnAttacked(info);
	}

	public override void ServerInit()
	{
		NavMeshBuildSettings settingsByIndex;
		base.ServerInit();
		if (Barricade.nonWalkableArea < 0)
		{
			Barricade.nonWalkableArea = NavMesh.GetAreaFromName("Not Walkable");
		}
		if (Barricade.animalAgentTypeId < 0)
		{
			settingsByIndex = NavMesh.GetSettingsByIndex(1);
			Barricade.animalAgentTypeId = settingsByIndex.agentTypeID;
		}
		if (this.NavMeshVolumeAnimals == null)
		{
			this.NavMeshVolumeAnimals = base.gameObject.AddComponent<NavMeshModifierVolume>();
			this.NavMeshVolumeAnimals.area = Barricade.nonWalkableArea;
			this.NavMeshVolumeAnimals.AddAgentType(Barricade.animalAgentTypeId);
			this.NavMeshVolumeAnimals.center = Vector3.zero;
			this.NavMeshVolumeAnimals.size = Vector3.one;
		}
		if (!this.canNpcSmash)
		{
			if (Barricade.humanoidAgentTypeId < 0)
			{
				settingsByIndex = NavMesh.GetSettingsByIndex(0);
				Barricade.humanoidAgentTypeId = settingsByIndex.agentTypeID;
			}
			if (this.NavMeshVolumeHumanoids == null)
			{
				this.NavMeshVolumeHumanoids = base.gameObject.AddComponent<NavMeshModifierVolume>();
				this.NavMeshVolumeHumanoids.area = Barricade.nonWalkableArea;
				this.NavMeshVolumeHumanoids.AddAgentType(Barricade.humanoidAgentTypeId);
				this.NavMeshVolumeHumanoids.center = Vector3.zero;
				this.NavMeshVolumeHumanoids.size = Vector3.one;
				return;
			}
		}
		else if (this.NpcTriggerBox == null)
		{
			this.NpcTriggerBox = (new GameObject("NpcTriggerBox")).AddComponent<NPCBarricadeTriggerBox>();
			this.NpcTriggerBox.Setup(this);
		}
	}
}