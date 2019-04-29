using Apex;
using Apex.AI;
using System;
using System.Collections.Generic;

namespace Apex.AI.Visualization
{
	public abstract class ActionWithOptionsVisualizerComponent<T, TOption> : CustomGizmoGUIVisualizerComponent<T, IList<ScoredOption<TOption>>>
	where T : ActionWithOptions<TOption>
	{
		private Dictionary<IAIContext, List<ScoredOption<TOption>>> _buffers;

		protected ActionWithOptionsVisualizerComponent()
		{
		}

		protected override IList<ScoredOption<TOption>> GetDataForVisualization(T aiEntity, IAIContext context, Guid aiId)
		{
			List<ScoredOption<TOption>> scoredOptions;
			IList<TOption> options = this.GetOptions(context);
			if (this._buffers.TryGetValue(context, out scoredOptions))
			{
				scoredOptions.Clear();
				scoredOptions.EnsureCapacity<ScoredOption<TOption>>(options.Count);
			}
			else
			{
				Dictionary<IAIContext, List<ScoredOption<TOption>>> aIContexts = this._buffers;
				List<ScoredOption<TOption>> scoredOptions1 = new List<ScoredOption<TOption>>(options.Count);
				scoredOptions = scoredOptions1;
				aIContexts[context] = scoredOptions1;
			}
			aiEntity.GetAllScores(context, options, scoredOptions);
			return scoredOptions;
		}

		protected abstract IList<TOption> GetOptions(IAIContext context);
	}
}