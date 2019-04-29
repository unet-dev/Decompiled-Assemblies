using System;
using UnityEngine;

public class ColliderInfo : MonoBehaviour
{
	public const ColliderInfo.Flags FlagsNone = 0;

	public const ColliderInfo.Flags FlagsEverything = ColliderInfo.Flags.Usable | ColliderInfo.Flags.Shootable | ColliderInfo.Flags.Melee | ColliderInfo.Flags.Opaque | ColliderInfo.Flags.Airflow;

	public const ColliderInfo.Flags FlagsDefault = ColliderInfo.Flags.Usable | ColliderInfo.Flags.Shootable | ColliderInfo.Flags.Melee | ColliderInfo.Flags.Opaque;

	[InspectorFlags]
	public ColliderInfo.Flags flags = ColliderInfo.Flags.Usable | ColliderInfo.Flags.Shootable | ColliderInfo.Flags.Melee | ColliderInfo.Flags.Opaque;

	public ColliderInfo()
	{
	}

	public bool Filter(HitTest info)
	{
		switch (info.type)
		{
			case HitTest.Type.ProjectileEffect:
			case HitTest.Type.Projectile:
			{
				if ((int)(this.flags & ColliderInfo.Flags.Shootable) != 0)
				{
					break;
				}
				return false;
			}
			case HitTest.Type.MeleeAttack:
			{
				if ((int)(this.flags & ColliderInfo.Flags.Melee) != 0)
				{
					break;
				}
				return false;
			}
			case HitTest.Type.Use:
			{
				if ((int)(this.flags & ColliderInfo.Flags.Usable) != 0)
				{
					break;
				}
				return false;
			}
		}
		return true;
	}

	public bool HasFlag(ColliderInfo.Flags f)
	{
		return (this.flags & f) == f;
	}

	public void SetFlag(ColliderInfo.Flags f, bool b)
	{
		if (b)
		{
			this.flags |= f;
			return;
		}
		this.flags &= ~f;
	}

	[Flags]
	public enum Flags
	{
		Usable = 1,
		Shootable = 2,
		Melee = 4,
		Opaque = 8,
		Airflow = 16
	}
}