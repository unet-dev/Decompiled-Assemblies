using System;
using UnityEngine;

public class EffectMount : EntityComponent<BaseEntity>, IClientComponent
{
	public GameObject effectPrefab;

	public GameObject spawnedEffect;

	public GameObject mountBone;

	public EffectMount()
	{
	}

	public void SetOn(bool isOn)
	{
		if (this.spawnedEffect)
		{
			GameManager.Destroy(this.spawnedEffect, 0f);
		}
		this.spawnedEffect = null;
		if (isOn)
		{
			this.spawnedEffect = UnityEngine.Object.Instantiate<GameObject>(this.effectPrefab);
			this.spawnedEffect.transform.rotation = this.mountBone.transform.rotation;
			this.spawnedEffect.transform.position = this.mountBone.transform.position;
			this.spawnedEffect.transform.parent = this.mountBone.transform;
			this.spawnedEffect.SetActive(true);
		}
	}
}