using System;

public static class HitAreaUtil
{
	public static string Format(HitArea area)
	{
		if ((int)area == 0)
		{
			return "None";
		}
		if (area == (HitArea.Head | HitArea.Chest | HitArea.Stomach | HitArea.Arm | HitArea.Hand | HitArea.Leg | HitArea.Foot))
		{
			return "Generic";
		}
		return area.ToString();
	}
}