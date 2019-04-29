using Apex.AI;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Apex.AI.Visualization
{
	internal class CompositeQualifierVisualizer : QualifierVisualizer, ICompositeVisualizer
	{
		private List<IContextualScorer> _scorers;

		IList Apex.AI.Visualization.ICompositeVisualizer.children
		{
			get
			{
				return this._scorers;
			}
		}

		internal CompositeQualifierVisualizer(ICompositeScorer q, SelectorVisualizer parent) : base((IQualifier)q, parent)
		{
			this._scorers = new List<IContextualScorer>(q.scorers.Count);
			for (int i = 0; i < q.scorers.Count; i++)
			{
				this._scorers.Add(new ScorerVisualizer(q.scorers[i], this));
			}
		}

		void Apex.AI.Visualization.ICompositeVisualizer.Add(object item)
		{
			IContextualScorer contextualScorer = item as IContextualScorer;
			if (contextualScorer == null)
			{
				return;
			}
			this._scorers.Add(new ScorerVisualizer(contextualScorer, this));
		}

		public override void Reset()
		{
			base.lastScore = null;
			base.breakPointHit = false;
			int count = this._scorers.Count;
			for (int i = 0; i < count; i++)
			{
				((ScorerVisualizer)this._scorers[i]).Reset();
			}
		}

		public override float Score(IAIContext context)
		{
			ICustomVisualizer customVisualizer;
			float single = ((ICompositeScorer)base.qualifier).Score(context, this._scorers);
			base.lastScore = new float?(single);
			if (VisualizationManager.TryGetVisualizerFor(base.qualifier.GetType(), out customVisualizer))
			{
				customVisualizer.EntityUpdate(base.qualifier, context, base.parent.parent.id);
			}
			return single;
		}
	}
}