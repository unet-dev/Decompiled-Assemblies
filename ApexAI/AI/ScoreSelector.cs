using System;
using System.Collections.Generic;

namespace Apex.AI
{
	[FriendlyName("Highest Score Wins", "The qualifier with the highest score is selected")]
	public class ScoreSelector : Selector
	{
		public ScoreSelector()
		{
		}

		public override IQualifier Select(IAIContext context, IList<IQualifier> qualifiers, IDefaultQualifier defaultQualifier)
		{
			int count = qualifiers.Count;
			float single = defaultQualifier.score;
			IQualifier qualifier = null;
			for (int i = 0; i < count; i++)
			{
				IQualifier item = qualifiers[i];
				if (!item.isDisabled)
				{
					float single1 = item.Score(context);
					if (single1 > single)
					{
						single = single1;
						qualifier = item;
					}
				}
			}
			if (qualifier == null)
			{
				return defaultQualifier;
			}
			return qualifier;
		}
	}
}