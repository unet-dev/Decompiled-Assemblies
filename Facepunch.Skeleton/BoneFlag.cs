using System;

namespace Facepunch
{
	[Flags]
	public enum BoneFlag
	{
		Left = 1,
		Middle = 2,
		Right = 4,
		Pelvis = 8,
		Hip = 16,
		Knee = 32,
		Foot = 64,
		Arm = 128,
		Elbow = 256,
		Spine = 512,
		Head = 1024,
		Eye = 2048,
		Finger = 4096,
		Thumb = 8192
	}
}