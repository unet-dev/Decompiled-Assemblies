using Apex.AI;
using System;

namespace Apex.AI.Visualization
{
	public sealed class BreakpointCondition
	{
		public float scoreThreshold;

		public CompareOperator compareOperator = CompareOperator.Equals;

		public BreakpointCondition()
		{
		}

		public bool Evaluate(float? score)
		{
			float? nullable;
			float single;
			if (!score.HasValue)
			{
				return false;
			}
			switch (this.compareOperator)
			{
				case CompareOperator.LessThan:
				{
					nullable = score;
					single = this.scoreThreshold;
					return nullable.GetValueOrDefault() < single & nullable.HasValue;
				}
				case CompareOperator.LessThanOrEquals:
				{
					nullable = score;
					single = this.scoreThreshold;
					return nullable.GetValueOrDefault() <= single & nullable.HasValue;
				}
				case CompareOperator.Equals:
				{
					nullable = score;
					single = this.scoreThreshold;
					return nullable.GetValueOrDefault() == single & nullable.HasValue;
				}
				case CompareOperator.NotEquals:
				{
					nullable = score;
					single = this.scoreThreshold;
					return !(nullable.GetValueOrDefault() == single & nullable.HasValue);
				}
				case CompareOperator.GreaterThanOrEquals:
				{
					nullable = score;
					single = this.scoreThreshold;
					return nullable.GetValueOrDefault() >= single & nullable.HasValue;
				}
				case CompareOperator.GreaterThan:
				{
					nullable = score;
					single = this.scoreThreshold;
					return nullable.GetValueOrDefault() > single & nullable.HasValue;
				}
				default:
				{
					nullable = score;
					single = this.scoreThreshold;
					return !(nullable.GetValueOrDefault() == single & nullable.HasValue);
				}
			}
		}

		public override string ToString()
		{
			switch (this.compareOperator)
			{
				case CompareOperator.LessThan:
				{
					return string.Concat("Score < ", this.scoreThreshold);
				}
				case CompareOperator.LessThanOrEquals:
				{
					return string.Concat("Score <= ", this.scoreThreshold);
				}
				case CompareOperator.Equals:
				{
					return string.Concat("Score == ", this.scoreThreshold);
				}
				case CompareOperator.NotEquals:
				{
					return string.Concat("Score != ", this.scoreThreshold);
				}
				case CompareOperator.GreaterThanOrEquals:
				{
					return string.Concat("Score >= ", this.scoreThreshold);
				}
				case CompareOperator.GreaterThan:
				{
					return string.Concat("Score > ", this.scoreThreshold);
				}
				default:
				{
					return string.Concat("Score != ", this.scoreThreshold);
				}
			}
		}
	}
}