using System;
using System.Collections.Generic;

public class ProcessProceduralObjects : ProceduralComponent
{
	public override bool RunOnCache
	{
		get
		{
			return true;
		}
	}

	public ProcessProceduralObjects()
	{
	}

	public override void Process(uint seed)
	{
		List<ProceduralObject> proceduralObjects = SingletonComponent<WorldSetup>.Instance.ProceduralObjects;
		if (!World.Cached)
		{
			for (int i = 0; i < proceduralObjects.Count; i++)
			{
				ProceduralObject item = proceduralObjects[i];
				if (item)
				{
					item.Process();
				}
			}
		}
		proceduralObjects.Clear();
	}
}