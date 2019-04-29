using Apex.AI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Apex.AI.Visualization
{
	internal sealed class AILinkActionVisualizer : ConnectorActionVisualizer
	{
		private UtilityAIVisualizer _linkedAI;

		internal AILinkActionVisualizer(AILinkAction action, IQualifierVisualizer parent) : base(action, parent)
		{
		}

		internal override void Init()
		{
			IUtilityAI utilityAI = ((AILinkAction)base.action).linkedAI;
			if (utilityAI != null)
			{
				List<UtilityAIVisualizer> utilityAIVisualizers = base.parent.parent.parent.linkedAIs;
				this._linkedAI = utilityAIVisualizers.FirstOrDefault<UtilityAIVisualizer>((UtilityAIVisualizer lai) => lai.id == utilityAI.id);
				if (this._linkedAI == null)
				{
					this._linkedAI = new UtilityAIVisualizer(utilityAI);
					utilityAIVisualizers.Add(this._linkedAI);
				}
			}
		}

		public override IAction Select(IAIContext context)
		{
			base.parent.parent.parent.PostExecute();
			if (this._linkedAI == null)
			{
				return null;
			}
			return this._linkedAI.Select(context);
		}
	}
}