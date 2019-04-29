using System;

public class NeedsMouseWheel : ListComponent<NeedsMouseWheel>
{
	public NeedsMouseWheel()
	{
	}

	public static bool AnyActive()
	{
		return ListComponent<NeedsMouseWheel>.InstanceList.Count > 0;
	}
}