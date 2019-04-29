using System;

namespace Mono.Cecil
{
	public struct ArrayDimension
	{
		private int? lower_bound;

		private int? upper_bound;

		public bool IsSized
		{
			get
			{
				if (this.lower_bound.HasValue)
				{
					return true;
				}
				return this.upper_bound.HasValue;
			}
		}

		public int? LowerBound
		{
			get
			{
				return this.lower_bound;
			}
			set
			{
				this.lower_bound = value;
			}
		}

		public int? UpperBound
		{
			get
			{
				return this.upper_bound;
			}
			set
			{
				this.upper_bound = value;
			}
		}

		public ArrayDimension(int? lowerBound, int? upperBound)
		{
			this.lower_bound = lowerBound;
			this.upper_bound = upperBound;
		}

		public override string ToString()
		{
			if (!this.IsSized)
			{
				return string.Empty;
			}
			return string.Concat(this.lower_bound, "...", this.upper_bound);
		}
	}
}