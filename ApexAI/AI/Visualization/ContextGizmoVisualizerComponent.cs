using Apex.AI;
using System;
using UnityEngine;

namespace Apex.AI.Visualization
{
	public abstract class ContextGizmoVisualizerComponent : ContextVisualizerComponent
	{
		protected ContextGizmoVisualizerComponent()
		{
		}

		protected abstract void DrawGizmos(IAIContext context);

		private void OnDrawGizmos()
		{
			if (!Application.isPlaying || !base.enabled)
			{
				return;
			}
			ContextGizmoVisualizerComponent contextGizmoVisualizerComponent = this;
			base.DoDraw(new Action<IAIContext>(contextGizmoVisualizerComponent.DrawGizmos));
		}
	}
}