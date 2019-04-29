using Apex.AI;
using System;

namespace Apex.AI.Visualization
{
	public abstract class ContextGizmoVisualizerComponent<T> : ContextGizmoVisualizerComponent
	where T : IAIContext
	{
		protected ContextGizmoVisualizerComponent()
		{
		}

		protected sealed override void DrawGizmos(IAIContext context)
		{
			this.DrawGizmos((T)context);
		}

		protected abstract void DrawGizmos(T context);
	}
}