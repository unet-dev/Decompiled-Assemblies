using System;

public abstract class ModelConditionTest : PrefabAttribute
{
	protected ModelConditionTest()
	{
	}

	public abstract bool DoTest(BaseEntity ent);

	protected override Type GetIndexedType()
	{
		return typeof(ModelConditionTest);
	}
}