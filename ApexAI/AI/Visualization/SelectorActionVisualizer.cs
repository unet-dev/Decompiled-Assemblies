using Apex.AI;
using System;

namespace Apex.AI.Visualization
{
	internal sealed class SelectorActionVisualizer : ConnectorActionVisualizer
	{
		private ISelect _connectedSelector;

		internal SelectorActionVisualizer(SelectorAction action, IQualifierVisualizer parent) : base(action, parent)
		{
		}

		internal override void Init()
		{
			this._connectedSelector = base.parent.parent.parent.FindSelector(((SelectorAction)base.action).selector.id);
		}

		public override IAction Select(IAIContext context)
		{
			IAction action = this._connectedSelector.Select(context);
			if (action == null)
			{
				base.parent.parent.parent.PostExecute();
			}
			return action;
		}
	}
}