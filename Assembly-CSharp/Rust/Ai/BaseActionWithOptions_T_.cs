using Apex.AI;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Rust.Ai
{
	public abstract class BaseActionWithOptions<T> : ActionWithOptions<T>
	{
		private string DebugName;

		public BaseActionWithOptions()
		{
			this.DebugName = base.GetType().Name;
		}

		public abstract void DoExecute(BaseContext context);

		public override void Execute(IAIContext context)
		{
			BaseContext baseContext = context as BaseContext;
			if (baseContext != null)
			{
				this.DoExecute(baseContext);
			}
		}

		public bool TryGetBest(BaseContext context, IList<T> options, bool allScorersMustScoreAboveZero, out T best, out float bestScore)
		{
			bestScore = Single.MinValue;
			best = default(T);
			for (int i = 0; i < options.Count; i++)
			{
				float single = 0f;
				bool flag = true;
				for (int j = 0; j < base.scorers.Count; j++)
				{
					if (!base.scorers[j].isDisabled)
					{
						float single1 = base.scorers[j].Score(context, options[i]);
						if (!allScorersMustScoreAboveZero || single1 > 0f)
						{
							single += single1;
						}
						else
						{
							flag = false;
							break;
						}
					}
				}
				if (flag && single > bestScore)
				{
					bestScore = single;
					best = options[i];
				}
			}
			return best != null;
		}
	}
}