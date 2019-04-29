using System;
using System.Collections.Generic;
using UnityEngine;

public class ConstructionGrade : PrefabAttribute
{
	[NonSerialized]
	public Construction construction;

	public BuildingGrade gradeBase;

	public GameObjectRef skinObject;

	internal List<ItemAmount> _costToBuild;

	public List<ItemAmount> costToBuild
	{
		get
		{
			if (this._costToBuild != null)
			{
				return this._costToBuild;
			}
			this._costToBuild = new List<ItemAmount>();
			foreach (ItemAmount itemAmount in this.gradeBase.baseCost)
			{
				this._costToBuild.Add(new ItemAmount(itemAmount.itemDef, Mathf.Ceil(itemAmount.amount * this.construction.costMultiplier)));
			}
			return this._costToBuild;
		}
	}

	public float maxHealth
	{
		get
		{
			if (!this.gradeBase || !this.construction)
			{
				return 0f;
			}
			return this.gradeBase.baseHealth * this.construction.healthMultiplier;
		}
	}

	public ConstructionGrade()
	{
	}

	protected override Type GetIndexedType()
	{
		return typeof(ConstructionGrade);
	}
}