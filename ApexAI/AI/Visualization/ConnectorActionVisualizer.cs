using Apex.AI;
using System;

namespace Apex.AI.Visualization
{
	internal abstract class ConnectorActionVisualizer : ActionVisualizer, IConnectorAction, IAction
	{
		internal ConnectorActionVisualizer(IAction action, IQualifierVisualizer parent) : base(action, parent)
		{
		}

		internal override void Execute(IAIContext context, bool doCallback)
		{
		}

		public abstract IAction Select(IAIContext context);
	}
}