using Apex.AI;
using System;

namespace Apex.AI.Visualization
{
	internal class ActionRequiresTerminationVisualizer : ActionVisualizer, IRequireTermination
	{
		private IRequireTermination _action;

		internal ActionRequiresTerminationVisualizer(IAction action, IQualifierVisualizer parent) : base(action, parent)
		{
			this._action = (IRequireTermination)action;
		}

		public void Terminate(IAIContext context)
		{
			this._action.Terminate(context);
		}
	}
}