using System;

[Serializable]
public class FloatConditions
{
	public FloatConditions.Condition[] conditions;

	public FloatConditions()
	{
	}

	public bool AllTrue(float val)
	{
		FloatConditions.Condition[] conditionArray = this.conditions;
		for (int i = 0; i < (int)conditionArray.Length; i++)
		{
			if (!conditionArray[i].Test(val))
			{
				return false;
			}
		}
		return true;
	}

	[Serializable]
	public struct Condition
	{
		public FloatConditions.Condition.Types type;

		public float @value;

		public bool Test(float val)
		{
			switch (this.type)
			{
				case FloatConditions.Condition.Types.Equal:
				{
					return val == this.@value;
				}
				case FloatConditions.Condition.Types.NotEqual:
				{
					return val != this.@value;
				}
				case FloatConditions.Condition.Types.Higher:
				{
					return val > this.@value;
				}
				case FloatConditions.Condition.Types.Lower:
				{
					return val < this.@value;
				}
			}
			return false;
		}

		public enum Types
		{
			Equal,
			NotEqual,
			Higher,
			Lower
		}
	}
}