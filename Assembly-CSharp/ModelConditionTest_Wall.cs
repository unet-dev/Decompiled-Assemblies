using System;

public class ModelConditionTest_Wall : ModelConditionTest
{
	public ModelConditionTest_Wall()
	{
	}

	public override bool DoTest(BaseEntity ent)
	{
		if (ModelConditionTest_WallTriangleLeft.CheckCondition(ent))
		{
			return false;
		}
		return !ModelConditionTest_WallTriangleRight.CheckCondition(ent);
	}
}