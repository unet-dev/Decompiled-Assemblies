using Apex.Serialization;
using System;

namespace Rust.Ai
{
	public class HasCurrentWeaponType : BaseScorer
	{
		[ApexSerialization(defaultValue=NPCPlayerApex.WeaponTypeEnum.None)]
		public NPCPlayerApex.WeaponTypeEnum @value;

		public HasCurrentWeaponType()
		{
		}

		public override float GetScore(BaseContext c)
		{
			if (c.GetFact(NPCPlayerApex.Facts.CurrentWeaponType) != (byte)this.@value)
			{
				return 0f;
			}
			return 1f;
		}
	}
}