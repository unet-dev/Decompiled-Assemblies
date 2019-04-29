using Apex.AI;
using System;

namespace Apex.AI.Visualization
{
	internal interface IQualifierVisualizer : IQualifier, ICanBeDisabled
	{
		BreakpointCondition breakpointCondition
		{
			get;
			set;
		}

		bool breakPointHit
		{
			get;
			set;
		}

		bool isBreakPoint
		{
			get;
			set;
		}

		bool isHighScorer
		{
			get;
		}

		float? lastScore
		{
			get;
		}

		SelectorVisualizer parent
		{
			get;
		}

		IQualifier qualifier
		{
			get;
		}

		void Init();

		void Reset();
	}
}