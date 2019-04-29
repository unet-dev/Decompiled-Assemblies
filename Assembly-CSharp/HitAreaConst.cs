using System;

public static class HitAreaConst
{
	public const HitArea Nothing = 0;

	public const HitArea Everything = HitArea.Head | HitArea.Chest | HitArea.Stomach | HitArea.Arm | HitArea.Hand | HitArea.Leg | HitArea.Foot;
}