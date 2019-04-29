using System;
using System.Collections.Generic;

namespace Apex.AI
{
	[FriendlyName("First Score Wins", "The first qualifier to score above the score of the Default Qualifier, is selected.")]
	public class PrioritySelector : Selector
	{
		public PrioritySelector()
		{
		}

		public override IQualifier Select(IAIContext context, IList<IQualifier> qualifiers, IDefaultQualifier defaultQualifier)
		{
			int count = qualifiers.Count;
			float single = defaultQualifier.score;
			for (int i = 0; i < count; i++)
			{
				IQualifier item = qualifiers[i];
				if (!item.isDisabled && item.Score(context) > single)
				{
					return item;
				}
			}
			return defaultQualifier;
		}
	}
}