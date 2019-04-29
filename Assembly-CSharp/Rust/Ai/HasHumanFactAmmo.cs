using Apex.Serialization;
using System;

namespace Rust.Ai
{
	public class HasHumanFactAmmo : BaseScorer
	{
		[ApexSerialization(defaultValue=NPCPlayerApex.AmmoStateEnum.Full)]
		public NPCPlayerApex.AmmoStateEnum @value;

		[ApexSerialization]
		public bool requireRanged;

		[ApexSerialization(defaultValue=HasHumanFactAmmo.EqualityEnum.Equal)]
		public HasHumanFactAmmo.EqualityEnum Equality;

		public HasHumanFactAmmo()
		{
		}

		public override float GetScore(BaseContext c)
		{
			if (this.requireRanged && c.GetFact(NPCPlayerApex.Facts.CurrentWeaponType) == 1)
			{
				if (this.Equality <= HasHumanFactAmmo.EqualityEnum.Equal)
				{
					return 0f;
				}
				return 1f;
			}
			byte fact = c.GetFact(NPCPlayerApex.Facts.CurrentAmmoState);
			switch (this.Equality)
			{
				case HasHumanFactAmmo.EqualityEnum.Greater:
				{
					if (fact >= (byte)this.@value)
					{
						return 0f;
					}
					return 1f;
				}
				case HasHumanFactAmmo.EqualityEnum.Gequal:
				{
					if (fact > (byte)this.@value)
					{
						return 0f;
					}
					return 1f;
				}
				case HasHumanFactAmmo.EqualityEnum.Lequal:
				{
					if (fact < (byte)this.@value)
					{
						return 0f;
					}
					return 1f;
				}
				case HasHumanFactAmmo.EqualityEnum.Lesser:
				{
					if (fact <= (byte)this.@value)
					{
						return 0f;
					}
					return 1f;
				}
				default:
				{
					if (fact == (byte)this.@value)
					{
						break;
					}
					else
					{
						return 0f;
					}
				}
			}
			return 1f;
		}

		public enum EqualityEnum
		{
			Greater,
			Gequal,
			Equal,
			Lequal,
			Lesser
		}
	}
}