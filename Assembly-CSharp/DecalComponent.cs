using System;

public abstract class DecalComponent : PrefabAttribute
{
	protected DecalComponent()
	{
	}

	protected override Type GetIndexedType()
	{
		return typeof(DecalComponent);
	}
}