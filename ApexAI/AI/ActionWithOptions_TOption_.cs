using Apex.Serialization;
using System;
using System.Collections.Generic;

namespace Apex.AI
{
	public abstract class ActionWithOptions<TOption> : IAction, ICanClone
	{
		[ApexSerialization]
		[MemberCategory(null, 10000)]
		protected List<IOptionScorer<TOption>> _scorers;

		public IList<IOptionScorer<TOption>> scorers
		{
			get
			{
				return this._scorers;
			}
		}

		protected ActionWithOptions()
		{
			this._scorers = new List<IOptionScorer<TOption>>();
		}

		public virtual void CloneFrom(object other)
		{
			ActionWithOptions<TOption> actionWithOption = other as ActionWithOptions<TOption>;
			if (actionWithOption == null)
			{
				return;
			}
			foreach (IOptionScorer<TOption> _scorer in actionWithOption._scorers)
			{
				this._scorers.Add(_scorer);
			}
		}

		public abstract void Execute(IAIContext context);

		public void GetAllScores(IAIContext context, IList<TOption> options, IList<ScoredOption<TOption>> optionsBuffer)
		{
			int count = options.Count;
			for (int i = 0; i < count; i++)
			{
				TOption item = options[i];
				float single = 0f;
				int num = this._scorers.Count;
				for (int j = 0; j < num; j++)
				{
					IOptionScorer<TOption> optionScorer = this._scorers[j];
					if (!optionScorer.isDisabled)
					{
						single += optionScorer.Score(context, item);
					}
				}
				optionsBuffer.Add(new ScoredOption<TOption>(item, single));
			}
		}

		public TOption GetBest(IAIContext context, IList<TOption> options)
		{
			TOption tOption = default(TOption);
			float single = Single.MinValue;
			int count = options.Count;
			for (int i = 0; i < count; i++)
			{
				TOption item = options[i];
				float single1 = 0f;
				int num = this._scorers.Count;
				for (int j = 0; j < num; j++)
				{
					IOptionScorer<TOption> optionScorer = this._scorers[j];
					if (!optionScorer.isDisabled)
					{
						single1 += optionScorer.Score(context, item);
					}
				}
				if (single1 > single)
				{
					tOption = item;
					single = single1;
				}
			}
			return tOption;
		}
	}
}