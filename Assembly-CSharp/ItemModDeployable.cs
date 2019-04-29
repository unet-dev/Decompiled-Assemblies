using System;
using UnityEngine;

public class ItemModDeployable : MonoBehaviour
{
	public GameObjectRef entityPrefab = new GameObjectRef();

	[Header("Tooltips")]
	public bool showCrosshair;

	public string UnlockAchievement;

	public ItemModDeployable()
	{
	}

	public Deployable GetDeployable(BaseEntity entity)
	{
		if (entity.gameManager.FindPrefab(this.entityPrefab.resourcePath) == null)
		{
			return null;
		}
		return entity.prefabAttribute.Find<Deployable>(this.entityPrefab.resourceID);
	}

	internal void OnDeployed(BaseEntity ent, BasePlayer player)
	{
		if (player.IsValid() && !string.IsNullOrEmpty(this.UnlockAchievement))
		{
			player.GiveAchievement(this.UnlockAchievement);
		}
	}
}