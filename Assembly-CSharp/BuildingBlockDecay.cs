using ConVar;
using System;
using UnityEngine;

public class BuildingBlockDecay : Decay
{
	private bool isFoundation;

	public BuildingBlockDecay()
	{
	}

	protected override void AttributeSetup(GameObject rootObj, string name, bool serverside, bool clientside, bool bundling)
	{
		this.isFoundation = name.Contains("foundation");
	}

	public override float GetDecayDelay(BaseEntity entity)
	{
		BuildingBlock buildingBlock = entity as BuildingBlock;
		return base.GetDecayDelay((buildingBlock ? buildingBlock.grade : BuildingGrade.Enum.Twigs));
	}

	public override float GetDecayDuration(BaseEntity entity)
	{
		BuildingBlock buildingBlock = entity as BuildingBlock;
		return base.GetDecayDuration((buildingBlock ? buildingBlock.grade : BuildingGrade.Enum.Twigs));
	}

	public override bool ShouldDecay(BaseEntity entity)
	{
		int num;
		if (ConVar.Decay.upkeep)
		{
			return true;
		}
		if (this.isFoundation)
		{
			return true;
		}
		BuildingBlock buildingBlock = entity as BuildingBlock;
		if (buildingBlock)
		{
			num = (int)buildingBlock.grade;
		}
		else
		{
			num = 0;
		}
		return num == 0;
	}
}