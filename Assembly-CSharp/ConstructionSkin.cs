using System;
using System.Collections.Generic;
using UnityEngine;

public class ConstructionSkin : BasePrefab
{
	private ColliderBatch[] colliderBatches;

	private List<GameObject> conditionals;

	public ConstructionSkin()
	{
	}

	private void CreateConditionalModels(BuildingBlock parent)
	{
		ConditionalModel[] conditionalModelArray = PrefabAttribute.server.FindAll<ConditionalModel>(this.prefabID);
		for (int i = 0; i < (int)conditionalModelArray.Length; i++)
		{
			if (parent.GetConditionalModel(i))
			{
				GameObject gameObject = conditionalModelArray[i].InstantiateSkin(parent);
				if (gameObject != null)
				{
					if (this.conditionals == null)
					{
						this.conditionals = new List<GameObject>();
					}
					this.conditionals.Add(gameObject);
				}
			}
		}
	}

	public void Destroy(BuildingBlock parent)
	{
		this.DestroyConditionalModels(parent);
		parent.gameManager.Retire(base.gameObject);
	}

	private void DestroyConditionalModels(BuildingBlock parent)
	{
		if (this.conditionals == null)
		{
			return;
		}
		for (int i = 0; i < this.conditionals.Count; i++)
		{
			parent.gameManager.Retire(this.conditionals[i]);
		}
		this.conditionals.Clear();
	}

	public int DetermineConditionalModelState(BuildingBlock parent)
	{
		ConditionalModel[] conditionalModelArray = PrefabAttribute.server.FindAll<ConditionalModel>(this.prefabID);
		int num = 0;
		for (int i = 0; i < (int)conditionalModelArray.Length; i++)
		{
			if (conditionalModelArray[i].RunTests(parent))
			{
				num = num | 1 << (i & 31);
			}
		}
		return num;
	}

	public void Refresh(BuildingBlock parent)
	{
		this.DestroyConditionalModels(parent);
		if (parent.isServer)
		{
			this.RefreshColliderBatching();
		}
		this.CreateConditionalModels(parent);
	}

	private void RefreshColliderBatching()
	{
		if (this.colliderBatches == null)
		{
			this.colliderBatches = base.GetComponentsInChildren<ColliderBatch>(true);
		}
		for (int i = 0; i < (int)this.colliderBatches.Length; i++)
		{
			this.colliderBatches[i].Refresh();
		}
	}
}