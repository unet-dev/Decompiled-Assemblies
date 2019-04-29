using System;
using System.Collections.Generic;

namespace Rust
{
	public class DamageTypeList
	{
		public float[] types = new float[22];

		public DamageTypeList()
		{
		}

		public void Add(DamageType index, float amount)
		{
			this.Set(index, this.Get(index) + amount);
		}

		public void Add(List<DamageTypeEntry> entries)
		{
			foreach (DamageTypeEntry entry in entries)
			{
				this.Add(entry.type, entry.amount);
			}
		}

		public float Get(DamageType index)
		{
			return this.types[(int)index];
		}

		public DamageType GetMajorityDamageType()
		{
			int num = 0;
			float single = 0f;
			for (int i = 0; i < (int)this.types.Length; i++)
			{
				float single1 = this.types[i];
				if (!float.IsNaN(single1) && !float.IsInfinity(single1) && single1 >= single)
				{
					num = i;
					single = single1;
				}
			}
			return (DamageType)num;
		}

		public bool Has(DamageType index)
		{
			return this.Get(index) > 0f;
		}

		public bool IsBleedCausing()
		{
			DamageType majorityDamageType = this.GetMajorityDamageType();
			if (majorityDamageType == DamageType.Bite || majorityDamageType == DamageType.Slash || majorityDamageType == DamageType.Stab || majorityDamageType == DamageType.Bullet)
			{
				return true;
			}
			return majorityDamageType == DamageType.Arrow;
		}

		public bool IsMeleeType()
		{
			DamageType majorityDamageType = this.GetMajorityDamageType();
			if (majorityDamageType == DamageType.Blunt || majorityDamageType == DamageType.Slash)
			{
				return true;
			}
			return majorityDamageType == DamageType.Stab;
		}

		public void Scale(DamageType index, float amount)
		{
			this.Set(index, this.Get(index) * amount);
		}

		public void ScaleAll(float amount)
		{
			for (int i = 0; i < (int)this.types.Length; i++)
			{
				this.Scale((DamageType)i, amount);
			}
		}

		public void Set(DamageType index, float amount)
		{
			this.types[(int)index] = amount;
		}

		public float Total()
		{
			float single = 0f;
			for (int i = 0; i < (int)this.types.Length; i++)
			{
				float single1 = this.types[i];
				if (!float.IsNaN(single1) && !float.IsInfinity(single1))
				{
					single += single1;
				}
			}
			return single;
		}
	}
}