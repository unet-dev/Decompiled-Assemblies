using Apex.AI;
using System;
using System.Runtime.CompilerServices;

namespace Apex.AI.Visualization
{
	internal class ScorerVisualizer : IContextualScorer, ICanBeDisabled
	{
		private IContextualScorer _scorer;

		private CompositeQualifierVisualizer _parent;

		public bool CanInvalidatePlan
		{
			get
			{
				return this._scorer.CanInvalidatePlan;
			}
			set
			{
				this._scorer.CanInvalidatePlan = value;
			}
		}

		public bool isDisabled
		{
			get
			{
				return this._scorer.isDisabled;
			}
			set
			{
				this._scorer.isDisabled = value;
			}
		}

		internal string lastScore
		{
			get;
			private set;
		}

		internal CompositeQualifierVisualizer parent
		{
			get
			{
				return this._parent;
			}
		}

		internal IContextualScorer scorer
		{
			get
			{
				return this._scorer;
			}
		}

		internal ScorerVisualizer(IContextualScorer scorer, CompositeQualifierVisualizer parent)
		{
			this._scorer = scorer;
			this._parent = parent;
		}

		internal void Reset()
		{
			this.lastScore = "-";
		}

		public float Score(IAIContext context)
		{
			ICustomVisualizer customVisualizer;
			float single = this._scorer.Score(context);
			this.lastScore = single.ToString("f0");
			if (VisualizationManager.TryGetVisualizerFor(this._scorer.GetType(), out customVisualizer))
			{
				customVisualizer.EntityUpdate(this._scorer, context, this._parent.parent.parent.id);
			}
			return single;
		}
	}
}