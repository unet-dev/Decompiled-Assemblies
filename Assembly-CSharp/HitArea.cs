using System;

[Flags]
public enum HitArea
{
	Head = 1,
	Chest = 2,
	Stomach = 4,
	Arm = 8,
	Hand = 16,
	Leg = 32,
	Foot = 64
}