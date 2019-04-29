using Apex.AI;
using System;

namespace Apex.AI.Visualization
{
	public abstract class ContextGizmoGUIVisualizerComponent<T> : ContextGizmoGUIVisualizerComponent
	where T : IAIContext
	{
		protected ContextGizmoGUIVisualizerComponent()
		{
		}

		protected sealed override void DrawGizmos(IAIContext context)
		{
			this.DrawGizmos((T)context);
		}

		protected abstract void DrawGizmos(T context);

		protected sealed override void DrawGUI(IAIContext context)
		{
			this.DrawGUI((T)context);
		}

		protected abstract void DrawGUI(T context);
	}
}