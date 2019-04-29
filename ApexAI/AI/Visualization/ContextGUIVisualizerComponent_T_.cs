using Apex.AI;
using System;

namespace Apex.AI.Visualization
{
	public abstract class ContextGUIVisualizerComponent<T> : ContextGUIVisualizerComponent
	where T : IAIContext
	{
		protected ContextGUIVisualizerComponent()
		{
		}

		protected sealed override void DrawGUI(IAIContext context)
		{
			this.DrawGUI((T)context);
		}

		protected abstract void DrawGUI(T context);
	}
}