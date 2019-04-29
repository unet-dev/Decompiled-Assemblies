using System;

public class AttractionPoint : PrefabAttribute
{
	public string groupName;

	public AttractionPoint()
	{
	}

	protected override Type GetIndexedType()
	{
		return typeof(AttractionPoint);
	}
}